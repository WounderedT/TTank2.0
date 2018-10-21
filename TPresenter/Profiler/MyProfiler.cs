using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Filesystem;

namespace TPresenter.Profiler
{
    /// <summary>
    /// Simple profiler class.
    /// </summary>
    //TODO: Make this class thread-safe.
    public class MyProfiler: IDisposable
    {
        //Maximum amount of messages in memory. Estimated as 50MB of memory filled with 64B messeages. 
        private const uint MaxStepsInMemoryCount = 819200;

        private uint _stepsInMemoryCount;

        private List<ProfileMessage> _commitedMessages = new List<ProfileMessage>();
        private List<ProfileMessage> _uncommited = new List<ProfileMessage>();
        private List<long> _memoryPrint = new List<long>();
        //List<int> indices = new List<int>();
        private HiResTimer _clock = new HiResTimer();

        private ProfileMessage _current;
        private ProfileMessage _parent;

        ProfileMessage Current
        {
            get
            {
                if (_current != null)
                    return _current;
                else
                    return _uncommited[_uncommited.Count - 1];
            }
            set
            {
                if (_current != value)
                    _current = value;
            }
        }
        

        public MyProfiler()
        {
            ProfilerDataUtils.ProfilerDataDirPath = Path.Combine(FileProvider.ContentPath, "_ProfilingData", DateTime.Now.ToString("dd.MM.yyyy.hh.mm.ss"));
            ProfilerDataUtils.BaseLine = _clock.Value;
        }

        /// <summary>
        /// Begins new step.
        /// </summary>
        /// <param name="stepName"></param>
        public void BeginStep(string stepName)
        {
            ProfileMessage message = new ProfileMessage();
            message.Name = stepName;
            message.Start = _clock.Value;
            _uncommited.Add(message);
            _current = message;
        }

        /// <summary>
        /// Begins new sub-step.
        /// </summary>
        /// <param name="substepName">Name of the substep to add.</param>
        /// <param name="openSubsteps">Moves curent level to it's child if True. Default value is False.</param>
        public void BeginSubstep(string substepName, bool openSubsteps = false)
        {
            ProfileMessage message = new ProfileMessage();
            message.Name = substepName;
            message.Start = _clock.Value;
            if (openSubsteps)
            {
                Debug.Assert(_current.SubMessages == null, "OpenSubsteps flag is true, but several substeps already presented! Check BeginSubstep calls!");
                _current.SubMessages = new List<ProfileMessage>();
                _parent = _current;
                message.Parent = _current;
            }
            else
            {
                Debug.Assert(_parent != null, "Parent value is NULL! Previous substep closed substeps for this entry. Check EndSubstep calls!");
                message.Parent = _parent;
            }
            _parent.SubMessages.Add(message);
            _current = message;
        }

        /// <summary>
        /// Stops current profiler step.
        /// </summary>
        public void EndStep()
        {
            _current.End = _clock.Value;
            _stepsInMemoryCount++;
            Commit();
        }

        /// <summary>
        /// Ends current substep.
        /// </summary>
        /// <param name="closeSubsteps">Close current substep level and moves it to parent level. Default value is False.</param>
        public void EndSubstep(bool closeSubsteps = false)
        {
            _parent.SubMessages[_parent.SubMessages.Count - 1].End = _clock.Value;
            if (closeSubsteps)
            {
                _current = _parent;
                _parent = _current.Parent;
            }
            _stepsInMemoryCount++;
        }

        /// <summary>
        /// Stops all active steps including substeps.
        /// </summary>
        public void EndActiveSteps()
        {
            if(_uncommited.Count > 0)
                EndActiveSubsteps(_uncommited.Last());
            Commit();
        }

        void EndActiveSubsteps(ProfileMessage message)
        {
            if (message.SubMessages != null && message.SubMessages.Last().End == 0)
                EndActiveSubsteps(message.SubMessages.Last());
            if (message.End == 0)
            {
                message.End = _clock.Value;
                _stepsInMemoryCount++;
            }
        }

        public void Commit()
        {
            if(_uncommited.Count > 0)
            {
                _commitedMessages.AddRange(_uncommited);
                _uncommited.Clear();
            }
            if (_stepsInMemoryCount > MaxStepsInMemoryCount)
            {
                ProfilerDataUtils.DumpAsync(_commitedMessages);
                _commitedMessages.Clear();
                _stepsInMemoryCount = 0;
            }
        }

        public void AddMemeryUsagePrint()
        {
            _memoryPrint.Add(GC.GetTotalMemory(false));
        }

        public void Dispose()
        {
            EndActiveSteps();
            ProfilerDataUtils.DumpAsync(_commitedMessages, true).Wait();
        }

        /// <summary>
        /// Loads profiler data files and returns initialized profiler object. This method is used by ProfilerViewer tool.
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="messages"></param>
        /// <param name="memoryPrint"></param>
        public static void Load(string filepath, out List<ProfileMessage> messages, out List<long> memoryPrint)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Binder = new Binder();
            using (MemoryStream ms = new MemoryStream())
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                fs.CopyTo(ms);
                ms.Position = ms.Length - 8;
                byte[] messagesSizeBytes = new byte[8];
                ms.Read(messagesSizeBytes, 0, 8);
                var size = BitConverter.ToInt64(messagesSizeBytes, 0);
                ms.Position = 0;
                using(MemoryStream stream = new MemoryStream(ms.ToArray(), 0, (int)size))
                    messages = (List<ProfileMessage>)bf.Deserialize(stream);
                using (MemoryStream stream = new MemoryStream(ms.ToArray(), (int)size, (int)(ms.Length - size - 8)))
                    memoryPrint = (List<long>)bf.Deserialize(stream);
            }
        }

    }

    [Serializable]
    public class ProfileMessage
    {
        public long Start;
        public long End;
        public string Name;
        public List<ProfileMessage> SubMessages;
        public ProfileMessage Parent;

        /// <summary>
        /// Returns duration of profiler step in ticks.
        /// </summary>
        /// <returns></returns>
        public double GetDuration()
        {
            //return (double)(End - Start) * 100 / HiResTimer.Frequency; //3.6467203220783389E-07
            return End - Start; //3.6467203220783389E-07
        }

        public override string ToString()
        {
            return Name + "#" + GetDuration();
        }
    }

    /// <summary>
    /// High resolution time helper class. 
    /// This implementation was taken from https://msdn.microsoft.com/en-us/library/windows/desktop/dn553408(v=vs.85).aspx
    /// </summary>
    public class HiResTimer
    {
        private static Int64 frequency = 0; //Current performance-counter frequency in counts per second

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        static HiResTimer()
        {
            QueryPerformanceFrequency(out frequency);
        }

        public static Int64 Frequency
        {
            get
            {
                return frequency;
            }
        }

        public Int64 Value
        {
            get
            {
                Int64 tickCount = 0;
                QueryPerformanceCounter(out tickCount);
                return tickCount;
            }
        }
    }

    public class Binder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type tyType = null;

            string sShortAssemblyName = assemblyName.Split(',')[0];
            Assembly[] ayAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly ayAssembly in ayAssemblies)
            {
                if (sShortAssemblyName == ayAssembly.FullName.Split(',')[0])
                {
                    tyType = ayAssembly.GetType(typeName);
                    break;
                }
            }

            if (tyType == null && typeName.Contains("[["))
            {
                if (typeName.Contains("System.Collections.Generic.List`1[[TPresenter.Profiler.ProfileMessage, TPresenter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]"))
                    tyType = typeof(List<ProfileMessage>);
            }

            return tyType;
        }
    }
}

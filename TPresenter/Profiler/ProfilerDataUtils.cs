using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TPresenter.Profiler
{
    static class ProfilerDataUtils
    {
        private static byte _nextId = 0;
        private static string _profilerDataDirPath;

        private static Dictionary<string, Byte> _stepNames = new Dictionary<string, Byte>();
        private static Dictionary<Byte, List<DataMessage>> _processedDataMessages = new Dictionary<Byte, List<DataMessage>>();
        private static Dictionary<Byte, List<IndexerMessage>> _processedIndexerMessages = new Dictionary<Byte, List<IndexerMessage>>();
        private static Dictionary<Byte, Int32> _dataPositions = new Dictionary<Byte, Int32>();
        private static Dictionary<Byte, Double[]> _dataAbsoluteValues = new Dictionary<Byte, Double[]>();

        internal static string ProfilerDataDirPath
        {
            get { return _profilerDataDirPath; }
            set
            {
                if(_profilerDataDirPath != value)
                {
                    _profilerDataDirPath = value;
                    Directory.CreateDirectory(_profilerDataDirPath);
                }
            }
        }

        internal static Int64 BaseLine { get; set; }

        internal static Task DumpAsync(List<ProfileMessage> commited, bool dispose = false)
        {
            var messages = commited.GetInternalArray();
            var task = new Task(() =>
            {
                Write(messages, dispose);
            });
            task.Start();
            return task;
        }

        private static void VerifyMessages()
        {
            double netDuration = 0;
            foreach(var key in _processedDataMessages.Keys)
            {
                foreach(var entry in _processedDataMessages[key])
                {
                    if(entry.SubstepsSize != 0)
                    {
                        netDuration = entry.Duration;
                        foreach(var substep in entry.Substeps)
                        {
                            netDuration -= _processedDataMessages[substep.StepId][substep.PositionId].Duration;
                        }
                        Debug.Assert(netDuration > 0, "Net duration of the step cannot be less that zero!");
                    }
                }
            }
        }

        private static void Write(ProfileMessage[] messages, bool dispose)
        {
            foreach (ProfileMessage message in messages)
            {
                if(message != null)
                    ProcessMessage(message);
            }
            VerifyMessages();
            foreach (var entry in _processedDataMessages)
            {
                using (FileStream stream = new FileStream(Path.Combine(ProfilerDataDirPath, entry.Key + ".data"), FileMode.Append))
                    foreach (var dataMessage in entry.Value)
                        dataMessage.ToStream(stream);
            }
            foreach (var entry in _processedIndexerMessages)
            {
                using (FileStream stream = new FileStream(Path.Combine(ProfilerDataDirPath, entry.Key + ".indexer"), FileMode.Append))
                    foreach (var dataMessage in entry.Value)
                        dataMessage.ToStream(stream);
            }

            if (dispose)
            {
                using(StreamWriter stream = new StreamWriter(Path.Combine(ProfilerDataDirPath, "names")))
                    foreach(var name in _stepNames.Keys)
                        stream.WriteLine(name);
                foreach (var entry in _processedDataMessages)
                {
                    using (FileStream stream = new FileStream(Path.Combine(ProfilerDataDirPath, entry.Key + ".data"), FileMode.Append))
                    {
                        stream.Write(BitConverter.GetBytes(_dataAbsoluteValues[entry.Key][0]), 0, 8);
                        stream.Write(BitConverter.GetBytes(_dataAbsoluteValues[entry.Key][1]), 0, 8);
                    }
                }
            }
            else
            {
                foreach (var key in _processedDataMessages.Keys)
                    _processedDataMessages[key].Clear();
                foreach (var key in _processedIndexerMessages.Keys)
                    _processedIndexerMessages[key].Clear();
            }
        }
        
        private static DataSubmessage ProcessMessage(ProfileMessage message, bool isSubmessage = false)
        {
            DataSubmessage submessage = null;
            var entry = new DataMessage(message.SubMessages != null ? message.SubMessages.Count : 0);
            entry.Duration = message.GetDuration();
            byte id = GetStepId(message.Name);
            if (isSubmessage)
                submessage = new DataSubmessage(id, _processedDataMessages[id].Count);
            if(message.SubMessages != null)
            {
                for (int ind = 0; ind < message.SubMessages.Count; ind++)
                {
                    entry.Substeps[ind] = ProcessMessage(message.SubMessages[ind], true);
                }
            }
            _processedDataMessages[id].Add(entry);
            _processedIndexerMessages[id].Add(new IndexerMessage(entry.Size, GetDataPosition(id), message.End - BaseLine));
            _dataPositions[id] += entry.Size;

            if (entry.Duration < _dataAbsoluteValues[id][0])
                _dataAbsoluteValues[id][0] = entry.Duration;
            else if (entry.Duration > _dataAbsoluteValues[id][1])
                _dataAbsoluteValues[id][1] = entry.Duration;

            return submessage;
        }

        private static byte GetStepId(string name)
        {
            byte id;
            if (!_stepNames.TryGetValue(name, out id))
            {
                id = _nextId++;
                _stepNames.Add(name, id);
                _processedDataMessages.Add(id, new List<DataMessage>());
                _processedIndexerMessages.Add(id, new List<IndexerMessage>());
                _dataAbsoluteValues.Add(id, new Double[2]);
            }
            return id;
        }

        private static string GetStepName(byte id)
        {
            return (from entry in _stepNames
                    where entry.Value == id
                    select entry.Key).Single();
        }

        private static Int32 GetDataPosition(Byte id)
        {
            Int32 position;
            if(!_dataPositions.TryGetValue(id, out position))
            {
                _dataPositions.Add(id, 0);
                position = 0;
            }
            return position;
        }
    }
}

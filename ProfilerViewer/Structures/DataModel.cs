using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Profiler;

namespace ProfilerViewer.Structures
{
    public class DataModel
    {
        private int _scale;
        private int scaleMax;
        private int scaleValue;

        public Dictionary<string, List<Step>> data;

        public int Scale
        {
            get { return _scale; }
            set
            {
                if(_scale != value)
                {
                    _scale = value;
                    scaleValue = _scale / scaleMax;
                }
            }
        }

        public DataModel(int scaleMax)
        {
            this.scaleMax = scaleMax;
        }

        public void Init(List<ProfileMessage> messages)
        {
            data = new Dictionary<string, List<Step>>();
            var iterationStep = messages.Count / scaleMax;
            for(var ind = 0; ind < scaleMax; ind++)
            {
                LoadStep(messages[ind * iterationStep]);
            }
        }

        Step LoadStep(ProfileMessage message)
        {
            List<Step> steps;
            if(!data.TryGetValue(message.Name, out steps))
            {
                steps = new List<Step>();
                data.Add(message.Name, steps);
            }
            Step step = new Step();
            step.Name = message.Name;
            step.DurationTotal = message.GetDuration();
            if(message.SubMessages != null)
            {
                step.Substeps = new List<Step>();
                foreach (ProfileMessage m in message.SubMessages)
                {
                    step.Substeps.Add(LoadStep(m));
                }
            }
            data[message.Name].Add(step);
            return step;
        }
    }

    public class Step
    {
        private double _total;
        private double _net;
        private string _name;

        public double DurationTotal
        {
            get { return _total; }
            set { _total = value; }
        }

        public double DurationNet
        {
            get
            {
                if(_net == 0)
                {
                    if(Substeps != null)
                    {
                        double duration = 0;
                        foreach (Step s in Substeps)
                        {
                            duration += s.DurationTotal;
                        }
                        _net = _total - duration;
                    }
                    else
                    {
                        _net = _total;
                    }
                }
                return _net;
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public List<Step> Substeps;

        public override string ToString()
        {
            return Name;
        }
    }
}

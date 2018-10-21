using ProfilerViewer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using TPresenter.Profiler;

namespace ProfilerViewer.Structures
{
    /// <summary>
    /// Contains basic graph related information - <cref Polyline/> object to draw graph, graph's minimum and maximum values and scale
    /// </summary>
    public class GraphLine
    {
        private Int32 _maxWindowHeight;
        private Int32 _graphHeightPadding;
        private Int32 _xLineStep;

        public Double Min { get; private set; }
        public Double Max { get; private set; }
        public Double NetMin { get; private set; } = Double.MaxValue;
        public Double NetMax { get; private set; } = 0;
        public Int32[] Indexes { get; private set; }
        public Polyline Line { get; private set; }
        public Polyline NetLine { get; private set; }
        public Brush LineBrush { get; } = (SolidColorBrush)new BrushConverter().ConvertFromString("Blue");
        public Brush NetLineBrush { get; } = (SolidColorBrush)new BrushConverter().ConvertFromString("Green");

        public GraphLine(Dictionary<Int32, DataMessageExt> dataMessages, Int32[] dataIndexers, Double min, Double max, GraphBox graphBox)
        {
            Min = min;
            Max = max;
            Indexes = dataIndexers;
            PointCollection points = new PointCollection();
            Double valueWindow = Max - Min;
            _maxWindowHeight = graphBox.Y1 - graphBox.Y0;
            _graphHeightPadding = graphBox.Y0;
            _xLineStep = (graphBox.X1 - graphBox.X0) / dataIndexers.Length;
            Int32 x = 0;
            foreach (var index in Indexes)
            {
                double y = _graphHeightPadding + ((dataMessages[index].Duration - min) / valueWindow) * _maxWindowHeight;
                points.Add(new Point(x, y));
                x += _xLineStep;
            }
            Line = new Polyline();
            Line.Points = points;
        }

        public void BuildNetLine(Dictionary<Int32, DataMessageExt> dataMessages)
        {
            foreach(var index in Indexes)
            {
                if (dataMessages[index].NetDuration < NetMin)
                    NetMin = dataMessages[index].NetDuration;
                else if (dataMessages[index].NetDuration > NetMax)
                    NetMax = dataMessages[index].NetDuration;
            }
            PointCollection points = new PointCollection();
            Double valueWindow = NetMax - NetMin;
            Int32 x = 0;
            foreach (var index in Indexes)
            {
                double y = _graphHeightPadding + ((dataMessages[index].NetDuration - NetMin) / valueWindow) * _maxWindowHeight;
                points.Add(new Point(x, y));
                x += _xLineStep;
            }
            NetLine = new Polyline();
            NetLine.Points = points;
        }
    }
}

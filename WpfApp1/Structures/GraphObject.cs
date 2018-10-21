using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ProfilerViewer.Structures
{
    public class GraphObject
    {
        public double Min { get; private set; }
        public double Max { get; private set; }
        public int Scale { get; private set; }
        public Polyline Line { get; private set; }

        public void BuildGraph(List<double> data, int scale, int maxX, int minY, int maxY)
        {
            Scale = scale;
            float step = (float)data.Count / (float)scale;
            List<double> scaledData = data.Where((x, i) => (int)(i % step) == 0).ToList();
            Min = scaledData.Min();
            Max = scaledData.Max();
            double window = Max - Min;
            var max = step == 1 ? scale : scaledData.Count;
            PointCollection points = new PointCollection();
            for (int i = 0; i < max; i ++)
            {
                double y = maxY - ((scaledData[i] - Min) / window) * maxY;
                points.Add(new Point(i * maxX / scale, y == 0 ? minY : y));
            }
            Line = new Polyline();
            Line.Points = points;
        }
    }
}

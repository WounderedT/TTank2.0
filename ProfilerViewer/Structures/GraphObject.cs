using ProfilerViewer.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using TPresenter.Profiler;

namespace ProfilerViewer.Structures
{
    public class GraphObject: IDisposable
    {
        private Int32 _totalCount;
        private FileStream _indexerStream;
        private FileStream _dataStream;
        private GraphLine _currentLine;
        //This dictionary could potentially contain all .data records. "Expiration" mechanism should be developed to prevent
        // out of memory exception.
        private Dictionary<Int32, DataMessageExt> _dataMessages = new Dictionary<Int32, DataMessageExt>();
        private Dictionary<Int32, GraphLine> _defaultLines = new Dictionary<Int32, GraphLine>();
        private Dictionary<ZoomInfo, GraphLine> _zoomedLines = new Dictionary<ZoomInfo, GraphLine>();
        private Dictionary<Guid, GraphLine> _substepLines = new Dictionary<Guid, GraphLine>();

        public ZoomInfo CurrentZoom { get; private set; }

        public DataMessageExt this[Int32 index]
        {
            get
            {
                DataMessageExt message;
                if (!_dataMessages.TryGetValue(index, out message))
                    Console.WriteLine();
                return message;
            }
            set
            {
                if (_dataMessages.ContainsKey(index))
                    _dataMessages[index] = value;
                else
                    _dataMessages.Add(index, value);
            }
        }

        public GraphLine NetGraph
        {
            get
            {
                if (_currentLine.NetLine != null)
                    return _currentLine;
                else
                    return null;
            }
        }

        public GraphLine Graph
        {
            get { return _currentLine; }
        }

        ~GraphObject()
        {
            Dispose();
        }

        public GraphObject(string profilerDataPath, Int32 stepId)
        {
            string indexersPath = System.IO.Path.Combine(profilerDataPath, stepId + ".indexer");
            _indexerStream = new FileStream(indexersPath, FileMode.Open);
            _dataStream = new FileStream(System.IO.Path.Combine(profilerDataPath, stepId + ".data"), FileMode.Open);

            _totalCount = (Int32)(new FileInfo(indexersPath).Length / IndexerMessage.IndexerMessageSize);

            FileInfo info = new FileInfo(System.IO.Path.Combine(profilerDataPath, stepId + ".data"));
            _dataStream.Position = info.Length - 16;
            byte[] bytes = new byte[16];
            _dataStream.Read(bytes, 0, 16);
            AbsoluteValues = new Double[] { BitConverter.ToDouble(bytes, 0), BitConverter.ToDouble(bytes, 8) };
        }

        public Double[] AbsoluteValues { get; private set; }

        public GraphLine BuildNetGraph()
        {
            _currentLine.BuildNetLine(_dataMessages);
            return _currentLine;
        }

        public GraphLine GetOrBuildGraph(Int32 scale)
        {
            GraphLine line;
            if(!_defaultLines.TryGetValue(scale, out line))
            {
                line = BuildGraph(0, _totalCount - 1, ref scale);
                _defaultLines.Add(scale, line);
            }
            _currentLine = line;
            CurrentZoom = null;
            return line;
        }

        public GraphLine GetOrBuildGraph(ZoomInfo zoom)
        {
            GraphLine line;
            if (!_zoomedLines.TryGetValue(zoom, out line))
            {
                line = BuildGraph(zoom.LowerBound, zoom.UpperBound, ref zoom.Scale);
                _zoomedLines.Add(zoom, line);
            }
            _currentLine = line;
            CurrentZoom = zoom;
            return line;
        }

        public GraphLine GetOrBuildGraph(Guid graphId, List<Int32> stepIds)
        {
            GraphLine line;
            if(!_substepLines.TryGetValue(graphId, out line))
            {
                Double min = Double.MaxValue;
                Double max = 0;
                foreach (var index in stepIds)
                {
                    if (_dataMessages[index].Duration < min)
                        min = _dataMessages[index].Duration;
                    else if (_dataMessages[index].Duration > max)
                        max = _dataMessages[index].Duration;
                }
                _substepLines.Add(graphId, new GraphLine(_dataMessages, stepIds.ToArray(), min, max, GraphViewModel.GraphBox));
                line = _substepLines[graphId];
            }
            _currentLine = line;
            return line;
        }

        public Int32 RelativeIdToId(Int32 relativeId)
        {
            return _currentLine.Indexes[relativeId];
        }

        public DataMessage GetDataMessageByRelativeId(Int32 relativeId)
        {
            return GetDataMessageById(_currentLine.Indexes[relativeId]);
        }

        public DataMessage GetDataMessageById(Int32 id)
        {
            DataMessageExt message;
            if(!_dataMessages.TryGetValue(id, out message))
            {
                _indexerStream.Seek(id * IndexerMessage.IndexerMessageSize, SeekOrigin.Begin);
                var indexer = IndexerMessage.FromStream(_indexerStream);
                _dataStream.Seek(indexer.Position, SeekOrigin.Begin);
                message = DataMessageExt.FromStream(_dataStream, indexer.Size);
                _dataMessages.Add(id, message);
            }
            return message;
        }

        public Int32[] GetCurrentGraphIndexes()
        {
            return _currentLine.Indexes;
        }

        public void LoadSteps(List<Int32> positions)
        {
            Dictionary<int, IndexerMessage> indexers = new Dictionary<int, IndexerMessage>();
            DataMessageExt message;
            for (Int32 ind = 0; ind < positions.Count; ind++)
            {
                if (!_dataMessages.ContainsKey(positions[ind]))
                {
                    _indexerStream.Seek(positions[ind] * IndexerMessage.IndexerMessageSize, SeekOrigin.Begin);
                    indexers.Add(ind, IndexerMessage.FromStream(_indexerStream));
                }
            }
            for (Int32 ind = 0; ind < positions.Count; ind++)
            {
                if(indexers.ContainsKey(ind))
                {
                    _dataStream.Seek(indexers[ind].Position, SeekOrigin.Begin);
                    message = DataMessageExt.FromStream(_dataStream, indexers[ind].Size);
                    _dataMessages.AddOrUpdateOnNull(positions[ind], message);
                }
            }
        }

        public void Dispose()
        {
            _indexerStream.Dispose();
            _dataStream.Dispose();
        }

        private GraphLine BuildGraph(Int32 lowerBound, Int32 upperBound, ref Int32 scale)
        {
            lowerBound = (lowerBound <= 0) ? 1 : lowerBound;
            upperBound = (upperBound >= _totalCount) ? (_totalCount - 1) : upperBound;
            Int32 elementCount = upperBound - lowerBound;
            scale = elementCount > scale ? scale : elementCount;

            Int32[] dataIndexers = new Int32[scale];
            Int32 step = elementCount / scale;
            DataMessageExt dataMessage;
            Int32 dataInd = 0;
            Double min = Double.MaxValue;
            Double max = 0;
            List<Int32> newIndexes = new List<Int32>();
            for (Int32 ind = lowerBound; ind < upperBound; ind+= step)
            {
                if (!_dataMessages.TryGetValue(ind, out dataMessage))
                    newIndexes.Add(ind);
            }
            LoadSteps(newIndexes);
            for (Int32 ind = lowerBound; ind < upperBound; ind+= step)
            {
                if (dataInd == scale)
                    break;
                dataMessage = _dataMessages[ind];
                if (dataMessage.Duration < min)
                    min = dataMessage.Duration;
                else if (dataMessage.Duration > max)
                    max = dataMessage.Duration;
                dataIndexers[dataInd++] = ind;
            }
            return new GraphLine(_dataMessages, dataIndexers, min, max, GraphViewModel.GraphBox);
        }
    }

    public class ZoomInfo
    {
        public Int32 LowerBound;
        public Int32 UpperBound;
        public Int32 Scale;

        public ZoomInfo() { }

        public ZoomInfo(Int32 lowerBound, Int32 upperBound, Int32 scale)
        {
            LowerBound = lowerBound;
            UpperBound = upperBound;
            Scale = scale;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} : {2}", LowerBound, UpperBound, Scale);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if(obj is ZoomInfo zoom)
            {
                return LowerBound == zoom.LowerBound && UpperBound == zoom.UpperBound && Scale == zoom.Scale;
            }
            else
            {
                return false;
            }
        }
    }
}

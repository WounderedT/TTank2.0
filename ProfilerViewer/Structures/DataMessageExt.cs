using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Profiler;

namespace ProfilerViewer.Structures
{
    public class DataMessageExt : DataMessage
    {
        public Double NetDuration { get; set; }

        public DataMessageExt(Int32 capacity) : base(capacity) { }

        public DataMessageExt(DataMessage instance) : base(instance)
        {
            NetDuration = Duration;
        }

        public static new DataMessageExt FromStream(Stream stream, Int32 size)
        {
            return new DataMessageExt(DataMessage.FromStream(stream, size));
        }
    }
}

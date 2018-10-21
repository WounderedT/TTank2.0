using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TPresenter.Profiler;

namespace TPresenter.Test.TPresenter
{
	[TestFixture]
    class ProfilerDataTypesTest
    {
		[Test]
		public void DataMessageStreamTest()
        {
            DataMessage oldMessage = new DataMessage(5);
            oldMessage.Duration = 100;
            oldMessage.Substeps[0] = new DataSubmessage(10, 4421);
            oldMessage.Substeps[1] = new DataSubmessage(11, 532);
            oldMessage.Substeps[2] = new DataSubmessage(12, 3525);
            oldMessage.Substeps[3] = new DataSubmessage(13, 636);
            oldMessage.Substeps[4] = new DataSubmessage(14, 823);

			using(MemoryStream stream = new MemoryStream())
            {
                oldMessage.ToStream(stream);
                stream.Position = 0;
                var newMessage = DataMessage.FromStream(stream, oldMessage.Size);
                Assert.AreEqual(newMessage, oldMessage);
            }
        }

        [Test]
        public void IndexerMessageStreamTest()
        {
            IndexerMessage oldIndexer = new IndexerMessage(100, 11, 123456789);
            using(MemoryStream stream = new MemoryStream())
            {
                oldIndexer.ToStream(stream);
                stream.Position = 0;
                var newIndexer = IndexerMessage.FromStream(stream);
                Assert.AreEqual(newIndexer, oldIndexer);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Profiler
{
    public class DataMessage : IEquatable<DataMessage>
    {
        private const byte DataMessageHeaderSize = 12;

        public double Duration { get; set; }
        public int SubstepsSize { get { return DataSubmessage.Size * Substeps.Length; } }

        public int Size { get { return DataMessageHeaderSize + SubstepsSize; } }
        public DataSubmessage[] Substeps { get; }

        public DataMessage(int capacity)
        {
            Substeps = new DataSubmessage[capacity];
        }

        public DataMessage(DataMessage instance)
        {
            Duration = instance.Duration;
            Substeps = instance.Substeps;
        }

        public void ToStream(Stream stream)
        {
            byte[] bytes = new byte[SubstepsSize];
            stream.Write(BitConverter.GetBytes(Duration), 0, 8);
            stream.Write(BitConverter.GetBytes(SubstepsSize), 0, 4);
            for (int ind = 0; ind < Substeps.Length; ind++)
                Substeps[ind].ToBytes(bytes, DataSubmessage.Size * ind);
            stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Returns new instance of <cref DataMessage/> from <paramref name="stream"/> and advancing stream position by <paramref name="size"/>
        /// </summary>
        /// <param name="stream">Filestream to read bytes from</param>
        /// <param name="size">Size of the DataMessage instance</param>
        /// <returns></returns>
        public static DataMessage FromStream(Stream stream, int size)
        {
            byte[] bytes = new byte[size];
            stream.Read(bytes, 0, size);
            DataMessage entity = new DataMessage(BitConverter.ToInt32(bytes, 8) / DataSubmessage.Size);
            entity.Duration = BitConverter.ToDouble(bytes, 0);
            for (int ind = 0; ind < entity.Substeps.Length; ind++)
                entity.Substeps[ind] = DataSubmessage.FromBytes(bytes, DataMessageHeaderSize + DataSubmessage.Size * ind);
            return entity;
        }

        public override bool Equals(object obj)
        {
            return (obj is DataMessage) && Equals((DataMessage)obj);
        }

        public bool Equals(DataMessage other)
        {
            if (!(other != null && Duration == other.Duration && SubstepsSize == other.SubstepsSize))
                return false;
            for (int ind = 0; ind < Substeps.Length; ind++)
                if (!Substeps[ind].Equals(other.Substeps[ind]))
                    return false;
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class DataSubmessage : IEquatable<DataSubmessage>
    {
        public byte StepId;
        public int PositionId;

        public static int Size { get; } = 5;

        public DataSubmessage(byte stepId, int ind)
        {
            StepId = stepId;
            PositionId = ind;
        }

        public void ToBytes(byte[] bytes, int position)
        {
            bytes[position] = StepId;
            BitConverter.GetBytes(PositionId).CopyTo(bytes, position + 1);
        }

        public static DataSubmessage FromBytes(byte[] bytes, int position)
        {
            return new DataSubmessage(bytes[position], BitConverter.ToInt32(bytes, position + 1));
        }

        public override bool Equals(object obj)
        {
            return (obj is DataSubmessage) && Equals((DataSubmessage)obj);
        }

        public bool Equals(DataSubmessage other)
        {
            return other != null &&
                StepId == other.StepId &&
                PositionId == other.PositionId;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public struct IndexerMessage: IEquatable<IndexerMessage>
    {
        public Int32 Size;
        public Int64 Position;
        public Int64 Timestamp;

        public static Int32 IndexerMessageSize { get; } = 20;

        public IndexerMessage(Int32 size, Int64 position, Int64 timestamp)
        {
            Size = size;
            Position = position;
            Timestamp = timestamp;
        }

        public void ToStream(Stream stream)
        {
            stream.Write(BitConverter.GetBytes(Size), 0, 4);
            stream.Write(BitConverter.GetBytes(Position), 0, 8);
            stream.Write(BitConverter.GetBytes(Timestamp), 0, 8);
        }

        /// <summary>
        /// Returns new instance of <cref IndexerMessage/> from <paramref name="stream"/> and advance stream position by 12. 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static IndexerMessage FromStream(Stream stream)
        {
            byte[] bytes = new byte[20];
            stream.Read(bytes, 0, bytes.Length);
            return new IndexerMessage(BitConverter.ToInt32(bytes, 0), BitConverter.ToInt64(bytes, 4), BitConverter.ToInt64(bytes, 12));
        }

        public override bool Equals(object obj)
        {
            return (obj is IndexerMessage) && Equals((IndexerMessage)obj);
        }

        public bool Equals(IndexerMessage other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator == (IndexerMessage indexer1, IndexerMessage indexer2)
        {
            return indexer1.Size == indexer2.Size && indexer1.Position == indexer2.Position && indexer1.Timestamp == indexer2.Timestamp;
        }

        public static bool operator !=(IndexerMessage indexer1, IndexerMessage indexer2)
        {
            return indexer1.Size != indexer2.Size || indexer1.Position != indexer2.Position || indexer1.Timestamp != indexer2.Timestamp;
        }
    }
}

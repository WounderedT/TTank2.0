using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Wraps this object into an IEnumerable&lt;T&gt; consisting of a single item.
        /// </summary>
        /// <typeparam name="T"> Type of the object. </typeparam>
        /// <param name="item"> The instance than will be wrapped. </param>
        /// <returns> An IEnumerable&lt;T&gt; consisting of a single item. </returns>
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }
    }

    /// <summary>
    /// Extensions to BinaryReader to simplify loading .CMO scenes
    /// </summary>
    public static class BinaryReaderExtensions
    {
        /// <summary>
        /// Loads a string from the CMO file (WCHAR prefixed with uint length)
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public static string ReadCMO_wchar(this BinaryReader bReader)
        {
            int length = (int)bReader.ReadUInt32();
            if (length > 0)
            {
                var result = System.Text.Encoding.Unicode.GetString(bReader.ReadBytes(length * 2), 0, length * 2);
                return result.Substring(0, result.Length - 1);
            }
            else
                return null;
        }

        public static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return stuff;
        }

        /// <summary>
        /// Read a structure from binary reader
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="br"></param>
        public static T ReadStructure<T>(this BinaryReader bReader) where T : struct
        {
            return ByteArrayToStructure<T>(bReader.ReadBytes(Utilities.SizeOf<T>()));
        }

        /// <summary>
        /// Read <paramref name="count"/> instances of the structure from the binary reader.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="br"></param>
        /// <param name="count"></param>
        public static T[] ReadStructure<T>(this BinaryReader bReader, int count) where T : struct
        {
            T[] result = new T[count];
            for (int ind = 0; ind < count; ind++)
            {
                result[ind] = ByteArrayToStructure<T>(bReader.ReadBytes(Utilities.SizeOf<T>()));
            }
            return result;
        }

        /// <summary>
        /// Read <paramref name="count"/> UInt16s.
        /// </summary>
        /// <param name="br"></param>
        /// <param name="count"></param>
        public static ushort[] ReadInt16(this BinaryReader bReader, int count)
        {
            ushort[] result = new ushort[count];
            for (int ind = 0; ind < count; ind++)
            {
                result[ind] = bReader.ReadUInt16();
            }

            return result;
        }
    }

    /// <summary>
    /// Extension methods to Dictionary class.
    /// </summary>
    public static class DictionaryExtensions
    {
        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Dictionary<TKey, TValue> elemets)
        {
            foreach(KeyValuePair<TKey, TValue> keyValuePair in elemets)
            {
                if (!dictionary.ContainsKey(keyValuePair.Key))
                {
                    dictionary.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }
        }
    }
}

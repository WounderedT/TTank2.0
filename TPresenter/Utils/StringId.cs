using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace TPresenter
{
    //TODO: All Get_XXX and Set_XXX methods must have a lock for multithread access.
    [Serializable]
    public struct StringId : IXmlSerializable
    {
        public static readonly StringId NullOrEmpty;
        public static readonly IdComparerType Comparer = new IdComparerType();

        private static Dictionary<string, StringId> stringToId;
        private static Dictionary<StringId, string> idToString;
        private int id;
        
        private StringId(int id)
        {
            this.id = id;
        }

        public int Id
        {
            get { return id; }
        }

        public string String
        {
            get { return idToString[this]; }
        }

        public override string ToString()
        {
            return String;
        }

        public override int GetHashCode()
        {
            return id;
        }

        public override bool Equals(object obj)
        {
            return (obj is StringId) && Equals((StringId)obj);
        }

        public bool Equals(StringId obj)
        {
            return id == obj.id;
        }

        public static bool operator == (StringId lhs, StringId rhs)
        {
            return lhs.id == rhs.id;
        }

        public static bool operator != (StringId lhs, StringId rhs)
        {
            return lhs.id != rhs.id;
        }

        public static explicit operator int(StringId id)
        {
            return id.id;
        }

        public class IdComparerType : IComparer<StringId>, IEqualityComparer<StringId>
        {
            public int Compare(StringId x, StringId y)
            {
                return x.id - y.id;
            }

            public bool Equals(StringId x, StringId y)
            {
                return x.id == y.id;
            }

            public int GetHashCode(StringId obj)
            {
                return obj.id;
            }
        }

        static StringId()
        {
            stringToId = new Dictionary<string, StringId>();
            idToString = new Dictionary<StringId, string>(Comparer);

            NullOrEmpty = GetOrCompute("");
        }

        public static StringId GetOrCompute(string str)
        {
            StringId result;

            if (str == null)
                result = NullOrEmpty;
            else if(!stringToId.TryGetValue(str, out result))
            {
                result = new StringId(stringToId.Count);
                idToString.Add(result, str);
                stringToId.Add(str, result);
            }

            return result;
        }

        public static StringId[] GetOrCompute(string[] strArray)
        {
            StringId[] result = new StringId[strArray.Length];
            for (var ind = 0; ind < strArray.Length; ind++)
                result[ind] = GetOrCompute(strArray[ind]);
            return result;
        }

        public static StringId Get(string str)
        {
            return stringToId[str];
        }

        public static bool TryGet(string str, out StringId id)
        {
            return stringToId.TryGetValue(str, out id);
        }

        public static StringId TryGet(string str)
        {
            StringId id;
            stringToId.TryGetValue(str, out id);
            return id;
        }

        public static bool IsKnown(StringId id)
        {
            return idToString.ContainsKey(id);
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            var str = reader.ReadElementContentAsString();
            if (stringToId.ContainsKey(str))
            {
                id = stringToId[str].Id;
            }
            else
            {
                id = stringToId.Count;
                idToString.Add(this, str);
                stringToId.Add(str, this);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteString(idToString[this]);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TPresenter.Serialization
{
    /// <summary>
    /// This class manages type-dependant XML serializers.
    /// </summary>
    public class XmlSerializerManager
    {
        private static Dictionary<Type, XmlSerializer> _typeBasedSerializers = new Dictionary<Type, XmlSerializer>();

        public static XmlSerializer GetSerializer(Type type)
        {
            return _typeBasedSerializers[type];
        }

        public static XmlSerializer GetOrCreateSerializer(Type type)
        {
            XmlSerializer serializer;
            if (!_typeBasedSerializers.TryGetValue(type, out serializer))
                serializer = CreateSerializer(type);
            return serializer;
        }

        private static XmlSerializer CreateSerializer(Type type)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            _typeBasedSerializers.Add(type, serializer);
            return serializer;
        }
    }
}

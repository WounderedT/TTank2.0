using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Serialization;

namespace TPresenter.Game.Builders
{
    //This class is currently using XML for serialization of objects. It SHOULD BE replaced with something more lightweight.
    public class EntityBuilderSerializer
    {
        public static void Serialize(String path, Builder_Entity entityBuilder)
        {
            var serializer = XmlSerializerManager.GetOrCreateSerializer(entityBuilder.GetType());
        }

        public static T Deserialize<T>(String path) where T : Builder_Entity
        {
            var serializer = XmlSerializerManager.GetOrCreateSerializer(typeof(T));
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                return (T)serializer.Deserialize(stream);
            }
        }

        public static T Copy<T>(T entityBuilder) where T: Builder_Entity
        {
            var serializer = XmlSerializerManager.GetOrCreateSerializer(entityBuilder.GetType());
            using (MemoryStream memoryStream = new MemoryStream())
            {
                serializer.Serialize(memoryStream, entityBuilder);
                memoryStream.Position = 0;
                return (T)serializer.Deserialize(memoryStream);
            }
        }

    }
}

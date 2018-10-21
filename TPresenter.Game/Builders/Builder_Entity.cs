using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using TPresenter.Filesystem;
using TPresenter.Game.Interfaces;
using TPresenter.Serialization;

namespace TPresenter.Game.Builders
{
    [Serializable]
    public class BuilderEntityCollection<T> where T: Builder_Entity
    {
        public List<T> BuilderEntityList { get; set; } = new List<T>();
    }

    [Serializable]
    [XmlInclude(typeof(Builder_ObjectEntity))]
    public class Builder_Entity
    {
        public EntityFlags EntityFlags { get; set; }
        public StringId Id { get; set; }
        public String Name { get; set; }


        public virtual void Serialize(String path)
        {
            EntityBuilderSerializer.Serialize(path, this);
        }

        public virtual Builder_Entity Copy()
        {
            return EntityBuilderSerializer.Copy(this);
        }

        public static void WriteBuilderDefinitions<T>(BuilderEntityCollection<T> entityCollection) where T : Builder_Entity
        {
            XmlSerializer serializer = XmlSerializerManager.GetOrCreateSerializer(typeof(BuilderEntityCollection<T>));
            using (FileStream fileStream = new FileStream(Path.Combine(FileProvider.ContentPath, typeof(T).Name + "Definitions.xml"), FileMode.Create))
            using (XmlTextWriter xmlTextWriter = new XmlTextWriter(fileStream, Encoding.Unicode))
            {
                xmlTextWriter.Formatting = Formatting.Indented;
                xmlTextWriter.Indentation = 4;
                serializer.Serialize(xmlTextWriter, entityCollection);
            }
        }

        public static Dictionary<StringId, Builder_Entity> LoadBuilderDefinitions<T>() where T : Builder_Entity
        {
            Dictionary<StringId, Builder_Entity> collection = new Dictionary<StringId, Builder_Entity>(StringId.Comparer);
            String definitionsPath = Path.Combine(FileProvider.ContentPath, typeof(T).Name + "Definitions.xml");
            if (File.Exists(definitionsPath))
            {
                using (XmlTextReader xReader = new XmlTextReader(definitionsPath))
                {
                    var serializer = XmlSerializerManager.GetOrCreateSerializer(typeof(BuilderEntityCollection<T>));
                    var result = serializer.Deserialize(xReader) as BuilderEntityCollection<T>;
                    foreach(T entry in result.BuilderEntityList)
                    {
                        collection.Add(entry.Id, entry);
                    }
                }
            }
            return collection;
        }
    }
}

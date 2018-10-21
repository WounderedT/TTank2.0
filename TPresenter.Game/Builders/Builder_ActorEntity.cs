using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TPresenter.Game.Builders
{
    [Serializable]
    public class Builder_ActorEntity: Builder_SkinnedEntity
    {
        [XmlArrayItem("ModelId")]
        public List<StringId> ModelParts { get; set; }

        public static void WriteBuilderDefinitions(BuilderEntityCollection<Builder_ActorEntity> entityCollection)
        {
            WriteBuilderDefinitions<Builder_ActorEntity>(entityCollection);
        }

        public static new Dictionary<StringId, Builder_Entity> LoadBuilderDefinitions()
        {
            return LoadBuilderDefinitions<Builder_ActorEntity>();
        }
    }
}

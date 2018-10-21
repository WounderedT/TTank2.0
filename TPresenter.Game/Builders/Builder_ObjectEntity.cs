using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace TPresenter.Game.Builders
{
    [Serializable]
    public class Builder_ObjectEntity: Builder_Entity
    {
        public StringId ModelId { get; set; }
        public Matrix WorldMatrix { get; set; } = Matrix.Identity;

        public static void WriteBuilderDefinitions(BuilderEntityCollection<Builder_ObjectEntity> entityCollection)
        {
            WriteBuilderDefinitions<Builder_ObjectEntity>(entityCollection);
        }

        public static Dictionary<StringId, Builder_Entity> LoadBuilderDefinitions()
        {
            return LoadBuilderDefinitions<Builder_ObjectEntity>();
        }
    }
}

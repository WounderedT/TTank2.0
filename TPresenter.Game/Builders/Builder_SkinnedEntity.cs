using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Game.Builders
{
    [Serializable]
    public class Builder_SkinnedEntity: Builder_Entity
    {
        public StringId SkeletonId { get; set; }
        public Matrix WorldMatrix { get; set; } = Matrix.Identity;

        public static void WriteBuilderDefinitions(BuilderEntityCollection<Builder_SkinnedEntity> entityCollection)
        {
            WriteBuilderDefinitions<Builder_SkinnedEntity>(entityCollection);
        }

        public static Dictionary<StringId, Builder_Entity> LoadBuilderDefinitions()
        {
            return LoadBuilderDefinitions<Builder_SkinnedEntity>();
        }
    }
}

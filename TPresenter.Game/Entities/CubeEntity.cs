using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Game.Builders;

namespace TPresenter.Game.Entities
{
    [EntityBuilderType(typeof(Builder_CubeEntity))]
    public class CubeEntity: Entity
    {
        public Dictionary<StringId, Entity> Objects { get; set; } = new Dictionary<StringId, Entity>(StringId.Comparer);

        public override void Init(Builder_Entity builder)
        {
            base.Init(builder);
            Builder_CubeEntity builderCell = builder as Builder_CubeEntity;
            WorldMatrix = builderCell.WorldMatrix;
            foreach(Builder_CubeEntity_ObjectEntry entry in builderCell.Objects)
            {
                var entity = Factory_Enitty.CreateInstance(entry.Id);
                entity.Init(Factory_Enitty.GetBuilder(entry.Id));
                Matrix newWorld;
                Matrix mEntry = entry.Transformations;
                Matrix.Multiply(ref mEntry, ref entity.WorldMatrix, out newWorld);
                entity.SetWorldMatrix(newWorld);
                Objects.Add(entity.Id, entity);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Game.Builders;
using TPresenter.Game.Interfaces;
using TPresenter.Render.GeometryStage.Model;
using TPresenter.Render.Messages;
using TPresenter.Render.RenderProxy;

namespace TPresenter.Game.Entities
{
    [EntityBuilderTypeAttribute(typeof(Builder_ObjectEntity))]
    public class ObjectEntity: Entity, IObjectEntity
    {
        public bool SkipIfTooSmall
        {
            get { return (Flags & EntityFlags.SkipIfTooSmall) != 0; }
            set
            {
                if (value)
                    Flags |= EntityFlags.SkipIfTooSmall;
                else
                    Flags &= ~EntityFlags.SkipIfTooSmall;
            }
        }

        public bool Visible
        {
            get { return (Flags & EntityFlags.Visible) != 0; }
            set
            {
                if (value)
                    Flags |= EntityFlags.Visible;
                else
                    Flags &= ~EntityFlags.Visible;
            }
        }

        public MyModel Model { get; private set; }

        //this will be made private in future
        public ObjectEntity() { }

        public ObjectEntity(MyModel model, IEntity parent = null, string name = "", EntityFlags flags = EntityFlags.Default) : base(parent, name, flags)
        {
            Model = model;
            Name = model.Name;
        }

        public override void Init(Builder_Entity builderEntity)
        {
            base.Init(builderEntity);

            var builder = builderEntity as Builder_ObjectEntity;
            Model = ModelManager.GetOrLoadModel(builder.ModelId);
            WorldMatrix = builder.WorldMatrix * Model.Parts[0].BindShapeMatrix;
        }

        public virtual void Draw()
        {
            RenderMessageSetRenderInstance message = new RenderMessageSetRenderInstance();
            message.Model = Model;
            message.WorldMatrix = WorldMatrix;
            MyRenderProxy.render.MessageQueue.Enqueue(message);
        }
    }
}

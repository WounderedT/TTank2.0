using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Game.Builders;
using TPresenter.Game.Interfaces;
using TPresenter.Render.Animation;
using TPresenter.Render.GeometryStage.Model;
using TPresenter.Render.Resources;

namespace TPresenter.Game.Entities
{
    [EntityBuilderTypeAttribute(typeof(Builder_SkinnedEntity))]
    public class SkinnedEntity : Entity, IDrawableEntity
    {
        private AnimationController _animationController = new AnimationController();

        public AnimationController AnimationController { get { return _animationController; } }

        public Matrix[] BoneTransformation { get { return _animationController.BoneTransformation; } }
        public Skeleton Skeleton { get; set; }

        public List<int> BoneParents;

        public override void Init(Builder_Entity builderEntity)
        {
            base.Init(builderEntity);

            var builder = builderEntity as Builder_SkinnedEntity;
            Skeleton = ModelManager.GetOrLoadSkeleton(builder.SkeletonId);
            _animationController.InitCharacterPose(Skeleton);
        }

        public void PLayAnimation(StringId name)
        {
            _animationController.PlayAnimation(Skeleton, name);
        }

        public virtual void Draw()
        {
            
        }
    }
}

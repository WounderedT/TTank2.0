using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Game.Interfaces;
using TPresenter.Render.Animation;
using TPresenter.Render.GeometryStage.Model;
using TPresenter.Render.Resources;

namespace TPresenter.Game.Entity
{
    public class MySkinnedEntity : MyEntity
    {
        private AnimationController animationController = new AnimationController();

        public AnimationController AnimationController { get { return animationController; } }

        public Matrix[] BoneTransformation { get { return animationController.BoneTransformation; } }
        public Skeleton Skeleton { get; set; }

        public List<int> BoneParents;

        //public MySkinnedEntity(MyModel model) : base(model)
        //{
        //    //ResolveBones();
        //    animationController.InitCharacterPose(this.Model);
        //}

        public void PLayAnimation(string name)
        {
            animationController.PlayAnimation(this.Skeleton, name);
        }

        //void ResolveBones()
        //{
        //    if(Parent is MySkinnedEntity)
        //    {
        //        var toResolve = Model.Parts[0].Bones.Where(w => w.ParentIndex == -1).ToList();
        //        BoneParents = new List<int>(toResolve.Count);
        //        foreach (Bone bone in toResolve)
        //        {
        //            for (int i = 0; i < Parent.Model.Parts[0].Bones.Count; i++)
        //            {
        //                if (Parent.Model.Parts[0].Bones[i].BoneName.Equals(bone.BoneName))
        //                    BoneParents.Add(i);
        //            }
        //        }
        //    }
        //}
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using TPresenter.Game.Entities;
using TPresenter.Game.Interfaces;
using TPresenter.Game.Models;
using TPresenter.Game.Utils;
using TPresenter.Render.GeometryStage.Model;
using TPresenter.Utils;
using TPresenterMath;
using TPresenter.Render.RenderProxy;
using TPresenter.Render.Messages;
using TPresenter.Render.Resources;
using TPresenter.Game.Builders;

namespace TPresenter.Game.Actor
{
    [EntityBuilderType(typeof(Builder_ActorEntity))]
    public class ActorEntity : SkinnedEntity, ICameraController
    {
        bool isFirstPerson = false; //  default should be true. Debug only.
        bool forceFirstPerson = false; //  default should be true. Debug only.

        //public List<MySkinnedEntity> Parts = new List<MySkinnedEntity>(7);
        public List<MyModel> Parts = new List<MyModel>(7);

        public bool IsInFirstPersionView
        {
            get { return isFirstPerson; }
            set { isFirstPerson = value; }
        }
        public bool ForceFirstPersion
        {
            get { return forceFirstPerson; }
            set { forceFirstPerson = value; }
        }

        public override void Init(Builder_Entity builderEntity)
        {
            base.Init(builderEntity);

            var builder = builderEntity as Builder_ActorEntity;

            foreach(StringId modelPart in builder.ModelParts)
            {
                Parts.Add(ModelManager.GetOrLoadSkinnedModel(modelPart, Skeleton));
            }
        }
        
        public override void Update(long timeStamp)
        {
            //Vector3 defaut = new Vector3(0, 0, -20);
            //Vector3 max = new Vector3(0, 0, 20);
            //if(WorldMatrix.TranslationVector.Z > max.Z)
            //{
            //    WorldMatrix.TranslationVector = defaut;
            //}
            //else
            //{
            //    Vector3 vector = WorldMatrix.TranslationVector;
            //     vector.Z += 0.0249f;
            //    WorldMatrix.TranslationVector = vector;
            //}
            //PLayAnimation("skyrim-anim-idle");
            //PLayAnimation("skyrim-anim-fm-walkforward");
            PLayAnimation(StringId.GetOrCompute(@"character\bruxa\skyrim-anim-xpms-fm-walkforward"));
        }

        public override void Draw()
        {
            foreach(MyModel model in Parts)
            {
                RenderMessageSetRenderInstanceSkinned message = new RenderMessageSetRenderInstanceSkinned();
                message.Model = model;
                message.SkinMatrices = new Matrix[AnimationController.BoneTransformation.Length];
                for (var i = 0; i < AnimationController.BoneTransformation.Length; i++)
                {
                    ModelBone bone = new ModelBone();
                    if(model.Parts[0].ModelBones.TryGetValue(Skeleton.Bones[i].BoneId, out bone))
                        message.SkinMatrices[i] = Matrix.Transpose(model.Parts[0].BindShapeMatrix * bone.InvBindPose * AnimationController.BoneTransformation[i]);
                    else
                        message.SkinMatrices[i] = Matrix.Transpose(model.Parts[0].BindShapeMatrix * AnimationController.BoneTransformation[i]);
                }
                message.WorldMatrix = WorldMatrix;
                MyRenderProxy.render.MessageQueue.Enqueue(message);
            }
        }

        public void ControlCamera(Camera currentCamera)
        {
            throw new NotImplementedException();
        }

        public bool HandlePickUp()
        {
            throw new NotImplementedException();
        }

        public bool HandleUse()
        {
            throw new NotImplementedException();
        }

        public void OnAssumeControl(ICameraController previousCameraController)
        {
            throw new NotImplementedException();
        }

        public void OnReleaseControl(ICameraController newCameraController)
        {
            throw new NotImplementedException();
        }

        public void Rotate(Vector2 rotationIndicator, float rollIndicator)
        {
            throw new NotImplementedException();
        }

        public void RotateStopped()
        {
            throw new NotImplementedException();
        }
    }
}

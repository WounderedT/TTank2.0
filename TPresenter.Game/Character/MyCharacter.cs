using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using TPresenter.Game.Components;
using TPresenter.Game.Entity;
using TPresenter.Game.Interfaces;
using TPresenter.Game.Models;
using TPresenter.Game.Utils;
using TPresenter.Render.GeometryStage.Model;
using TPresenter.Utils;
using TPresenterMath;
using TPresenter.Render.RenderProxy;
using TPresenter.Render.Messages;
using TPresenter.Render.Resources;

namespace TPresenter.Game.Character
{
    public class MyCharacter : MySkinnedEntity, ICameraController
    {
        //public MyModel Model { get; set; } //  this should not be here. MyModel reference should be removed from the IMyEntity!

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

        bool closed = false;
        Matrix localMatrix = Matrix.Identity;
        Matrix worldMatrix = Matrix.Identity;

        //public MyEntityFlags Flags { get; set; }
        //public long EntityId { get; set; }
        //public string Name { get; set; }

        //public bool Closed { get { return closed; } }

        //public bool Save
        //{
        //    get { return (Flags & MyEntityFlags.Save) != 0; }
        //    set
        //    {
        //        if (value)
        //            Flags |= MyEntityFlags.Save;
        //        else
        //            Flags &= ~MyEntityFlags.Save;
        //    }
        //}
        //public MyEntityUpdateEnum NeedsUpdate { get; set; }

        //public IMyEntity Parent { get; }

        //public bool NeedsDraw
        //{
        //    get { return (Flags & MyEntityFlags.NeedsDraw) != 0; }
        //    set
        //    {
        //        if (value)
        //            Flags |= MyEntityFlags.NeedsDraw;
        //        else
        //            Flags &= ~MyEntityFlags.NeedsDraw;
        //    }
        //}
        //public bool SkipIfTooSmall
        //{
        //    get { return (Flags & MyEntityFlags.SkipIfTooSmall) != 0; }
        //    set
        //    {
        //        if (value)
        //            Flags |= MyEntityFlags.SkipIfTooSmall;
        //        else
        //            Flags &= ~MyEntityFlags.SkipIfTooSmall;
        //    }
        //}
        //public bool Visible
        //{
        //    get { return (Flags & MyEntityFlags.Visible) != 0; }
        //    set
        //    {
        //        if (value)
        //            Flags |= MyEntityFlags.Visible;
        //        else
        //            Flags &= ~MyEntityFlags.Visible;
        //    }
        //}
        //public bool InvalidateOnMove
        //{
        //    get { return (Flags & MyEntityFlags.InvalidateOnMove) != 0; }
        //    set
        //    {
        //        if (value)
        //            Flags |= MyEntityFlags.InvalidateOnMove;
        //        else
        //            Flags &= ~MyEntityFlags.InvalidateOnMove;
        //    }
        //}

        //public Matrix LocalMatrix
        //{
        //    get { return localMatrix; }
        //    set { localMatrix = value; }
        //}
        //public Matrix WorldMatrix
        //{
        //    get { return worldMatrix; }
        //    set { worldMatrix = value; }
        //}
        //public Matrix WorldMatrixInvScaled { get { return Matrix.Invert(WorldMatrix); } }
        //public Matrix WorldMatrixNormalizedInv { get { return Matrix.Invert(Matrix_T.Normalize(WorldMatrix)); } }

        //public BoundingBox LocalAABB { get; set; }
        //public BoundingSphere LocalVolume { get; set; }
        //public Vector3 LocalVolumeOffset { get; set; }

        //public event Action<IMyEntity> OnClose;
        //public event Action<IMyEntity> OnClosing;
        //public event Action<IMyEntity> OnMarkForClose;
        //public event Action<IMyEntity> OnPhysicsChanged;

        // This method should recieve full character configurations
        public void Init()
        {
            NeedsUpdate = MyEntityUpdateEnum.EACH_FRAME;
            //Skeleton = ModelManager.GetOrLoadSkeleton("skyrim-skeleton_female");
            Skeleton = ModelManager.GetOrLoadSkeleton("skyrim-xpms-skeleton_female");
            //Parts.Add(new MySkinnedEntity(ModelManager.GetOrLoadModel("characters\\female\\female_body"), this));
            //Parts.Add(new MySkinnedEntity(ModelManager.GetOrLoadModel("characters\\female\\female_feet"), Parts[0]));
            //Parts.Add(new MySkinnedEntity(ModelManager.GetOrLoadModel("characters\\female\\female_hands"), Parts[0]));
            //Parts.Add(new MySkinnedEntity(ModelManager.GetOrLoadModel("characters\\female\\female_head_default"), Parts[0]));
            //Parts.Add(new MySkinnedEntity(ModelManager.GetOrLoadModel("characters\\female\\femaleeyes\\01"), Parts[0]));
            //Parts.Add(new MySkinnedEntity(ModelManager.GetOrLoadModel("characters\\female\\femalehair\\92"), Parts[0]));
            //ColladaModel model = new ColladaModel("body");
            //Parts.Add(new MySkinnedEntity(ModelManager.GetOrLoadModel("body"), this));
            //Parts.Add(new MySkinnedEntity(ModelManager.GetOrLoadModel("head"), this));
            //Parts.Add(new MySkinnedEntity(ModelManager.GetOrLoadModel("hair"), this));
            //Parts.Add(new MySkinnedEntity(ModelManager.GetOrLoadModel("eyes"), this));
            //Parts.Add(new MySkinnedEntity(ModelManager.GetOrLoadModel("womanteeth"), this));
            //Parts.Add(ModelManager.GetOrLoadSkinnedModel("skyrim-body-joined-xpms", Skeleton));
            Parts.Add(ModelManager.GetOrLoadSkinnedModel("bruxa-xpms", Skeleton));
            //Parts.Add(ModelManager.GetOrLoadSkinnedModel("skyrim-xpms-head", Skeleton));
            //Parts.Add(ModelManager.GetOrLoadSkinnedModel("skyrim-092", Skeleton));
            //Parts.Add(ModelManager.GetOrLoadSkinnedModel("skyrim-08", Skeleton));
            //Parts.Add(ModelManager.GetOrLoadSkinnedModel("skyrim-xpms-hands", Skeleton));
            //Parts.Add(ModelManager.GetOrLoadSkinnedModel("skyrim-xpms-feet", Skeleton));
            //Parts.Add(ModelManager.GetOrLoadSkinnedModel("bruxa-xpms", Skeleton));
            //Parts.Add(ModelManager.GetOrLoadSkinnedModel("skyrim-body", Skeleton));
            //Parts.Add(ModelManager.GetOrLoadSkinnedModel("skyrim-head", Skeleton));
            //Parts.Add(ModelManager.GetOrLoadSkinnedModel("skyrim-092", Skeleton));
            //Parts.Add(ModelManager.GetOrLoadSkinnedModel("skyrim-08", Skeleton));
            //Parts.Add(ModelManager.GetOrLoadSkinnedModel("skyrim-hands", Skeleton));
            //Parts.Add(ModelManager.GetOrLoadSkinnedModel("skyrim-feet", Skeleton));
            //Parts.Add(new MySkinnedEntity(ModelManager.GetOrLoadModel("5x5_testbox"), this));

            //Parts.Add(new MySkinnedEntity(ModelManager.GetOrLoadModel("characters\\bruxa\\body"), this));
            //Parts.Add(new MySkinnedEntity(ModelManager.GetOrLoadModel("characters\\bruxa\\femaleteeth"), this));
            //Parts.Add(new MySkinnedEntity(ModelManager.GetOrLoadModel("characters\\bruxa\\head\\head"), this));
            //Parts.Add(new MySkinnedEntity(ModelManager.GetOrLoadModel("characters\\bruxa\\eyes\\08"), this));
            //Parts.Add(new MySkinnedEntity(ModelManager.GetOrLoadModel("characters\\bruxa\\hair\\01"), this));

            worldMatrix = Matrix.Scaling(0.25f) * Matrix_T.RotationX(90);
            AnimationController.InitCharacterPose(Skeleton);
            //worldMatrix = Matrix.Scaling(0.025f);
        }
        
        public void Update(long timeStamp)
        {
            //PLayAnimation("skyrim-anim-idle");
            //PLayAnimation("skyrim-anim-fm-walkforward");
            PLayAnimation("skyrim-anim-xpms-fm-walkforward");
        }

        public void Draw()
        {
            foreach(MyModel model in Parts)
            {
                RenderMessageSetRenderInstanceSkinned message = new RenderMessageSetRenderInstanceSkinned();
                message.Model = model;
                message.SkinMatrices = new Matrix[AnimationController.BoneTransformation.Length];
                for (var i = 0; i < AnimationController.BoneTransformation.Length; i++)
                {
                    ModelBone bone = new ModelBone();
                    if(model.Parts[0].ModelBones.TryGetValue(Skeleton.Bones[i].BoneName, out bone))
                        message.SkinMatrices[i] = Matrix.Transpose(model.Parts[0].BindShapeMatrix * bone.InvBindPose * AnimationController.BoneTransformation[i]);
                    else
                        message.SkinMatrices[i] = Matrix.Transpose(model.Parts[0].BindShapeMatrix * AnimationController.BoneTransformation[i]);
                }
                message.WorldMatrix = worldMatrix;
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

        //bool IMyEntity.MarkedForClose()
        //{
        //    throw new NotImplementedException();
        //}

        //string IMyEntity.GetFrendlyName()
        //{
        //    throw new NotImplementedException();
        //}

        //void IMyEntity.Close()
        //{
        //    throw new NotImplementedException();
        //}

        //void IMyEntity.Delete()
        //{
        //    throw new NotImplementedException();
        //}

        //IMyEntity IMyEntity.GetTopMostParent(Type type)
        //{
        //    throw new NotImplementedException();
        //}

        //void IMyEntity.SetLocalMatrix(Matrix localMatrix)
        //{
        //    throw new NotImplementedException();
        //}

        //void IMyEntity.GetChildren(List<IMyEntity> children, Func<IMyEntity, bool> collect)
        //{
        //    throw new NotImplementedException();
        //}

        //void IMyEntity.DebugDraw()
        //{
        //    throw new NotImplementedException();
        //}

        //void IMyEntity.DebugDrawInvalidTriangles()
        //{
        //    throw new NotImplementedException();
        //}

        //float IMyEntity.GetDistanceBetweenCameraAndBoundingSphere()
        //{
        //    throw new NotImplementedException();
        //}

        //float IMyEntity.GetDistanceBetweenCameraAndPosition()
        //{
        //    throw new NotImplementedException();
        //}

        //float IMyEntity.GetLargestDistanceBetweenCameraAndBoundingSphere()
        //{
        //    throw new NotImplementedException();
        //}

        //float IMyEntity.GetSmallestDistanceBetweenCameraAndBoundingSphere()
        //{
        //    throw new NotImplementedException();
        //}

        //Matrix IMyEntity.GetViewMatrix()
        //{
        //    throw new NotImplementedException();
        //}

        //Matrix IMyEntity.GetWorldMatrixNormalizedInv()
        //{
        //    throw new NotImplementedException();
        //}

        //void IMyEntity.SetWorldMatrix(Matrix worldMatrix)
        //{
        //    throw new NotImplementedException();
        //}

        //void IMyEntity.SetPosition(Vector3 position)
        //{
        //    throw new NotImplementedException();
        //}

        //bool IMyEntity.GetIntersectionWithLine(ref LineD line, out IntersectionResultLineTriangle? triangle, IntersectionFlags flags)
        //{
        //    throw new NotImplementedException();
        //}

        //Vector3? IMyEntity.GetIntersectionWithLineAndBoundingSphere(ref LineD line, float boudingSphereRadiusMultiplier)
        //{
        //    throw new NotImplementedException();
        //}

        //bool IMyEntity.GetIntersectionWithSphere(ref BoundingSphere sphere)
        //{
        //    throw new NotImplementedException();
        //}

        //void IMyEntity.GetTrianglesIntersectionSphere(ref BoundingSphere sphere, Vector3? referenceNormalVector, float? maxAngle, List<TriangleVertexNormals> retTriangles, int maxNeighbourTriangles)
        //{
        //    throw new NotImplementedException();
        //}

        //bool IMyEntity.DoOverlapSphereTest(float sphereRadius, Vector3 spherePos)
        //{
        //    throw new NotImplementedException();
        //}
    }
}

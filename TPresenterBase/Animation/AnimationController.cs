using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Render.GeometryStage.Model;
using TPresenterMath;

namespace TPresenter.Render.Animation
{
    public class AnimationController
    {
        Matrix[] boneTransformation;
        StringId currentAnimationId;
        MyAnimation currentAnim;
        System.Diagnostics.Stopwatch clock = new System.Diagnostics.Stopwatch();

        public Matrix[] BoneTransformation { get { return boneTransformation; } }

        public void InitCharacterPose(Skeleton skeleton)
        {
            boneTransformation = new Matrix[skeleton.Bones.Count];
            for (var i = 0; i < boneTransformation.Length; i++)
            {
                boneTransformation[i] = skeleton.Bones[i].BoneLocalTransform;
            }
            // Apply parent bone transforms
            // We assume here that the first bone has no parent
            // and that each parent bone appears before children
            for (var i = 1; i < boneTransformation.Length; i++)
            {
                var bone = skeleton.Bones[i];
                // ParentIndex == -1 means root bone
                if (bone.ParentIndex > -1)
                {
                    var parentTransform = boneTransformation[bone.ParentIndex];
                    boneTransformation[i] = (boneTransformation[i] * parentTransform);
                }
            }
            //for (var i = 0; i < boneTransformation.Length; i++)
            //{
            //    //boneTransformation[i] = Matrix.Transpose(skeleton.BindShapeMatrix * skeleton.Bones[i].InvBindPose * boneTransformation[i]);
            //    boneTransformation[i] = Matrix.Transpose(skeleton.Bones[i].InvBindPose * boneTransformation[i]);
            //}
        }

        public void PlayAnimation(Skeleton skeleton, StringId animationName)
        {
            if (string.IsNullOrEmpty(currentAnimationId.String))
                clock.Start();

            if (String.IsNullOrEmpty(currentAnimationId.String))
            {
                currentAnimationId = animationName;
                currentAnim = AnimationManager.GetOrLoadAnimation(animationName);
            }
                

            //  If requested animation is new we must either merge it with current animation (e.g. weapon aming, forward-side walking) or replace current animation with it.
            if (currentAnimationId != animationName)
            {

            }

            for (var i = 0; i < boneTransformation.Length; i++)
            {
                boneTransformation[i] = skeleton.Bones[i].BoneLocalTransform;
            }

            var time = clock.ElapsedMilliseconds / 1000.0f;

            foreach(var jointAnim in currentAnim.JointAnimations)
            {
                int jointIndex = skeleton.ListIndexByBoneId[jointAnim.jointName];
                boneTransformation[jointIndex] = jointAnim.keyFrames[jointAnim.keyFrames.Length - 1].Transoformation;
                for (var i = 0; i < jointAnim.keyFrames.Length; i++)
                {
                    if (jointAnim.keyFrames[i].timestamp > time)
                    {
                        var frame = jointAnim.keyFrames[i];
                        if (i == 0)
                        {
                            boneTransformation[jointIndex] = frame.Transoformation;
                        }
                        else
                        {
                            KeyFrame previousKeyFrame = jointAnim.keyFrames[i - 1];
                            float amount = (float)((time - previousKeyFrame.timestamp) / (frame.timestamp - previousKeyFrame.timestamp));

                            boneTransformation[jointIndex] = Matrix.Scaling(MathHelper.Lerp(previousKeyFrame.scaling, frame.scaling, amount))
                                * Matrix.RotationQuaternion(Quaternion.Slerp(previousKeyFrame.rotation, frame.rotation, amount))
                                * Matrix.Translation(MathHelper.Lerp(previousKeyFrame.position, frame.position, amount));
                        }
                        break;
                    }
                }
            }

            for (var i = 1; i < boneTransformation.Length; i++)
            {
                var bone = skeleton.Bones[i];
                if (bone.ParentIndex > -1)
                {
                    var parentTransform = boneTransformation[bone.ParentIndex];
                    boneTransformation[i] = (boneTransformation[i] * parentTransform);
                }
            }
            //for (var i = 0; i < boneTransformation.Length; i++)
            //{
            //    //boneTransformation[i] = Matrix.Transpose(skeleton.BindShapeMatrix * skeleton.Bones[i].InvBindPose * boneTransformation[i]);
            //    boneTransformation[i] = Matrix.Transpose(skeleton.Bones[i].InvBindPose * boneTransformation[i]);
            //}
            if (currentAnim.JointAnimations[0].keyFrames.Last().timestamp <= time)
                clock.Restart();
        }
    }
}

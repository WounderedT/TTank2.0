using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using TPresenter.Game.Interfaces;
using TPresenterMath;
using TPresenter.Utils;
using TPresenter.Render.RenderProxy;
using TTank20Game.Game.Platform;

namespace TPresenter.Game.Utils
{
    public class Camera : ICamera
    {
        #region Camera Constants
        //Default far plane distance
        public const float DefaultFarPlaneDistance = 20000;

        //Near clip plane for "near" objects, near objects are cockpit, cockpit glass and weapons
        private const float NearForNearObjects = 0.1f;

        //Far clip plane for "near" objects, near objects are cockpit, cockpit glass and weapons
        private const float FarForNearObjects = 100.0f;

        #endregion

        #region Camera Properties

        public float NearPlaneDistance = 0.05f;

        public float FarPlaneDistance = DefaultFarPlaneDistance;

        public float FieldOfView;

        public Vector3 PreviousPosition;

        public ViewportF Viewport;                                  //Current viewport.

        public Matrix WorldMatrix = Matrix.Identity;                //World matrix is cached inversion of view matrix.
        public Matrix ViewMatrix = Matrix.Identity;                 //This is view matrix when camera in real position.
        public Matrix ProjectionMatrix = Matrix.Identity;           
        public Matrix ProjectionMatrixFar = Matrix.Identity;        //Projection matrix for far objects.
        public Matrix ViewProjectionMatrix = Matrix.Identity;       //View-projection matrix when camera in real position.
        public Matrix ViewProjectionMatrixFar = Matrix.Identity;    //View-projection matrix for far objects when camera in real position.

        public BoundingBox boundingBox;                             //Bounding box calculated from bounding frustum, updated every draw.
        public BoundingSphere boundingSphere;                       //Bounding sphere calculated from bounding frustum, update every draw.

        public BoundingFrustum BoundingFrustrum = new BoundingFrustum(Matrix.Identity);
        public BoundingFrustum BoundingFrustrumFar = new BoundingFrustum(Matrix.Identity);

        public CameraZoomProperties Zoom;

        public float AspectRatio { get; private set; }

        /// <summary>
        /// Member that shakes with the camera.
        /// </summary>
        public readonly CameraShake CameraShake = new CameraShake();

        /// <summary>
        /// Member that implements camera spring.
        /// </summary>
        //public readonly CameraSpring CameraSpring = new CameraSpring();

        /// <summary>
        /// Current view matrix without translation part.
        /// </summary>
        public Matrix VeiwMatrixAtZero
        {
            get
            {
                Matrix rtnMatrix = ViewProjectionMatrix;
                rtnMatrix.M14 = 0;
                rtnMatrix.M24 = 0;
                rtnMatrix.M34 = 0;
                rtnMatrix.M41 = 0;
                rtnMatrix.M42 = 0;
                rtnMatrix.M43 = 0;
                rtnMatrix.M44 = 1;
                return rtnMatrix;
            }
        }

        /// <summary>
        /// Forward vector of camera world matrix.
        /// </summary>
        public Vector3 ForwardVector
        {
            get { return WorldMatrix.Forward; }
        }

        /// <summary>
        /// Left vector of camera world matrix.
        /// </summary>
        public Vector3 LeftVector
        {
            get { return WorldMatrix.Left; }
        }

        /// <summary>
        /// Up vector of camera world matrix
        /// </summary>
        public Vector3 UpVector
        {
            get { return WorldMatrix.Up; }
        }

        /// <summary>
        /// Field of view in degrees
        /// </summary>
        public float FieldOfViewDegree
        {
            get { return MathHelper.ToDegrees(FieldOfView); }
            set { FieldOfView = MathHelper.ToRadians(value); }
        }

        /// <summary>
        /// Field of view with zoom enabled.
        /// </summary>
        public  float FovWithZoom
        {
            get { return Zoom.GetFOV(); }
        }

        /// <summary>
        /// Get position of the camera.
        /// </summary>
        public Vector3 Position
        {
            get { return WorldMatrix.TranslationVector; }
        }

        public Vector3 PreviousPostition
        {
            get { return PreviousPosition; }
        }

        public Vector2 ViewportOffset
        {
            get { return new Vector2(Viewport.X, Viewport.Y); }
        }

        public Vector2 VewportSize
        {
            get { return new Vector2(Viewport.Width, Viewport.Height); }
        }

        Matrix ICamera.ViewMatrix
        {
            get { return ViewMatrix; }
        }

        Matrix ICamera.WorldMatrix
        {
            get { return WorldMatrix; }
        }

        Matrix ICamera.ProjectionMatrix
        {
            get { return ProjectionMatrix; }
        }

        float ICamera.NearPlaneDistance
        {
            get { return NearPlaneDistance; }
        }

        float ICamera.FarPlaneDistance
        {
            get { return FarPlaneDistance; }
        }

        public float NearForNearObject
        {
            get { return NearForNearObjects; }
        }

        public float FarForNearObject
        {
            get { return FarForNearObjects; }
        }

        public float FieldOfViewAngle
        {
            get { return FieldOfViewDegree; }
        }

        #endregion

        public Camera(float fieldOfView, ViewportF currentSceneViewport)
        {
            FieldOfView = fieldOfView;
            Zoom = new CameraZoomProperties(this);
            UpdateScreenSize(currentSceneViewport);
        }

        public void UpdateScreenSize(ViewportF currentViewport)
        {
            Viewport = currentViewport;
            PreviousPosition = Vector3.Zero;
            BoundingFrustrum = new BoundingFrustum(Matrix.Identity);
            AspectRatio = Viewport.Width / Viewport.Height;
        }

        public void Update(float updateStepTime)
        {
            Zoom.Update(updateStepTime);

            Vector3 newCameraPosOffset = Vector3.Zero;
            if (CameraShake.ShakeEnabled)
            {
                Vector3 shakePos, shakeDir;
                CameraShake.UpdateShake(updateStepTime, out shakePos, out shakeDir);
                newCameraPosOffset += shakePos;
            }

            if (newCameraPosOffset != Vector3.Zero)
            {
                Vector3 newCameraPosOffsetRotated;
                Vector3D.Rotate(ref newCameraPosOffset, ref ViewMatrix, out newCameraPosOffsetRotated);
                ViewMatrix.TranslationVector = newCameraPosOffsetRotated;
            }

            UpdatePropertiesInternal(ViewMatrix);
        }

        public void SetViewMatrix(Matrix newViewMatrix)
        {
            PreviousPosition = Position;
            UpdatePropertiesInternal(newViewMatrix);

        }

        public void SetViewMatrixToRender()
        {
            MyRenderProxy.SetCameraViewMatrix(ViewMatrix, ProjectionMatrix, GetSafeNear(), Zoom.GetFOV(), NearPlaneDistance,
                FarPlaneDistance, NearForNearObjects, FarForNearObjects, Position);
        }

        private void UpdatePropertiesInternal(Matrix viewMatrix)
        {
            ViewMatrix = viewMatrix;
            Matrix.Invert(ref ViewMatrix, out WorldMatrix);

            ProjectionMatrix = MatrixD.CreatePerspectiveFieldOfViewLh(FovWithZoom, AspectRatio, GetSafeNear(), FarPlaneDistance);
            ProjectionMatrixFar = MatrixD.CreatePerspectiveFieldOfViewLh(FovWithZoom, AspectRatio, GetSafeNear(), 100000);
            //ProjectionMatrix = MatrixD.CreatePerspectiveFovLh(FovWithZoom, AspectRatio, GetSafeNear(), FarPlaneDistance);
            //ProjectionMatrixFar = MatrixD.CreatePerspectiveFovLh(FovWithZoom, AspectRatio, GetSafeNear(), 100000);
        
            ViewProjectionMatrix = ViewMatrix * ProjectionMatrix;
            ViewProjectionMatrixFar = ViewMatrix * ProjectionMatrixFar;

            UpdateBoundingFrustum();
        }

        float GetSafeNear()
        {
            return Math.Min(4, NearPlaneDistance);
        }

        void UpdateBoundingFrustum()
        {
            BoundingFrustrum.Matrix = ViewProjectionMatrix;
            BoundingFrustrumFar.Matrix = ViewProjectionMatrixFar;

            boundingBox = BoundingBoxD.CreateInvalid();
            boundingBox.Include(ref BoundingFrustrum);

            boundingSphere = MyUtils.GetBoundingSphereFromBoundingBox(ref boundingBox);
        }

        /// <summary>
        /// Checks if specified bouding sphere is in actual bounding frustum
        /// </summary>
        /// <returns></returns>
        public bool IsInFrustum(ref BoundingSphere boundingSphere)
        {
            ContainmentType result;
            BoundingFrustrum.Contains(ref boundingSphere, out result);
            return result != ContainmentType.Disjoint;
        }

        public double GetDistanceFromPoint(Vector3 position)
        {
            return Vector3.Distance(Position, position);
        }

        /// <summary>
        /// Gets screen coordinates of 3d world pos in 0 - 1 distance where 1.0 is screen width (for X) or height (for Y).
        /// </summary>
        /// <param name="worldPos">World position.</param>
        /// <returns>Screen coordinate in 0-1 distance.</returns>
        public Vector3 WorldToScreen(ref Vector3 worldPos)
        {
            return Vector3D.Transform(worldPos, ViewProjectionMatrix);
        }

        /// <summary>
        /// Gets normalized world space line from screen space coordinates.
        /// </summary>
        /// <param name="screenCoordinates"></param>
        /// <returns></returns>
        public LineD WorldLineFromScreen(Vector2 screenCoordinates)
        {
            var matViewProjInv = Matrix.Invert(ViewProjectionMatrix);

            var raySource = new Vector4(
                (2.0f * screenCoordinates.X) / Viewport.Width - 1.0f,
                1.0f - (2.0f * screenCoordinates.Y) / Viewport.Height,
                0.0f,
                1.0f
                );

            var rayTarget = new Vector4(
                (2.0f * screenCoordinates.X) / Viewport.Width - 1.0f,
                1.0f - (2.0f * screenCoordinates.Y) / Viewport.Height,
                0.0f,
                1.0f
                );

            var raySourceWorld = Vector4.Transform(raySource, matViewProjInv);
            var rayTargetWorld = Vector4.Transform(rayTarget, matViewProjInv);

            raySourceWorld /= raySourceWorld.W;
            rayTargetWorld /= rayTargetWorld.W;

            return new LineD(new Vector3(raySourceWorld.X, raySourceWorld.Y, raySourceWorld.Z), new Vector3(rayTargetWorld.X, rayTargetWorld.Y, rayTargetWorld.Z));
        }

        public double GetDistanceWithFOV(Vector3 position)
        {
            throw new NotImplementedException();
        }

        public bool IsInFrustum(ref BoundingBox boundingBox)
        {
            throw new NotImplementedException();
        }

        public bool IsInFrustum(BoundingBox boundingBox)
        {
            throw new NotImplementedException();
        }

        public Vector3 WorldToScreen(ref Vector2 worldPos)
        {
            throw new NotImplementedException();
        }
    }
}

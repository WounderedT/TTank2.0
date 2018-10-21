using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Game.Components;
using TPresenter.Game.Entity;
using TPresenter.Game.Models;
using TPresenter.Render.GeometryStage.Model;
using TPresenter.Utils;
using TPresenterMath;

namespace TPresenter.Game.Interfaces
{
    [Flags]
    public enum MyEntityFlags
    {
        /// <summary>
        /// No flags
        /// </summary>
        None = 1 << 0,

        /// <summary>
        /// Specify whether this entity is visible or not.
        /// </summary>
        Visible = 1 << 1,

        /// <summary>
        /// Specify whether this entity should be saved or not.
        /// </summary>
        Save = 1 << 3,

        /// <summary>
        /// Specify whether this is "near" entity (e.g weapon) or not.
        /// </summary>
        Near = 1 << 4,

        /// <summary>
        /// 
        /// </summary>
        NeedsUpdate = 1 << 5,

        NeedsResolveCastShadow = 1 << 6,

        FastCastShadowResolve = 1 << 7,

        SkipIfTooSmall = 1 << 8,

        NeedsUpdate10 = 1 << 9,
        NeedsUpdate100 = 1 << 10, 

        /// <summary>
        /// Specify whether Draw method should be called for this entity.
        /// </summary>
        NeedsDraw = 1 << 11,

        /// <summary>
        /// Specify whether renderobjects should be invalidated if object is moved.
        /// </summary>
        InvalidateOnMove = 1 << 12,

        /// <summary>
        /// Specify whether Draw method should be called for this entity (call from parent only).
        /// </summary>
        NeeedsDrawFromParrent = 1 << 13,

        /// <summary>
        /// Specify whether LOD shadow should be drawn as a box.
        /// </summary>
        ShadowBoxLod = 1 << 14,

        /// <summary>
        /// Specify whether this entity should be rendered using dithering or simulated transparency.
        /// </summary>
        Transparent = 1 << 15,

        /// <summary>
        /// Specify whether entity should be updated before first frame.
        /// </summary>
        NeedsUpdateBeforeNextFrame = 1 << 16,

        DrawOutsideViewDistance = 1 << 17, 

        IsIngamePrunningStructureObject = 1 << 18,

        Default = MyEntityFlags.Visible | MyEntityFlags.SkipIfTooSmall | MyEntityFlags.Save | MyEntityFlags.NeedsResolveCastShadow | MyEntityFlags.InvalidateOnMove,
    }

    public enum MyEntityUpdateEnum
    {
        NONE = 0,               // no update
        EACH_FRAME = 1,         // each 0.016s, 60 FPS
        EACH_10TH_FRAME = 2,    // each 0.166s, 6 FPS
        EACH_100TH_FRAME = 3,   // each 1.666s, 0.6 FPS

        BEFORE_NEXT_FRAME = 8,
    }

    //Base interface for all entities. Will be updated in the future.
    public interface IMyEntity
    {
        #region Core

        MyEntityFlags Flags { get; set; }
        long EntityId { get; set; }
        string Name { get; set; }
        bool MarkedForClose();
        bool Closed { get; }
        bool Save { get; set; }

        event Action<IMyEntity> OnClose;
        event Action<IMyEntity> OnClosing;
        event Action<IMyEntity> OnMarkForClose;

        string GetFrendlyName();
        void Close();
        void Delete();

        #endregion

        MyEntityUpdateEnum NeedsUpdate { get; set; }
        void Update(long frameTimeStamp);

        #region Hierarchy

        IMyEntity Parent { get; }
        Matrix LocalMatrix { get; set; }

        IMyEntity GetTopMostParent(Type type = null);
        void SetLocalMatrix(Matrix localMatrix);
        void GetChildren(List<IMyEntity> children, Func<IMyEntity, bool> collect = null);

        #endregion

        #region Render

        bool NeedsDraw { get; set; }
        bool SkipIfTooSmall { get; set; }
        bool Visible { get; set; }

        void DebugDraw();
        void DebugDrawInvalidTriangles();

        #endregion

        #region Scene

        bool InvalidateOnMove { get; }
        Matrix WorldMatrix { get; set; }
        Matrix WorldMatrixInvScaled { get; }
        Matrix WorldMatrixNormalizedInv { get; }
        float GetDistanceBetweenCameraAndBoundingSphere();
        float GetDistanceBetweenCameraAndPosition();
        float GetLargestDistanceBetweenCameraAndBoundingSphere();
        float GetSmallestDistanceBetweenCameraAndBoundingSphere();
        Matrix GetViewMatrix();
        Matrix GetWorldMatrixNormalizedInv();
        void SetWorldMatrix(Matrix worldMatrix);
        void SetPosition(Vector3 position);

        #endregion

        #region Model

        MyModel Model { get; set; }
        bool GetIntersectionWithLine(ref LineD line, out IntersectionResultLineTriangle? triangle, IntersectionFlags flags);
        Vector3? GetIntersectionWithLineAndBoundingSphere(ref LineD line, float boudingSphereRadiusMultiplier);
        bool GetIntersectionWithSphere(ref BoundingSphere sphere);
        void GetTrianglesIntersectionSphere(ref BoundingSphere sphere, Vector3? referenceNormalVector, float? maxAngle, List<TriangleVertexNormals> retTriangles, int maxNeighbourTriangles);
        bool DoOverlapSphereTest(float sphereRadius, Vector3 spherePos);
        BoundingBox LocalAABB { get; set; }
        BoundingSphere LocalVolume { get; set; }
        Vector3 LocalVolumeOffset { get; set; }

        #endregion

        #region Physics

        event Action<IMyEntity> OnPhysicsChanged;

        #endregion


    }
}

using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Game.Builders;
using TPresenter.Game.Entities;
using TPresenter.Game.Models;
using TPresenter.Render.GeometryStage.Model;
using TPresenter.Utils;
using TPresenterMath;

namespace TPresenter.Game.Interfaces
{
    [Flags]
    public enum EntityFlags
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
        /// Specify whether this entity should be skipped onscreen size is too small
        /// </summary>
        SkipIfTooSmall = 1 << 4,

        Default = EntityFlags.SkipIfTooSmall | EntityFlags.Save
    }

    //Base interface for all entities. Will be updated in the future.
    public interface IEntity
    {
        #region Core

        EntityFlags Flags { get; set; }
        StringId Id { get; set; }
        string Name { get; set; }

        void Delete();
        void Update(long frameTimeStamp);   //TODO: This method should not be here. Find correct update strategy ASAP.
        void Init(Builder_Entity builderEntity);

        #endregion

        #region Hierarchy

        IEntity Parent { get; }
        List<IEntity> Children { get; }

        IEntity GetTopMostParent(Type type = null);

        #endregion

        #region Scene
        Vector3 Position { get; set; }
        ref Matrix LocalMatrix { get; }
        ref Matrix WorldMatrix { get; }
        Matrix WorldMatrixInvScaled { get; }
        Matrix WorldMatrixNormalizedInv { get; }
        Matrix GetViewMatrix();
        Matrix GetWorldMatrixNormalizedInv();

        float GetDistanceBetweenCameraAndBoundingSphere();
        float GetDistanceBetweenCameraAndPosition();

        void SetLocalMatrix(Matrix matrix);
        void SetWorldMatrix(Matrix matrix);
        #endregion

        #region Model

        BoundingBox LocalAABB { get; set; }
        BoundingSphere LocalVolume { get; set; }
        Vector3 LocalVolumeOffset { get; set; }
        Vector3? GetIntersectionWithLineAndBoundingSphere(ref LineD line, float boudingSphereRadiusMultiplier);
        bool GetIntersectionWithLine(ref LineD line, out IntersectionResultLineTriangle? triangle);
        bool GetIntersectionWithSphere(ref BoundingSphere sphere);
        bool DoOverlapSphereTest(float sphereRadius, Vector3 spherePos);

        #endregion

        #region Physics



        #endregion
    }
}

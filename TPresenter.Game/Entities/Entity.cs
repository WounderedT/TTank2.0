using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using TPresenter.Game.Interfaces;
using TPresenter.Game.Models;
using TPresenter.Utils;
using TPresenterMath;
using TPresenter.Render.GeometryStage.Model;
using TPresenter.Game.Builders;

namespace TPresenter.Game.Entities
{
    public class Entity : IEntity
    {
        private Matrix _localMatrix = Matrix.Identity;
        private Matrix _worldMatrix = Matrix.Identity;

        public EntityFlags Flags { get; set; }
        public StringId Id { get; set; }
        public string Name { get; set; }

        public IEntity Parent { get; }
        public List<IEntity> Children { get; }  //TODO: Create internal setter for this property

        public Vector3 Position
        {
            get { return _worldMatrix.TranslationVector; }
            set { _worldMatrix.TranslationVector = value; }
        }
        public ref Matrix LocalMatrix
        {
            get { return ref _localMatrix; }
        }

        public ref Matrix WorldMatrix
        {
            get { return ref _worldMatrix; }
        }
        public Matrix WorldMatrixInvScaled { get { return Matrix.Invert(WorldMatrix); } }
        public Matrix WorldMatrixNormalizedInv { get { return Matrix.Invert(Matrix_T.Normalize(WorldMatrix)); } }

        public BoundingBox LocalAABB { get; set; }
        public BoundingSphere LocalVolume { get; set; }
        public Vector3 LocalVolumeOffset { get; set; }

        public Entity() { }

        public Entity(IEntity parent = null, string name = "", EntityFlags flags = EntityFlags.Default)
        {
            Parent = parent;
            Name = name;
            Flags = flags;
        }

        public virtual void Init(Builder_Entity builderEntity)
        {
            Flags = builderEntity.EntityFlags;
            Id = builderEntity.Id;
            Name = builderEntity.Name;
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public IEntity GetTopMostParent(Type type = null)
        {
            throw new NotImplementedException();
        }

        public Matrix GetViewMatrix()
        {
            throw new NotImplementedException();
        }

        public Matrix GetWorldMatrixNormalizedInv()
        {
            throw new NotImplementedException();
        }

        public void SetLocalMatrix(Matrix matrix)
        {
            _localMatrix = matrix;
        }

        public void SetWorldMatrix(Matrix matrix)
        {
            _worldMatrix = matrix;
        }

        public float GetDistanceBetweenCameraAndBoundingSphere()
        {
            throw new NotImplementedException();
        }

        public float GetDistanceBetweenCameraAndPosition()
        {
            throw new NotImplementedException();
        }

        public Vector3? GetIntersectionWithLineAndBoundingSphere(ref LineD line, float boudingSphereRadiusMultiplier)
        {
            throw new NotImplementedException();
        }

        public bool GetIntersectionWithLine(ref LineD line, out IntersectionResultLineTriangle? triangle)
        {
            throw new NotImplementedException();
        }

        public bool GetIntersectionWithSphere(ref BoundingSphere sphere)
        {
            throw new NotImplementedException();
        }

        public bool DoOverlapSphereTest(float sphereRadius, Vector3 spherePos)
        {
            throw new NotImplementedException();
        }

        public virtual void Update(Int64 timeStamp)
        {
        }
    }
}

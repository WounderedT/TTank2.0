using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using TPresenter.Game.Components;
using TPresenter.Game.Interfaces;
using TPresenter.Game.Models;
using TPresenter.Utils;
using TPresenterMath;
using TPresenter.Render.GeometryStage.Model;

namespace TPresenter.Game.Entity
{
    public class MyEntity : IMyEntity
    {
        bool closed = false;
        Matrix localMatrix = Matrix.Identity;
        Matrix worldMatrix = Matrix.Identity;

        public MyEntityFlags Flags { get; set; }
        public long EntityId { get; set; }
        public string Name { get; set; }

        public bool Closed { get { return closed; } }

        public bool Save
        {
            get { return (Flags & MyEntityFlags.Save) != 0; }
            set
            {
                if (value)
                    Flags |= MyEntityFlags.Save;
                else
                    Flags &= ~MyEntityFlags.Save;
            }
        }
        public MyEntityUpdateEnum NeedsUpdate { get; set; }

        public IMyEntity Parent { get; }

        public bool NeedsDraw
        {
            get { return (Flags & MyEntityFlags.NeedsDraw) != 0; }
            set
            {
                if (value)
                    Flags |= MyEntityFlags.NeedsDraw;
                else
                    Flags &= ~MyEntityFlags.NeedsDraw;
            }
        }
        public bool SkipIfTooSmall
        {
            get { return (Flags & MyEntityFlags.SkipIfTooSmall) != 0; }
            set
            {
                if (value)
                    Flags |= MyEntityFlags.SkipIfTooSmall;
                else
                    Flags &= ~MyEntityFlags.SkipIfTooSmall;
            }
        }
        public bool Visible
        {
            get { return (Flags & MyEntityFlags.Visible) != 0; }
            set
            {
                if (value)
                    Flags |= MyEntityFlags.Visible;
                else
                    Flags &= ~MyEntityFlags.Visible;
            }
        }
        public bool InvalidateOnMove
        {
            get { return (Flags & MyEntityFlags.InvalidateOnMove) != 0; }
            set
            {
                if (value)
                    Flags |= MyEntityFlags.InvalidateOnMove;
                else
                    Flags &= ~MyEntityFlags.InvalidateOnMove;
            }
        }

        public Matrix LocalMatrix
        {
            get { return localMatrix; }
            set { localMatrix = value; }
        }
        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set { worldMatrix = value; }
        }
        public Matrix WorldMatrixInvScaled { get { return Matrix.Invert(WorldMatrix); } }
        public Matrix WorldMatrixNormalizedInv { get { return Matrix.Invert(Matrix_T.Normalize(WorldMatrix)); } }

        public BoundingBox LocalAABB { get; set; }
        public BoundingSphere LocalVolume { get; set; }
        public Vector3 LocalVolumeOffset { get; set; }

        public event Action<IMyEntity> OnClose;
        public event Action<IMyEntity> OnClosing;
        public event Action<IMyEntity> OnMarkForClose;
        public event Action<IMyEntity> OnPhysicsChanged;

        public MyModel Model { get; set; }

        public MyEntity() { }

        public MyEntity(MyModel model, IMyEntity parent = null, string name = "",  MyEntityFlags flags = MyEntityFlags.Default)
        {
            Model = model;
            Parent = parent;
            if (String.IsNullOrEmpty(name))
                Name = model.Name;
            else
                Name = name;
            Flags = flags;
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void DebugDraw()
        {
            throw new NotImplementedException();
        }

        public void DebugDrawInvalidTriangles()
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public bool DoOverlapSphereTest(float sphereRadius, Vector3 spherePos)
        {
            throw new NotImplementedException();
        }

        public void GetChildren(List<IMyEntity> children, Func<IMyEntity, bool> collect = null)
        {
            throw new NotImplementedException();
        }

        public float GetDistanceBetweenCameraAndBoundingSphere()
        {
            throw new NotImplementedException();
        }

        public float GetDistanceBetweenCameraAndPosition()
        {
            throw new NotImplementedException();
        }

        public string GetFrendlyName()
        {
            throw new NotImplementedException();
        }

        public bool GetIntersectionWithLine(ref LineD line, out IntersectionResultLineTriangle? triangle, IntersectionFlags flags)
        {
            throw new NotImplementedException();
        }

        public Vector3? GetIntersectionWithLineAndBoundingSphere(ref LineD line, float boudingSphereRadiusMultiplier)
        {
            throw new NotImplementedException();
        }

        public bool GetIntersectionWithSphere(ref BoundingSphere sphere)
        {
            throw new NotImplementedException();
        }

        public float GetLargestDistanceBetweenCameraAndBoundingSphere()
        {
            throw new NotImplementedException();
        }

        public float GetSmallestDistanceBetweenCameraAndBoundingSphere()
        {
            throw new NotImplementedException();
        }

        public IMyEntity GetTopMostParent(Type type = null)
        {
            throw new NotImplementedException();
        }

        public void GetTrianglesIntersectionSphere(ref BoundingSphere sphere, Vector3? referenceNormalVector, float? maxAngle, List<TriangleVertexNormals> retTriangles, int maxNeighbourTriangles)
        {
            throw new NotImplementedException();
        }

        public Matrix GetViewMatrix()
        {
            return WorldMatrixNormalizedInv;
        }

        public Matrix GetWorldMatrixNormalizedInv()
        {
            return WorldMatrixNormalizedInv;
        }

        public bool MarkedForClose()
        {
            throw new NotImplementedException();
        }

        public void SetLocalMatrix(Matrix localMatrix)
        {
            this.localMatrix = localMatrix;
        }

        public void SetPosition(Vector3 position)
        {
            throw new NotImplementedException();
        }

        public void SetWorldMatrix(Matrix worldMatrix)
        {
            this.worldMatrix = worldMatrix;
        }

        public void Update(long frameTimeStamp)
        {
            throw new NotImplementedException();
        }
    }
}

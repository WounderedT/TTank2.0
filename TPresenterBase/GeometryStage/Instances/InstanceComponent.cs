using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Render.GeometryStage.Model
{
    public class InstanceComponent
    {
        public MyModel Model { get; private set; }
        public LoD Lod;
        public ITransformStrategy transformStrategy;

        public Matrix GetMatrix()
        {
            return transformStrategy.GetMatrix();
        }

        public InstanceComponent(MyModel model, Matrix matrix)
        {
            Model = model;
            SetInstanceTransformStrategy(matrix);
        }

        public void SetInstanceTransformStrategy(Matrix matrix)
        {
            transformStrategy = new InstanceTransformStrategy();
            transformStrategy.SetMatrix(matrix);
        }
    }

    public class SkinnedInstanceComponent : InstanceComponent
    {
        public Matrix[] SkinMatrices;

        public SkinnedInstanceComponent(MyModel model, Matrix matrix, Matrix[] skinMatrices) : base(model, matrix)
        {
            SkinMatrices = skinMatrices;
        }
    }

    public interface ITransformStrategy
    {
        Matrix GetMatrix();
        Vector3 GetTranslationVector();

        void SetMatrix(Matrix matrix);
    }

    public class InstanceTransformStrategy : ITransformStrategy
    {
        Matrix matrix = Matrix.Identity;
        Vector3 translationVector = Vector3.Zero;

        public Matrix GetMatrix()
        {
            return matrix;
        }

        public Vector3 GetTranslationVector()
        {
            return translationVector;
        }

        public void SetMatrix(Matrix matrix)
        {
            this.matrix = matrix;
            translationVector = new Vector3(matrix.M41, matrix.M42, matrix.M43);
        }
    }
}

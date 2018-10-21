using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenterMath;
using TPresenter.Utils;

namespace TPresenter
{
    public enum SpectatorCameraMovementEnum
    {
        UserControled,
        ConstantDelta,
        FreeMouse,
        Orbit,
        None
    }

    public class Spectator
    {
        //Empirical values.
        public const float DEFAULT_SPECTATOR_LINEAR_SPEED = 0.1f;
        public const float MIN_SPECTATOR_LINEAR_SPEED = 0.0001f;
        public const float MAX_SPECTATOR_LINEAR_SPEED = 8000.0f;

        public const float DEFAULT_SPECTATOR_ANGULAR_SPEED = 1f;
        public const float MIN_SPECTATOR_ANGULAR_SPEED = 0.0001f;
        public const float MAX_SPECTATOR_ANGULAR_SPEED = 6.0f;

        public readonly Vector3 DEFAULT_POSITION = new Vector3(1, 1, -2);

        private SpectatorCameraMovementEnum _spectatorCameraMovement = SpectatorCameraMovementEnum.UserControled;
        private Vector3 _targetDelta = Vector3.ForwardRH;  //Vector3.ForwardRH
        private Vector3? _up;

        private float _orbitX = 0;
        private float _orbitY = 0;

        protected float speedModeLinear = DEFAULT_SPECTATOR_LINEAR_SPEED;
        protected float speedModeAngular = DEFAULT_SPECTATOR_ANGULAR_SPEED;
        protected Matrix orientation = Matrix.Identity;
        protected bool orientationDirty = true;

        public static Spectator Static { get; set; }

        public Vector3 ThirdPersonCameraDelta { get; set; } = new Vector3(-10, 10, 10);

        public SpectatorCameraMovementEnum SpectatorCameraMovement
        {
            get { return _spectatorCameraMovement; }
            set
            {
                if (_spectatorCameraMovement != value)
                    OnChangingMode(_spectatorCameraMovement, value);

                _spectatorCameraMovement = value;
            }
        }

        public bool IsFirstPersonView { get; set; }

        public bool ForceFirstPersonCamera { get; set; }

        public Vector3 Position { get; set; }

        public float SpeedModeLinear
        {
            get { return speedModeLinear; }
            set { speedModeLinear = value; }
        }

        public float SpeedModeAngular
        {
            get { return speedModeAngular; }
            set { speedModeAngular = value; }
        }

        public Vector3 Target
        {
            get { return Position + _targetDelta; }
            set
            {
                var myTargetDelta = value - Position;
                orientationDirty = myTargetDelta != _targetDelta;
                _targetDelta = myTargetDelta;
                _up = null;
            }
        }

        public Matrix Orientation
        {
            get
            {
                if (orientationDirty)
                {
                    UpdateOrientation();
                    orientationDirty = false;
                }
                return orientation;
            }
        }

        protected virtual void OnChangingMode(SpectatorCameraMovementEnum oldMode, SpectatorCameraMovementEnum newMode) { }

        public Spectator()
        {
            Static = this;
        }

        public void SetTarget(Vector3 target, Vector3? up)
        {
            Target = target;
            orientationDirty |= this._up != up;
            _up = up;
        }

        public void UpdateOrientation()
        {
            var forward = MyUtils.Normilize(_targetDelta);
            forward = forward.LengthSquared() > 0 ? forward : Vector3.ForwardRH;
            orientation = MatrixD.CreateFromDir(forward, _up.HasValue ? _up.Value : Vector3.Up);
        }

        public void Rotate(Vector2 rotationIndicator, float rollIndicator)
        {
            MoveAndRotate(Vector3.Zero, rotationIndicator, rollIndicator);
        }

        public void RotateStopped()
        {
            MoveAndRotateStopped();
        }

        public virtual void MoveAndRotate(Vector3 moveIndicator, Vector2 rotationIndicator, float rollIndicator)
        {
            var oldPosition = Position;

            moveIndicator *= speedModeLinear;

            float amoutOfMovement = 0.1f;
            float amoutOfRotation = 0.0025f * speedModeAngular;

            Vector3 moveVector = (Vector3)moveIndicator * amoutOfMovement;
            switch (SpectatorCameraMovement)
            {
                case SpectatorCameraMovementEnum.UserControled:
                    {
                        if(rollIndicator != 0)
                        {
                            Vector3 right, up;
                            float rollAmount = rollIndicator * speedModeLinear * 0.1f;
                            rollAmount = MathHelper.Clamp(rollAmount, -0.02f, 0.02f);
                            MyUtils.VectorPlaneRotation(orientation.Up, orientation.Right, rollAmount, out up, out right);
                            orientation.Right = right;
                            orientation.Up = up;
                        }

                        if(rotationIndicator.X != 0)
                        {
                            Vector3 up, forward;
                            MyUtils.VectorPlaneRotation(orientation.Up, orientation.Forward, rotationIndicator.X * amoutOfRotation, out up, out forward);
                            orientation.Up = up;
                            orientation.Forward = forward;
                        }

                        if(rotationIndicator.Y != 0)
                        {
                            Vector3 right, forward;
                            MyUtils.VectorPlaneRotation(orientation.Right, orientation.Forward, -rotationIndicator.Y * amoutOfRotation, out right, out forward);
                            orientation.Right = right;
                            orientation.Forward = forward;
                        }

                        Position += Vector3D.Transform(moveVector, orientation);
                    }
                    break;
                case SpectatorCameraMovementEnum.Orbit:
                    {
                        _orbitX += rotationIndicator.X * 0.01f;
                        _orbitY += rotationIndicator.Y * 0.01f;

                        var delta = -_targetDelta;
                        Vector3 target = Position + _targetDelta;
                        Matrix invRot = Matrix.Invert(Orientation);
                        var deltaInv = Vector3D.Transform(delta, invRot);

                        rotationIndicator *= 0.01f;

                        Matrix rotationMatrix = MatrixD.CreateRotationX(_orbitX) * MatrixD.CreateRotationY(_orbitY) * MatrixD.CreateRotationZ(rollIndicator);
                        delta = Vector3D.Transform(deltaInv, rotationMatrix);

                        Position = target + delta;
                        _targetDelta = -delta;

                        var strafe = (orientation.Right * moveVector.X) + (orientation.Up * moveVector.Y);
                        Position += strafe;

                        var forwardDelta = orientation.Forward * moveVector.Z;
                        Position += forwardDelta;

                        _targetDelta -= forwardDelta;
                        orientation = rotationMatrix;
                    }
                    break;
                case SpectatorCameraMovementEnum.ConstantDelta:
                    {
                        _orbitX += rotationIndicator.X * 0.01f;
                        _orbitY += rotationIndicator.Y * 0.01f;

                        var delta = -_targetDelta;
                        Vector3 target = Position + _targetDelta;
                        Matrix invRot = Matrix.Invert(Orientation);
                        var deltaInv = Vector3D.Transform(delta, invRot);

                        rotationIndicator *= 0.0f;

                        Matrix rotationMatrix = MatrixD.CreateRotationX(_orbitX) * MatrixD.CreateRotationY(_orbitY) * MatrixD.CreateRotationZ(rollIndicator);
                        delta = Vector3D.Transform(deltaInv, rotationMatrix);

                        Position = target + delta;
                        _targetDelta = -delta;
                        orientation = rotationMatrix;
                    }
                    break;
            }
        }

        public virtual void Update() { }

        public virtual void MoveAndRotateStopped() { }

        public Matrix GetViewMatrix()
        {
            return Matrix.Invert(MatrixD.CreateWorld(Position, Orientation.Forward, Orientation.Up));
        }

        public void SetViewMatrix(Matrix matrix)
        {
            MyUtils.AssertIsValid(matrix);

            Matrix inverted = Matrix.Invert(matrix);
            Position = inverted.TranslationVector;
            orientation = Matrix.Identity;
            orientation.Right = inverted.Right;
            orientation.Up = inverted.Up;
            orientation.Forward = inverted.Forward;
            orientationDirty = false;
        }

        public void Reset()
        {
            //position = Vector3.Zero;
            Position = DEFAULT_POSITION;
            _targetDelta = Vector3.ForwardRH;
            ThirdPersonCameraDelta = new Vector3(-10, 10, -10);
            orientationDirty = true;
            _orbitX = 0;
            _orbitY = 0;
        }
    }
}

using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Utils;

namespace TPresenter.Game.Utils
{
    public class CameraShake
    {
        #region CameraShake Constants

        public const float MaxShake = 15.0f;
        public const float MaxShakePosX = 1.0f;
        public const float MaxShakePosY = 1.0f;
        public const float MaxShakePosZ = 1.0f;
        public const float MaxShakeDir = 0.2f;      //max shake dir difference x,z
        public const float Reduction = 0.2f;
        public const float Dampening = 0.95f;
        public const float OffConstant = 0.01f;
        public const float DirReduction = 0.35f;

        #endregion

        #region CameraShake Properties

        private bool shakeEnabled;
        private Vector3 shakePos;
        private Vector3 shakeDir;
        private float currentShakePosPower;
        private float currentShakeDirPower;

        public bool ShakeEnabled
        {
            get { return shakeEnabled; }
            set { shakeEnabled = value; }
        }
        public Vector3 ShakePos { get { return shakePos; } }
        public Vector3 ShakeDir { get { return shakeDir; } }

        #endregion

        public CameraShake()
        {
            shakeEnabled = false;
            currentShakeDirPower = 0.0f;
            currentShakePosPower = 0.0f;
        }

        #region CameraShake Methods

        public bool ShakeActive()
        {
            return shakeEnabled;
        }

        public void AddShake(float shakePower)
        {
            if (MyUtils.IsZero(shakePower))
                return;

            float pow = shakePower / MaxShake;

            if (currentShakePosPower < pow)
                currentShakePosPower = pow;
            if (currentShakeDirPower < pow * DirReduction)
                currentShakeDirPower = pow * DirReduction;

            shakePos = new Vector3(currentShakePosPower * MaxShakePosX, currentShakePosPower * MaxShakePosY, currentShakePosPower * MaxShakePosZ);
            shakeDir = new Vector3(currentShakeDirPower * MaxShakeDir, 0.0f, currentShakeDirPower * MaxShakeDir);
            shakeEnabled = true;
        }

        public void UpdateShake(float timeStep, out Vector3 outPos, out Vector3 outDir)
        {
            if (!shakeEnabled)
            {
                outPos = Vector3.Zero;
                outDir = Vector3.Zero;
                return;
            }

            shakePos.X *= MyUtils.GetRandomSign();
            shakePos.Y *= MyUtils.GetRandomSign();
            shakePos.Z *= MyUtils.GetRandomSign();

            outPos.X = shakePos.X * (Math.Abs(shakePos.X)) * Reduction;
            outPos.Y = shakePos.Y * (Math.Abs(shakePos.Y)) * Reduction;
            outPos.Z = shakePos.Z * (Math.Abs(shakePos.Z)) * Reduction;

            shakeDir.X = MyUtils.GetRandomSign();
            shakeDir.Y = MyUtils.GetRandomSign();
            shakeDir.Z = MyUtils.GetRandomSign();

            outDir.X = shakeDir.X * (Math.Abs(shakeDir.X)) * 100 * Reduction;
            outDir.Y = shakeDir.Y * (Math.Abs(shakeDir.Y)) * 100 * Reduction;
            outDir.Z = shakeDir.Z * (Math.Abs(shakeDir.Z)) * 100 * Reduction;

            currentShakePosPower *= (float)Math.Pow(Dampening, timeStep * 60.0f);
            currentShakeDirPower *= (float)Math.Pow(Dampening, timeStep * 60.0f);

            if (currentShakeDirPower < 0.0f)
                currentShakeDirPower = 0.0f;
            if (currentShakePosPower < 0.0f)
                currentShakePosPower = 0.0f;

            shakePos = new Vector3(currentShakePosPower * MaxShakePosX, currentShakePosPower * MaxShakePosY, currentShakePosPower * MaxShakePosZ);
            shakeDir = new Vector3(currentShakeDirPower * MaxShakeDir, 0.0f, currentShakeDirPower * MaxShakeDir);

            if(currentShakeDirPower < OffConstant && currentShakeDirPower < OffConstant)
            {
                currentShakeDirPower = 0.0f;
                currentShakePosPower = 0.0f;
                shakeEnabled = false;
            }
        }

        #endregion
    }
}

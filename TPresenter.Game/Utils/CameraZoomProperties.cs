using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Game.Utils;
using TPresenterMath;

namespace TTank20Game.Game.Platform
{
    public enum CameraZoomOperationType
    {
        NoZoom,
        ZoomingIn,
        ZoomingOut,
        Zoomed
    }

    public class CameraZoomProperties
    {
        static readonly float FIELD_OF_VIEW_MIN = MathHelper.ToRadians(40);

        float ZoomTime = 0.075f;

        float currentZoomTime;

        CameraZoomOperationType zoomType = CameraZoomOperationType.NoZoom;

        float FOV;
        float zoomLevel;

        Camera camera;

        public bool ApplyToFov { get; set; }

        public CameraZoomProperties(Camera camera)
        {
            this.camera = camera;
            Update(0.0f);
        }

        public void Update(float updateStepSize)
        {
            switch (zoomType)
            {
                case CameraZoomOperationType.NoZoom:
                    break;
                case CameraZoomOperationType.ZoomingIn:
                    {
                        if(currentZoomTime <= ZoomTime)
                        {
                            currentZoomTime += updateStepSize;
                            if(currentZoomTime >= ZoomTime)
                            {
                                currentZoomTime = ZoomTime;
                                zoomType = CameraZoomOperationType.Zoomed;
                            }
                        }
                    }
                    break;
                case CameraZoomOperationType.ZoomingOut:
                    {
                        if(currentZoomTime >= 0)
                        {
                            currentZoomTime -= updateStepSize;
                            if(currentZoomTime <= 0)
                            {
                                currentZoomTime = 0;
                                zoomType = CameraZoomOperationType.NoZoom;
                            }
                        }
                    }
                    break;
            }

            zoomLevel = 1 - currentZoomTime / ZoomTime;

            FOV = ApplyToFov ? MathHelper.Lerp(FIELD_OF_VIEW_MIN, camera.FieldOfView, zoomLevel) : camera.FieldOfView;
        }

        public void ResetZoom()
        {
            zoomType = CameraZoomOperationType.NoZoom;
            currentZoomTime = 0.0f;
        }

        public void SetZoom(CameraZoomOperationType zoomType)
        {
            this.zoomType = zoomType;
        }

        public float GetZoomLevel()
        {
            return zoomLevel;
        }

        public float GetFOV()
        {
            return MathHelper.Clamp(FOV, MathConstants.EPSILON, (float)Math.PI - MathConstants.EPSILON);
        }
    }
}

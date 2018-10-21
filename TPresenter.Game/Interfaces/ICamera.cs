using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenterMath;

namespace TPresenter.Game.Interfaces
{
    public interface ICamera
    {
        Vector3 Position { get; }
        Vector3 PreviousPostition { get; }
        Vector2 ViewportOffset { get; }
        Vector2 VewportSize { get; }
        Matrix ViewMatrix { get; }
        Matrix WorldMatrix { get; }
        Matrix ProjectionMatrix { get; }

        float NearPlaneDistance { get; }
        float FarPlaneDistance { get; }
        float NearForNearObject { get; }
        float FarForNearObject { get; }

        float FieldOfViewAngle { get; }
        float FovWithZoom { get; }

        double GetDistanceWithFOV(Vector3 position);
        bool IsInFrustum(ref BoundingBox boundingBox);
        bool IsInFrustum(ref BoundingSphere boundingSphere);
        bool IsInFrustum(BoundingBox boundingBox);

        /// <summary>
        /// Gets screen coordinates of 3d world position in 0 - 1 distance where 1.0 is screen width (for X) or height (for Y).
        /// Y is from bottom to top.
        /// </summary>
        /// <param name="worldPos">World position.</param>
        /// <returns>Screen coordinate in 0-1 distance.</returns>
        Vector3 WorldToScreen(ref Vector2 worldPos);
        LineD WorldLineFromScreen(Vector2 screenCoords);
    }
}

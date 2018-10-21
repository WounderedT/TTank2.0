using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenterMath
{
    //This class will represent vector with 2 components. It will temporary store extension methods for vector2 since we cannot derive it from SharpDX.Vector2
    //TODO: Rename struct to Vector2 (currently will introduce ambiguity with SharpDX.Vector2)
    public struct Vector2_T
    {

    }

    public static class Vector2Extensions
    {
        public static void Rotate(this Vector2 vector, double angle)
        {
            float tmpX = vector.X;
            vector.X = vector.X * (float)Math.Cos(angle) - vector.Y * (float)Math.Sin(angle);
            vector.Y = vector.Y * (float)Math.Cos(angle) - tmpX * (float)Math.Sin(angle);
        }
    }
}

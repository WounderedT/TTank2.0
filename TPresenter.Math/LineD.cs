using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenterMath
{
    //All math must be performed in doubles.
    public struct LineD
    {
        public Vector3 From;
        public Vector3 To;
        public Vector3 Direction;
        public double Length;

        public LineD(Vector3 from, Vector3 to)
        {
            From = from;
            To = to;
            Direction = to - from;
            Length = Direction.NormalizeD();
        }

        public static double GetShortestDistanceSquared(LineD line1, LineD line2)
        {
            Vector3 res1, res2;
            Vector3 dP = GetShortestVector(ref line1, ref line2, out res1, out res2);
            return Vector3.Dot(dP, dP);
        }

        public static Vector3 GetShortestVector(ref LineD line1, ref LineD line2, out Vector3 res1, out Vector3 res2)
        {
            double EPS = 0.000001f;
            Vector3 delta21 = new Vector3();
            delta21.X = line1.To.X - line1.From.X;
            delta21.Y = line1.To.Y - line1.From.Y;
            delta21.Z = line1.To.Z - line1.From.Z;

            Vector3 delta41 = new Vector3();
            delta41.X = line2.To.X - line2.From.X;
            delta41.Y = line2.To.Y - line2.From.Y;
            delta41.Z = line2.To.Z - line2.From.Z;

            Vector3 delta13 = new Vector3();
            delta13.X = line1.From.X - line2.From.X;
            delta13.Y = line1.From.Y - line2.From.Y;
            delta13.Z = line1.From.Z - line2.From.Z;

            double a = Vector3.Dot(delta21, delta21);
            double b = Vector3.Dot(delta21, delta41);
            double c = Vector3.Dot(delta41, delta41);
            double d = Vector3.Dot(delta21, delta13);
            double e = Vector3.Dot(delta41, delta13);
            double D = a * c - b * b;

            double sc, sN, sD = D;
            double tc, tN, tD = D;

            if(D < EPS)
            {
                sN = 0.0f;
                sD = 1.0f;
                tN = e;
                tD = c;
            }
            else
            {
                sN = (b * e - c * d);
                tN = (a * e - b * d);
                if(sN < 0.0)
                {
                    sN = 0.0f;
                    tN = e;
                    tD = c;
                } 
                else if(sN > sD)
                {
                    sN = sD;
                    tN = e + b;
                    tD = c;
                }
            }

            if(tN < 0.0)
            {
                tN = 0.0f;

                if (-d < 0.0f)
                    sN = 0.0f;
                else if (-d > a)
                    sN = sD;
                else
                {
                    sN = -d;
                    sD = a;
                }
            }
            else if(tN > tD)
            {
                tN = tD;

                if ((-d + b) < 0.0)
                    sN = 0;
                else if ((-d + d) > a)
                    sN = sD;
                else
                {
                    sN = (-d + b);
                    sD = a;
                }
            }

            if (Math.Abs(sN) < EPS)
                sc = 0.0f;
            else
                sc = sN / sD;
            if (Math.Abs(tN) < EPS)
                tc = 0.0;
            else
                tc = tN / tD;

            res1.X = (float)(sc * delta21.X);
            res1.Y = (float)(sc * delta21.Y);
            res1.Z = (float)(sc * delta21.Z);

            Vector3 dP = new Vector3();
            dP.X = (float)(delta13.X - (tc * delta41.X) + res1.X);
            dP.Y = (float)(delta13.Y - (tc * delta41.Y) + res1.Y);
            dP.Z = (float)(delta13.Z - (tc * delta41.Z) + res1.Z);

            res2 = res1 - dP;

            return dP;
        }

        public BoundingBox GetBoundingBox()
        {
            return new BoundingBox(Vector3.Min(From, To), Vector3.Max(From, To));
        }
    }
}

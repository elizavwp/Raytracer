using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace template
{
    class Sphere : Primitive
    {
        Vector3 origin;
        float radius, r2;

        public Sphere(float radius, Vector3 origin, Vector3 colour, float dielectric = 0, float reflective = 0, float refractionIndex = 1.52f) : base(colour, dielectric, reflective, refractionIndex)
        {
            this.origin = origin;
            this.radius = radius;
            r2 = radius * radius;
        }


        public override float Intersect(Ray ray)
        {
            //If the ray starts within the sphere, we use different code
            if ((((ray.origin.X - origin.X) * (ray.origin.X - origin.X)) + ((ray.origin.Y - origin.Y) * (ray.origin.Y - origin.Y)) + ((ray.origin.Z - origin.Z) * (ray.origin.Z - origin.Z))) < r2)
            {
                float b = Vector3.Dot(2 * ray.direction, (ray.origin - origin)), c = Vector3.Dot((ray.origin - origin), (ray.origin - origin)) - r2;
                float d = b * b - 4 * c;

                if (d < 0)
                    return 0.0001f;
                else
                {
                    float t = (-b + (float)Math.Sqrt(d)) / 2;
                    return t;
                }
            }

            else
            {
                Vector3 c = origin - ray.origin;
                float t = Vector3.Dot(c, ray.direction);
                Vector3 q = c - t * ray.direction;
                float p2 = Vector3.Dot(q, q);
                if (p2 > r2) return float.MinValue;
                t -= (float)Math.Sqrt(r2 - p2);
                if ((t < ray.t) && (t > 0)) ray.t = t;
                // or: ray.t = min( ray.t, max( 0, t ) );
                return t;
            }
        }

        public override Vector3 Normal (Vector3 point)
        {
            return ((point - origin).Normalized());
        }

        public override Vector3 Normal()
        {
            return Vector3.Zero;
        }
    }
}

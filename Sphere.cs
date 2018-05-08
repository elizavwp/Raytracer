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
        float radius, r2;

        public Sphere(float radius, Vector3 origin) : base(origin)
        {
            this.radius = radius;
            r2 = radius * radius;
        }


        public override float Intersect(Ray ray)
        {
            Vector3 c = origin - ray.origin;
            float t = Vector3.Dot(c, ray.direction);
            Vector3 q = c - t * ray.direction;
            float p2 = Vector3.Dot(q, q);
            if (p2 > r2) return -1;
            t -= (float)Math.Sqrt(r2 - p2);
            if ((t < ray.t) && (t > 0)) ray.t = t;
            // or: ray.t = min( ray.t, max( 0, t ) );
            return t;

        }
    }
}

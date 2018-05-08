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
    class Plane : Primitive
    {
        public Vector3 normal, p1, p2, p3;
        public float d;
        public Plane(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            Vector3 p1p2 = new Vector3(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
            Vector3 p1p3 = new Vector3(p1.X - p3.X, p1.Y - p3.Y, p1.Z - p3.Z);

            normal = Vector3.Cross(p1p2, p1p3);
            normal.Normalize();

            d = -(Vector3.Dot(p1, normal));
        }

        public Plane(Vector3 normal, float d)
        {
            this.normal = normal;
            this.d = d;
        }

        public override float Intersect(Ray ray)
        {
            return -(Vector3.Dot(ray.origin, normal) + d) / Vector3.Dot(ray.direction, normal);
        }

    }
}

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
        Vector3 normal;
        float d;
        public Plane(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 origin) : base(origin)
            {
                
            }

        public override float Intersect(Ray ray)
        {
            return -(Vector3.Dot(ray.origin, normal) + d) / Vector3.Dot(ray.direction, normal);
        }

    }
}

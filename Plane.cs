﻿using System;
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
        public Plane(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 colour, float dielectric = 0, float reflective = 0, float refractionIndex = 1.52f) : base(colour, dielectric, reflective, refractionIndex)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            Vector3 p1p2 = p1 - p2;
            Vector3 p1p3 = p1 - p3;

            normal = Vector3.Cross(p1p2, p1p3);
            normal.Normalize();

            d = -(Vector3.Dot(p1, normal));
        }

        public override float Intersect(Ray ray)
        {
            return -(Vector3.Dot(ray.origin, normal) + d) / Vector3.Dot(ray.direction, normal);
        }

        public override Vector3 Normal(Vector3 point)
        {
            return normal;
        }

        public override Vector3 Normal()
        {
            return normal;
        }

    }
}

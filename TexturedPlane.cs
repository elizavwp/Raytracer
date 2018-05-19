using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace template
{
    class TexturedPlane : Primitive
    {
        public Vector3 normal;
        public float d;

        //A textured plane is made by looking at the normal, the tangent and the bitangent of the plane, and the value d from the implicit form of the plane
        public TexturedPlane(Vector3 p1p2, Vector3 p1p3, Vector3 normal, float d, Vector3 colour, string texture, float dielectric = 0, float reflective = 0, float refractionIndex = 1.52f, float radius = 0) : base(colour, dielectric, reflective, refractionIndex)
        {
            this.p1p2 = p1p2;
            this.p1p3 = p1p3;
            this.normal = normal;
            this.d = d;
            this.texture = new Bitmap(texture);
        }

        //Find the intersection point with a ray using the formula from the slides
        public override float Intersect(Ray ray)
        {
            return -(Vector3.Dot(ray.origin, normal) + d) / Vector3.Dot(ray.direction, normal);
        }

        //The normal of the plane is the same for every point on the plane so we disregard the given point
        public override Vector3 Normal(Vector3 point)
        {
            return normal;
        }
    }
}

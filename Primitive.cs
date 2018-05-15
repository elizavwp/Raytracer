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
    abstract class Primitive
    {
        public Vector3 colour, p1p2, p1p3;
        public Bitmap texture;
        public float dielectric, reflective;
        public Primitive(Vector3 colour, float dielectric = 0, float reflective = 0)
        {
            this.colour = colour;
            this.dielectric = dielectric;
            this.reflective = reflective;
        }

        public abstract float Intersect(Ray ray);

        public abstract Vector3 Normal(Vector3 point);
        public abstract Vector3 Normal();

    }
}

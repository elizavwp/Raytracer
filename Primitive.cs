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
    abstract class Primitive
    {
        public Vector3 colour;
        public float glass, reflective;
        public Primitive(Vector3 colour, float glass = 0.0f, float reflective = 0.0f)
        {
            this.colour = colour;
            this.glass = glass;
            this.reflective = reflective;
        }

        public abstract float Intersect(Ray ray);

        public abstract Vector3 Normal(Vector3 point);

    }
}

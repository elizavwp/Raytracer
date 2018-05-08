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
        public Vector3 origin;

        public Primitive(Vector3 origin)
        {
            this.origin = origin;
        }

        public abstract float Intersect(Ray ray);

    }
}

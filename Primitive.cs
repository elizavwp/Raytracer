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
        public Primitive()
        { }

        public abstract float Intersect(Ray ray);

    }
}

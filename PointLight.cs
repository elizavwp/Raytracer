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
    class PointLight
    {
        public Vector3 origin, colour;
        public float brightness;

        public PointLight(Vector3 origin, float brightness, Vector3 colour)
        {
            this.origin = origin;
            this.brightness = brightness;
            this.colour = colour;
        }
    }
}

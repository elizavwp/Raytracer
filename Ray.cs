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
    class Ray
    {
        public Vector3 origin, direction;
        public float t;

        public Ray(Vector3 origin, Vector3 direction, float t)
        {
            this.origin = origin;
            this.direction = Normalize(direction);
            this.t = t;
        }

        public Vector3 Normalize(Vector3 direction)
        {
            float length;
            length = (float)Math.Sqrt((direction.X * direction.X) + (direction.Y * direction.Y) + (direction.Z * direction.Z));
            return new Vector3(direction.X / length, direction.Y / length, direction.Z / length);
        }
    }
}

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

        public Ray(Vector3 origin, Vector3 direction)
        {
            this.origin = origin;
            this.direction = Normalize(direction);
        }

        public Vector3 Normalize(Vector3 direction)
        {
            float length;
            length = (float)Math.Sqrt((direction.X * direction.X) + (direction.Y * direction.Y) + (direction.Z * direction.Z));
            return new Vector3(direction.X / length, direction.Y / length, direction.Z / length);
        }

        //Find the point on the ray given a certain distance
        public Vector3 FindPoint(float dist)
        {
            return (origin + dist * direction);
        }

        public float FindDist(Vector3 point)
        {
            return ((point.X - origin.X) / direction.X);
        }

        public void ShadowAcneFix(float epsilon)
        {
            origin += epsilon * direction;
        }
    }
}

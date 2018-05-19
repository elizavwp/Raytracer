using System;
using OpenTK;

namespace template
{
    class Ray
    {
        public Vector3 origin, direction;
        public float t;

        //Each Ray has an origin and a direction
        public Ray(Vector3 origin, Vector3 direction)
        {
            this.origin = origin;

            //We normalize the direction straight away, in case we forget it later
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

        //Find the distance to a given point
        public float FindDist(Vector3 point)
        {
            return ((point.X - origin.X) / direction.X);
        }

        //Prevent Shadowacne (or reflection acne) given a certain epsilon
        public void ShadowAcneFix(float epsilon)
        {
            origin += epsilon * direction;
        }
    }
}

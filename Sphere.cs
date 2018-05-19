using System;
using OpenTK;

namespace template
{
    class Sphere : Primitive
    {
        float r2;

        //Each sphere has an origin and a radius
        public Sphere(float radius, Vector3 origin, Vector3 colour, float dielectric = 0, float reflective = 0, float refractionIndex = 1.52f) : base(colour, dielectric, reflective, refractionIndex)
        {
            this.origin = origin;
            this.radius = radius;

            //To make sure we don't have to calculate r^2 to many times, we calculate it once we make the circle and the save it as a variable
            r2 = radius * radius;
        }


        public override float Intersect(Ray ray)
        {
            //If the ray starts within the sphere, we use the normal ray-s[here intersection code given in the slides
            if ((((ray.origin.X - origin.X) * (ray.origin.X - origin.X)) + ((ray.origin.Y - origin.Y) * (ray.origin.Y - origin.Y)) + ((ray.origin.Z - origin.Z) * (ray.origin.Z - origin.Z))) < r2)
            {
                float b = Vector3.Dot(2 * ray.direction, (ray.origin - origin)), c = Vector3.Dot((ray.origin - origin), (ray.origin - origin)) - r2;
                float d = b * b - 4 * c;

                //if d is negative there is no intersection so return 0.0001f which gets picked up as there being no intersection
                if (d < 0)
                    return 0.0001f;
                else
                {
                    float t = (-b + (float)Math.Sqrt(d)) / 2;
                    return t;
                }
            }

            //If the ray starts outside the circle we can use faster intersection code
            else
            {
                Vector3 c = origin - ray.origin;
                float t = Vector3.Dot(c, ray.direction);
                Vector3 q = c - t * ray.direction;
                float p2 = Vector3.Dot(q, q);
                if (p2 > r2) return float.MinValue;
                t -= (float)Math.Sqrt(r2 - p2);
                if ((t < ray.t) && (t > 0)) ray.t = t;
                return t;
            }
        }

        //Find the normal by calculating the direction from the origin of the sphere to the point and normalizing the found vector
        public override Vector3 Normal (Vector3 point)
        {
            return ((point - origin).Normalized());
        }
    }
}

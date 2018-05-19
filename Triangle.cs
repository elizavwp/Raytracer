using OpenTK;

namespace template
{
    class Triangle : Primitive
    {
        public Vector3 normal;
        public float d;

        //A triangle is made using it's three corners
        public Triangle(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 colour, float dielectric = 0, float reflective = 0, float refractionIndex = 1.52f) : base(colour, dielectric, reflective, refractionIndex)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            p1p2 = p2 - p1;
            p1p3 = p3 - p1;

            normal = Vector3.Cross(p1p2, p1p3);
            normal.Normalize();

            d = -(Vector3.Dot(p1, normal));
        }

        //Find the intersection point of the traingle and a ray using the Möller-Trumbore intersection algorithm
        public override float Intersect(Ray ray)
        {
            Vector3 h = Vector3.Cross(ray.direction, p1p3);
            float a = Vector3.Dot(p1p2, h);

            if (a > -0.0001 && a < 0.0001)
                return float.MaxValue;

            float f = 1 / a;
            Vector3 s = ray.origin - p1;
            float u = f * (Vector3.Dot(s, h));

            if (u < 0.0f || u > 1.0f)
                return float.MaxValue;

            Vector3 q = Vector3.Cross(s, p1p2);
            float v = f * Vector3.Dot(ray.direction, q);

            if (v < 0.0f || u + v > 1.0f)
                return float.MaxValue;

            // At this stage we can compute t to find out where the intersection point is on the line.
            float t = f * Vector3.Dot(p1p3, q);

            return t;
        }
        
        //The normal for a triangle is the same for every point so we disregard the given point
        public override Vector3 Normal(Vector3 point)
        {
            return normal;
        }
    }
}

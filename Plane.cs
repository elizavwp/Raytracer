using OpenTK;

namespace template
{
    class Plane : Primitive
    {
        public Vector3 normal;
        public float d;

        //Each plane is created using 3 corner points on the plane
        public Plane(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 colour, float dielectric = 0, float reflective = 0, float refractionIndex = 1.52f) : base(colour, dielectric, reflective, refractionIndex)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            Vector3 p1p2 = p2 - p1;
            Vector3 p1p3 = p3 - p1;

            normal = Vector3.Cross(p1p2, p1p3);
            normal.Normalize();

            d = -(Vector3.Dot(p1, normal));
        }

        //Find the intersection point for a ray using the formula given in the slides
        public override float Intersect(Ray ray)
        {
            return -(Vector3.Dot(ray.origin, normal) + d) / Vector3.Dot(ray.direction, normal);
        }


        //Find the normal of the plane (wich is the same for each point on the plane so we disregard the point given)
        public override Vector3 Normal(Vector3 point)
        {
            return normal;
        }
    }
}

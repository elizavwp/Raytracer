using System.Drawing;
using OpenTK;

namespace template
{
    abstract class Primitive
    {
        public Vector3 colour, p1p2, p1p3, origin, p1, p2, p3;
        public Bitmap texture;
        public float dielectric, reflective, refractionIndex, diffuse, radius;

        //Each primitive has a colour and percentages for how dielectric, reflective and diffuse it is, and a refraction index if it is dielectric
        //The other variables (origin and radius, p1, p2, p3, p1p2, p1p3 and texture) are used when we acces (for instance) a sphere as a primitive
        public Primitive(Vector3 colour, float dielectric = 0, float reflective = 0, float refractionIndex = 1.52f)
        {
            this.colour = colour;
            this.dielectric = dielectric;
            this.reflective = reflective;
            this.refractionIndex = refractionIndex;
            diffuse = 1f - (dielectric + reflective);
        }

        public abstract float Intersect(Ray ray);

        public abstract Vector3 Normal(Vector3 point);

    }
}

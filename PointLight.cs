using OpenTK;

namespace template
{
    class PointLight
    {
        public Vector3 origin, colour;
        public float brightness;

        //A point light has an origin a brightness and a colour
        public PointLight(Vector3 origin, float brightness, Vector3 colour)
        {
            this.origin = origin;
            this.brightness = brightness;
            this.colour = colour;
        }
    }
}

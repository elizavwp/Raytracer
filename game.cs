using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
namespace template
{

    class Game
    {
        // member variables
        public Surface screen;
        public Camera camera;
        public List<Primitive> primitives;
        public List<PointLight> lights;

        // variables for the primary rays

        Bitmap skybox = new Bitmap("../../assets/skybox.png");

        // initialize
        public void Init()
        {
            camera = new Camera();

            //Add primitives to the scene
            primitives = new List<Primitive>();
            primitives.Add(new TexturedPlane(new Vector3(1, 0, 0), new Vector3(0, 0, -1), new Vector3(0, -1, 0), 1, new Vector3(1), "../../assets/tiles.png", 0, 0.2f));
            primitives.Add(new Sphere(3, new Vector3(-1.5f, -2, -13), new Vector3(0.1f, 1f, 0.1f)));
            primitives.Add(new Sphere(1, new Vector3(3, 0, -8), new Vector3(1f, 0.7f, 0.7f), 0, 0.8f));
            primitives.Add(new Sphere(8, new Vector3(11, -7, -23), new Vector3(0.9f, 0.4f, 1f)));
            primitives.Add(new Sphere(1, new Vector3(-4, 0, -8), new Vector3(0.3f, 0.9f, 0.9f)));
            primitives.Add(new Sphere(0.5f, new Vector3(0, 0.5f, -4), new Vector3(1f, 1f, 1f)));

            //Add Lightsources to the scene
            lights = new List<PointLight>();
            lights.Add(new PointLight(new Vector3(0, -10, -1), 0.8f, new Vector3(0.5f, 1, 1)));
            lights.Add(new PointLight(new Vector3(0, -15, -5), 0.4f, new Vector3(1, 0.5f, 0)));

        }
        // tick: renders one frame
        public void Tick()
        {
            screen.Clear(0);
            screen.Line(512, 0, 512, 512, 0xffffff);

            //Shoot rays
            for (int x = 0; x < camera.pixels.GetLength(0); x++)
                for (int y = 0; y < camera.pixels.GetLength(1); y++)
                {

                    screen.pixels[x + y * screen.width] = VectorToInt(ShootRay(camera.pixels[x, y], x, y, 0));
                    ClampInt(screen.pixels[x + y * screen.width], 1.0f);
                }

            screen.Print("FOV = " + camera.fov, 5, 5, 0xffffff);
        }

        public Vector3 ShootRay(Ray ray, int x, int y, int recursion)
        {
            Ray shadowRay;
            Vector3 intersect, shadowRayDir, eIncoming, eReflected, textureColour, finalColour;
            Vector2 textureLocation;
            Primitive closestPrim = new Plane(Vector3.Zero, 0, Vector3.Zero);
            float t, tClosestPrim = float.MaxValue, shadowT, shadowPrimT, epsilon = 0.001f, lightBrightness;
            bool occluder = false;
            textureLocation = Vector2.Zero;
            finalColour = Vector3.Zero;
            tClosestPrim = float.MaxValue;

            //Find nearest primitive
            foreach (Primitive p in primitives)
            {
                //Find the distance from the eye to the intersection
                t = p.Intersect(ray);

                //Make sure it's the closest one yet (and that is indeed visible by checking t > 0)
                if (t > 0 && t < tClosestPrim)
                {
                    tClosestPrim = t;
                    closestPrim = p;
                }
            }

            //There is a primitive visible from the pixel draw that primitive
            if (tClosestPrim < float.MaxValue && recursion <= 64)
            {

                //Intersection Point
                intersect = ray.FindPoint(tClosestPrim);

                //Check wether the primitive is reflective
                if (closestPrim.reflective > 0f)
                {
                    float reflection = closestPrim.reflective;

                    //Find new direction for the reflected ray
                    Vector3 intersectNormal = closestPrim.Normal(intersect);
                    intersectNormal.Normalize();
                    Vector3 newDirection = ray.direction - 2 * (Vector3.Dot(ray.direction, intersectNormal) * intersectNormal);

                    //Shoot reflection ray
                    Ray reflectedRay = new Ray(intersect, newDirection);
                    reflectedRay.ShadowAcneFix(epsilon);
                    Vector3 primColour = closestPrim.colour;
                    Vector3 temp = ShootRay(reflectedRay, x, y, ++recursion);
                    finalColour += reflection * (EntrywiseProduct(temp, primColour));
                }

                if (closestPrim.reflective < 1.0f)
                {
                    //Check for each light if a it can be seen from the intersection point
                    foreach (PointLight light in lights)
                    {
                        occluder = false;

                        //Calculate the Ray from the intersection point to the light
                        shadowRayDir = (light.origin - intersect);
                        shadowRayDir.Normalize();
                        shadowRay = new Ray(intersect, shadowRayDir);

                        //Find the distance to the light
                        shadowT = shadowRay.FindDist(light.origin);

                        //Make sure we don't get shadow acne
                        shadowRay.ShadowAcneFix(epsilon);
                        shadowT -= 2 * epsilon;

                        //Use the distance to the light to check if there are any occluders
                        foreach (Primitive prim in primitives)
                        {
                            //Check if there is an intersection by making sure the ShadowPrimT is >0,
                            //and make sure it is an occlusion by making sure it's <shadowT
                            shadowPrimT = prim.Intersect(shadowRay);
                            if (shadowPrimT > 0 && shadowPrimT < shadowT)
                            {
                                occluder = true;
                                break;
                            }
                        }

                        //If there is an occluder check the next light
                        if (occluder)
                            continue;
                        else
                        {
                            //Distance attenuation
                            lightBrightness = light.brightness - (light.brightness / (shadowT * shadowT));
                            eIncoming = light.colour * lightBrightness;

                            //N dot L
                            eIncoming *= Vector3.Dot(closestPrim.Normal(intersect), shadowRayDir);

                            //Light absorbtion
                            //
                            //Check wether the primitive is textured
                            if (closestPrim.GetType() == typeof(TexturedPlane))
                            {
                                textureLocation = EntrywiseProduct(new Vector2(Frac(Vector2.Dot(intersect.Xz, closestPrim.p1p2.Xz)), Frac(Vector2.Dot(intersect.Xz, closestPrim.p1p3.Xz))), new Vector2((closestPrim.texture.Width - 1), (closestPrim.texture.Height - 1)));
                                textureColour = ColourToVector(closestPrim.texture.GetPixel((int)textureLocation.X, (int)textureLocation.Y));
                                eReflected = EntrywiseProduct(eIncoming, textureColour);
                            }

                            else eReflected = EntrywiseProduct(eIncoming, closestPrim.colour);

                            //Colouring pixel
                            finalColour += (1 - closestPrim.reflective) * eReflected;
                        }
                    }
                    //Make sure the individual colourvalues don't exceed 1
                    ClampVector(ref finalColour, 1.0f);
                }

                    //Return colour
                    return finalColour;
            }

            //If there is no primitive visible from a certain pixel, we draw a sky box
            else
            {
                //The 3 * x and 3 * y are done to make the skyboxtexture less zoomed in
                textureLocation = EntrywiseProduct(new Vector2(Frac(3 * x / (float)skybox.Width), Frac(3 * y / (float)skybox.Height)), new Vector2((skybox.Width - 1), (skybox.Height - 1)));
                return ColourToVector(skybox.GetPixel((int)textureLocation.X, (int)textureLocation.Y));
            }
        }

        public Vector3 EntrywiseProduct(Vector3 vector1, Vector3 vector2)
        {
            return new Vector3(vector1.X * vector2.X, vector1.Y * vector2.Y, vector1.Z * vector2.Z);
        }

        public Vector2 EntrywiseProduct(Vector2 vector1, Vector2 vector2)
        {
            return new Vector2(vector1.X * vector2.X, vector1.Y * vector2.Y);
        }

        public int ClampInt(int colour, float clampHigh, float clampLow = float.MinValue)
        {
            Vector3 colourVector = intToVector(colour);
            ClampVector(ref colourVector, clampHigh, clampLow);
            return VectorToInt(colourVector);
        }
        public void ClampVector(ref Vector3 vector, float clampHigh, float clampLow = float.MinValue)
        {
            //X component
            if (vector.X < clampLow)
                vector.X = clampLow;
            if (vector.X > clampHigh)
                vector.X = clampHigh;

            //Y component
            if (vector.Y < clampLow)
                vector.Y = clampLow;
            if (vector.Y > clampHigh)
                vector.Y = clampHigh;

            //Z component
            if (vector.Z < clampLow)
                vector.Z = clampLow;
            if (vector.Z > clampHigh)
                vector.Z = clampHigh;
        }

        public Vector3 intToVector(int colour)
        {
            return new Vector3(((colour >> 16) & 255) / 255f, ((colour >> 8) & 255) / 255f, (colour & 255) / 255f);
        }

        public Vector3 ColourToVector(Color4 colour)
        {
            return new Vector3(colour.R, colour.G, colour.B);
        }

        public int VectorToInt(Vector3 colour)
        {
            return (((int)(colour.X * 255) << 16) + ((int)(colour.Y * 255) << 8) + (int)(colour.Z * 255));
        }

        //Gets the value after the decimal point of a float
        public float Frac(float i)
        {
            return (float)(i - Math.Floor(i));
        }
    }
}// namespace Template
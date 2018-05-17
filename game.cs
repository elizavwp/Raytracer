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
using OpenTK.Input;
namespace template
{

    class Game
    {
        // member variables
        public Surface screen;
        public Camera camera;
        public List<Primitive> primitives;
        public List<PointLight> lights;
        public List<Tuple<Vector3, Vector3, int>> debugRays;
        public bool debugRay = false;
        public Vector2 cameraDebug;
        public int[] debugColour;

        // variables for the camera movement
        float x, y, z, yRotation, xRotation, newFov = 90;
        bool keyPressed;

        Bitmap skybox = new Bitmap("../../assets/skybox.png");

        // initialize
        public void Init()
        {
            camera = new Camera();

            //Add primitives to the scene
            primitives = new List<Primitive>();
            primitives.Add(new TexturedPlane(new Vector3(1, 0, 0), new Vector3(0, 0, -1), new Vector3(0, -1, 0), 1, new Vector3(1), Vector3.Zero, "../../assets/tiles.png", 0, 0.2f));
            primitives.Add(new Sphere(3, new Vector3(-1.5f, -2, -13), new Vector3(0.1f, 1f, 0.1f)));
            primitives.Add(new Sphere(1, new Vector3(3, 0, -8), new Vector3(1f, 0.7f, 0.7f), 0, 0.8f));
            primitives.Add(new Sphere(8, new Vector3(11, -7, -23), new Vector3(0.9f, 0.4f, 1f)));
            primitives.Add(new Sphere(1, new Vector3(-1, 0, -5.5f), new Vector3(0.5f, 1f, 1f), 1f, 0, 1.52f));
            primitives.Add(new Sphere(0.5f, new Vector3(0, 0.5f, -4), new Vector3(1f, 1f, 1f)));

            //Add Lightsources to the scene
            lights = new List<PointLight>();
            lights.Add(new PointLight(new Vector3(-10, -10, -1), 0.8f, new Vector3(0.5f, 1, 1)));
            lights.Add(new PointLight(new Vector3(10, -15, -5), 0.4f, new Vector3(1, 0.5f, 0)));

            //Colours for the debugrays
            debugColour = new int[] { 0xffff00, 0xff00ff, 0x00ff00, 0x00ffff, 0xff0000, 0x0000ff, 0x000000, 0x000000 };

            //The List of rays to draw in the debug window
            debugRays = new List<Tuple<Vector3, Vector3, int>>();

        }
        // tick: renders one frame
        public void Tick()
        {
            CheckMovement();

            screen.Clear(0);
            screen.Line(512, 0, 512, 512, 0xffffff);

            //Shoot rays
            for (int x = 0; x < camera.pixels.GetLength(0); x++)
                for (int y = 0; y < camera.pixels.GetLength(1); y++)
                {
                    //Make sure we only draw an apropriate amount of debugrays
                    if (y == 255 && x % 30 == 0)
                        debugRay = true;

                    screen.pixels[x + y * screen.width] = VectorToInt(ShootRay(camera.pixels[x, y], x, y, 0));

                    debugRay = false;
                }

            screen.Print("FOV = " + camera.fov, 517, 5, 0xffffff);

            DebugScreen(primitives, lights, camera, debugRays);
            debugRays.Clear();
        }

        public Vector3 ShootRay(Ray ray, int x, int y, int recursion, float refractionIndex = 1.0f)
        {
            Ray shadowRay;
            Vector3 intersect, shadowRayDir, eIncoming, eReflected, textureColour, finalColour;
            Vector2 textureLocation;
            Primitive closestPrim = new Plane(Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero);
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
            if (tClosestPrim < float.MaxValue && recursion <= 6)
            {

                //Intersection Point
                intersect = ray.FindPoint(tClosestPrim);

                //Draw this ray in the debug window
                if (debugRay && closestPrim.GetType() == typeof(Sphere))
                    debugRays.Add(new Tuple<Vector3, Vector3, int>(camera.origin, intersect, debugColour[recursion]));

                //Check wether the primitive is reflective
                if (closestPrim.reflective > 0f)
                {
                    float reflection = closestPrim.reflective;

                    //Find new direction for the reflected ray
                    Vector3 intersectNormal = closestPrim.Normal(intersect);
                    intersectNormal.Normalize();
                    //Mirror the direction of the ray
                    Vector3 newDirection = ray.direction - 2 * (Vector3.Dot(ray.direction, intersectNormal) * intersectNormal);

                    //Shoot reflection ray
                    Ray reflectedRay = new Ray(intersect, newDirection);
                    reflectedRay.ShadowAcneFix(epsilon);
                    Vector3 primColour = closestPrim.colour;
                    //Shoot the reflected ray and add it's colour to the final colour
                    Vector3 reflectionRay = ShootRay(reflectedRay, x, y, ++recursion);
                    finalColour += reflection * (EntrywiseProduct(reflectionRay, primColour));
                }

                if (closestPrim.dielectric > 0f)
                {
                    float primRefractionIndex = closestPrim.refractionIndex;

                    //Find new direction for the reflected ray
                    Vector3 intersectNormal = closestPrim.Normal(intersect);
                    intersectNormal.Normalize();

                    //Mirror the direction of the ray
                    Vector3 newDirection = ray.direction - 2 * (Vector3.Dot(ray.direction, intersectNormal) * intersectNormal);

                    //Shoot reflection ray
                    Ray reflectedRay = new Ray(intersect, newDirection);
                    reflectedRay.ShadowAcneFix(epsilon);
                    Vector3 primColour = closestPrim.colour;
                    Vector3 reflection = ShootRay(reflectedRay, x, y, ++recursion);

                    //Find new direction for the refracted ray
                    float incomingTheta = Vector3.Dot(intersectNormal, ray.direction);

                    //if the dotproducte is larger then 1 or smaller then 0 we needed to invert one of the 2 vectors
                    if (incomingTheta > 1 || incomingTheta < 0)
                        incomingTheta = -incomingTheta;
                    
                    //Calculate the k value mentioned in the slides to check if there is total internal reflection
                    float k = 1 - ((refractionIndex / primRefractionIndex) * (refractionIndex / primRefractionIndex)) * (1 - (incomingTheta * incomingTheta));
                    Vector3 T = ((refractionIndex / primRefractionIndex) * ray.direction) + (intersectNormal * (refractionIndex/primRefractionIndex * incomingTheta - (float)Math.Sqrt(k)));

                    //Calculate what part is refracted, in the formula for Fr the refractedangle is picked because with incoming angle the Fr was > 0
                    float R0 = (refractionIndex - primRefractionIndex) / (refractionIndex + primRefractionIndex);
                    R0 *= R0;
                    float Fr = R0 + (1 - R0) * (float)Math.Pow(1 - incomingTheta, 5);

                    //Create the refracted ray
                    Ray refractedRay = new Ray(intersect, T);
                    refractedRay.ShadowAcneFix(epsilon);

                    //Shoot the refracted ray
                    Vector3 refraction = ShootRay(refractedRay, x, y, ++recursion, closestPrim.refractionIndex);

                    //Calculate the colours for the refraction and the reflection part
                    if (k >= 0)
                        finalColour += ((Fr) * (closestPrim.dielectric * EntrywiseProduct(reflection, primColour))) + ((1 - Fr) * (closestPrim.dielectric * EntrywiseProduct(refraction, primColour)));
                    else finalColour += closestPrim.dielectric * EntrywiseProduct(reflection, primColour);
                }

                if (closestPrim.diffuse > 0f)
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
                            //Draw this shadowray in the debug window
                            if (debugRay)
                                debugRays.Add(new Tuple<Vector3, Vector3, int>(intersect, light.origin, debugColour[recursion]));

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
                            finalColour +=  closestPrim.diffuse * eReflected;
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

        public void DebugScreen(List<Primitive> primitives, List<PointLight> lights, Camera camera, List<Tuple<Vector3, Vector3, int>> debugRays)
        {
            cameraDebug = new Vector2(768, 400) + (10 * camera.origin.Xz);

            foreach (Tuple<Vector3, Vector3, int> r in debugRays)
            {
                if (r.Item3 != debugColour[0])
                    continue;
                Vector2 rOrigin = (r.Item1.Xz);
                Vector2 relativeOrigin = 10 * rOrigin + cameraDebug;
                Vector2 rEnd = (r.Item2.Xz);
                Vector2 relativeEnd = 10 * rEnd + cameraDebug;

                screen.Line((int)relativeOrigin.X, (int)relativeOrigin.Y, (int)relativeEnd.X, (int)relativeEnd.Y, r.Item3);

            }

            foreach (PointLight l in lights)
            {

                Vector2 lOrigin = l.origin.Xz;
                Vector2 relativeOrigin = 10 * lOrigin + cameraDebug;

                for (float i = 0; i < 360; i++)
                {
                    int x = (int)(relativeOrigin.X + (4 * Math.Cos(i)));
                    int y = (int)(relativeOrigin.Y + (4 * Math.Sin(i)));
                    screen.pixels[x + y * screen.width] = 0x999999;
                }
            }


            foreach (Primitive p in primitives)
            {
                if (p.GetType() == typeof(Sphere))
                {
                    Vector2 pOrigin = p.origin.Xz;
                    Vector2 relativeOrigin = 10 * pOrigin + cameraDebug;

                    for (float i = 0; i < 360; i++)
                    {
                        int x = (int)(relativeOrigin.X + ((p.radius * 10) * Math.Cos(i)));
                        int y = (int)(relativeOrigin.Y + ((p.radius * 10) * Math.Sin(i)));
                        screen.pixels[x + y * screen.width] = VectorToInt(p.colour);
                    }
                }
                else if (p.Normal().Y != 1 || p.Normal().Y != -1)
                {
                    //Will be added if we have any other plane then the floor plane
                    continue;
                }
            }


            screen.pixels[(int)cameraDebug.X + (int)cameraDebug.Y * screen.width] = 0xffffff;
            screen.Line((int)cameraDebug.X + (10 * (int)camera.screen.p1.X), (int)cameraDebug.Y + (10 * (int)camera.screen.p1.Z), (int)cameraDebug.X + (10 * (int)camera.screen.p2.X), (int)cameraDebug.Y + (10 * (int)camera.screen.p2.Z), 0xffffff);

        }

        public void CheckMovement()
        {
            keyPressed = false;

            if (Keyboard.GetState().IsKeyDown(Key.Space))
            {
                keyPressed = true;
                y -= 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Key.ShiftLeft))
            {
                keyPressed = true;
                y += 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Key.W))
            {
                keyPressed = true;
                z -= 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Key.S))
            {
                keyPressed = true;
                z += 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Key.A))
            {
                keyPressed = true;
                x -= 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Key.D))
            {
                keyPressed = true;
                x += 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Key.KeypadPlus))
            {
                keyPressed = true;
                newFov += 5;
            }
            if (Keyboard.GetState().IsKeyDown(Key.KeypadMinus))
            {
                keyPressed = true;
                newFov -= 5;
            }
            if (Keyboard.GetState().IsKeyDown(Key.Q))
            {
                keyPressed = true;
                xRotation -= 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Key.E))
            {
                keyPressed = true;
                xRotation += 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Key.R))
            {
                keyPressed = true;
                yRotation -= 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Key.F))
            {
                keyPressed = true;
                yRotation += 0.1f;
            }


            if (keyPressed)
                camera.UpdateCamera(x, y, z, yRotation, xRotation, newFov);
        }

        public Vector3 EntrywiseProduct(Vector3 vector1, Vector3 vector2)
        {
            return new Vector3(vector1.X * vector2.X, vector1.Y * vector2.Y, vector1.Z * vector2.Z);
        }

        public Vector2 EntrywiseProduct(Vector2 vector1, Vector2 vector2)
        {
            return new Vector2(vector1.X * vector2.X, vector1.Y * vector2.Y);
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
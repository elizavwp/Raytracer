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

    class Game
    {
        // member variables
        public Surface screen;
        public Camera camera;
        public List<Primitive> primitives;
        public List<PointLight> lights;

        // variables for the primary rays
        Ray shadowRay;
        Vector3 intersect, shadowRayDir;
        float t, closestPrim = float.MaxValue, shadowT, shadowPrimT, epsilon = 0.002f;
        bool occluder = false;

        // initialize
        public void Init()
        {
            camera = new Camera();

            //Add primitives to the scene
            primitives = new List<Primitive>();
            primitives.Add(new Plane(new Vector3(0, 1, 0), -1));
            primitives.Add(new Sphere(3, new Vector3(0, -2, -10)));

            //Add Lightsources to the scene
            lights = new List<PointLight>();
            lights.Add(new PointLight(new Vector3(0, -10, -1), 0.8f, 0xffff00));
            lights.Add(new PointLight(new Vector3(0, -1, -15), 0.3f, 0xff00ff));

        }
        // tick: renders one frame
        public void Tick()
        {
            screen.Clear(0);
            screen.Line(513, 0, 513, 512, 0xffffff);

            ShootRays();
        }

        public void ShootRays()
        {
            for (int x = 0; x < camera.pixels.GetLength(0); x++)
                for (int y = 0; y < camera.pixels.GetLength(1); y++)
                {
                    closestPrim = float.MaxValue;
                    //Find neares primitive
                    foreach (Primitive p in primitives)
                    {
                        //Find the distance from the eye to the intersection
                        t = p.Intersect(camera.pixels[x, y]);

                        //Make sure it's the closest one yet (and that is indeed visible by checking t > 0)
                        if (t > 0 && t < closestPrim)
                            closestPrim = t;
                    }

                    //There is an intersection
                    if (closestPrim < float.MaxValue)
                    {
                        //Intersection Point
                        intersect = camera.pixels[x, y].FindPoint(closestPrim);

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
                            else screen.pixels[x + y * screen.width] += (int)(light.colour * light.brightness);

                        }
                    }
                }
        }
    }
}// namespace Template
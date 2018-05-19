using System;
using OpenTK;

namespace template
{
    class Camera
    {
        public Ray[,] pixels;
        public Vector3 origin, direction, centre;
        public float fov, fovDist;
        public Plane screen;
        public Camera()
        {
            origin = Vector3.Zero;

            //Calculate the distance of the screen from the camera using an FOV
            fov = 90;
            fovDist = 1 / (float)Math.Tan(((fov * (Math.PI / 180.0f)) / 2));
            direction = new Vector3(0, 0, -1);

            //Calculate where the centre of the screen should be according to the camera position, the direction and the FOV
            centre = origin + fovDist * direction;

            //Create the screen plane using the centre of the screen and the positions of the top-left, top-right and botom-left corners
            screen = new Plane(centre + new Vector3(-1, -1, 0), centre + new Vector3(1, -1, 0), centre + new Vector3(-1, 1, 0), Vector3.Zero);
            pixels = new Ray[512, 512];

            //Create the primary rays
            PrimaryRays();
        }

        public void PrimaryRays()
        {
            for (float u = 0; u < 512; u++)
            {
                for (float v = 0; v < 512; v++)
                {
                    //Find the direction of each primary ray and create the ray
                    Vector3 puv = screen.p1 + u / 511 * (screen.p2 - screen.p1) + v / 511 * (screen.p3 - screen.p1);
                    pixels[(int)u, (int)v] = new Ray(origin, puv - origin);
                }
            }
        }

        public void UpdateCamera(float x, float y, float z, float yRotation, float xRotation, float newFov)
        {
            //If a key is pressed recalculate all the values and recalculate each primary ray with the new values.
            origin = new Vector3(x, y, z);
            fov = newFov;
            fovDist = 1 / (float)Math.Tan(((fov * (Math.PI / 180.0f)) / 2));
            direction = new Vector3(xRotation, yRotation, -1);
            centre = origin + fovDist * direction;
            screen = new Plane(centre + new Vector3(-1, -1, 0), centre + new Vector3(1, -1, 0), centre + new Vector3(-1, 1, 0), Vector3.Zero);
            pixels = new Ray[512, 512];
            PrimaryRays();
        }
    }
}

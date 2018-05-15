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
    class Camera
    {
        public Ray[,] pixels;
        public Vector3 origin, direction, centre;
        public float fov, fovDist;
        public Plane screen;
        public Camera()
        {
            origin = Vector3.Zero;
            fov = 90;
            fovDist = 1 / (float)Math.Tan(((fov * (Math.PI / 180.0f)) / 2));
            direction = new Vector3(0, 0, -1);
            centre = origin + fovDist * direction;
            screen = new Plane(centre + new Vector3(-1, -1, 0), centre + new Vector3(1, -1, 0), centre + new Vector3(-1, 1, 0), Vector3.Zero);
            pixels = new Ray[512, 512];
            PrimaryRays();
        }

        public void PrimaryRays()
        {
            for (float u = 0; u < 512; u++)
            {
                for (float v = 0; v < 512; v++)
                {
                    Vector3 puv = screen.p1 + u / 511 * (screen.p2 - screen.p1) + v / 511 * (screen.p3 - screen.p1);
                    pixels[(int)u, (int)v] = new Ray(origin, puv - origin);
                }
            }
        }
    }
}

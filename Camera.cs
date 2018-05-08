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
        Vector3 origin, direction, centre;
        int fov;
        Plane screen;
        public Camera()
        {
            origin = Vector3.Zero;
            fov = 1;
            direction = new Vector3(0, 0, -1);
            centre = origin + fov * direction;
            screen = new Plane(centre + new Vector3(-1, -1, 0), centre + new Vector3(1, -1, 0), centre + new Vector3(-1, 1, 0));
        }
    }
}

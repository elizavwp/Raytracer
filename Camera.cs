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
        Vector3 origin, direction;
        public Camera()
        {
            origin = Vector3.Zero;
            direction = new Vector3(0, 0, 1);
        }
    }
}

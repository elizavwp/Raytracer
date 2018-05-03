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
        Vector3 origin;
        public Camera(Vector3 origin)
        {
            this.origin = origin;
        }
    }
}

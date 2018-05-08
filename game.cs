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
        // initialize
        public void Init()
        {
            camera = new Camera();
            primitives = new List<Primitive>();
            primitives.Add(new Sphere(3, new Vector3(0, 0, -7)));
            primitives.Add(new Plane(new Vector3(1, 0, 0), 10));
        }
        // tick: renders one frame
        public void Tick()
        {
            screen.Clear(0);
            screen.Line(513, 0, 513, 512, 0xffffff);

            //Shoot Rays
            for (int x = 0; x < camera.pixels.GetLength(0); x++)
                for (int y = 0; y < camera.pixels.GetLength(1); y++)
                    foreach (Primitive p in primitives)
                    {
                        float color = p.Intersect(camera.pixels[x, y]);
                        if (color > 0)
                            screen.pixels[x + y * screen.width] = 0xffffff * (int)color;
                    }
        }
    }

} // namespace Template
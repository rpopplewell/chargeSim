using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using ChargeSim;
using System.Collections.Generic;

namespace ChargeSimVisualizer
{
    class GraphicsMain
    {
        [STAThread]
        public static void Main()
        {
            GraphicsMain graphics = new GraphicsMain();
            ChargeSystem sim = new ChargeSystem();
            List<ChargeSystem.Charge> particles = new List<ChargeSystem.Charge>();
            ChargeSystem.Charge particle = new ChargeSystem.Charge(0,0,0,0);

            /*
            * Charge System Initial Conditions
            */
          ////////////////////////////////////////
            for (int i = -5; i <= 5; i++) {
                sim.NewCharge(i, 0, 1E-4, 1E-5);
            }
            sim.NewBoundary(-50, 50, 0, 0);
          ////////////////////////////////////////

            using (var app = new GameWindow(900, 600))
            {
                app.Load += (sender, e) =>
                {
                    // setup settings, load textures, sounds
                    app.Title = "ChargeSim Visualizer";
                    app.VSync = VSyncMode.On;
                };

                app.Resize += (sender, e) =>
                {
                    GL.Viewport(0, 0, app.Width, app.Height);
                };

                app.UpdateFrame += (sender, e) =>
                {
                    // Update Particles
                    sim.UpdateSystem();
                    particles = sim.GetSystemState();
                    if (app.Keyboard[Key.Escape])
                    {
                        app.Exit();
                    }
                };

                app.RenderFrame += (sender, e) =>
                {
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                    GL.MatrixMode(MatrixMode.Projection);
                    GL.LoadIdentity();

                    GL.Ortho(-50.0, 50.0, -35.0, 35.0, 0.0, 4.0);
                    for (int i = 0; i < particles.Count; i++) {
                        GL.Begin(PrimitiveType.LineLoop);
                        particle = particles[i];
                        graphics.DrawCircle(particle.x, particle.y, 1, 20);
                        GL.End();
                    }
                    app.SwapBuffers();
                };
                app.Run(60.0);
            }
        }

        void DrawCircle(double cx, double cy, double r, double num_segments) {
            double theta = 2 * 3.1415926 / num_segments;
            double tangetial_factor = Math.Tan(theta);
            double radial_factor = Math.Cos(theta);
            double x = r;
            double y = 0;

            for (int ii = 0; ii < num_segments; ii++) {
                GL.Vertex2(x + cx, y + cy);
                double tx = -y;
                double ty = x; 
                x += tx * tangetial_factor;
                y += ty * tangetial_factor;
                x *= radial_factor;
                y *= radial_factor;
            }
        }
    }
}
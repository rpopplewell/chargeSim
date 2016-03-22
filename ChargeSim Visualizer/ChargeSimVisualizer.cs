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
            for (int i = -4; i <= 4; i++) {
                for (int j = -4; j <= 4; j++) {
                    sim.NewCharge(i*0.5, j*0.5, 1, 1);
                }
            }
            sim.NewBoundary(-4, 4, -4, 4);
            ////////////////////////////////////////

            using (var app = new GameWindow(900, 900))
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
                    for (int i = 0; i < 5; i++) {
                        sim.UpdateSystem();
                    }
                    particles = sim.GetSystemState();
                    if (Keyboard.GetState()[Key.Escape])
                    {
                        app.Exit();
                    }
                };

                app.RenderFrame += (sender, e) =>
                {
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                    GL.MatrixMode(MatrixMode.Projection);
                    GL.LoadIdentity();

                    GL.Ortho(-10.0, 10.0, -10.0, 10.0, 0.0, 4.0);
                    for (int i = 0; i < particles.Count; i++) {
                        particle = particles[i];
                        GL.Begin(PrimitiveType.TriangleFan);
                        graphics.DrawCircle(particle.x, particle.y, 0.05, 20.0);
                        GL.End();
                    }
                    app.SwapBuffers();
                };
                app.Run(200.0);
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
using System;
using System.Collections.Generic;
using System.IO;
using Apos.Camera;
using Apos.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace DocImages {
    /// <summary>
    /// Renders the images in the docs folder. Run it from anywhere inside the repo:
    /// dotnet run --project Tools/DocImages
    /// Each shot resizes the back buffer to the image's size so the viewports cover it, draws to a render target and saves it.
    /// </summary>
    public class Program : Game {
        public Program() {
            _graphics = new GraphicsDeviceManager(this) {
                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 450,
            };
        }

        protected override void LoadContent() {
            _sb = new ShapeBatch(GraphicsDevice);
            _effect = new BasicEffect(GraphicsDevice) { VertexColorEnabled = true };
        }

        protected override void Update(GameTime gameTime) {
            string docs = FindDocs();
            foreach (var (path, width, height, draw) in Shots()) {
                Shot(Path.Combine(docs, path), width, height, draw);
                Console.WriteLine(path);
            }
            Exit();
        }

        private IEnumerable<(string, int, int, Action)> Shots() {
            yield return ("getting-started/world.png", 800, 450, () => {
                using DefaultViewport viewport = new(GraphicsDevice, Window);
                DrawWorld(new Camera(viewport));
            });
            yield return ("getting-started/moved.png", 800, 450, () => {
                using DefaultViewport viewport = new(GraphicsDevice, Window);
                DrawWorld(new Camera(viewport) {
                    XY = new Vector2(100, 50),
                    Scale = new Vector2(2f, 2f),
                    Rotation = MathHelper.PiOver4,
                });
            });

            yield return ("camera/view-rect.png", 800, 450, () => {
                using DefaultViewport viewport = new(GraphicsDevice, Window);
                Camera camera = new(viewport) {
                    XY = new Vector2(100, 50),
                    Scale = new Vector2(2f, 2f),
                    Rotation = MathHelper.PiOver4,
                };
                Camera overview = new(viewport) {
                    XY = new Vector2(100, 50),
                    Scale = new Vector2(0.6f),
                };
                DrawWorld(overview, after: () => {
                    Vector2[] corners = [
                        camera.ScreenToWorld(0, 0),
                        camera.ScreenToWorld(viewport.Width, 0),
                        camera.ScreenToWorld(viewport.Width, viewport.Height),
                        camera.ScreenToWorld(0, viewport.Height),
                    ];
                    for (int i = 0; i < corners.Length; i++) {
                        _sb.FillLine(corners[i], corners[(i + 1) % corners.Length], 2.5f, Color.White);
                    }
                    RectangleF rect = camera.ViewRect;
                    _sb.BorderRectangle(new Vector2(rect.X, rect.Y), new Vector2(rect.Width, rect.Height), Yellow, 5f, dash: new DashStyle(20f, 14f));
                });
            });

            yield return ("viewports/split-screen.png", 800, 450, () => {
                using SplitViewport left = new(GraphicsDevice, Window, 0f, 0f, 0.5f, 1f);
                using SplitViewport right = new(GraphicsDevice, Window, 0.5f, 0f, 1f, 1f);
                Vector2 player1 = new(-200, 20);
                Vector2 player2 = new(150, -30);
                void Players() {
                    _sb.DrawCircle(player1, 22f, Blue, Color.White, 4f);
                    _sb.DrawCircle(player2, 22f, Orange, Color.White, 4f);
                }
                DrawWorld(new Camera(left) { XY = player1 }, after: Players);
                DrawWorld(new Camera(right) { XY = player2, Scale = new Vector2(1.4f) }, after: Players);

                _sb.Begin();
                _sb.FillLine(new Vector2(right.X, 0), new Vector2(right.X, right.Height), 1.5f, Color.White);
                _sb.End();
            });
            yield return ("viewports/density-wide.png", 800, 450, () => DrawDensity());
            yield return ("viewports/density-tall.png", 450, 600, () => DrawDensity());

            yield return ("depth/parallax-1.png", 800, 450, () => DrawParallax(0f));
            yield return ("depth/parallax-2.png", 800, 450, () => DrawParallax(300f));

            yield return ("3d/pillars.png", 800, 450, () => {
                using DefaultViewport viewport = new(GraphicsDevice, Window);
                Camera camera = new(viewport) { Rotation = 0.2f };
                (Vector2 Center, Color Color)[] pillars = [
                    (new(-300, -110), Blue),
                    (new(-60, -150), Red),
                    (new(210, -110), Yellow),
                    (new(-210, 120), Green),
                    (new(60, 110), Purple),
                    (new(310, 100), Orange),
                ];
                const float half = 45f;
                const float depth = -0.5f;

                camera.SetViewport();

                _sb.Begin(camera.GetView(depth));
                DrawGrid();
                foreach (var p in pillars) {
                    _sb.FillRectangle(p.Center - new Vector2(half), new Vector2(half * 2f), p.Color * 0.25f);
                }
                _sb.End();

                List<VertexPositionColor> walls = [];
                foreach (var p in pillars) {
                    Vector2[] corners = [
                        p.Center + new Vector2(-half, -half),
                        p.Center + new Vector2(half, -half),
                        p.Center + new Vector2(half, half),
                        p.Center + new Vector2(-half, half),
                    ];
                    Color top = Color.Lerp(p.Color, Color.Black, 0.3f);
                    Color bottom = Color.Lerp(p.Color, Color.Black, 0.8f);
                    for (int i = 0; i < corners.Length; i++) {
                        Vector2 a = corners[i];
                        Vector2 b = corners[(i + 1) % corners.Length];
                        walls.Add(new(new Vector3(a, 0f), top));
                        walls.Add(new(new Vector3(b, 0f), top));
                        walls.Add(new(new Vector3(b, depth), bottom));
                        walls.Add(new(new Vector3(b, depth), bottom));
                        walls.Add(new(new Vector3(a, depth), bottom));
                        walls.Add(new(new Vector3(a, 0f), top));
                    }
                }
                _effect.World = Matrix.Identity;
                _effect.View = camera.GetView3D();
                _effect.Projection = camera.GetProjection3D();
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                GraphicsDevice.RasterizerState = RasterizerState.CullNone;
                GraphicsDevice.BlendState = BlendState.Opaque;
                foreach (EffectPass pass in _effect.CurrentTechnique.Passes) {
                    pass.Apply();
                    GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, walls.ToArray(), 0, walls.Count / 3);
                }

                _sb.Begin(camera.View);
                foreach (var p in pillars) {
                    _sb.FillRectangle(p.Center - new Vector2(half), new Vector2(half * 2f), p.Color);
                }
                _sb.End();

                camera.ResetViewport();
            });
        }

        private void DrawWorld(Camera camera, float z = 0f, Action? after = null) {
            camera.SetViewport();
            _sb.Begin(camera.GetView(z));
            DrawGrid();
            _sb.FillCircle(new Vector2(-220, -90), 55f, Blue);
            _sb.FillRectangle(new Vector2(60, -170), new Vector2(150, 90), Red, 16f);
            _sb.FillHexagon(new Vector2(240, 70), 60f, Yellow, 8f);
            _sb.FillTriangle(new Vector2(-130, 60), new Vector2(-30, 170), new Vector2(-230, 170), Green, 8f);
            _sb.FillCircle(new Vector2(40, 40), 30f, Purple);
            _sb.FillCircle(Vector2.Zero, 6f, Color.White);
            after?.Invoke();
            _sb.End();
            camera.ResetViewport();
        }

        private void DrawGrid() {
            for (int i = -1500; i <= 1500; i += 50) {
                Color c = i == 0 ? Axis : i % 250 == 0 ? GridMajor : GridMinor;
                _sb.FillLine(new Vector2(i, -1500), new Vector2(i, 1500), 1f, c);
                _sb.FillLine(new Vector2(-1500, i), new Vector2(1500, i), 1f, c);
            }
        }

        private void DrawDensity() {
            using DensityViewport viewport = new(GraphicsDevice, Window, 400, 400);
            DrawWorld(new Camera(viewport), after: () => {
                _sb.BorderRectangle(new Vector2(-198, -198), new Vector2(396, 396), Color.White, 3f, dash: new DashStyle(14f, 10f));
            });
        }

        private void DrawParallax(float x) {
            using DefaultViewport viewport = new(GraphicsDevice, Window);
            Camera camera = new(viewport) { X = x };
            camera.SetViewport();

            _sb.Begin(camera.GetView(-3));
            _sb.FillCircle(new Vector2(-1300, -250), 450f, new Color(30, 41, 82));
            _sb.FillCircle(new Vector2(-50, -550), 350f, new Color(37, 48, 95));
            _sb.FillCircle(new Vector2(1100, -200), 500f, new Color(30, 41, 82));
            _sb.FillCircle(new Vector2(2300, -450), 400f, new Color(37, 48, 95));
            _sb.End();

            _sb.Begin(camera.GetView(-1));
            for (int i = 0; i < 12; i++) {
                float height = 200 + i * 137 % 250;
                _sb.FillRectangle(new Vector2(-900 + i * 220, 450 - height), new Vector2(160, height + 100), new Color(55, 72, 130), 12f);
            }
            _sb.End();

            _sb.Begin(camera.View);
            _sb.FillRectangle(new Vector2(-2000, 140), new Vector2(4000, 200), new Color(20, 83, 45));
            _sb.FillRectangle(new Vector2(-330, 60), new Vector2(80, 80), Red, 10f);
            _sb.FillRectangle(new Vector2(180, 60), new Vector2(80, 80), Yellow, 10f);
            _sb.FillRectangle(new Vector2(560, 60), new Vector2(80, 80), Purple, 10f);
            _sb.DrawCircle(new Vector2(x, 100), 40f, Blue, Color.White, 4f);
            _sb.End();

            camera.ResetViewport();
        }

        private void Shot(string path, int width, int height, Action draw) {
            if (_graphics.PreferredBackBufferWidth != width || _graphics.PreferredBackBufferHeight != height) {
                _graphics.PreferredBackBufferWidth = width;
                _graphics.PreferredBackBufferHeight = height;
                _graphics.ApplyChanges();
            }

            using RenderTarget2D target = new(GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            GraphicsDevice.SetRenderTarget(target);
            GraphicsDevice.Clear(Background);
            draw();
            GraphicsDevice.SetRenderTarget(null);

            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            using FileStream f = File.Create(path);
            target.SaveAsPng(f, width, height);
        }

        private static string FindDocs() {
            for (DirectoryInfo? d = new(AppContext.BaseDirectory); d != null; d = d.Parent) {
                if (Directory.Exists(Path.Combine(d.FullName, "docs")) && Directory.Exists(Path.Combine(d.FullName, "Source"))) {
                    return Path.Combine(d.FullName, "docs");
                }
            }
            throw new Exception("Couldn't find the docs folder.");
        }

        private static readonly Color Background = new(17, 24, 39);
        private static readonly Color GridMinor = new(26, 34, 50);
        private static readonly Color GridMajor = new(40, 50, 68);
        private static readonly Color Axis = new(70, 80, 100);
        private static readonly Color Blue = new(96, 165, 250);
        private static readonly Color Red = new(248, 113, 113);
        private static readonly Color Yellow = new(250, 204, 21);
        private static readonly Color Green = new(74, 222, 128);
        private static readonly Color Purple = new(192, 132, 252);
        private static readonly Color Orange = new(251, 146, 60);

        private readonly GraphicsDeviceManager _graphics;
        private ShapeBatch _sb = null!;
        private BasicEffect _effect = null!;

        static void Main() {
            using Program p = new();
            p.Run();
        }
    }
}

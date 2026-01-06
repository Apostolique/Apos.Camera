using System;
using Apos.Camera;
using Apos.Input;
using Apos.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameProject {
    public class GameRoot : Game {
        public GameRoot() {
            _graphics = new GraphicsDeviceManager(this);
#if KNI
            _graphics.GraphicsProfile = GraphicsProfile.FL10_0;
#else
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
#endif
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
        }

        protected override void Initialize() {
            base.Initialize();
        }

        protected override void LoadContent() {
            _s = new SpriteBatch(GraphicsDevice);
            _sb = new ShapeBatch(GraphicsDevice, Content);

            InputHelper.Setup(this);

            _superViewport1 = new SplitViewport(GraphicsDevice, Window, 0f, 0f, 0.3f, 0.7f);
            _superViewport2 = new SplitViewport(GraphicsDevice, Window, 0f, 0.7f, 0.7f, 1f);
            _superViewport3 = new SplitViewport(GraphicsDevice, Window, 0.7f, 0.3f, 1f, 1f);
            _superViewport4 = new SplitViewport(GraphicsDevice, Window, 0.3f, 0f, 1f, 0.3f);
            _superViewport5 = new SplitViewport(GraphicsDevice, Window, 0.3f, 0.3f, 0.7f, 0.7f);
            _superViewport6 = new DensityViewport(GraphicsDevice, Window, 1000, 1000);
            _camera1 = new Camera(_superViewport1);
            _camera2 = new Camera(_superViewport2);
            _camera3 = new Camera(_superViewport3);
            _camera4 = new Camera(_superViewport4);
            _camera5 = new Camera(_superViewport5);
            _camera6 = new Camera(_superViewport6);

            _apos = Content.Load<Texture2D>("apos");
            _pixel = Content.Load<Texture2D>("pixel");

            _basicEffect = new BasicEffect(GraphicsDevice);

            VertexPositionColor[] vertices = new VertexPositionColor[6];
            vertices[0] = new VertexPositionColor(new Vector3(0, 0, -1), Color.Red);
            vertices[1] = new VertexPositionColor(new Vector3(339, 0, -1), Color.Red);
            vertices[2] = new VertexPositionColor(new Vector3(339, 0, 0), Color.Blue);

            vertices[3] = new VertexPositionColor(new Vector3(339, 0, 0), Color.Blue);
            vertices[4] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Blue);
            vertices[5] = new VertexPositionColor(new Vector3(0, 0, -1), Color.Red);

            _vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 6, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(vertices);
        }

        protected override void UnloadContent() {
            _superViewport1.Dispose();
            _superViewport2.Dispose();
        }

        protected override void Update(GameTime gameTime) {
            InputHelper.UpdateSetup();

            if (_quit.Pressed())
                Exit();

            if (SetSplitViews.Pressed()) {
                _currentView = View.Split;
            }
            if (SetDensityView.Pressed()) {
                _currentView = View.Density;
            }

            if (_currentView == View.Split) {
                UpdateCameraInput(_camera1, 1);
                UpdateCameraInput(_camera2, 2);
                UpdateCameraInput(_camera3, 3);
                UpdateCameraInput(_camera4, 4);
                UpdateCameraInput(_camera5, 5);
            } else if (_currentView == View.Density) {
                UpdateCameraInput(_camera6, 6);
            }

            InputHelper.UpdateCleanup();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            if (_currentView == View.Split) {
                DrawCamera(_camera1);
                DrawCamera(_camera2);
                DrawCamera(_camera3);
                DrawCamera(_camera4);
                DrawCamera(_camera5);
            } else if (_currentView == View.Density) {
                DrawCamera(_camera6);
            }

            _s.Begin();
            if (_currentView == View.Split) {
                DrawViewportBorder(_s, _camera1);
                DrawViewportBorder(_s, _camera2);
                DrawViewportBorder(_s, _camera3);
                DrawViewportBorder(_s, _camera4);
                DrawViewportBorder(_s, _camera5);
            } else if (_currentView == View.Density) {
                DrawViewportBorder(_s, _camera6);
            }
            _s.End();

            base.Draw(gameTime);
        }

        private void DrawCamera(Camera c) {
            c.SetViewport();
            _sb.Begin(c.GetView(-1));
            _sb.FillCircle(new Vector2(150, -70), 50f, Color.White);
            _sb.End();

            _s.Begin(transformMatrix: c.GetView(-1));
            _s.Draw(_apos, new Vector2(0, 0), Color.White);
            _s.Draw(_apos, new Vector2(200, 0), Color.White);
            _s.End();

            _basicEffect.World = Matrix.Identity;
            _basicEffect.View = c.GetView3D();
            _basicEffect.Projection = c.GetProjection3D();

            _basicEffect.VertexColorEnabled = true;

            GraphicsDevice.SetVertexBuffer(_vertexBuffer);

            RasterizerState rasterizerState = new RasterizerState { CullMode = CullMode.None };

            GraphicsDevice.RasterizerState = rasterizerState;

            foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes) {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
            }

            _s.Begin(transformMatrix: c.View);
            _s.Draw(_apos, Vector2.Zero, Color.White);
            _s.Draw(_apos, new Vector2(200, 0), Color.White);
            _s.End();

            _s.Begin();
            _s.Draw(_apos, -c.VirtualViewport.XY + c.WorldToScreen(_mouseWorld), Color.White);
            _s.End();

            c.ResetViewport();
        }
        private void DrawViewportBorder(SpriteBatch s, Camera c) {
            IVirtualViewport v = c.VirtualViewport;

            s.Draw(_pixel, new Rectangle(v.X, v.Y - 1, v.Width, 2), Color.White);
            s.Draw(_pixel, new Rectangle(v.X + v.Width - 1, v.Y, 2, v.Height), Color.White);
            s.Draw(_pixel, new Rectangle(v.X, v.Y + v.Height - 1, v.Width, 2), Color.White);
            s.Draw(_pixel, new Rectangle(v.X - 1, v.Y, 2, v.Height), Color.White);
        }

        private bool CameraContains(Camera camera, int x, int y) {
            IVirtualViewport v = camera.VirtualViewport;
            return !(x <= v.X || v.X + v.Width < x || y <= v.Y || v.Y + v.Height < y);
        }
        private void UpdateCameraInput(Camera c, int index) {
            int x = InputHelper.NewMouse.X;
            int y = InputHelper.NewMouse.Y;

            if (!_isDragged && CameraContains(c, x, y) || _isDragged && _currentCamera == index) {
                _currentCamera = index;
                if (MouseCondition.Scrolled()) {
                    int scrollDelta = MouseCondition.ScrollDelta;
                    SetZoom(c, MathF.Min(MathF.Max(GetZoom(c) - scrollDelta * 0.001f, 0.2f), 10f));
                }

                if (RotateLeft.Pressed()) {
                    c.Rotation += MathHelper.PiOver4;
                }
                if (RotateRight.Pressed()) {
                    c.Rotation -= MathHelper.PiOver4;
                }

                if (Forward.Held()) {
                    c.Z -= 0.1f;
                }
                if (Backward.Held()) {
                    c.Z += 0.1f;
                }

                if (IncreaseFocal.Held()) {
                    var temp = c.Z / c.FocalLength;

                    c.FocalLength += 0.01f;

                    c.Z = c.FocalLength * temp;
                }
                if (DecreaseFocal.Held()) {
                    var temp = c.Z / c.FocalLength;

                    c.FocalLength -= 0.01f;

                    c.Z = c.FocalLength * temp;
                }

                if (ResetCamera.Pressed()) {
                    c.Scale = new Vector2(1f, 1f);
                    c.XY = new Vector2(0f, 0f);
                    c.Z = 2f;
                    c.Rotation = 0f;
                    c.FocalLength = 1f;
                }

                _mouseWorld = c.ScreenToWorld(x, y);

                if (CameraDrag.Pressed()) {
                    _dragAnchor = _mouseWorld;
                    _isDragged = true;
                }
                if (_isDragged && CameraDrag.HeldOnly()) {
                    c.XY += _dragAnchor - _mouseWorld;
                    _mouseWorld = _dragAnchor;
                }
                if (_isDragged && CameraDrag.Released()) {
                    _isDragged = false;
                }
            }
        }

        private float GetZoom(Camera camera) {
            return MathF.Log(camera.ScaleToZ(camera.Scale.X, 0f) + 1);
        }
        private void SetZoom(Camera camera, float value) {
            camera.Scale = new Vector2(camera.ZToScale(MathF.Exp(value) - 1, 0f));
        }

        GraphicsDeviceManager _graphics;
        SpriteBatch _s;
        ShapeBatch _sb;

        ICondition _quit =
            new AnyCondition(
                new KeyboardCondition(Keys.Escape),
                new GamePadCondition(GamePadButton.Back, 0)
            );
        ICondition RotateLeft = new KeyboardCondition(Keys.OemComma);
        ICondition RotateRight = new KeyboardCondition(Keys.OemPeriod);

        ICondition CameraDrag = new MouseCondition(MouseButton.MiddleButton);
        ICondition ResetCamera = new KeyboardCondition(Keys.Space);
        ICondition SetSplitViews = new KeyboardCondition(Keys.D1);
        ICondition SetDensityView = new KeyboardCondition(Keys.D2);

        ICondition Forward = new KeyboardCondition(Keys.W);
        ICondition Backward = new KeyboardCondition(Keys.S);

        ICondition IncreaseFocal = new KeyboardCondition(Keys.D);
        ICondition DecreaseFocal = new KeyboardCondition(Keys.E);

        enum View {
            Split,
            Density
        }
        View _currentView = View.Split;

        Texture2D _apos;
        Texture2D _pixel;

        Vector2 _mouseWorld = Vector2.Zero;
        Vector2 _dragAnchor = Vector2.Zero;
        bool _isDragged = false;

        int _currentCamera = 0;

        IVirtualViewport _superViewport1;
        IVirtualViewport _superViewport2;
        IVirtualViewport _superViewport3;
        IVirtualViewport _superViewport4;
        IVirtualViewport _superViewport5;
        IVirtualViewport _superViewport6;
        Camera _camera1;
        Camera _camera2;
        Camera _camera3;
        Camera _camera4;
        Camera _camera5;
        Camera _camera6;

        VertexBuffer _vertexBuffer;
        BasicEffect _basicEffect;
    }
}

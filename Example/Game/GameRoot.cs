using System;
using Apos.Camera;
using Apos.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameProject {
    public class GameRoot : Game {
        public GameRoot() {
            _graphics = new GraphicsDeviceManager(this);
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
        }

        protected override void Initialize() {
            base.Initialize();
        }

        protected override void LoadContent() {
            _s = new SpriteBatch(GraphicsDevice);

            InputHelper.Setup(this);

            _superViewport1 = new SplitViewport(GraphicsDevice, Window, 0f, 0f, 0.3f, 0.7f);
            _superViewport2 = new SplitViewport(GraphicsDevice, Window, 0f, 0.7f, 0.7f, 1f);
            _superViewport3 = new SplitViewport(GraphicsDevice, Window, 0.7f, 0.3f, 1f, 1f);
            _superViewport4 = new SplitViewport(GraphicsDevice, Window, 0.3f, 0f, 1f, 0.3f);
            _superViewport5 = new SplitViewport(GraphicsDevice, Window, 0.3f, 0.3f, 0.7f, 0.7f);
            _camera1 = new Camera(_superViewport1);
            _camera2 = new Camera(_superViewport2);
            _camera3 = new Camera(_superViewport3);
            _camera4 = new Camera(_superViewport4);
            _camera5 = new Camera(_superViewport5);

            _apos = Content.Load<Texture2D>("apos");
            _pixel = Content.Load<Texture2D>("pixel");
        }

        protected override void UnloadContent() {
            _superViewport1.Dispose();
            _superViewport2.Dispose();
        }

        protected override void Update(GameTime gameTime) {
            InputHelper.UpdateSetup();

            if (_quit.Pressed())
                Exit();

            UpdateCameraInput(_camera1, 1);
            UpdateCameraInput(_camera2, 2);
            UpdateCameraInput(_camera3, 3);
            UpdateCameraInput(_camera4, 4);
            UpdateCameraInput(_camera5, 5);

            InputHelper.UpdateCleanup();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            DrawCamera(_camera1);
            DrawCamera(_camera2);
            DrawCamera(_camera3);
            DrawCamera(_camera4);
            DrawCamera(_camera5);

            _s.Begin();
            DrawViewportBorder(_s, _camera1);
            DrawViewportBorder(_s, _camera2);
            DrawViewportBorder(_s, _camera3);
            DrawViewportBorder(_s, _camera4);
            DrawViewportBorder(_s, _camera5);
            _s.End();

            base.Draw(gameTime);
        }

        private void DrawCamera(Camera c) {
            c.SetViewport();
            _s.Begin(transformMatrix: c.View);
            _s.Draw(_apos, Vector2.Zero, Color.White);
            _s.Draw(_apos, _mouseWorld, Color.White);
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

            if (!_isDragged && CameraContains(c, x, y) || _isDragged && _current == index) {
                _current = index;
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

        ICondition _quit =
            new AnyCondition(
                new KeyboardCondition(Keys.Escape),
                new GamePadCondition(GamePadButton.Back, 0)
            );
        ICondition RotateLeft = new KeyboardCondition(Keys.OemComma);
        ICondition RotateRight = new KeyboardCondition(Keys.OemPeriod);

        ICondition CameraDrag = new MouseCondition(MouseButton.MiddleButton);

        Texture2D _apos;
        Texture2D _pixel;

        Vector2 _mouseWorld = Vector2.Zero;
        Vector2 _dragAnchor = Vector2.Zero;
        bool _isDragged = false;

        int _current = 0;

        IVirtualViewport _superViewport1;
        IVirtualViewport _superViewport2;
        IVirtualViewport _superViewport3;
        IVirtualViewport _superViewport4;
        IVirtualViewport _superViewport5;
        Camera _camera1;
        Camera _camera2;
        Camera _camera3;
        Camera _camera4;
        Camera _camera5;
    }
}

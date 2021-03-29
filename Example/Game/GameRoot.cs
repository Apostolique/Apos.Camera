using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Apos.Input;
using Apos.Camera;
using System;

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
            _camera1 = new Camera(GraphicsDevice, _superViewport1);
            _camera2 = new Camera(GraphicsDevice, _superViewport2);
            _camera3 = new Camera(GraphicsDevice, _superViewport3);
            _camera4 = new Camera(GraphicsDevice, _superViewport4);
            _camera5 = new Camera(GraphicsDevice, _superViewport5);

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

            int x = InputHelper.NewMouse.X;
            int y = InputHelper.NewMouse.Y;

            if (!_isDragged && CameraContains(_camera1, x, y) || _isDragged && _current == 1) {
                _current = 1;
                UpdateCameraInput(_camera1);
            }
            if (!_isDragged && CameraContains(_camera2, x, y) || _isDragged && _current == 2) {
                _current = 2;
                UpdateCameraInput(_camera2);
            }
            if (!_isDragged && CameraContains(_camera3, x, y) || _isDragged && _current == 3) {
                _current = 3;
                UpdateCameraInput(_camera3);
            }
            if (!_isDragged && CameraContains(_camera4, x, y) || _isDragged && _current == 4) {
                _current = 4;
                UpdateCameraInput(_camera4);
            }
            if (!_isDragged && CameraContains(_camera5, x, y) || _isDragged && _current == 5) {
                _current = 5;
                UpdateCameraInput(_camera5);
            }


            InputHelper.UpdateCleanup();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            _camera1.SetViewport();
            _s.Begin(transformMatrix: _camera1.View);
            _s.Draw(_apos, Vector2.Zero, Color.White);
            _s.Draw(_apos, _mouseWorld, Color.White);
            _s.End();
            _camera1.ResetViewport();

            _camera2.SetViewport();
            _s.Begin(transformMatrix: _camera2.View);
            _s.Draw(_apos, Vector2.Zero, Color.White);
            _s.Draw(_apos, _mouseWorld, Color.White);
            _s.End();
            _camera2.ResetViewport();

            _camera3.SetViewport();
            _s.Begin(transformMatrix: _camera3.View);
            _s.Draw(_apos, Vector2.Zero, Color.White);
            _s.Draw(_apos, _mouseWorld, Color.White);
            _s.End();
            _camera3.ResetViewport();

            _camera4.SetViewport();
            _s.Begin(transformMatrix: _camera4.View);
            _s.Draw(_apos, Vector2.Zero, Color.White);
            _s.Draw(_apos, _mouseWorld, Color.White);
            _s.End();
            _camera4.ResetViewport();

            _camera5.SetViewport();
            _s.Begin(transformMatrix: _camera5.View);
            _s.Draw(_apos, Vector2.Zero, Color.White);
            _s.Draw(_apos, _mouseWorld, Color.White);
            _s.End();
            _camera5.ResetViewport();

            _s.Begin();
            DrawViewportBorder(_s, _camera1);
            DrawViewportBorder(_s, _camera2);
            DrawViewportBorder(_s, _camera3);
            DrawViewportBorder(_s, _camera4);
            DrawViewportBorder(_s, _camera5);
            _s.End();

            base.Draw(gameTime);
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
        private void UpdateCameraInput(Camera camera) {
            if (MouseCondition.Scrolled()) {
                int scrollDelta = MouseCondition.ScrollDelta;
                SetZoom(camera, MathF.Min(MathF.Max(GetZoom(camera) - scrollDelta * 0.001f, 0.2f), 10f));
            }

            if (RotateLeft.Pressed()) {
                camera.Rotation += MathHelper.PiOver4;
            }
            if (RotateRight.Pressed()) {
                camera.Rotation -= MathHelper.PiOver4;
            }

            _mouseWorld = camera.ScreenToWorld(InputHelper.NewMouse.X, InputHelper.NewMouse.Y);

            if (CameraDrag.Pressed()) {
                _dragAnchor = _mouseWorld;
                _isDragged = true;
            }
            if (_isDragged && CameraDrag.HeldOnly()) {
                camera.XY += _dragAnchor - _mouseWorld;
                _mouseWorld = _dragAnchor;
            }
            if (_isDragged && CameraDrag.Released()) {
                _isDragged = false;
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

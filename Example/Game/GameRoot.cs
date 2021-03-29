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

            _superViewport1 = new HalfViewport(GraphicsDevice, Window, true);
            _superViewport2 = new HalfViewport(GraphicsDevice, Window, false);
            _camera1 = new Camera(GraphicsDevice, _superViewport1);
            _camera2 = new Camera(GraphicsDevice, _superViewport2);

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

            if ((!_isDragged && InputHelper.NewMouse.X < GraphicsDevice.PresentationParameters.BackBufferWidth / 2 || _isDragged && _editing == 1)) {
                _editing = 1;
                UpdateCameraInput(_camera1);
            } else if (!_isDragged && InputHelper.NewMouse.X >= GraphicsDevice.PresentationParameters.BackBufferWidth / 2 || _isDragged && _editing == 2) {
                _editing = 2;
                UpdateCameraInput(_camera2);
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

            _s.Begin();
            _s.Draw(_pixel, new Rectangle(0 + GraphicsDevice.PresentationParameters.BackBufferWidth / 2, 0, 2, GraphicsDevice.PresentationParameters.BackBufferHeight), Color.White);
            _s.End();

            base.Draw(gameTime);
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

        int _editing = 0;
        Camera _camera1;
        Camera _camera2;
        Texture2D _apos;
        Texture2D _pixel;

        Vector2 _mouseWorld = Vector2.Zero;
        Vector2 _dragAnchor = Vector2.Zero;
        bool _isDragged = false;

        IVirtualViewport _superViewport1;
        IVirtualViewport _superViewport2;
    }
}

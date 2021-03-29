using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apos.Camera {
    public class HalfViewport : IVirtualViewport {
        public HalfViewport(GraphicsDevice graphicsDevice, GameWindow window, bool isLeft) {
            _graphicsDevice = graphicsDevice;
            IsLeft = isLeft;

            _window = window;
            _window.ClientSizeChanged += OnClientSizeChanged;
            OnClientSizeChanged(this, EventArgs.Empty);
        }

        public void Dispose() {
            _window.ClientSizeChanged -= OnClientSizeChanged;
        }

        public bool IsLeft { get; set; }

        public int X => _viewport.X;
        public int Y => _viewport.Y;
        public int Width => _viewport.Width;
        public int Height => _viewport.Height;

        public Vector2 XY => new Vector2(X, Y);

        public Vector2 Origin => _origin;

        public float VirtualWidth => Width;
        public float VirtualHeight => Height;

        public void Set() {
            _oldViewport = _graphicsDevice.Viewport;
            _graphicsDevice.Viewport = _viewport;
        }
        public void Reset() {
            _graphicsDevice.Viewport = _oldViewport;
        }

        public Matrix Transform(Matrix view) {
            return view;
        }

        private void OnClientSizeChanged(object sender, EventArgs e) {
            int halfWidth = _graphicsDevice.PresentationParameters.BackBufferWidth / 2;

            if (IsLeft) {
                _viewport = new Viewport(0, 0, halfWidth, _graphicsDevice.PresentationParameters.BackBufferHeight);
            } else {
                _viewport = new Viewport(0 + halfWidth, 0, halfWidth, _graphicsDevice.PresentationParameters.BackBufferHeight);
            }

            _origin = new Vector2(_viewport.Width / 2, _viewport.Height / 2);
        }

        private GraphicsDevice _graphicsDevice;
        private GameWindow _window;
        private Viewport _viewport;
        private Viewport _oldViewport;
        private Vector2 _origin;
    }
}

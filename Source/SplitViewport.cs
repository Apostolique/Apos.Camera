using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apos.Camera {
    public class SplitViewport : IVirtualViewport {
        public SplitViewport(GraphicsDevice graphicsDevice, GameWindow window, float left, float top, float right, float bottom) {
            _graphicsDevice = graphicsDevice;

            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;

            _window = window;
            _window.ClientSizeChanged += OnClientSizeChanged;
            OnClientSizeChanged(this, EventArgs.Empty);
        }

        public void Dispose() {
            _window.ClientSizeChanged -= OnClientSizeChanged;
        }

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
            int gWidth = _graphicsDevice.PresentationParameters.BackBufferWidth;
            int gHeight = _graphicsDevice.PresentationParameters.BackBufferHeight;

            _viewport = new Viewport((int)(gWidth * _left), (int)(gHeight * _top), (int)(gWidth * (_right - _left)), (int)(gHeight * (_bottom - _top)));

            _origin = new Vector2(_viewport.Width / 2f, _viewport.Height / 2f);
        }

        private GraphicsDevice _graphicsDevice;
        private GameWindow _window;
        private Viewport _viewport;
        private Viewport _oldViewport;
        private Vector2 _origin;

        private float _left;
        private float _top;
        private float _right;
        private float _bottom;
    }
}

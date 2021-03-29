using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apos.Camera {
    public class DensityViewport : IVirtualViewport {
        public DensityViewport(GraphicsDevice graphicsDevice, GameWindow window, float targetWidth, float targetHeight) {
            _graphicsDevice = graphicsDevice;
            _window = window;

            TargetWidth = targetWidth;
            TargetHeight = targetHeight;

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

        public float TargetWidth { get; set; }
        public float TargetHeight { get; set; }

        public float VirtualWidth => _virtualWidth;
        public float VirtualHeight => _virtualHeight;
        public float Ratio { get; set; }

        public Matrix GetScaleMatrix() {
            return Matrix.CreateScale(Ratio, Ratio, 1f);
        }

        public void Set() {
            _oldViewport = _graphicsDevice.Viewport;
            _graphicsDevice.Viewport = _viewport;
        }
        public void Reset() {
            _graphicsDevice.Viewport = _oldViewport;
        }

        public Matrix Transform(Matrix view) {
            return view * GetScaleMatrix();
        }

        private void OnClientSizeChanged(object sender, EventArgs e) {
            _viewport = new Viewport(0, 0, _graphicsDevice.PresentationParameters.BackBufferWidth, _graphicsDevice.PresentationParameters.BackBufferHeight);

            float ratioWidth = _viewport.Width / TargetWidth;
            float ratioHeight = _viewport.Height / TargetHeight;

            if (ratioWidth < ratioHeight) {
                Ratio = ratioWidth;

                _virtualWidth = _viewport.Width / ratioWidth;
                _virtualHeight = _viewport.Height / ratioWidth;
            } else {
                Ratio = ratioHeight;

                _virtualWidth = _viewport.Width / ratioHeight;
                _virtualHeight = _viewport.Height / ratioHeight;
            }

            _origin = new Vector2(_viewport.Width / 2f, _viewport.Height / 2f);
        }

        private GraphicsDevice _graphicsDevice;
        private GameWindow _window;

        private float _virtualWidth;
        private float _virtualHeight;

        private Viewport _viewport;
        private Viewport _oldViewport;

        private Vector2 _origin;
    }
}

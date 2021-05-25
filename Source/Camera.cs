using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Apos.Camera {
    public class Camera {
        public Camera(IVirtualViewport virtualViewport) {
            VirtualViewport = virtualViewport;
        }

        public float X {
            get => _xy.X;
            set {
                _xy.X = value;
                _xyz.X = value;
            }
        }
        public float Y {
            get => _xy.Y;
            set {
                _xy.Y = value;
                _xyz.Y = value;
            }
        }
        public float Z {
            get => _xyz.Z;
            set {
                _xyz.Z = value;
            }
        }

        public float FocalLength {
            get => _focalLength;
            set {
                _focalLength = value > 0.01f ? value : 0.01f;
            }
        }

        public float Rotation { get; set; } = 0f;
        public Vector2 Scale { get; set; } = Vector2.One;

        public Vector2 XY {
            get => _xy;
            set {
                _xy = value;
                _xyz.X = value.X;
                _xyz.Y = value.Y;
            }
        }
        public Vector3 XYZ {
            get => _xyz;
            set {
                _xyz = value;
            }
        }

        public IVirtualViewport VirtualViewport {
            get;
            set;
        }

        public void SetViewport() {
            VirtualViewport.Set();
        }
        public void ResetViewport() {
            VirtualViewport.Reset();
        }

        public Matrix View => GetView(0) ;
        public Matrix ViewInvert => GetViewInvert(0);

        public Matrix GetView(float z = 0) {
            float scaleZ = ZToScale(_xyz.Z, z);
            return VirtualViewport.Transform(
                // Matrix.CreateTranslation(new Vector3(-VirtualViewport.Origin, 0f)) * // This makes the camera position be at the top left
                Matrix.CreateTranslation(new Vector3(-XY, 0f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Scale.X, Scale.Y, 1f) *
                Matrix.CreateScale(scaleZ, scaleZ, 1f) *
                Matrix.CreateTranslation(new Vector3(VirtualViewport.Origin, 0f)));
        }
        public Matrix GetView3D() {
            return
                Matrix.CreateLookAt(XYZ, new Vector3(XY, Z - 1), new Vector3((float)Math.Sin(Rotation), (float)Math.Cos(Rotation), 0)) *
                Matrix.CreateScale(Scale.X, -Scale.Y, 1f);
        }
        public Matrix GetViewInvert(float z = 0) => Matrix.Invert(GetView(z));

        public Matrix GetProjection() {
            return Matrix.CreateOrthographicOffCenter(0, VirtualViewport.Width, VirtualViewport.Height, 0, 0, 1);
        }
        public Matrix GetProjection3D(float nearPlaneDistance = 0.01f, float farPlaneDistance = 100f) {
            var aspect = VirtualViewport.VirtualWidth / (float)VirtualViewport.VirtualHeight;
            var fov = (float)Math.Atan(VirtualViewport.VirtualHeight / 2f / FocalLength) * 2f;

            return Matrix.CreatePerspectiveFieldOfView(fov, aspect, nearPlaneDistance, farPlaneDistance);
        }

        public float ScaleToZ(float scale, float targetZ) {
            if (scale == 0) {
                return float.MaxValue;
            }
            return FocalLength / scale + targetZ;
        }
        public float ZToScale(float z, float targetZ) {
            if (z - targetZ == 0) {
                return float.MaxValue;
            }
            return 1f / ((z - targetZ) / FocalLength);
        }

        public float WorldToScreenScale(float z = 0f) => Vector2.Distance(WorldToScreen(0f, 0f, z), WorldToScreen(1f, 0f, z));
        public float ScreenToWorldScale(float z = 0f) => Vector2.Distance(ScreenToWorld(0f, 0f, z), ScreenToWorld(1f, 0f, z));

        public Vector2 WorldToScreen(float x, float y, float z = 0f) => WorldToScreen(new Vector2(x, y), z);
        public Vector2 WorldToScreen(Vector2 xy, float z = 0f) {
            return Vector2.Transform(xy, GetView(z)) + VirtualViewport.XY;
        }
        public Vector2 ScreenToWorld(float x, float y, float z = 0f) => ScreenToWorld(new Vector2(x, y), z);
        public Vector2 ScreenToWorld(Vector2 xy, float z = 0f) {
            return Vector2.Transform(xy - VirtualViewport.XY, GetViewInvert(z));
        }

        public bool IsZVisible(float z, float minDistance = 0.1f) {
            float scaleZ = ZToScale(Z, z);
            float maxScale = ZToScale(minDistance, 0f);

            return scaleZ > 0 && scaleZ < maxScale;
        }

        public RectangleF ViewRect => GetViewRect(0);
        public RectangleF GetViewRect(float z = 0) {
            var frustum = GetBoundingFrustum(z);
            var corners = frustum.GetCorners();
            var a = corners[0];
            var b = corners[1];
            var c = corners[2];
            var d = corners[3];

            var left = Math.Min(Math.Min(a.X, b.X), Math.Min(c.X, d.X));
            var right = Math.Max(Math.Max(a.X, b.X), Math.Max(c.X, d.X));

            var top = Math.Min(Math.Min(a.Y, b.Y), Math.Min(c.Y, d.Y));
            var bottom = Math.Max(Math.Max(a.Y, b.Y), Math.Max(c.Y, d.Y));

            var width = right - left;
            var height = bottom - top;

            return new RectangleF(left, top, width, height);
        }
        public BoundingFrustum GetBoundingFrustum(float z = 0) {
            // TODO: Use 3D view and projection?
            Matrix view = GetView(z);
            Matrix projection = GetProjection();
            return new BoundingFrustum(view * projection);
        }

        private Vector2 _xy = Vector2.Zero;
        private Vector3 _xyz = new Vector3(Vector2.Zero, 1f);
        private float _focalLength = 1f;
    }
}

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
                Matrix.CreateTranslation(new Vector3(-XY, 0f)) *
                Matrix.CreateTranslation(new Vector3(-VirtualViewport.Origin, 0f)) * // This makes the camera position be at the top left
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Scale.X, Scale.Y, 1f) *
                Matrix.CreateScale(scaleZ, scaleZ, 1f) *
                Matrix.CreateTranslation(new Vector3(VirtualViewport.Origin, 0f)));
        }
        public Matrix GetViewInvert(float z = 0) => Matrix.Invert(GetView(z));

        public float ScaleToZ(float scale, float targetZ) {
            if (scale + targetZ == 0) {
                return 0f;
            }
            return 1f / scale + targetZ;
        }
        public float ZToScale(float z, float targetZ) {
            if (z - targetZ == 0) {
                return 0f;
            }
            return 1f / (z - targetZ);
        }

        public float WorldToScreenScale(float z = 0f) => Vector2.Distance(WorldToScreen(0f, 0f, z), WorldToScreen(1f, 0f, z));
        public float ScreenToWorldScale(float z = 0f) => Vector2.Distance(ScreenToWorld(0f, 0f, z), ScreenToWorld(1f, 0f, z));

        public Vector2 WorldToScreen(float x, float y, float z) => WorldToScreen(new Vector2(x, y), z);
        public Vector2 WorldToScreen(Vector2 xy, float z) {
            return Vector2.Transform(xy, GetView(z)) + VirtualViewport.XY;
        }
        public Vector2 ScreenToWorld(float x, float y, float z = 0f) => ScreenToWorld(new Vector2(x, y), z);
        public Vector2 ScreenToWorld(Vector2 xy, float z) {
            return Vector2.Transform(xy - VirtualViewport.XY, GetViewInvert(z));
        }

        public bool IsZVisible(float z, float minDistance = 0.1f) {
            float scaleZ = ZToScale(Z, z);
            float maxScale = ZToScale(minDistance, 0f);

            return scaleZ > 0 && scaleZ < maxScale;
        }

        private Vector2 _xy = Vector2.Zero;
        private Vector3 _xyz = new Vector3(Vector2.Zero, 1f);
    }
}

using System;
using Microsoft.Xna.Framework;

namespace Apos.Camera {
    public interface IVirtualViewport : IDisposable {
        int X { get; }
        int Y { get; }
        int Width { get; }
        int Height { get; }

        Vector2 XY { get; }

        Vector2 Origin { get; }

        float VirtualWidth { get; }
        float VirtualHeight { get; }

        Matrix Transform(Matrix view);
        void Set();
        void Reset();
    }
}

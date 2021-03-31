using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apos.Camera {
    public class DefaultViewport : SplitViewport {
        public DefaultViewport(GraphicsDevice graphicsDevice, GameWindow window) : base(graphicsDevice, window, 0f, 0f, 1f, 1f) { }
    }
}

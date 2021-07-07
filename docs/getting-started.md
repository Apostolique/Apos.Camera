# Getting started
Quick guide to get started with Apos.Camera.

## Install

Install using the following dotnet command:

```
dotnet add package Apos.Camera
```

## Setup

Import the library with:

```csharp
using Apos.Camera;
```

Create a default viewport and pass it to the camera:

```csharp
IVirtualViewport defaultViewport = new DefaultViewport(GraphicsDevice, Window);
Camera camera = new Camera(defaultViewport);
```

Before drawing, set the viewport and pass the view matrix to the SpriteBatch:

```csharp
camera.SetViewport();
spriteBatch.Begin(transformMatrix: camera.View);

// Your draw code.

spriteBatch.End();
camera.ResetViewport();
```

You can move the camera around:

```csharp
camera.XY = new Vector2(100, 50);
camera.Scale = Vector2(2f, 2f);
camera.Rotation = MathHelper.PiOver4;
```

You can draw with parallax. First draw the background, then draw the foreground.

```csharp
protected override void Draw(GameTime gameTime) {
    camera.SetViewport();
    DrawBackground();
    DrawForeground();
    camera.ResetViewport();
}

private void DrawBackground() {
    // This gives you a matrix that is pushed under the ground plane.
    spriteBatch.Begin(transformMatrix: camera.GetView(-1));

    // Draw the background.

    spriteBatch.End();
}

private void DrawForeground() {
    spriteBatch.Begin(transformMatrix: camera.View);

    // Draw the foreground.

    spriteBatch.End();
}

```

You can do parallax zooming by moving on the Z axis:

```csharp
camera.Z = 10f;
```

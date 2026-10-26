# Apos.Camera
Camera library for MonoGame and KNI.

[![Discord](https://img.shields.io/discord/355231098122272778.svg)](https://discord.gg/MonoGame) [![NuGet](https://img.shields.io/nuget/v/Apos.Camera.svg)](https://www.nuget.org/packages/Apos.Camera/) [![NuGet](https://img.shields.io/nuget/dt/Apos.Camera.svg)](https://www.nuget.org/packages/Apos.Camera/)

## Description

This library gives you a 2D camera that you can move, zoom and rotate. It draws into viewports, so the same world can show up in a split screen or at a fixed size in any window. The camera also has a depth, which gives you parallax layers and 3D matrices that line up with them.

## Features

* Position, zoom and rotation
* Conversions between screen and world coordinates
* View bounds and frustums for culling
* Viewports for the whole window, split screens, and a fixed area that fits any window size
* Parallax layers, and parallax zooming by moving the camera on the Z axis
* 3D view and projection matrices that line up with the 2D layers
* Works with MonoGame and [KNI](https://github.com/kniengine/kni)

## Documentation

* [Getting started](https://apostolique.github.io/Apos.Camera/getting-started/)
* [Camera](https://apostolique.github.io/Apos.Camera/camera/)
* [Viewports](https://apostolique.github.io/Apos.Camera/viewports/)
* [Depth](https://apostolique.github.io/Apos.Camera/depth/)
* [3D](https://apostolique.github.io/Apos.Camera/3d/)

## Getting started

Install with:

```
dotnet add package Apos.Camera
```

Create a viewport and a camera, then draw between `SetViewport` and `ResetViewport` with the camera's view:

```csharp
using Apos.Camera;

public class Game1 : Game {
    GraphicsDeviceManager _graphics;
    SpriteBatch _spriteBatch;
    Camera _camera;

    public Game1() {
        _graphics = new GraphicsDeviceManager(this);
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _camera = new Camera(new DefaultViewport(GraphicsDevice, Window));
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Black);

        _camera.SetViewport();
        _spriteBatch.Begin(transformMatrix: _camera.View);
        // Your draw code.
        _spriteBatch.End();
        _camera.ResetViewport();

        base.Draw(gameTime);
    }
}
```

Read the [Getting started](https://apostolique.github.io/Apos.Camera/getting-started/) guide for the full walkthrough. For a bigger sample, check the [example code](https://github.com/Apostolique/Apos.Camera/blob/main/Example/Game/GameRoot.cs).

## Other projects you might like

* [Apos.Gui](https://github.com/Apostolique/Apos.Gui) - UI library for MonoGame.
* [Apos.Input](https://github.com/Apostolique/Apos.Input) - Polling input library for MonoGame.
* [Apos.Shapes](https://github.com/Apostolique/Apos.Shapes) - Shape rendering library for MonoGame.
* [Apos.History](https://github.com/Apostolique/Apos.History) - A C# library that makes it easy to handle undo and redo.
* [Apos.Content](https://github.com/Apostolique/Apos.Content) - Content builder library for MonoGame.
* [Apos.Framework](https://github.com/Apostolique/Apos.Framework) - Game architecture for MonoGame.
* [AposGameStarter](https://github.com/Apostolique/AposGameStarter) - MonoGame project starter. Common files to help create a game faster.

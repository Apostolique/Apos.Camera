# Apos.Camera
Camera library for MonoGame.

[![Discord](https://img.shields.io/discord/355231098122272778.svg)](https://discord.gg/N9t26Uv)

## Documentation

* [Getting started](https://apostolique.github.io/Apos.Camera/getting-started/)

## Build

[![NuGet](https://img.shields.io/nuget/v/Apos.Camera.svg)](https://www.nuget.org/packages/Apos.Camera/) [![NuGet](https://img.shields.io/nuget/dt/Apos.Camera.svg)](https://www.nuget.org/packages/Apos.Camera/)

## Features

* Viewport support
* Parallax

## Usage samples

```csharp
IVirtualViewport defaultViewport = new DefaultViewport(GraphicsDevice, Window);
Camera camera = new Camera(GraphicsDevice, defaultViewport);

camera.SetViewport();
spriteBatch.Begin(transformMatrix: camera.View);
// Your draw code.
spriteBatch.End();
camera.ResetViewport();
```

## Other projects you might like

* [Apos.Gui](https://github.com/Apostolique/Apos.Gui) - UI library for MonoGame.
* [Apos.Input](https://github.com/Apostolique/Apos.Gui) -  Polling input library for MonoGame.
* [Apos.History](https://github.com/Apostolique/Apos.History) - A C# library that makes it easy to handle undo and redo.
* [Apos.Content](https://github.com/Apostolique/Apos.Content) - Content builder library for MonoGame.
* [Apos.Framework](https://github.com/Apostolique/Apos.Framework) - Game architecture for MonoGame.
* [AposGameStarter](https://github.com/Apostolique/AposGameStarter) - MonoGame project starter. Common files to help create a game faster.

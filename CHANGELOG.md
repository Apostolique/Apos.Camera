# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

- Nothing yet!

## [0.5.0] - 2026-10-10

### Added

- `GetBoundingFrustum3D(nearPlaneDistance, farPlaneDistance)`, the frustum of the 3D view and projection.

### Changed

- `DensityViewport.Ratio` is read only.

### Fixed

- Setting `DensityViewport.TargetWidth` or `TargetHeight` takes effect right away instead of on the next window resize.
- Split viewports that share an edge no longer leave a 1 pixel gap between them.

## [0.4.1] - 2026-01-06

### Fixed

- The KNI build is published as `Apos.Camera.KNI`.

## [0.4.0] - 2026-01-06

### Added

- A KNI build.

### Changed

- Targets `net8.0` instead of `netstandard2.0`.
- Depends on MonoGame.Extended 5.3.1.

## [0.3.2] - 2021-12-11

### Fixed

- Setting `XYZ` updates `X`, `Y`, and `XY`.

## [0.3.1] - 2021-05-25

### Changed

- `FocalLength` can't go below 0.01.

### Fixed

- `ScaleToZ` returns the right depth when `targetZ` isn't 0.
- `ScaleToZ` and `ZToScale` return `float.MaxValue` instead of 0 when they would divide by zero.

## [0.3.0] - 2021-05-05

### Added

- `FocalLength`.
- `GetView3D()` and `GetProjection3D(nearPlaneDistance, farPlaneDistance)` for drawing in 3D.
- `GetProjection()`, the orthographic projection of the viewport.

### Changed

- `ScaleToZ` and `ZToScale` take the focal length into account.

## [0.2.4] - 2021-04-17

### Fixed

- NuGet packaging.

## [0.2.3] - 2021-04-11

### Fixed

- `GetViewRect` covers the whole view when the camera is rotated.
- `GetViewRect(z)` uses `z` instead of always measuring at 0.

## [0.2.2] - 2021-04-02

### Added

- `ViewRect`, `GetViewRect(z)`, and `GetBoundingFrustum(z)`.
- Depends on MonoGame.Extended.

### Changed

- `Camera` no longer takes a `GraphicsDevice`.
- `z` is optional on `WorldToScreen` and `ScreenToWorld`.
- The camera's position is the center of the view again.

### Fixed

- `DensityViewport` is centered on its virtual size.

## [0.2.1] - 2021-03-30

### Fixed

- `DefaulViewport` is renamed to `DefaultViewport`.

## [0.2.0] - 2021-03-29

### Added

- `SplitViewport`, a viewport that covers any fraction of the window.
- `WorldToScreenScale(z)`.

### Changed

- The camera's position is the top left of the view instead of its center.

### Removed

- `HalfViewport`. A `SplitViewport` covering half the window replaces it.

### Fixed

- `ScreenToWorldScale(z)` uses `z` instead of the camera's own depth.

## [0.1.1] - 2021-03-29

### Added

- Everything!

[Unreleased]: https://github.com/Apostolique/Apos.Camera/compare/v0.5.0...HEAD
[0.5.0]: https://github.com/Apostolique/Apos.Camera/compare/v0.4.1...v0.5.0
[0.4.1]: https://github.com/Apostolique/Apos.Camera/compare/v0.4.0...v0.4.1
[0.4.0]: https://github.com/Apostolique/Apos.Camera/compare/v0.3.2...v0.4.0
[0.3.2]: https://github.com/Apostolique/Apos.Camera/compare/v0.3.1...v0.3.2
[0.3.1]: https://github.com/Apostolique/Apos.Camera/compare/v0.3.0...v0.3.1
[0.3.0]: https://github.com/Apostolique/Apos.Camera/compare/v0.2.4...v0.3.0
[0.2.4]: https://github.com/Apostolique/Apos.Camera/compare/v0.2.3...v0.2.4
[0.2.3]: https://github.com/Apostolique/Apos.Camera/compare/v0.2.2...v0.2.3
[0.2.2]: https://github.com/Apostolique/Apos.Camera/compare/v0.2.1...v0.2.2
[0.2.1]: https://github.com/Apostolique/Apos.Camera/compare/v0.2.0...v0.2.1
[0.2.0]: https://github.com/Apostolique/Apos.Camera/compare/v0.1.1...v0.2.0
[0.1.1]: https://github.com/Apostolique/Apos.Camera/releases/tag/v0.1.1

# Changelog

## [Unreleased]

### Added
- `UIGradientMap` shader and component: recolors grayscale UI sprites through a ramp from a `GradientMapPalette`, with fades between rows and ramp alpha support.
- `GradientMapPalette` asset: gradient ramps (`GradientMapRamp` sub-assets, referenced directly) baked into one shared material, so recolored graphics still batch.
- `StateButton`: a `Button` that raises `StateChanged` and has an `IsAvailable` flag for buttons that stay selectable but look unusable.
- `SelectableStateHandler<T>` with `GradientMapStateHandler` and `GraphicColorStateHandler`: per-state values (plus unavailable overrides and a code-driven override) with fades.
- `LayoutBackground`: sizes a child to cover its laid-out siblings plus padding, and to zero when there are none; works under any layout group.

### Changed
- `SubAssetList<T>` now only stores its items. The names list and `SyncSubAssetList` are gone; use the Sub-Asset tools instead.

### Added (editor)
- Sub-Asset tools for any `SubAssetList` field: an inspector line with the count and an Add button, and Project window right-click `Tools > Sub-Assets >` Add, Rename, Duplicate, Delete, Move Up/Down and Repair List.

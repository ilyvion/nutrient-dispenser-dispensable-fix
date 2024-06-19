# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.3.0] - 2024-06-19

### Added

-   Added the ability to define what a nutrient paste dispenser dispenses using XML alone. See README for more details.

## [0.2.0] - 2024-06-11

### Added

-   Added a patch for a bug in RimWorld where if you have dispensers without hoppers or without a way to fill those hoppers, pawns can sometimes end up choosing one that isn't actually usable, and if this happens, they won't eat anything at all, despite needing to.

## [0.1.1] - 2024-06-10

### Fixed

-   Need to patch `ThingListGroupHelper.Includes` as well, which is involved in deciding what is a food source.

## [0.1.0] - 2024-06-10

### Added

-   First implementation of the mod.

[Unreleased]: https://github.com/ilyvion/nutrient-dispenser-dispensable-fix/compare/v0.3.0...HEAD
[0.3.0]: https://github.com/ilyvion/nutrient-dispenser-dispensable-fix/releases/tag/v0.2.0...v0.3.0
[0.2.0]: https://github.com/ilyvion/nutrient-dispenser-dispensable-fix/releases/tag/v0.1.1...v0.2.0
[0.1.1]: https://github.com/ilyvion/nutrient-dispenser-dispensable-fix/releases/tag/v0.1.0...v0.1.1
[0.1.0]: https://github.com/ilyvion/nutrient-dispenser-dispensable-fix/releases/tag/v0.1.0

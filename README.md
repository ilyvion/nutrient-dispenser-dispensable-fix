[![RimWorld 1.5](https://img.shields.io/badge/RimWorld-1.5-brightgreen.svg)](http://rimworldgame.com/) [![Build](https://github.com/ilyvion/nutrient-dispenser-dispensable-fix/actions/workflows/ci.yml/badge.svg)](https://github.com/ilyvion/nutrient-dispenser-dispensable-fix/actions/workflows/ci.yml)

This mod only does only one thing -- it patches RimWorld to use `Building_NutrientPasteDispenser.DispensableDef` when choosing what to dispense and pick as a meal instead of using the hard-coded value of `ThingDefOf.MealNutrientPaste`. This won't change anything for the regular Nutrient paste dispenser, since for it those are one and the same, but it means that modders can rely on that property for their custom dispensers.

## License

Licensed under either of

-   Apache License, Version 2.0, ([LICENSE.Apache-2.0](LICENSE.Apache-2.0) or http://www.apache.org/licenses/LICENSE-2.0)
-   MIT license ([LICENSE.MIT](LICENSE.MIT) or http://opensource.org/licenses/MIT)

at your option.

`SPDX-License-Identifier: Apache-2.0 OR MIT`

### Contribution

Unless you explicitly state otherwise, any contribution intentionally submitted
for inclusion in the work by you, as defined in the Apache-2.0 license, shall be
dual licensed as above, without any additional terms or conditions.

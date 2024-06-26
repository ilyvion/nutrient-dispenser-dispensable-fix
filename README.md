[![RimWorld 1.5](https://img.shields.io/badge/RimWorld-1.5-brightgreen.svg)](http://rimworldgame.com/) [![Build](https://github.com/ilyvion/nutrient-dispenser-dispensable-fix/actions/workflows/ci.yml/badge.svg)](https://github.com/ilyvion/nutrient-dispenser-dispensable-fix/actions/workflows/ci.yml)

This mod corrects some bugs that nutrient paste dispensers have in vanilla Rimworld, both observable and not. For a player, using this mod will only really fix a single bug in RimWorld where if you have dispensers without hoppers or without a way to fill those hoppers, pawns can sometimes end up choosing one that isn't actually usable, and if this happens, they won't eat anything at all, despite needing to.

Most of the fixes in this mod will instead help mod creators by making it so inheriting from the vanilla nutrient paste dispenser will work as one might expect instead of using hard-coded values from the vanilla nutrient paste meal everywhere.

## The full list of fixes this mod does is as follows:

-   Where the game has been hardcoded to use `ThingDefOf.MealNutrientPaste`, instead use `Building_NutrientPasteDispenser.DispensableDef`. While this doesn't directly affect vanilla, it allows subclasses of `Building_NutrientPasteDispenser` to dispense something else than the vanilla nutrient paste. This type of fix has been applies to:
    -   `Building_NutrientPasteDispenser.TryDispenseFood`
    -   `FoodUtility.BestFoodSourceOnMap`
    -   `JobDriver_FoodDeliver.GetReport`
    -   `JobDriver_FoodFeedPatient.GetReport`
-   In `FoodUtility.BestFoodSourceOnMap`, where the logic allows choosing an "empty" dispenser: also require the dispenser to have an adjacent, accessible hopper. Without this additional check, the game can pick a dispenser without a hopper, thinking it can be filled later, but since one without hoppers can't, that job can never happen, and the pawn just ends up not eating at all.
-   In `ThingListGroupHelper.Includes`, use `ThingDef.IsFoodDispenser` to decide whether a given def is a food dispenser instead of a hardcoded check against exactly `Building_NutrientPasteDispenser`. This allows inheriting from `Building_NutrientPasteDispenser` to work as expected.

## Making a custom nutrient dispenser with XML only

Besides fixing things about nutrient paste dispensers, this mod also intends to make it easier to make custom dispensers using XML only. To that extent, I've created a custom `thingClass` called `ilvyion.NutrientDispenserDispensableFix.Building_NutrientPasteDispenserImproved` which, in combination with a `<modExtensions>` called `ilvyion.NutrientDispenserDispensableFix.ImprovedNutrientPasteDispenserModExtension` lets you define things that would normally require code in XML.

Currently the only supported feature of this `ImprovedNutrientPasteDispenserModExtension` is to allow you to set what is dispensed, but I'm open to adding more features if anyone needs them.

To use this XML-only nutrient dispenser, you make a `ThingDef` like the following:

```xml
<ThingDef ParentName="BuildingBase">
    <thingClass>ilvyion.NutrientDispenserDispensableFix.Building_NutrientPasteDispenserImproved</thingClass>
    <modExtensions>
        <li Class="ilvyion.NutrientDispenserDispensableFix.ImprovedNutrientPasteDispenserModExtension">
            <!-- This is where you set your dispensed product -->
            <dispensableDef>MealNutrientPaste</dispensableDef>
        </li>
    </modExtensions>
    <!-- ... and the rest of the owl... er, ThingDef. -->
</ThingDef>
```

You can look at an example dispenser definition included with this mod in [Defs/ImprovedNutrientPasteDispenser.xml](Defs/ImprovedNutrientPasteDispenser.xml). It does not show up in the game directly, but can be spawned in using the debug tools. It is set up to dispense simple meals.

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

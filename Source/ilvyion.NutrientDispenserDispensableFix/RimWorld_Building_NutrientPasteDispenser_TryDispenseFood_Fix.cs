
using System.Reflection;
using System.Reflection.Emit;

namespace ilvyion.NutrientDispenserDispensableFix;

[HarmonyPatch(typeof(Building_NutrientPasteDispenser), nameof(Building_NutrientPasteDispenser.TryDispenseFood))]
internal static class RimWorld_Building_NutrientPasteDispenser_TryDispenseFood_Fix
{
    private static readonly FieldInfo _field_ThingDefOf_MealNutrientPaste = AccessTools.Field(
        typeof(ThingDefOf), nameof(ThingDefOf.MealNutrientPaste));

    private static readonly MethodInfo _method_Building_NutrientPasteDispenser_DispensableDef_get
        = AccessTools.PropertyGetter(
            typeof(Building_NutrientPasteDispenser), nameof(Building_NutrientPasteDispenser.DispensableDef));

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var originalInstructionList = instructions.ToList();

        var codeMatcher = new CodeMatcher(instructions);

        codeMatcher.SearchForward(i => i.opcode == OpCodes.Ldsfld && i.operand is FieldInfo f && f == _field_ThingDefOf_MealNutrientPaste);
        if (!codeMatcher.IsValid)
        {
            NutrientDispenserDispensableFixMod.Error("Could not patch Building_NutrientPasteDispenser.TryDispenseFood, IL does not match expectations: field access to ThingDefOf.MealNutrientPaste not found.");
            return originalInstructionList;
        }

        // Remove the hard coded loading of ThingDefOf.MealNutrientPaste
        codeMatcher.RemoveInstruction();
        // Instead, load the value of this.DispensableDef
        codeMatcher.Insert([
            // == this.DispensableDef
            new(OpCodes.Ldarg_0),
            new(OpCodes.Callvirt, _method_Building_NutrientPasteDispenser_DispensableDef_get),
        ]);

        return codeMatcher.Instructions();
    }
}

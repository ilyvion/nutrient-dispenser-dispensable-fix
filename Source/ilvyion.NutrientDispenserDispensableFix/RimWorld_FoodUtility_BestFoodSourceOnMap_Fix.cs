using System.Reflection;
using System.Reflection.Emit;

namespace ilvyion.NutrientDispenserDispensableFix;

// Based on code from
// <https://github.com/PeteTimesSix/TranspilerExamples/blob/main/Source/HarmonyPatchExamples/CompilerGeneratedClasses.cs>

[HarmonyPatch]
internal static class RimWorld_FoodUtility_BestFoodSourceOnMap_Fix
{
    private static readonly CodeMatch[] toMatch =
    [
        new(OpCodes.Ldarg_1),
        new(OpCodes.Isinst, typeof(Building_NutrientPasteDispenser)),
        new(OpCodes.Dup),
    ];

    //Check if we can find the methods to patch. Otherwise, Harmony throws an error if given no methods in HarmonyTargetMethods.
    [HarmonyPrepare]
    public static bool ShouldPatch()
    {
        // We can reuse the same method as HarmonyTargetMethods will use afterward.
        var methods = CalculateMethods();

        //check that we have one and only one match. If we get more, the match is giving false positives.
        if (methods.Count() == 1)
        {
            return true;
        }
        else
        {
            NutrientDispenserDispensableFixMod.Error("Could not patch FoodUtility.BestFoodSourceOnMap, could not locate instance check of Building_NutrientPasteDispenser.");
            return false;
        }

    }

    [HarmonyTargetMethods]
    public static IEnumerable<MethodBase> CalculateMethods()
    {
        //Find all possible candidates, both from the wrapping type and all nested types.
        var candidates = AccessTools.GetDeclaredMethods(typeof(FoodUtility)).ToHashSet();
        candidates.AddRange(typeof(FoodUtility).GetNestedTypes(AccessTools.all).SelectMany(t => AccessTools.GetDeclaredMethods(t)));

        //check all candidates for the target instructions, return those that match.
        foreach (var method in candidates)
        {
            var instructions = PatchProcessor.GetCurrentInstructions(method);
            //var matched = instructions.Matches(toMatch); // Available in Harmony 2.3+
            var matched = new CodeMatcher(instructions).MatchStartForward(toMatch).IsValid;
            if (matched)
                yield return method;
        }
        yield break;
    }

    private static readonly FieldInfo _field_ThingDefOf_MealNutrientPaste = AccessTools.Field(
        typeof(ThingDefOf), nameof(ThingDefOf.MealNutrientPaste));

    private static readonly MethodInfo _method_Building_NutrientPasteDispenser_DispensableDef_get
        = AccessTools.PropertyGetter(
            typeof(Building_NutrientPasteDispenser), nameof(Building_NutrientPasteDispenser.DispensableDef));

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        const string method = "FoodUtility.BestFoodSourceOnMap";

        var originalInstructionList = instructions.ToList();

        var codeMatcher = new CodeMatcher(instructions);

        codeMatcher.SearchForward(i => i.opcode == OpCodes.Ldsfld && i.operand is FieldInfo f && f == _field_ThingDefOf_MealNutrientPaste);
        if (!codeMatcher.IsValid)
        {
            NutrientDispenserDispensableFixMod.Error($"Could not patch {method}, IL does not match expectations: field access to ThingDefOf.MealNutrientPaste not found.");
            return originalInstructionList;
        }

        // Remove the hard coded loading of ThingDefOf.MealNutrientPaste
        do
        {
            codeMatcher.RemoveInstruction();
            codeMatcher.InsertAndAdvance([
                // == dispenser
                new(OpCodes.Ldloc_1),
                // == dispenser.DispensableDef
                new(OpCodes.Callvirt, _method_Building_NutrientPasteDispenser_DispensableDef_get),
            ]);

            codeMatcher.SearchForward(i => i.opcode == OpCodes.Ldsfld && i.operand is FieldInfo f && f == _field_ThingDefOf_MealNutrientPaste);
        } while (codeMatcher.IsValid);

        return codeMatcher.Instructions();
    }
}

using System.Reflection;
using System.Reflection.Emit;
using Verse.AI;

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
#if v1_5
        new(OpCodes.Dup),
#endif
        new(OpCodes.Stloc_1),
    ];

    //Check if we can find the methods to patch. Otherwise, Harmony throws an error if given no methods in HarmonyTargetMethods.
    [HarmonyPrepare]
    private static bool ShouldPatch()
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
    private static IEnumerable<MethodBase> CalculateMethods()
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

    private static readonly MethodInfo _method_HasAdjacentHoppers = AccessTools.Method(typeof(RimWorld_FoodUtility_BestFoodSourceOnMap_Fix), nameof(HasAdjacentHoppers));

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        const string method = "FoodUtility.BestFoodSourceOnMap";

        var originalInstructionList = instructions.ToList();

        var codeMatcher = new CodeMatcher(instructions, generator);

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

        codeMatcher.Start();
        codeMatcher.SearchForward(i => i.opcode == OpCodes.Ldfld && i.operand is FieldInfo f && f.Name == "getter");
        if (!codeMatcher.IsValid)
        {
            NutrientDispenserDispensableFixMod.Error($"Could not patch {method}, IL does not match expectations: field access to allowDispenserEmpty not found.");
            return originalInstructionList;
        }
        var getterField = (FieldInfo)codeMatcher.Operand;

        codeMatcher.Start();
        codeMatcher.SearchForward(i => i.opcode == OpCodes.Ldfld && i.operand is FieldInfo f && f.Name == "allowDispenserEmpty");
        if (!codeMatcher.IsValid)
        {
            NutrientDispenserDispensableFixMod.Error($"Could not patch {method}, IL does not match expectations: field access to allowDispenserEmpty not found.");
            return originalInstructionList;
        }
        var allowDispenserEmptyField = (FieldInfo)codeMatcher.Operand;
        codeMatcher.Advance(1);
        if (codeMatcher.Opcode != OpCodes.Brtrue_S)
        {
            NutrientDispenserDispensableFixMod.Error($"Could not patch {method}, IL does not match expectations: [brtrue.s] expected but not found; was {codeMatcher.Opcode}.");
            return originalInstructionList;
        }

        // Save these for later, we need to change them *after* we've used the
        // successLabel value below
        ref var successOpCode = ref codeMatcher.Opcode;
        ref var successLabel = ref codeMatcher.Operand;

        // Grab the label for failure
        codeMatcher.Advance(3);
        if (codeMatcher.Opcode != OpCodes.Brfalse_S)
        {
            NutrientDispenserDispensableFixMod.Error($"Could not patch {method}, IL does not match expectations: [brfalse.s] expected but not found; was {codeMatcher.Opcode}.");
            return originalInstructionList;
        }
        var failureLabel = codeMatcher.Operand;

        // Add check for adjacent hoppers if `allowDispenserEmpty` is true
        codeMatcher.Advance(-2);
        codeMatcher.InsertAndAdvance([
            // == dispenser
            new(OpCodes.Ldloc_1),
            // == this.getter
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldfld, getterField),
            // == HasAdjacentHoppers(dispenser, this.getter)
            new(OpCodes.Call, _method_HasAdjacentHoppers),
            // If true, we're good.
            new(OpCodes.Brtrue_S, successLabel),
        ]);

        // We only check `HasEnoughFeedstockInHoppers` if `allowDispenserEmpty` was false
        codeMatcher.Insert([
            // == this.allowDispenserEmpty
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldfld,allowDispenserEmptyField),
            new(OpCodes.Brtrue_S, failureLabel)
        ]);
        codeMatcher.CreateLabel(out var hasEnoughFeedstockInHoppersNewLabel);

        // Now that we've used `successLabel`, we can overwrite these
        successOpCode = OpCodes.Brfalse_S;
        successLabel = hasEnoughFeedstockInHoppersNewLabel;

        return codeMatcher.Instructions();
    }

    private static bool HasAdjacentHoppers(Building_NutrientPasteDispenser dispenser, Pawn reacher)
    {
        for (int i = 0; i < dispenser.AdjCellsCardinalInBounds.Count; i++)
        {
            Building edifice = dispenser.AdjCellsCardinalInBounds[i].GetEdifice(dispenser.Map);
            if (edifice != null && edifice.IsHopper() && reacher.CanReach(edifice, PathEndMode.Touch, Danger.Deadly) && !edifice.IsBurning())
            {
                return true;
            }
        }
        return false;
    }
}

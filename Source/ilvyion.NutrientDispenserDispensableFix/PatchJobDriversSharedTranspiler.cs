using System.Reflection;
using System.Reflection.Emit;

namespace ilvyion.NutrientDispenserDispensableFix;

internal static class PatchJobDriversSharedTranspiler
{
    private static readonly MethodInfo _method_LocalTargetInfo_Thing_get
        = AccessTools.PropertyGetter(typeof(LocalTargetInfo), nameof(LocalTargetInfo.Thing));

    private static readonly FieldInfo _field_ThingDefOf_MealNutrientPaste = AccessTools.Field(
        typeof(ThingDefOf), nameof(ThingDefOf.MealNutrientPaste));

    private static readonly MethodInfo _method_Building_NutrientPasteDispenser_DispensableDef_get
        = AccessTools.PropertyGetter(
            typeof(Building_NutrientPasteDispenser), nameof(Building_NutrientPasteDispenser.DispensableDef));

    public static IEnumerable<CodeInstruction> Transpiler(string method, IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var originalInstructionList = instructions.ToList();

        var codeMatcher = new CodeMatcher(instructions);

        codeMatcher.SearchForward(i => i.opcode == OpCodes.Call && i.operand is MethodInfo m && m == _method_LocalTargetInfo_Thing_get);
        if (!codeMatcher.IsValid)
        {
            NutrientDispenserDispensableFixMod.Error($"Could not patch {method}, IL does not match expectations: access to LocalTargetInfo.Thing not found.");
            return originalInstructionList;
        }

        var npd = generator.DeclareLocal(typeof(Building_NutrientPasteDispenser));

        // After job.GetTarget(TargetIndex.A).Thing has been loaded, let's store it in a local
        codeMatcher.Advance(2);
        codeMatcher.Insert([
            // == var npd = (Building_NutrientPasteDispenser)job.GetTarget(TargetIndex.A).Thing
            new(OpCodes.Stloc, npd.LocalIndex),
            new(OpCodes.Ldloc, npd.LocalIndex)
        ]);

        // This usage of ThingDefOf.MealNutrientPaste happens twice
        for (var i = 0; i < 2; i++)
        {
            codeMatcher.SearchForward(i => i.opcode == OpCodes.Ldsfld && i.operand is FieldInfo f && f == _field_ThingDefOf_MealNutrientPaste);
            if (!codeMatcher.IsValid)
            {
                NutrientDispenserDispensableFixMod.Error($"Could not patch {method}, IL does not match expectations: field access to ThingDefOf.MealNutrientPaste not found.");
                return originalInstructionList;
            }

            // Remove the hard coded loading of ThingDefOf.MealNutrientPaste
            codeMatcher.RemoveInstruction();
            // Instead, load the value of npd.DispensableDef
            codeMatcher.Insert([
                // == npd.DispensableDef
                new(OpCodes.Ldloc, npd.LocalIndex),
                new(OpCodes.Callvirt, _method_Building_NutrientPasteDispenser_DispensableDef_get),
            ]);
        }

        return codeMatcher.Instructions();
    }
}

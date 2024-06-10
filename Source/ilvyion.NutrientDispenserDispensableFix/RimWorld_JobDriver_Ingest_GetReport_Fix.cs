using System.Reflection;
using System.Reflection.Emit;

namespace ilvyion.NutrientDispenserDispensableFix;

[HarmonyPatch(typeof(JobDriver_Ingest), nameof(JobDriver_Ingest.GetReport))]
internal static class RimWorld_JobDriver_Ingest_GetReport_Fix
{
    private static readonly MethodInfo _method_JobDriver_Ingest_IngestibleSource_get
        = AccessTools.PropertyGetter(typeof(JobDriver_Ingest), "IngestibleSource");

    private static readonly FieldInfo _field_JobDriver_Ingest_usingNutrientPasteDispenser = AccessTools.Field(
        typeof(JobDriver_Ingest), "usingNutrientPasteDispenser");

    private static readonly FieldInfo _field_ThingDefOf_MealNutrientPaste = AccessTools.Field(
        typeof(ThingDefOf), nameof(ThingDefOf.MealNutrientPaste));

    private static readonly MethodInfo _method_Building_NutrientPasteDispenser_DispensableDef_get
        = AccessTools.PropertyGetter(
            typeof(Building_NutrientPasteDispenser), nameof(Building_NutrientPasteDispenser.DispensableDef));

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        const string method = "JobDriver_Ingest.GetReport";

        var originalInstructionList = instructions.ToList();

        var codeMatcher = new CodeMatcher(instructions);

        codeMatcher.SearchForward(i => i.opcode == OpCodes.Ldfld && i.operand is FieldInfo f && f == _field_JobDriver_Ingest_usingNutrientPasteDispenser);
        if (!codeMatcher.IsValid)
        {
            NutrientDispenserDispensableFixMod.Error($"Could not patch {method}, IL does not match expectations: field access to JobDriver_Ingest.usingNutrientPasteDispenser not found.");
            return originalInstructionList;
        }
        codeMatcher.RemoveInstruction();

        var npd = generator.DeclareLocal(typeof(Building_NutrientPasteDispenser));

        codeMatcher.InsertAndAdvance([
            new(OpCodes.Call, _method_JobDriver_Ingest_IngestibleSource_get),
            new(OpCodes.Isinst, typeof(Building_NutrientPasteDispenser)),
            new(OpCodes.Stloc, npd.LocalIndex),
            new(OpCodes.Ldloc, npd.LocalIndex)
        ]);
        codeMatcher.Advance(1);
        if (codeMatcher.Instruction.opcode != OpCodes.Ldarg_0)
        {
            NutrientDispenserDispensableFixMod.Error($"Could not patch {method}, IL does not match expectations: ldarg.0 not found before ThingDefOf.MealNutrientPaste.");
            return originalInstructionList;
        }

        var dispensableDef = generator.DeclareLocal(typeof(ThingDef));

        // Load ((Building_NutrientPasteDispenser)IngestibleSource).DispensableDef into our dispensableDef local
        codeMatcher.InsertAndAdvance([
            // npd
            new(OpCodes.Ldloc, npd.LocalIndex),
            // == npd.DispensableDef
            new(OpCodes.Callvirt, _method_Building_NutrientPasteDispenser_DispensableDef_get),
            // == var dispensableDef = npd.DispensableDef
            new(OpCodes.Stloc, dispensableDef.LocalIndex)
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
            // Then, load our local
            codeMatcher.Insert([
                // == dispensableDef
                new(OpCodes.Ldloc, dispensableDef.LocalIndex),
            ]);
        }

        return codeMatcher.Instructions();
    }
}

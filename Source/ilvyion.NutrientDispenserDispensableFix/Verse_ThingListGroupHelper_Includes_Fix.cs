using System.Reflection;
using System.Reflection.Emit;

namespace ilvyion.NutrientDispenserDispensableFix;

[HarmonyPatch(typeof(ThingListGroupHelper), nameof(ThingListGroupHelper.Includes))]
internal static class Verse_ThingListGroupHelper_Includes_Fix
{

    private static readonly FieldInfo _field_JobDriver_Ingest_usingNutrientPasteDispenser = AccessTools.Field(
        typeof(JobDriver_Ingest), "usingNutrientPasteDispenser");

    private static readonly FieldInfo _field_ThingDef_thingClass = AccessTools.Field(typeof(ThingDef), nameof(ThingDef.thingClass));

    private static readonly MethodInfo _method_ThingDef_IsFoodDispenser_get = AccessTools.PropertyGetter(typeof(ThingDef), nameof(ThingDef.IsFoodDispenser));

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        const string method = "ThingListGroupHelper.Includes";

        var originalInstructionList = instructions.ToList();

        var codeMatcher = new CodeMatcher(instructions);

        // Do it twice, because there are two instances of this.
        for (var i = 0; i < 2; i++)
        {
            codeMatcher.SearchForward(i => i.opcode == OpCodes.Ldtoken && i.operand is Type t && t == typeof(Building_NutrientPasteDispenser));
            if (!codeMatcher.IsValid)
            {
                NutrientDispenserDispensableFixMod.Error($"Could not patch {method}, IL does not match expectations: typeof(Building_NutrientPasteDispenser) not found.");
                return originalInstructionList;
            }
            codeMatcher.Advance(-1);
            if (codeMatcher.Opcode != OpCodes.Ldfld || codeMatcher.Operand is not FieldInfo f || f != _field_ThingDef_thingClass)
            {
                NutrientDispenserDispensableFixMod.Error($"Could not patch {method}, IL does not match expectations: expected def.thingClass.");
                return originalInstructionList;
            }

            codeMatcher.RemoveInstructions(4);

            codeMatcher.Insert([
                new(OpCodes.Call, _method_ThingDef_IsFoodDispenser_get),
            ]);
        }

        return codeMatcher.Instructions();
    }
}

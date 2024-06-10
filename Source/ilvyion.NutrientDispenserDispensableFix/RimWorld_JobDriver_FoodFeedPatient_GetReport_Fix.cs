using System.Reflection.Emit;

namespace ilvyion.NutrientDispenserDispensableFix;

[HarmonyPatch(typeof(JobDriver_FoodFeedPatient), nameof(JobDriver_FoodFeedPatient.GetReport))]
internal static class RimWorld_JobDriver_FoodFeedPatient_GetReport_Fix
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        return PatchJobDriversSharedTranspiler.Transpiler(nameof(JobDriver_FoodFeedPatient.GetReport), instructions, generator);
    }
}

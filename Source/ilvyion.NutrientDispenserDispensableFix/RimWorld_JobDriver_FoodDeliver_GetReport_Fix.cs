using System.Reflection;
using System.Reflection.Emit;

namespace ilvyion.NutrientDispenserDispensableFix;

[HarmonyPatch(typeof(JobDriver_FoodDeliver), nameof(JobDriver_FoodDeliver.GetReport))]
internal static class RimWorld_JobDriver_FoodDeliver_GetReport_Fix
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        return PatchJobDriversSharedTranspiler.Transpiler(nameof(JobDriver_FoodDeliver.GetReport), instructions, generator);
    }
}

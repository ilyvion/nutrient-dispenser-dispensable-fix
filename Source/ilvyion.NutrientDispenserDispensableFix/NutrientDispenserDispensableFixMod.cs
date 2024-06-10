using System.Reflection;

namespace ilvyion.NutrientDispenserDispensableFix;

public class NutrientDispenserDispensableFixMod : Mod
{
    public NutrientDispenserDispensableFixMod(ModContentPack content) : base(content)
    {
        //Harmony.DEBUG = true;
        new Harmony(content.ModMetaData.PackageId).PatchAll(Assembly.GetExecutingAssembly());
        //Harmony.DEBUG = false;
    }

    public static void Error(string msg)
    {
        Log.Error($"[ilyvion's Nutrient Dispenser Dispensable Fix] {msg}");
    }
}

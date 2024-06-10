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

    public static void Message(string msg)
    {
        Log.Message($"[ilyvion's Nutrient Dispenser Dispensable Fix] {msg}");
    }

    public static void Dump(string msg, object thing)
    {
        Log.Message($"[ilyvion's Nutrient Dispenser Dispensable Fix] {msg}: {thing}");
    }
}

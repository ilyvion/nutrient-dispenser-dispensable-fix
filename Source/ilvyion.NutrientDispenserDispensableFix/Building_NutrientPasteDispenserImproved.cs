
namespace ilvyion.NutrientDispenserDispensableFix;

public class Building_NutrientPasteDispenserImproved : Building_NutrientPasteDispenser
{
    private ImprovedNutrientPasteDispenserModExtension ImprovedNutrientPasteDispenserModExtension
    {
        get
        {
            if (!def.HasModExtension<ImprovedNutrientPasteDispenserModExtension>())
            {
                throw new Exception("The Building_NutrientPasteDispenserImproved expects a <modExtensions> with a ImprovedNutrientPasteDispenserModExtension in it to work properly. See the README for more details.");
            }
            return def.GetModExtension<ImprovedNutrientPasteDispenserModExtension>();
        }
    }

    public override ThingDef DispensableDef
    {
        get
        {
            return ImprovedNutrientPasteDispenserModExtension.dispensableDef;
        }
    }
}

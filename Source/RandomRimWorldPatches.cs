using HarmonyLib;
using Verse;

namespace RandomRimWorldPatches
{
    [StaticConstructorOnStartup]
    public class RandomRimWorldPatchesMod
    {
        static RandomRimWorldPatchesMod()
        {
            var harmony = new Harmony("sarcasimo.randomrimworldpatches");
            harmony.PatchAll();
            Log.Message("[Random RimWorld Patches] Harmony patches applied successfully!");
        }
    }
}

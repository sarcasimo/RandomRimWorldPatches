using HarmonyLib;
using Verse;

namespace RandomRimWorldPatches
{
    [StaticConstructorOnStartup]
    public class RandomRimWorldPatchesMod
    {
        static RandomRimWorldPatchesMod()
        {
            // Initialize Harmony (even though we're not using patches)
            var harmony = new Harmony("sarcasimo.randomrimworldpatches");
            // No patches to apply - VEF handles everything!
            Log.Message("[Random RimWorld Patches] Loaded successfully! Using VEF for functionality.");
        }
    }
}
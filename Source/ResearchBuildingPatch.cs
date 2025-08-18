using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace RandomRimWorldPatches
{
    // Patch ResearchProjectDef.CanStartNow so the compact HiTech bench grants access
    // when the vanilla HiTechResearchBench is absent but the compact bench is present.
    [HarmonyPatch(typeof(ResearchProjectDef), "CanStartNow")]
    public static class ResearchBuildingPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result, ResearchProjectDef __instance)
        {
            // If the project is already available, no change needed.
            if (__result) return;

            // Only adjust projects that specifically require the HiTechResearchBench.
            if (__instance.requiredResearchBuilding?.defName != "HiTechResearchBench") return;

            // If a vanilla HiTechResearchBench is present and powered anywhere, respect that (no change).
            bool hasVanillaBench = Find.Maps.Any(map =>
                map.listerBuildings.allBuildingsColonist.Any(building =>
                    building.def.defName == "HiTechResearchBench" &&
                    building.GetComp<CompPowerTrader>()?.PowerOn != false));

            if (hasVanillaBench) return;

            // If the compact HiTech bench exists and is powered, allow the project to be started.
            bool hasCompactBench = Find.Maps.Any(map =>
                map.listerBuildings.allBuildingsColonist.Any(building =>
                    building.def.defName == "ResearchBenchHiTechCompact" &&
                    building.GetComp<CompPowerTrader>()?.PowerOn != false));

            if (hasCompactBench)
            {
                __result = true;
            }
        }
    }
}

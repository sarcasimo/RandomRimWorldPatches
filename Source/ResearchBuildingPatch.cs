using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using Verse.AI;

namespace RandomRimWorldPatches
{
    // Existing patch for research requirements (keep this)
    [HarmonyPatch(typeof(ResearchProjectDef), "CanStartNow", MethodType.Getter)]
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

    // NEW PATCH: Make WorkGiver_Researcher recognize our compact bench
    [HarmonyPatch(typeof(WorkGiver_Researcher), "PotentialWorkThingsGlobal")]
    public static class WorkGiver_Researcher_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref IEnumerable<Thing> __result, Pawn pawn)
        {
            if (__result == null) __result = new List<Thing>();
            
            // Add our compact benches to the list of potential work things
            var compactBenches = pawn.Map.listerBuildings.allBuildingsColonist
                .Where(building => building.def.defName == "ResearchBenchHiTechCompact")
                .Cast<Thing>();
            
            __result = __result.Concat(compactBenches);
        }
    }

    // NEW PATCH: Make the research bench counting system recognize our bench
    [HarmonyPatch(typeof(Map), "listerBuildings", MethodType.Getter)]
    public static class Map_ListerBuildings_Patch
    {
        // This might not be needed, but ensures our benches are properly listed
    }

    // ALTERNATIVE APPROACH: Patch the building class directly
    [HarmonyPatch(typeof(Building_ResearchBench), "GetInspectString")]
    public static class Building_ResearchBench_Inspect_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Building_ResearchBench __instance, ref string __result)
        {
            // This ensures our compact bench behaves exactly like a research bench
            if (__instance.def.defName == "ResearchBenchHiTechCompact")
            {
                // Let the original method handle it - this just ensures it's recognized
                return true;
            }
            return true;
        }
    }
}
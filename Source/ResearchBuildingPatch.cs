using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace RandomRimWorldPatches
{
    [HarmonyPatch(typeof(ResearchManager), "GetAvailableResearchProjects")]
    public class ResearchBuildingPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref List<ResearchProjectDef> __result)
        {
            // Find all research projects that require HiTechResearchBench
            var hiTechResearchProjects = DefDatabase<ResearchProjectDef>.AllDefs
                .Where(x => x.requiredResearchBuilding?.defName == "HiTechResearchBench");

            foreach (var project in hiTechResearchProjects)
            {
                // Check if player has the compact version
                bool hasCompactBench = Find.Maps.Any(map => 
                    map.listerBuildings.allBuildingsColonist.Any(building => 
                        building.def.defName == "ResearchBenchHiTechCompact" && 
                        building.GetComp<CompPowerTrader>()?.PowerOn != false));

                // If they have compact bench but not vanilla, add the project to available list
                if (hasCompactBench && !__result.Contains(project))
                {
                    bool hasVanillaBench = Find.Maps.Any(map => 
                        map.listerBuildings.allBuildingsColonist.Any(building => 
                            building.def.defName == "HiTechResearchBench" && 
                            building.GetComp<CompPowerTrader>()?.PowerOn != false));
                    
                    // Only add if they don't already have access via vanilla bench
                    if (!hasVanillaBench && project.CanStartNow)
                    {
                        __result.Add(project);
                    }
                }
            }
        }
    }
}

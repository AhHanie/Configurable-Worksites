using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace SK_Configurable_Worksites.Patches
{
    [HarmonyPatch]
    public static class WorksiteSpawnConditionsPatcher
    {
        public static bool Prepare()
        {
            return ModSettings.relaxedWorksiteSpawningConditions;
        }

        public static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (Type type in GenTypes.AllTypes)
            {
                if (type == typeof(SitePartWorker_WorkSite) || !typeof(SitePartWorker_WorkSite).IsAssignableFrom(type) || type.IsAbstract)
                {
                    continue;
                }

                MethodInfo canSpawnOn = type.GetMethod("CanSpawnOn", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (canSpawnOn != null && canSpawnOn.DeclaringType == type)
                {
                    yield return canSpawnOn;
                }
            }
        }

        public static bool Prefix(SitePartWorker_WorkSite __instance, PlanetTile tile, ref bool __result)
        {
            __result = __instance.LootThings(tile).Any();
            return false;
        }
    }
}

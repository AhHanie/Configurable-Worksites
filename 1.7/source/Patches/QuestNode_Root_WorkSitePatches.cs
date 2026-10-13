using HarmonyLib;
using RimWorld.QuestGen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace SK_Configurable_Worksites.Patches
{
    public static class PotentialSiteTilesLambdas
    {
        public const float VanillaMinSpawnDist = 3f;
        public const float VanillaMaxSpawnRange = 9f;

        public static readonly MethodBase MaxRangeFilter = FindLambda(typeof(bool), VanillaMaxSpawnRange);
        public static readonly MethodBase MinRangeVisitor = FindLambda(typeof(void), VanillaMinSpawnDist);

        private static MethodBase FindLambda(System.Type returnType, float constant)
        {
            MethodInfo potentialSiteTiles = AccessTools.Method(typeof(QuestNode_Root_WorkSite), nameof(QuestNode_Root_WorkSite.PotentialSiteTiles));
            if (potentialSiteTiles == null)
            {
                return null;
            }

            foreach (MethodInfo lambda in AccessTools.Lambdas(potentialSiteTiles))
            {
                if (lambda.ReturnType == returnType && LoadsFloat(lambda, constant))
                {
                    return lambda;
                }
            }

            return null;
        }

        private static bool LoadsFloat(MethodBase method, float constant)
        {
            return PatchProcessor.ReadMethodBody(method).Any(i => i.Key == OpCodes.Ldc_R4 && i.Value is float value && value == constant);
        }

        public static bool LogMissing(MethodBase lambda, string description)
        {
            if (lambda != null)
            {
                return true;
            }

            Log.Warning("[Configurable Worksites] Could not find the " + description + " in QuestNode_Root_WorkSite.PotentialSiteTiles; that setting will have no effect.");
            return false;
        }
    }

    [HarmonyPatch]
    public static class PotentialSiteTilesMaxRangePatch
    {
        public static bool Prepare()
        {
            return PotentialSiteTilesLambdas.LogMissing(PotentialSiteTilesLambdas.MaxRangeFilter, "maximum spawn range check");
        }

        public static MethodBase TargetMethod()
        {
            return PotentialSiteTilesLambdas.MaxRangeFilter;
        }

        [HarmonyPostfix]
        [HarmonyInfix(PotentialSiteTilesLambdas.VanillaMaxSpawnRange)]
        public static float MaxSpawnRange(float __result)
        {
            return ModSettings.worksiteMaxSpawnRange;
        }
    }

    [HarmonyPatch]
    public static class PotentialSiteTilesMinRangePatch
    {
        public static bool Prepare()
        {
            return PotentialSiteTilesLambdas.LogMissing(PotentialSiteTilesLambdas.MinRangeVisitor, "minimum spawn range check");
        }

        public static MethodBase TargetMethod()
        {
            return PotentialSiteTilesLambdas.MinRangeVisitor;
        }

        [HarmonyPostfix]
        [HarmonyInfix(PotentialSiteTilesLambdas.VanillaMinSpawnDist)]
        public static float MinSpawnDistance(float __result)
        {
            return ModSettings.worksiteMinSpawnRange;
        }
    }
}

using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System.Collections.Generic;
using Verse;

namespace SK_Configurable_Worksites.Patches
{
    public static class QuestNodeRootWorkSitePatches
    {
        [HarmonyPatch(typeof(QuestNode_Root_WorkSite), "PotentialSiteTiles")]
        public static class PotentialSiteTiles
        {
            public static bool Prefix(PlanetTile root, ref List<PlanetTile> __result)
            {
                List<PlanetTile> tiles = new List<PlanetTile>();
                root.Layer.Filler.FloodFill(root, (PlanetTile p) => !Find.World.Impassable(p) && Find.WorldGrid.ApproxDistanceInTiles(p, root) <= ModSettings.worksiteMaxSpawnRange, delegate (PlanetTile p)
                {
                    if (Find.WorldGrid.ApproxDistanceInTiles(p, root) >= ModSettings.worksiteMinSpawnRange && Find.World.landmarks?[p] == null)
                    {
                        tiles.Add(p);
                    }
                });
                __result = tiles;
                return false;
            }
        }
    }
}
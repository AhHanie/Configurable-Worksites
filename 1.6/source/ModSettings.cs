using RimWorld;
using System.Collections.Generic;
using Verse;

namespace SK_Configurable_Worksites
{
    public class ModSettings : Verse.ModSettings
    {
        public const float DefaultMinSpawnRange = 3f;
        public const float DefaultMaxSpawnRange = 9f;

        public static float worksiteMinSpawnRange = DefaultMinSpawnRange;
        public static float worksiteMaxSpawnRange = DefaultMaxSpawnRange;
        public static Dictionary<string, float> biomeCampSelectionWeights = new Dictionary<string, float>();
        public static readonly Dictionary<string, float> defaultBiomeCampSelectionWeights = new Dictionary<string, float>();
        public static bool relaxedWorksiteSpawningConditions = false;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref worksiteMinSpawnRange, "worksiteMinSpawnRange", DefaultMinSpawnRange);
            Scribe_Values.Look(ref worksiteMaxSpawnRange, "worksiteMaxSpawnRange", DefaultMaxSpawnRange);
            Scribe_Collections.Look(ref biomeCampSelectionWeights, "biomeCampSelectionWeights", LookMode.Value, LookMode.Value);
            Scribe_Values.Look(ref relaxedWorksiteSpawningConditions, "relaxedWorksiteSpawningConditions", false);
            if (biomeCampSelectionWeights == null)
            {
                biomeCampSelectionWeights = new Dictionary<string, float>();
            }
        }

        public static float GetBiomeCampSelectionWeight(BiomeDef biome)
        {
            if (biomeCampSelectionWeights.TryGetValue(biome.defName, out float value))
            {
                return value;
            }

            return biome.campSelectionWeight;
        }

        public static void SetBiomeCampSelectionWeight(BiomeDef biome, float value)
        {
            if (defaultBiomeCampSelectionWeights.TryGetValue(biome.defName, out float defaultValue) && value == defaultValue)
            {
                biomeCampSelectionWeights.Remove(biome.defName);
                biome.campSelectionWeight = defaultValue;
                return;
            }

            biomeCampSelectionWeights[biome.defName] = value;
            biome.campSelectionWeight = value;
        }

        public static void ApplyBiomeCampSelectionWeights()
        {
            foreach (KeyValuePair<string, float> entry in biomeCampSelectionWeights)
            {
                BiomeDef biome = DefDatabase<BiomeDef>.GetNamedSilentFail(entry.Key);
                biome.campSelectionWeight = entry.Value;
            }
        }

        public static void CaptureDefaultBiomeCampSelectionWeights()
        {
            if (defaultBiomeCampSelectionWeights.Count > 0)
            {
                return;
            }

            foreach (BiomeDef biome in DefDatabase<BiomeDef>.AllDefsListForReading)
            {
                if (!defaultBiomeCampSelectionWeights.ContainsKey(biome.defName))
                {
                    defaultBiomeCampSelectionWeights.Add(biome.defName, biome.campSelectionWeight);
                }
            }
        }

        public static void ResetToDefaults()
        {
            CaptureDefaultBiomeCampSelectionWeights();

            worksiteMinSpawnRange = DefaultMinSpawnRange;
            worksiteMaxSpawnRange = DefaultMaxSpawnRange;
            relaxedWorksiteSpawningConditions = false;

            biomeCampSelectionWeights.Clear();

            foreach (KeyValuePair<string, float> entry in defaultBiomeCampSelectionWeights)
            {
                BiomeDef biome = DefDatabase<BiomeDef>.GetNamedSilentFail(entry.Key);
                if (biome != null)
                {
                    biome.campSelectionWeight = entry.Value;
                }
            }
        }
    }
}
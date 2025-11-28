using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace SK_Configurable_Worksites
{
    public static class ModSettingsWindow
    {
        private static BiomeDef selectedBiome = DefDatabase<BiomeDef>.AllDefsListForReading.OrderBy(b => b.label).FirstOrDefault();
        private static float campWeight = ModSettings.GetBiomeCampSelectionWeight(selectedBiome);
        private static float oldCampWeight = campWeight;

        public static void Draw(Rect parent)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(parent);

            listing.Label("SKCW.ConfigureSpawnDistances".Translate());

            Rect minRect = listing.GetRect(Text.LineHeight);
            Widgets.Label(minRect.LeftHalf(), "SKCW.MinimumSpawnRangeLabel".Translate(ModSettings.worksiteMinSpawnRange.ToString("F1")));
            ModSettings.worksiteMinSpawnRange = Widgets.HorizontalSlider(minRect.RightHalf(), ModSettings.worksiteMinSpawnRange, 1f, 50f, false, null, null, null, 0.1f);

            Rect maxRect = listing.GetRect(Text.LineHeight);
            Widgets.Label(maxRect.LeftHalf(), "SKCW.MaximumSpawnRangeLabel".Translate(ModSettings.worksiteMaxSpawnRange.ToString("F1")));
            ModSettings.worksiteMaxSpawnRange = Widgets.HorizontalSlider(maxRect.RightHalf(), ModSettings.worksiteMaxSpawnRange, 1f, 30f, false, null, null, null, 0.1f);

            if (ModSettings.worksiteMaxSpawnRange < ModSettings.worksiteMinSpawnRange)
            {
                ModSettings.worksiteMaxSpawnRange = ModSettings.worksiteMinSpawnRange;
            }

            listing.GapLine();
            listing.Label("SKCW.ConfigureBiomeWeights".Translate());

            Rect dropdownRect = listing.GetRect(Text.LineHeight);
            Widgets.Label(dropdownRect.LeftHalf(), "SKCW.BiomeLabel".Translate());
            Widgets.Dropdown(dropdownRect.RightHalf(), selectedBiome, (_) => selectedBiome, GenerateBiomeOptions, selectedBiome?.LabelCap ?? "SKCW.SelectBiomePlaceholder".Translate());
            Rect sliderRect = listing.GetRect(Text.LineHeight);
            Widgets.Label(sliderRect.LeftHalf(), $"SKCW.CampSelectionWeightLabel".Translate(campWeight.ToString("F1")));
            campWeight = Widgets.HorizontalSlider(sliderRect.RightHalf(), campWeight, 0f, 2f, false, null, null, null, 0.1f);

            if (campWeight != oldCampWeight)
            {
                ModSettings.SetBiomeCampSelectionWeight(selectedBiome, campWeight);
                oldCampWeight = campWeight;
            }

            listing.GapLine();
            listing.CheckboxLabeled("SKCW.RelaxedWorksiteSpawningLabel".Translate(), ref ModSettings.relaxedWorksiteSpawningConditions, "SKCW.RelaxedWorksiteSpawningTooltip".Translate());
            listing.GapLine();
            Rect resetRect = listing.GetRect(Text.LineHeight);
            if (Widgets.ButtonText(resetRect, "SKCW.ResetToDefaultsLabel".Translate()))
            {
                ModSettings.ResetToDefaults();
            }
            TooltipHandler.TipRegion(resetRect, "SKCW.ResetToDefaultsTooltip".Translate());
            listing.GapLine();
            listing.Label("SKCW.SettingsRestartNotice".Translate());
            listing.End();
        }

        private static IEnumerable<Widgets.DropdownMenuElement<BiomeDef>> GenerateBiomeOptions(BiomeDef _)
        {
            IEnumerable<BiomeDef> biomes = DefDatabase<BiomeDef>.AllDefsListForReading.OrderBy(b => b.label);
            foreach (BiomeDef biome in biomes)
            {
                yield return new Widgets.DropdownMenuElement<BiomeDef>
                {
                    option = new FloatMenuOption(biome.LabelCap, () =>
                    {
                        selectedBiome = biome;
                        oldCampWeight = campWeight;
                        campWeight = ModSettings.GetBiomeCampSelectionWeight(selectedBiome);
                    }),
                    payload = biome
                };
            }
        }
    }
}
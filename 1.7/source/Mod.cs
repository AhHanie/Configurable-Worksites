using HarmonyLib;
using UnityEngine;
using Verse;

namespace SK_Configurable_Worksites
{
    public class Mod: Verse.Mod
    {
        public static Harmony instance;
        public Mod(ModContentPack content)
           : base(content)
        {
            GetSettings<ModSettings>();
            instance = new Harmony("rimworld.sk.configurableworksites");
            instance.PatchAll();
            LongEventHandler.ExecuteWhenFinished(InitBiomeWeights);
        }

        public override string SettingsCategory()
        {
            return "Configurable Worksites";
        }

        public override void DoSettingsWindowContents(Rect rect)
        {
            ModSettingsWindow.Draw(rect);
            base.DoSettingsWindowContents(rect);
        }

        public static void InitBiomeWeights()
        {
            ModSettings.CaptureDefaultBiomeCampSelectionWeights();
            ModSettings.ApplyBiomeCampSelectionWeights();
        }
    }
}

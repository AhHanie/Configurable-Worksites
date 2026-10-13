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
            instance = new Harmony("rimworld.sk.configurableworksites");
            LongEventHandler.QueueLongEvent(Init, "Configurable Worksites Init", doAsynchronously: true, null);
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

        public void Init()
        {
            GetSettings<ModSettings>();
            ModSettings.CaptureDefaultBiomeCampSelectionWeights();
            ModSettings.ApplyBiomeCampSelectionWeights();
            instance.PatchAll();
        }
    }
}

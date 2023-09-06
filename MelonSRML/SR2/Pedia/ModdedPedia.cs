using Il2CppMonomiPark.SlimeRancher.UI.Pedia;
using MelonSRML.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Localization;

namespace MelonSRML.SR2.Pedia
{
    public abstract class ModdedPedia
    {
        public abstract string PediaName { get; }
        public abstract PediaCategoryEnum PediaCategory { get; }
        public abstract IdentifiableType PediaIdent { get; }
        public abstract string PediaIntro { get; }
        public abstract bool AutoUnlocked { get; }
        public abstract ModdedPediaPage[] PediaPages { get; }

        public static List<PediaEntry> ModdedPedias = new List<PediaEntry>();

        public PediaEntry GetPedia()
        {
            PediaEntryCategory pediaEntryCategory = SRSingleton<SceneContext>.Instance.PediaDirector.entryCategories.items.ToArray().First(x => x.name == PediaCategory.ToString());
            PediaEntryCategory basePediaEntryCategory = SRSingleton<SceneContext>.Instance.PediaDirector.entryCategories.items.ToArray().First(x => x.name == PediaCategory.ToString());
            PediaEntry pediaEntry = basePediaEntryCategory.items.ToArray().First();
            IdentifiablePediaEntry identifiablePediaEntry = ScriptableObject.CreateInstance<IdentifiablePediaEntry>();

            LocalizedString intro = TranslationPatcher.AddTranslation("Pedia", $"m.intro.{PediaIdent.localizationSuffix}", PediaIntro);
            foreach (ModdedPediaPage page in PediaPages)
            {
                TranslationPatcher.AddTranslation("PediaPage", $"m.{page.PediaPageName}.{PediaIdent.localizationSuffix}.page.{page.PediaPageNumber}", page.PediaPageText);
            }
            
            identifiablePediaEntry.hideFlags |= HideFlags.HideAndDontSave;
            identifiablePediaEntry.name = PediaName;
            identifiablePediaEntry.identifiableType = PediaIdent;
            identifiablePediaEntry.template = pediaEntry.template;
            identifiablePediaEntry.title = PediaIdent.localizedName;
            identifiablePediaEntry.description = intro;
            identifiablePediaEntry.isUnlockedInitially = AutoUnlocked;
            identifiablePediaEntry.actionButtonLabel = pediaEntry.actionButtonLabel;
            identifiablePediaEntry.infoButtonLabel = pediaEntry.infoButtonLabel;

            if (!pediaEntryCategory.items.Contains(identifiablePediaEntry))
                pediaEntryCategory.items.Add(identifiablePediaEntry);
            if (!ModdedPedias.Contains(identifiablePediaEntry))
                ModdedPedias.Add(identifiablePediaEntry);

            return identifiablePediaEntry;
        }

        public enum PediaCategoryEnum
        {
            Tutorials,
            Slimes,
            Resources,
            Ranch,
            World,
            Science
        }
    }
    
    public class ModdedPediaPage
    {
        public string PediaPageName;
        public string PediaPageText;
        public int PediaPageNumber = 1;

        public ModdedPediaPage(string name, string text, int number)
        {
            PediaPageName = name;
            PediaPageText = text;
            PediaPageNumber = number;
        }

        public ModdedPediaPage(string name, string text)
        {
            PediaPageName = name;
            PediaPageText = text;
        }
    }
}

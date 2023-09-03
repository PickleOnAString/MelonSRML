using HarmonyLib;
using MelonSRML.SR2;
using MelonSRML.SR2.Idents;
using MelonSRML.SR2.Slime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MelonSRML.Patches.SaveSystem
{
    [HarmonyPatch(typeof(AutoSaveDirector), "Awake")]
    public static class PatchAutoSaveDirectorAwake
    {
        public static void Prefix(AutoSaveDirector __instance)
        {
            Debug.Log("Start Save Patching");
            foreach (ModdedSlime slime in ModdedSlime.moddedSlimes)
            {
                SRLookup.Get<IdentifiableTypeGroup>("BaseSlimeGroup").memberTypes.Add(slime.definition);
                SRLookup.Get<IdentifiableTypeGroup>("VaccableBaseSlimeGroup").memberTypes.Add(slime.definition);
                SRLookup.Get<IdentifiableTypeGroup>("SlimesGroup").memberTypes.Add(slime.definition);
                __instance.identifiableTypes.memberTypes.Add(slime.definition);
            }

            foreach (ModdedIdent ident in ModdedIdent.moddedIdents)
            {
                Debug.Log(ident.ident.name);
                __instance.identifiableTypes.memberTypes.Add(ident.ident);
            }

            foreach (IdentifiableType plort in ModdedIdent.moddedPlorts)
            {
                SRLookup.Get<IdentifiableTypeGroup>("PlortGroup").memberTypes.Add(plort);
            }
        }
    }
}

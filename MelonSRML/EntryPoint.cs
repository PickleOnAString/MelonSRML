global using UnityEngine;
global using MelonLoader;
global using static MelonLoader.MelonLogger;
using MelonSRML.Patches;
using MelonSRML.SR2.Slime;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppMonomiPark.SlimeRancher.Damage;
using MelonSRML.Console;
using MelonSRML.EnumPatcher;
using MelonSRML.SR2.Ranch;
using System.Linq;
using System;
using Il2Cpp;
using static UnityEngine.UI.InputField;
using MelonSRML.SR2.Idents;

namespace MelonSRML
{
    internal class EntryPoint : MelonPlugin
    {
        internal static List<SRMLMelonMod> registeredMods = new List<SRMLMelonMod>();

        internal static bool interruptMenuLoad = false;
        internal static bool interruptGameLoad = false;
        internal static LoadingError error;

        internal static Transform prefabParent;

        internal static Damage KillObject;
        internal static MethodInfo TryCast = AccessTools.Method(typeof(Il2CppObjectBase), nameof(Il2CppObjectBase.TryCast));

        public override void OnInitializeMelon()
        {
            
            if (KillObject == null)
            {
                KillObject = new Damage
                {
                    damageSource = ScriptableObject.CreateInstance<DamageSourceDefinition>()
                };
                KillObject.damageSource.hideFlags |= HideFlags.HideAndDontSave;
                KillObject.damageSource.logMessage = "RemoveCommand.Execute";
            }
            
            /*
            This currently doesn't work
            ClassInjector.RegisterTypeInIl2Cpp<ModdedSlimeSubbehavior>();
            CustomSlimeSubbehaviorPatches.moddedType = Il2CppType.Of<ModdedSlimeSubbehavior>();
            */

            ClassInjector.RegisterTypeInIl2Cpp<ModdedPlotUpgrader>();
            Console.Console.Init();


            HarmonyInstance.PatchAll();
            SystemContext.IsModded = true;

            Debug.Log("LOADING SLIMES...");
            Type[] slimes = Assembly.GetExecutingAssembly().GetTypes().Where(type => type.IsSubclassOf(typeof(ModdedSlime))).ToArray();
            foreach (var slime in slimes)
            {
                ModdedSlime moddedSlimeInstance = (ModdedSlime)Activator.CreateInstance(slime);
                moddedSlimeInstance.Init();
                Debug.Log("Loaded 1 slime");
            }

            Type[] moddedIdents = Assembly.GetExecutingAssembly().GetTypes().Where(type => type.IsSubclassOf(typeof(ModdedIdent))).ToArray();
            foreach (var moddedIdent in moddedIdents)
            {
                ModdedIdent moddedIdentInstance = (ModdedIdent)Activator.CreateInstance(moddedIdent);
                moddedIdentInstance.Init();
                Debug.Log("Loaded 1 slime");
            }
        }

        public override void OnPreModsLoaded()
        {
            OnMelonRegistered.Subscribe(x =>
            {
                if (x is not SRMLMelonMod mod) return;

                registeredMods.Add(mod);
                EnumHolderResolver.RegisterAllEnums(mod);
            });
        }
        public override void OnApplicationQuit()
        {
            KeyBindManager.Pull();
        }
    }
}


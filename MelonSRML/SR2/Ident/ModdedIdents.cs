using Il2CppMonomiPark.SlimeRancher.UI;
using MelonSRML.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MelonSRML.SR2.Idents
{
    public abstract class ModdedIdent
    {
        public abstract string name { get; }
        public abstract string displayName { get; }
        public abstract Color color { get; }
        public abstract string localizationSuffix { get; }
        public virtual IdentifiableTypeGroup foodGroup { get; }
        public virtual Type type { get; } = Type.OTHER;

        public virtual MarketUI.PlortEntry plortEntry => null;
        public virtual EconomyDirector.ValueMap valueMap => null;

        public IdentifiableType ident { get; private set; }

        public static List<ModdedIdent> moddedIdents = new List<ModdedIdent>();
        public static List<ModdedIdent> moddedPlorts = new List<ModdedIdent>();

        public void Init()
        {
            Debug.Log("Modded Ident: " + moddedIdents.Count);
            moddedIdents.Add(this);

            ident = ScriptableObject.CreateInstance<IdentifiableType>();
            Debug.Log(ident);
            ident.hideFlags |= HideFlags.HideAndDontSave;
            ident.name = name;
            ident.color = color;
            ident.localizationSuffix = localizationSuffix;
            ident.foodGroup = foodGroup;

            Debug.Log("TESTER");
            switch (type)
            {
                case Type.PLORT:
                    Debug.Log("NAHHHH");
                    ident.IsPlort = true;
                    moddedPlorts.Add(this);
                    break;
                case Type.ANIMAL:
                    ident.IsAnimal = true;
                    break;
                case Type.OTHER:
                    break;
            }

            MelonEvents.OnSceneWasLoaded.Subscribe(OnSceneWasLoaded);
        }

        public virtual void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            switch (sceneName)
            {
                case "SystemCore":
                    {
                        break;
                    }
                case "GameCore":
                    {
                        ident.localizedName = TranslationPatcher.AddTranslation("Actor", $"l.{localizationSuffix}", displayName);
                        ident.prefab = GetPrefab();
                        break;
                    }
            }
        }

        /// <summary>
        /// Gets the prefab of the <c>ModdedIdent</c>
        /// </summary>
        /// <returns></returns>
        public abstract GameObject GetPrefab();

        public enum Type
        {
            PLORT,
            ANIMAL,
            OTHER
        }
    }
}

using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.UI;
using MelonSRML.Utils;
using MelonSRML.Utils.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MelonSRML.SR2.Slime
{
    public abstract class ModdedSlime
    {
        public abstract string slimeName { get; }

        public abstract string baseSlime { get; }

        public abstract SlimeDiet diet { get; }

        public abstract Color[] plate { get; }

        public virtual Color faceColor => Color.black;

        public virtual string slimeIcon => $"Assets.{slimeName.ToLower()}_slime_ico";

        public SlimeDefinition definition;
        
        public static List<ModdedSlime> moddedSlimes = new List<ModdedSlime>();

        public virtual void Init()
        {
            moddedSlimes.Add(this);

            SetupDefinition();

            MelonEvents.OnSceneWasLoaded.Subscribe(OnSceneWasLoaded);
        }
        
        public virtual void SetupDefinition()
        {
            definition = ScriptableObject.CreateInstance<SlimeDefinition>();
            definition.hideFlags |= HideFlags.HideAndDontSave;
            definition.name = slimeName;
            definition.color = plate[0];
            definition.localizationSuffix = slimeName.ToLower() + "_slime";
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
                        definition.localizedName = TranslationPatcher.AddTranslation("Actor", $"l.{slimeName.ToLower()}_slime", $"{slimeName} Slime");

                        SetupPrefab();

                        definition.Diet = diet;
                        definition.Diet.RefreshEatMap(SRSingleton<GameContext>.Instance.SlimeDefinitions, definition);

                        definition.icon = TextureUtils.ConvertSprite(LoadImage($"Assets.{slimeName.ToLower()}_slime_ico"));
                        definition.properties = UnityEngine.Object.Instantiate(SRLookup.Get<SlimeDefinition>("Pink").properties);
                        definition.defaultPropertyValues = UnityEngine.Object.Instantiate(SRLookup.Get<SlimeDefinition>("Pink")).defaultPropertyValues;

                        SetupAppearance();
                        break;
                    }
                case "zoneCore":
                    {
                        RegisterSlime();
                        break;
                    }
            }

            OnSceneAddSpawners(sceneName);
        }
        
        public virtual void SetupPrefab()
        {
            definition.prefab = SRLookup.CopyPrefab(SRLookup.Get<GameObject>("slimePink"));
            definition.prefab.name = slimeName.ToLower() + "Slime";

            definition.prefab.GetComponent<Identifiable>().tag = slimeName + " Slime";
            definition.prefab.GetComponent<Identifiable>().identType = definition;
            definition.prefab.GetComponent<SlimeEat>().slimeDefinition = definition;
        }
        public virtual void SetupAppearance()
        {
            SlimeAppearance slimeAppearance = UnityEngine.Object.Instantiate(SRLookup.Get<SlimeAppearance>("PinkDefault"));
            SlimeAppearanceApplicator slimeAppearanceApplicator = definition.prefab.GetComponent<SlimeAppearanceApplicator>();
            slimeAppearance.name = "default";
            slimeAppearanceApplicator.Appearance = slimeAppearance;
            slimeAppearanceApplicator.SlimeDefinition = definition;

            Material material2 = UnityEngine.Object.Instantiate(slimeAppearance.Structures[0].DefaultMaterials[0]);
            material2.hideFlags |= HideFlags.HideAndDontSave;
            material2.SetColor("_TopColor", plate[1]);
            material2.SetColor("_MiddleColor", plate[2]);
            material2.SetColor("_BottomColor", plate[3]);
            material2.SetColor("_SpecColor", plate[4]);
            slimeAppearance.Structures[0].DefaultMaterials[0] = material2;

            slimeAppearance.Face = UnityEngine.Object.Instantiate(SRLookup.Get<SlimeAppearance>("PinkDefault").Face);
            slimeAppearance.Face.name = slimeName + "Face";

            SlimeExpressionFace[] expressionFaces = new SlimeExpressionFace[0];
            foreach (SlimeExpressionFace slimeExpressionFace in slimeAppearance.Face.ExpressionFaces)
            {
                Material slimeEyes = UnityEngine.Object.Instantiate(slimeExpressionFace.Eyes);
                if (slimeEyes)
                {
                    slimeEyes.SetColor("_EyeRed", faceColor);
                    slimeEyes.SetColor("_EyeGreen", faceColor);
                    slimeEyes.SetColor("_EyeBlue", faceColor);
                }
                slimeExpressionFace.Eyes = slimeEyes;
                expressionFaces = expressionFaces.AddToArray(slimeExpressionFace);
            }
            slimeAppearance.Face.ExpressionFaces = expressionFaces;
            slimeAppearance.Face.OnEnable();

            slimeAppearance.Icon = TextureUtils.ConvertSprite(LoadImage($"Assets.{slimeName.ToLower()}_slime_ico"));
            slimeAppearance.SplatColor = plate[0];
            slimeAppearance.ColorPalette = new SlimeAppearance.Palette
            {
                Ammo = plate[0],
                Top = plate[2],
                Middle = plate[3],
                Bottom = plate[4]
            };
            definition.AppearancesDefault = new SlimeAppearance[] { slimeAppearance };
            slimeAppearance.hideFlags |= HideFlags.HideAndDontSave;
        }

        public virtual void RegisterSlime()
        {
            SRSingleton<SceneContext>.Instance.SlimeAppearanceDirector.RegisterDependentAppearances(SRLookup.Get<SlimeDefinition>(slimeName), SRLookup.Get<SlimeDefinition>(slimeName).AppearancesDefault[0]);
            SRSingleton<SceneContext>.Instance.SlimeAppearanceDirector.UpdateChosenSlimeAppearance(SRLookup.Get<SlimeDefinition>(slimeName), SRLookup.Get<SlimeDefinition>(slimeName).AppearancesDefault[0]);
            SRSingleton<GameContext>.Instance.SlimeDefinitions.Slimes = SRSingleton<GameContext>.Instance.SlimeDefinitions.Slimes.AddItem(definition).ToArray();
            SRSingleton<GameContext>.Instance.SlimeDefinitions.slimeDefinitionsByIdentifiable.TryAdd(definition, definition);
        }

        public virtual void OnSceneAddSpawners(string sceneName)
        {
            switch (sceneName.Contains("zoneFields"))
            {
                case true:
                    {
                        IEnumerable<DirectedSlimeSpawner> source = UnityEngine.Object.FindObjectsOfType<DirectedSlimeSpawner>();
                        foreach (DirectedSlimeSpawner directedSlimeSpawner in source)
                        {
                            foreach (DirectedActorSpawner.SpawnConstraint spawnConstraint in directedSlimeSpawner.constraints)
                            {
                                spawnConstraint.slimeset.members = spawnConstraint.slimeset.members.AddItem(new SlimeSet.Member
                                {
                                    prefab = definition.prefab,
                                    identType = definition,
                                    weight = 0.3f
                                }).ToArray();
                            }
                        }
                        break;
                    }
            }
        }

        public static Texture2D LoadImage(string filename)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(executingAssembly.GetName().Name + "." + filename + ".png");
            byte[] array = new byte[manifestResourceStream.Length];
            manifestResourceStream.Read(array, 0, array.Length);
            Texture2D texture2D = new Texture2D(1, 1);
            ImageConversion.LoadImage(texture2D, array);
            texture2D.filterMode = FilterMode.Bilinear;
            return texture2D;
        }
    }
}

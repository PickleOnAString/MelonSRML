using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.UI;
using MelonSRML.SR2;
using MelonSRML.SR2.Idents;
using MelonSRML.SR2.Pedia;
using MelonSRML.SR2.Slime;
using MelonSRML.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MelonSRML
{
    public class TestSlime : ModdedSlime
    {
        public override string slimeName => "Test";
        public override string baseSlime => "Pink";
        public override SlimeDiet diet => new SlimeDiet()
        {
            MajorFoodIdentifiableTypeGroups = new IdentifiableTypeGroup[] { SRLookup.Get<IdentifiableTypeGroup>("VeggieGroup") },
            MajorFoodGroups = new SlimeEat.FoodGroup[] { SlimeEat.FoodGroup.VEGGIES },
            ProduceIdents = new IdentifiableType[] { SRLookup.Get<IdentifiableType>("testPlort") },
            AdditionalFoodIdents = new IdentifiableType[] { SRLookup.Get<IdentifiableType>("PinkPlort"), SRLookup.Get<IdentifiableType>("Pink") },
            FavoriteIdents = new IdentifiableType[] { SRLookup.Get<IdentifiableType>("BeetVeggie") }
        };
        public override Color[] plate => new Color[]
        {
            Color.red,
            Color.blue,
            Color.red,
            Color.green,
            Color.blue
        };
    }

    public class TestPlort : ModdedIdent
    {
        public override string name => "testPlort";
        public override string displayName => "Test Plort";
        public override Color color => Color.red;
        public override string localizationSuffix => "test_plort";
        public override Type type => Type.PLORT;

        public override MarketUI.PlortEntry plortEntry => new MarketUI.PlortEntry
        {
            identType = ident
        };
        public override EconomyDirector.ValueMap valueMap => new EconomyDirector.ValueMap
        {
            accept = ident.prefab.GetComponent<Identifiable>(),
            fullSaturation = 7f,
            value = 50f
        };

        public override GameObject GetPrefab()
        {
            GameObject gameObject;
            gameObject = SRLookup.CopyPrefab(SRLookup.Get<IdentifiableType>("PinkPlort").prefab);
            gameObject.name = name;
            gameObject.GetComponent<Identifiable>().identType = this.ident;
            this.ident.icon = TextureUtils.ConvertSprite(ModdedSlime.LoadImage("Assets.test_plort_ico"));

            Material material = UnityEngine.Object.Instantiate(SRLookup.Get<GameObject>("plortPink").GetComponent<MeshRenderer>().sharedMaterial);
            material.SetColor("_TopColor", Color.green);
            material.SetColor("_MiddleColor", Color.yellow);
            material.SetColor("_BottomColor", Color.blue);
            gameObject.GetComponent<MeshRenderer>().sharedMaterial = material;

            return gameObject;
        }
    }

    public class TestSlimePedia : ModdedPedia
    {
        public override string PediaName => "Test";

        public override PediaCategoryEnum PediaCategory => PediaCategoryEnum.Slimes;

        public override IdentifiableType PediaIdent => SRLookup.Get<IdentifiableType>("Test");

        public override string PediaIntro => "A slime made for testing, testy, test, test test";

        public override bool AutoUnlocked => false;

        public override ModdedPediaPage[] PediaPages => new ModdedPediaPage[]
        {
            new ModdedPediaPage("slimeology", "A rainbow slime"),
            new ModdedPediaPage("plortonomics", "The Test plort is a full spectrum of colors, ranchers use it as a house decoation")
        };
    }
}

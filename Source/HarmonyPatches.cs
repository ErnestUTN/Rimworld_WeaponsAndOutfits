using Harmony;
using Verse;
using RimWorld;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;


namespace WeaponsAndOutfits
{
    [StaticConstructorOnStartup]
    class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var harmony = HarmonyInstance.Create("github.ErnestUTN.rimworld.mods.WeaponsAndOutfits");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
      
        [HarmonyPatch(typeof(Dialog_ManageOutfits), "DoWindowContents")]
        static class Patch_Dialog_ManageOutfits_DoWindowContents
        {
            private static Vector2 ptr2;
            
            static void Postfix(Dialog_ManageOutfits __instance , Outfit ___selOutfitInt)
            {
                if (___selOutfitInt == null)
                {
                    return;
                }

                Rect filterRect = new Rect(320f, 40f+40f+10f , 300f, 509f);
                ThingFilter equipmentFilter = Traverse.Create(___selOutfitInt).Field("equipmentFilter").GetValue<ThingFilter>();
                if (equipmentFilter == null)
                {
                    equipmentFilter = new ThingFilter();
                    equipmentFilter.SetAllow(ThingCategoryDefOf.Weapons, false, null, null);
                }
                ThingFilter UIequipmentFilter = equipmentFilter;
                ThingFilter equipmentFilterParent = new ThingFilter();
                equipmentFilterParent.SetAllow(ThingCategoryDefOf.Weapons, true, null, null);
                ThingFilterUI.DoThingFilterConfigWindow(filterRect,  ref ptr2, UIequipmentFilter, equipmentFilterParent, 16, null, null, false, null, null);
            }
        }
        
        [HarmonyPatch (typeof(Widgets),"FloatRange")]
        static class Patch_Widgets_FloatRange
        {
           
            static void Prefix(ref int id)
            {

                if (id == 1)
                {
                    System.Random rand = new System.Random();
                    id = rand.Next(1,1000);
                }
            }
        }

        [HarmonyPatch(typeof(Widgets), "QualityRange")]
        static class Patch_Widgets_QualityRange
        {

            static void Prefix(ref int id)
            {

                if (id == 876813230)
                {
                    System.Random rand = new System.Random();
                    id = rand.Next(876813230, 876813230+1000);
                }
            }
        }

        [HarmonyPatch(typeof(OutfitDatabase), nameof(OutfitDatabase.MakeNewOutfit))]
        static class OutfitDatabase_MakeNewOutfit_Patch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var oldConstructor = AccessTools.Constructor(typeof(Outfit), new[] { typeof(int), typeof(string) });
                var newConstructor = AccessTools.Constructor(typeof(ExtendedOutfit), new[] { typeof(int), typeof(string) });
                foreach (var instruction in instructions)
                {
                    if (instruction.opcode == OpCodes.Newobj && oldConstructor.Equals(instruction.operand))
                    {
                        instruction.operand = newConstructor;
                    }

                    yield return instruction;
                }
            }
        }

        [HarmonyPatch(typeof(OutfitDatabase), nameof(OutfitDatabase.ExposeData))]
        static class OutfitDatabase_ExposeData_Patch
        {
            static void Postfix(OutfitDatabase __instance, List<Outfit> ___outfits)
            {
                if (Scribe.mode != LoadSaveMode.LoadingVars)
                {
                    return;
                }

                if (___outfits.Any(i => i is ExtendedOutfit))
                {
                    return;
                }

                foreach (var outfit in ___outfits.ToList())
                {
                    ___outfits.Remove(outfit);

                    ___outfits.Add(ReplaceKnownVanillaOutfits(outfit));
                }

            }

            static Outfit ReplaceKnownVanillaOutfits(Outfit outfit)
            {
                var newOutfit = new ExtendedOutfit(outfit);
               

                return newOutfit;
            }
        }



    }
}

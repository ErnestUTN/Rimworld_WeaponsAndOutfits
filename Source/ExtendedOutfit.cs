using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace WeaponsAndOutfits
{
    public class ExtendedOutfit : Outfit, IExposable
    {
        public ThingFilter equipmentFilter = new ThingFilter(); 
        public ExtendedOutfit()
        {

        }

          new public void  ExposeData()
        {
            Scribe_Values.Look(ref uniqueId, "uniqueId");
            Scribe_Values.Look(ref label, "label");
            Scribe_Deep.Look(ref filter, "filter", new object[0]);
            Scribe_Deep.Look<ThingFilter>(ref this.equipmentFilter, "equipf", new object[0]);


        }
        public ExtendedOutfit(int uniqueId, string label) : base(uniqueId, label)
        {
            // Used by OutfitDatabase_MakeNewOutfit_Patch
        }

        public ExtendedOutfit(Outfit outfit) : base(outfit.uniqueId, outfit.label)
        {
            // Used by OutfitDatabase_ExposeData_Patch

            filter.CopyAllowancesFrom(outfit.filter);
        }



    }

}
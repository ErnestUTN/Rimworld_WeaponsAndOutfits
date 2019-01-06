using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using RimWorld;
using Harmony;

namespace WeaponsAndOutfits
{
    class JobGiver_OptimizeEquipment : ThinkNode_JobGiver
    {
        private void SetNextOptimizeTick(Pawn pawn)
        {
            pawn.mindState.nextApparelOptimizeTick = Find.TickManager.TicksGame + Rand.Range(6000, 9000);
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.outfits == null)
            {
                
                return null;
            }
            if (pawn.Faction != Faction.OfPlayer)
            {
               
                return null;
            }
            if (!DebugViewSettings.debugApparelOptimize)
            {
                if (Find.TickManager.TicksGame < pawn.mindState.nextApparelOptimizeTick)
                {
                    return null;
                }
            }

            Outfit currentOutfit = pawn.outfits.CurrentOutfit;
            ThingFilter _equipmentFilter = Traverse.Create(currentOutfit).Field("equipmentFilter").GetValue<ThingFilter>();
            Pawn_EquipmentTracker equipment = pawn.equipment;
            ThingWithComps thingWithComps = (equipment != null) ? equipment.Primary : null;

            if (!((thingWithComps != null && !_equipmentFilter.Allows(thingWithComps)) || thingWithComps == null))
                return null;
                
            Thing thing = null;
            List<Thing> list = pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Weapon);
            if (list.Count == 0)
            {
                this.SetNextOptimizeTick(pawn);
                return null;
            }
            for (int j = 0; j < list.Count; j++)
            {
                Thing thing2 = list[j];
                if (_equipmentFilter.Allows(thing2) && thing2.IsInAnyStorage() && !thing2.IsForbidden(pawn) && !thing2.IsBurning() && pawn.CanReserveAndReach(thing2, PathEndMode.OnCell, pawn.NormalMaxDanger(), 1, -1, null, false))
                {
                    thing = thing2;
                    break;
                }
            }
            if (thing == null)
            {
                this.SetNextOptimizeTick(pawn);
                return null;
            }
            return new Job(JobDefOf.Equip, thing);
                
            
            

        }

    }
}

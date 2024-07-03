using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace QoLX.Modules
{
	public class PartyLandClaim : BaseModule
	{
		public override void Load()
		{
			base.Load();
		}

		public override void Unload()
		{
			base.Unload();
		}
		
		public static bool IsRelativeToOwner(PersistentPlayerData lpRelative, PersistentPlayerData landProtectionBlockOwner)
		{
			if (lpRelative == landProtectionBlockOwner)
				return true;

			var world = GameManager.Instance.World;
			if (world == null)
				return false;

			var players = world.GetPlayers();
			if (players == null)
				return false;

			EntityPlayer? ownerPlayer = null;
			foreach (var player in players)
			{
				if (player.entityId != landProtectionBlockOwner.EntityId) 
					continue;
				
				ownerPlayer = player;
				break;
			}

			if (ownerPlayer == null)
				return false;

			var party = ownerPlayer.party;
			if (party == null)
				return false;

			return party.ContainsMember(lpRelative.EntityId);
		}
		
		[HarmonyTranspiler, HarmonyPatch(typeof(World), nameof(World.IsMyLandClaimInChunk))]
		private static IEnumerable<CodeInstruction> IsMyLandClaimInChunk_Patch(IEnumerable<CodeInstruction> instructions)
		{
			var il = instructions.ToList();
			
			var index = il.FindIndex(instruction => instruction.opcode == OpCodes.Call && (instruction.operand as MethodInfo)?.Name == nameof(World.IsLandProtectionValidForPlayer));
			if (index <= 0)
			{
				Debug.LogError($"[{nameof(QoLX)}.{nameof(PartyLandClaim)}] Failed transpiling 'IsMyLandClaimInChunk_Patch' not found!");
				return il;
			}
			
			il[index + 8].opcode = OpCodes.Brfalse_S;
			il.Insert(index + 8, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PartyLandClaim), nameof(IsRelativeToOwner))));
			
			return il;
		}
	}	
}
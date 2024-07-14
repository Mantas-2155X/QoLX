using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace QoLX.Modules
{
	public class BugSquasher : BaseModule
	{
		public override void Load()
		{
			base.Load();
		}

		public override void Unload()
		{
			base.Unload();
		}

		public static Block AltBlockCheck(Block[] placeAltBlockClasses, int _typeId)
		{
			if (_typeId >= placeAltBlockClasses.Length)
			{
				Debug.LogWarning($"[QoLX.{nameof(BugSquasher)}] Using fallback for AltBlockCheck for {_typeId}");
				return placeAltBlockClasses[^1];
			}

			return placeAltBlockClasses[_typeId];
		}
		
		[HarmonyPrefix, HarmonyPatch(typeof(GameManager), nameof(GameManager.GetEntityIDForLockedTileEntity))]
		public static bool GetEntityIDForLockedTileEntity_Patch(ref int __result, TileEntity te)
		{
			if (te == null)
			{
				Debug.LogWarning($"[QoLX.{nameof(BugSquasher)}] Preventing error spam at GetEntityIDForLockedTileEntity");
				
				__result = -1;
				return false;
			}

			return true;
		}
		
		[HarmonyTranspiler, HarmonyPatch(typeof(Block), nameof(Block.GetAltBlock))]
		private static IEnumerable<CodeInstruction> Block_Patch(IEnumerable<CodeInstruction> instructions)
		{
			var il = instructions.ToList();
			
			var index = il.FindIndex(instruction => instruction.opcode == OpCodes.Ret);
			if (index <= 0)
			{
				Debug.LogError($"[QoLX.{nameof(BugSquasher)}] Failed transpiling 'Block_Patch' not found!");
				return il;
			}
			
			il[index - 1] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BugSquasher), nameof(AltBlockCheck)));
			return il;
		}
	}
}
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
	}
}
using HarmonyLib;
using UnityEngine;

namespace QoLX.Modules
{
	public class EfficientCooking : BaseModule
	{
		public override void Load()
		{
			base.Load();
		}

		public override void Unload()
		{
			base.Unload();
		}

		public static bool AnyMaterialsToMelt(float[] currentMeltTimesLeft)
		{
			if (currentMeltTimesLeft == null)
				return false;

			foreach (var timeLeft in currentMeltTimesLeft)
				if (timeLeft != -2.14748365E+09f)
					return true;

			return false;
		}
		
		[HarmonyPostfix, HarmonyPatch(typeof(TileEntityWorkstation), nameof(TileEntityWorkstation.UpdateTick))]
		public static void UpdateTick_Patch(TileEntityWorkstation __instance)
		{
			if (!__instance.IsBurning || __instance.hasRecipeInQueue())
				return;

			if (__instance.isModuleUsed[4])
			{
				if (AnyMaterialsToMelt(__instance.currentMeltTimesLeft))
					return;
			}
			else
			{
				if (!__instance.inputIsEmpty())
					return;
			}
			
			Debug.Log($"[QoLX.{nameof(EfficientCooking)}] Turning off {__instance} because it has finished cooking");

			__instance.IsBurning = false;
			__instance.ResetTickTime();
		}
	}
}
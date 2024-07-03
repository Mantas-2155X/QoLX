using HarmonyLib;

namespace QoLX.Modules
{
	public class VehicleHonk : BaseModule
	{
		private static readonly int honkIndex = 8;
		private static readonly int repairIndex = 2;

		public override void Load()
		{
			base.Load();
		}

		public override void Unload()
		{
			base.Unload();
		}
		
		[HarmonyPostfix, HarmonyPatch(typeof(EntityVehicle), nameof(EntityVehicle.GetActivationCommands))]
		public static void GetActivationCommands_Patch(EntityActivationCommand[] __result)
		{
			var honkAction = __result[honkIndex];
			var repairAction = __result[repairIndex];

			__result[repairIndex] = honkAction;
			__result[honkIndex] = repairAction;
		}
		
		[HarmonyPrefix, HarmonyPatch(typeof(EntityVehicle), nameof(EntityVehicle.CheckInteractionRequest))]
		public static void CheckInteractionRequest_Patch(ref int _requestId)
		{
			if (_requestId == honkIndex)
			{
				_requestId = repairIndex;
			}
			else if (_requestId == repairIndex)
			{
				_requestId = honkIndex;
			}
		}
		
		[HarmonyPrefix, HarmonyPatch(typeof(EntityVehicle), nameof(EntityVehicle.OnEntityActivated))]
		public static void OnEntityActivated_Patch(ref int _indexInBlockActivationCommands)
		{
			if (_indexInBlockActivationCommands == honkIndex)
			{
				_indexInBlockActivationCommands = repairIndex;
			}
			else if (_indexInBlockActivationCommands == repairIndex)
			{
				_indexInBlockActivationCommands = honkIndex;
			}
		}
	}
}
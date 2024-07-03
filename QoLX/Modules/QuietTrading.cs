using System.Collections.Generic;
using HarmonyLib;

namespace QoLX.Modules
{
	public class QuietTrading : BaseModule
	{
		private static readonly List<string> mutedVoiceLines = new List<string>
		{
			"trade",
			"sale_accepted",
			"sale_declined"
		};
		
		public override void Load()
		{
			base.Load();
		}

		public override void Unload()
		{
			base.Unload();
		}
		
		[HarmonyPrefix, HarmonyPatch(typeof(EntityTrader), nameof(EntityTrader.PlayVoiceSetEntry))]
		public static bool PlayVoiceSetEntry_Patch(string name)
		{
			return !mutedVoiceLines.Contains(name);
		}
	}
}
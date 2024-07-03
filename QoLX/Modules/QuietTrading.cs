using System;
using System.Collections.Generic;
using HarmonyLib;
using Newtonsoft.Json.Linq;

namespace QoLX.Modules
{
	public class QuietTrading : BaseModule
	{
		public static readonly string MutedLinesSetting = $"{nameof(QuietTrading)}_MutedLines";

		private static readonly Dictionary<string, List<string>> cachedMutedLines = new Dictionary<string, List<string>>();
		
		public override void Load()
		{
			base.Load();
			
			var configEntry = QoLX.ConfigEntries[MutedLinesSetting];
			if (configEntry?.Item2 == null)
				return;

			List<SQuietTraderInfo>? quietTraderInfos;

			if (configEntry.Item2 is JArray jArray)
				quietTraderInfos = jArray.ToObject<List<SQuietTraderInfo>>();
			else
				quietTraderInfos = (List<SQuietTraderInfo>)configEntry.Item2;
			
			if (quietTraderInfos == null)
				return;

			foreach (var quietTraderInfo in quietTraderInfos)
			{
				if (!cachedMutedLines.ContainsKey(quietTraderInfo.Trader))
					cachedMutedLines.Add(quietTraderInfo.Trader, new List<string>());
				
				cachedMutedLines[quietTraderInfo.Trader].AddRange(quietTraderInfo.MutedLines);
			}
		}

		public override void Unload()
		{
			base.Unload();
		}
		
		[HarmonyPrefix, HarmonyPatch(typeof(EntityTrader), nameof(EntityTrader.PlayVoiceSetEntry))]
		public static bool PlayVoiceSetEntry_Patch(EntityTrader __instance, string name)
		{
			if (!cachedMutedLines.TryGetValue(__instance.NPCInfo.Id, out var mutedLines) || mutedLines == null) 
				return true;
			
			return !mutedLines.Contains(name);
		}
		
		[Serializable]
		public struct SQuietTraderInfo
		{
			public string Trader;
			public string[] MutedLines;
		}
	}
}
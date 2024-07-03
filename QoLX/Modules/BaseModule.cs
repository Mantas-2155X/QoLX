using HarmonyLib;
using QoLX.Interfaces;
using UnityEngine;

namespace QoLX.Modules
{
	public class BaseModule : IModule
	{
		public bool Loaded { get; private set; }
		
		public virtual void Load()
		{
			Debug.Log($"[QoLX] Loading module {GetType().Name}");
			
			var harmony = new Harmony(GetHarmonyID(this));
			harmony.PatchAll(GetType());

			Loaded = true;
		}

		public virtual void Unload()
		{
			Debug.Log($"[QoLX] Unloading module {GetType().Name}");
			
			Harmony.UnpatchID(GetHarmonyID(this));
			
			Loaded = false;
		}
		
		public string GetHarmonyID(IModule module)
		{
			return $"2155X.{nameof(QoLX)}.{module.GetType().Name}";
		}
	}
}
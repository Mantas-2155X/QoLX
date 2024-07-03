using System;

namespace QoLX.Config
{
	[Serializable]
	public class ConfigEntry
	{
		public string Key;
		public string Description;
		public object? Value;
	}
}
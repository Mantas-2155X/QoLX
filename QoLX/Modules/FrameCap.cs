using System;

namespace QoLX.Modules
{
	public class FrameCap : BaseModule
	{
		public static readonly string CapTargetSetting = $"{nameof(FrameCap)}_TargetFramerate";
		
		private int previousTarget;

		public override void Load()
		{
			base.Load();
			
			var configEntry = QoLX.ConfigEntries[CapTargetSetting];
			if (configEntry?.Item2 == null)
				return;

			var target = Convert.ToInt32(configEntry.Item2);
			if (target < 0)
				target = 0;

			var wait = GameManager.Instance.waitForTargetFPS;
			
			previousTarget = wait.TargetFPS;
			wait.TargetFPS = target;
		}

		public override void Unload()
		{
			base.Unload();

			GameManager.Instance.waitForTargetFPS.TargetFPS = previousTarget;
		}
	}
}
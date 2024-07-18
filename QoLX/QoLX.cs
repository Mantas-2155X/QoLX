using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using QoLX.Config;
using QoLX.Interfaces;
using QoLX.Modules;
using UnityEngine;

namespace QoLX
{
	public class QoLX : IModApi
	{
		public readonly List<IModule> Modules = new List<IModule>();

		public static readonly Dictionary<string, Tuple<string, object?>> ConfigEntries = new Dictionary<string, Tuple<string, object?>>
		{
			{nameof(VehicleHonk), new Tuple<string, object?>("Switch horn and repair action locations to prevent vehicles being accidentally repaired", true)},
			{nameof(PartyLandClaim), new Tuple<string, object?>("Allow party members to interact with land claimed blocks", true)},
			{nameof(QuietTrading), new Tuple<string, object?>("Stop select traders from rambling during trading", false)},
			{QuietTrading.MutedLinesSetting, new Tuple<string, object?>("Muted lines per-trader", new List<QuietTrading.SQuietTraderInfo>
			{
				new QuietTrading.SQuietTraderInfo
				{
					Trader = "traderrekt",
					MutedLines = new [] { "trade", "sale_accepted", "sale_declined" }
				}
			})},
			{nameof(BugSquasher), new Tuple<string, object?>("Fix various bugs left in the game code", true)},
			{nameof(EfficientCooking), new Tuple<string, object?>("Turn off cookers, forges, etc. once there is nothing else left to cook", true)},
			{nameof(FrameCap), new Tuple<string, object?>("Apply a framerate limiter on game boot", false)},
			{FrameCap.CapTargetSetting, new Tuple<string, object?>("Target Framerate (0 - Unlimited)", 50)}
		};
		
		public string ConfigPath = "config.json";

		public void InitMod(Mod modInstance)
		{
			UnloadModules();
			CreateModules();
			
			ReadConfig();

			LoadModules();
		}
		
		public void ReadConfig()
		{
			var dllDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var combinedConfigPath = Path.Combine(dllDirectory!, ConfigPath);

			if (!File.Exists(combinedConfigPath))
			{
				WriteConfig();
				return;
			}

			Debug.Log("[QoLX] Reading config");

			var text = File.ReadAllText(combinedConfigPath);
			var configFile = JsonConvert.DeserializeObject<ConfigFile>(text);
			
			if (configFile?.ConfigEntries == null)
			{
				WriteConfig();
				return;
			}

			foreach (var configEntry in configFile.ConfigEntries)
				ConfigEntries[configEntry.Key] = new Tuple<string, object?>(ConfigEntries[configEntry.Key].Item1, configEntry.Value);
			
			WriteConfig();
		}

		public void WriteConfig()
		{
			var configFile = new ConfigFile();
			configFile.ConfigEntries = new List<ConfigEntry>();

			foreach (var configEntry in ConfigEntries)
			{
				var fileEntry = new ConfigEntry();
				fileEntry.Key = configEntry.Key;
				fileEntry.Value = configEntry.Value.Item2;
				fileEntry.Description = configEntry.Value.Item1;
				
				configFile.ConfigEntries.Add(fileEntry);
			}

			var json = JsonConvert.SerializeObject(configFile, Formatting.Indented);
			if (string.IsNullOrEmpty(json))
			{
				Debug.LogError("[QoLX] Failed serializing config");
				return;
			}
			
			var dllDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var combinedConfigPath = Path.Combine(dllDirectory!, ConfigPath);
			
			Debug.Log("[QoLX] Writing config");

			File.WriteAllText(combinedConfigPath, json);
		}
		
		public void CreateModules()
		{
			Modules.Clear();
			
			var moduleTypes = GetType().Assembly.GetTypes();
			
			foreach (var type in moduleTypes)
			{
				if (typeof(IModule).IsAssignableFrom(type) && type != typeof(BaseModule) && type != typeof(IModule))
				{
					Debug.Log($"[QoLX] Found module {type.Name}");
					Modules.Add((IModule)Activator.CreateInstance(type));
				}
			}
		}

		public void LoadModules()
		{
			foreach (var module in Modules)
			{
				if (!IsModuleEnabled(module))
					continue;
				
				module.Load();
			}
		}
		
		public void UnloadModules()
		{
			foreach (var module in Modules)
			{
				if (!module.Loaded)
					continue;
				
				module.Unload();
			}
		}
		
		public bool IsModuleEnabled(IModule module)
		{
			var moduleName = module.GetType().Name;
			
			foreach (var configEntry in ConfigEntries)
			{
				if (configEntry.Key != moduleName) 
					continue;
				
				return Convert.ToBoolean(configEntry.Value.Item2);
			}

			return true;
		}
	}
}
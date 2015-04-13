using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wumpus.Common.Enums;

namespace Wumpus.Model.Settings
{
	public static class Levels
	{
		public static WumpusSetting GetSetting(int id)
		{
			var appSettings = Properties.Settings.Default;
			if (id >= appSettings.LevelNames.Length)
			{
				//todo hibakezelés
				id = 0;
			}
			var ret = new WumpusSetting
			{
				TrapNumberType = (TrapNumberType)appSettings.TrapNumberTypes[id],
				ArrowNumber = appSettings.ArrowNumbers[id],
				SettingName = appSettings.LevelNames[id],
				Size = appSettings.Sizes[id],
				TrapNumberMax = appSettings.TrapNumberMax[id],
				TrapNumberMin = appSettings.TrapNumberMins[id],
			};
			return ret;
		}

		public static int GetSettingCount()
		{
			return Properties.Settings.Default.LevelNames.Length;
		}
		public static void SetSettings(WumpusSetting settings, int id)
		{
			var appSettings = Properties.Settings.Default;
			if (appSettings.LevelNames.Length < id)
			{
				//error, no element with this id
				return;
			}
			appSettings.ArrowNumbers[id] = settings.ArrowNumber;
			appSettings.TrapNumberTypes[id] =(int) settings.TrapNumberType;
			appSettings.TrapNumberMax[id] = settings.TrapNumberMax;
			appSettings.TrapNumberMins[id] = settings.TrapNumberMin;
			appSettings.Sizes[id] = settings.Size;
			appSettings.LevelNames[id] = settings.SettingName;

		}
	}
}

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
			if (appSettings.LevelNames.Length < id)
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
	}
}

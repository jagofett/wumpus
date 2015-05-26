using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Xml.Serialization;
using Wumpus.Common.Enums;
using Wumpus.Model.Logic;
using Wumpus.Model.Settings;


namespace Wumpus.Model.Presistence
{
	public class WumpusFileDataAccess : IWumpusDataAccess
	{
		public void Save(string fileName, WumpusGameLogic model)
		{
			var writer = new StreamWriter(fileName);
			//settings
			writer.WriteLine(
				model.Setting.Size + " " + model.Setting.ArrowNumber + " " + model.Setting.SettingName + " " +
				(int)model.Setting.TrapNumberType + " " + model.Setting.TrapNumberMin + " " + model.Setting.TrapNumberMax);
			//player cord
			writer.WriteLine(model.PlayerCord.Item1 + " " + model.PlayerCord.Item2);
			//arrow, points, started
			writer.WriteLine(model.PlayerArrows + " " + model.PlayerPoints + " " + model.IsStarted);

			//cave
			var size = model.Setting.Size;
			for (var i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					var field = model[i, j];
					writer.WriteLine((int)field.FieldType + " " + field.Visible + " " + i + " " + j);
					var senses = String.Join(" ", field.SenseTypes.Select(x => (int)x));
					writer.WriteLine(senses);
				}
			}

			writer.Close();

		}

		public WumpusGameLogic LoadGame(string fileName)
		{
			try
			{
				var reader = new StreamReader(fileName);
				var model = new WumpusGameLogic(this);
				//setting
				var line = reader.ReadLine().Split(' ');
				var setting = new WumpusSetting
				{
					Size = Int32.Parse(line[0]),
					ArrowNumber = Int32.Parse(line[1]),
					SettingName = line[2],
					TrapNumberType = (TrapNumberType)Int32.Parse(line[3]),
					TrapNumberMin = Int32.Parse(line[4]),
					TrapNumberMax = Int32.Parse(line[5])
				};
				//player cord
				line = reader.ReadLine().Split(' ');
				var playerCord = new Tuple<int, int>(Int32.Parse(line[0]), Int32.Parse(line[1]));
				//arrow, points, started
				line = reader.ReadLine().Split(' ');
				var cArrow = Int32.Parse(line[0]);
				var cPoint = Int32.Parse(line[1]);
				var cStrated = Boolean.Parse(line[2]);

				//cave
				var cave = new List<WumpusField>();
				for (var i = 0; i < setting.Size; i++)
				{
					for (int j = 0; j < setting.Size; j++)
					{
						line = reader.ReadLine().Split(' ');
						var fieldType = (FieldType) int.Parse(line[0]);
						var visible =  bool.Parse(line[1]);
						var cord = new Tuple<int, int>(Int32.Parse(line[2]), int.Parse(line[3]));
						
						//sense
						var sense = new List<SenseType>();
						line = reader.ReadLine().Split(' ');
						if (!line.Any(string.IsNullOrWhiteSpace))
						{
							sense = line.Select(x => (SenseType) int.Parse(x)).ToList();
						}
						cave.Add(new WumpusField
						{
							Coordinates = cord,
							FieldType = fieldType,
							Visible = visible,
							SenseTypes = sense
						});
					}
				}
				model.Load(setting,playerCord,cArrow,cPoint,cave,cStrated);


				return model;
			}
			catch (Exception e)
			{
				//todo spec error
				throw;
			}
		}
	}
}

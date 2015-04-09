using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Wumpus.Common.Enums;
using Wumpus.Model.Settings;
using Pair = System.Tuple<int, int>;


namespace Wumpus.Model.Logic
{
    public class WumpusGameLogic
    {
	    public WumpusSetting Setting { get; private set; }

	    public Tuple<int,int> PlayerCord { get; private set; }
	    public int PlayerArrows { get; private set; }

	    private List<WumpusField> _cave;

	    public bool IsStarted { get; private set; }

	    public WumpusGameLogic() : this(Levels.GetSetting(0))
	    {}
		public WumpusGameLogic(WumpusSetting setting)
		{
			Setting = setting;
			IsStarted = false;



		}

	    public void StartGame()
	    {
			PlayerArrows = Setting.ArrowNumber;

			//player start at left bottom
			PlayerCord = new Tuple<int, int>(0, 0);

			//init the cave, with empty fields
			InitEmptyCave();
			_cave.First(x => x.Coordinates.Equals(PlayerCord)).FieldType = FieldType.Player;

			//crete the traps
			CreateTraps();

			//create the wumpus
			CreateWumpus();

			//create the gold
			CreateGold();


			//set senses
			CreateSenses();

		    IsStarted = true;
	    }

	    private void CreateSenses()
	    {
		    if (IsStarted) return;

		    foreach (var field in _cave)
		    {
			    var x = field.Coordinates.Item1;
			    var y = field.Coordinates.Item2;
			    var needCheckList = new List<Pair>
			    {
				    new Pair(x + 1, y),
				    new Pair(x - 1, y),
				    new Pair(x, y + 1),
				    new Pair(x, y - 1)
			    };
			    foreach (var check in needCheckList)
			    {
				    if (_cave.Any(w => w.Coordinates.Equals(check) && w.FieldType == FieldType.Trap) && !field.SenseTypes.Contains(SenseType.Breeze))
				    {
					    field.SenseTypes.Add(SenseType.Breeze);
				    }
					if (_cave.Any(w => w.Coordinates.Equals(check) && w.FieldType == FieldType.Wumpus) && !field.SenseTypes.Contains(SenseType.Smell))
					{
						field.SenseTypes.Add(SenseType.Smell);
					}
				}
			    if (field.FieldType == FieldType.Gold && !field.SenseTypes.Contains(SenseType.Glitter))
			    {
				    field.SenseTypes.Add(SenseType.Glitter);
			    }
			}
	    }

		/// <summary>
		/// Creates the wumpus. at random place - not adjacent to start position (0,0)
		/// </summary>
		private void CreateWumpus()
	    {
		    if (IsStarted) return;
			//todo check if no wumpus created jet
		    _cave.Where(f => FieldTrapSafe(f.Coordinates) && f.FieldType == FieldType.Empty)
			    .OrderBy(c => Guid.NewGuid())
			    .First()
			    .FieldType = FieldType.Wumpus;
	    }
		/// <summary>
		/// Creates the gold at random empty place
		/// </summary>
		private void CreateGold()
		{
			if (IsStarted) return;
			//todo check if no gold created jet
			_cave.Where(f => f.FieldType == FieldType.Empty)
				.OrderBy(c => Guid.NewGuid())
				.First()
				.FieldType = FieldType.Gold;
		}
		private void CreateTraps()
	    {
		    if(IsStarted) return;
		    var neededTrap = Setting.TrapNumberMax;
			if (Setting.TrapNumberType == TrapNumberType.MinMaxNumber)
		    {
			    var rnd = new Random();
			    neededTrap = rnd.Next(Setting.TrapNumberMin, Setting.TrapNumberMax + 1);
		    }
		    _cave.Where(f => FieldTrapSafe(f.Coordinates))
			    .OrderBy(fi => Guid.NewGuid())
			    .Take(neededTrap)
			    .ToList()
			    .ForEach(x => x.FieldType = FieldType.Trap);
	    }

	    private bool FieldTrapSafe(Tuple<int, int> field)
	    {
			if (field.Item1 == 0 && field.Item2 == 0)
            {
				return false;
			}
			if (field.Item1 == 0 && field.Item2 == 1)
			{
				return false;
			}
			if (field.Item1 == 1 && field.Item2 == 0)
			{
				return false;
			}

			return true;
	    }

		private void InitEmptyCave()
		{
			if (IsStarted) return;

			_cave?.Clear();
			_cave = new List<WumpusField>();
		    for (var i = 0; i < Setting.Size; i++)
		    {
			    for (var j = 0; j < Setting.Size; j++)
			    {
					_cave.Add(new WumpusField
				    {
					    Coordinates = new Tuple<int, int>(i, j),
					    FieldType = FieldType.Empty,
					    SenseTypes = new List<SenseType>(),
					    Visible = false
				    });
			    }
		    }

	    }

    }
}

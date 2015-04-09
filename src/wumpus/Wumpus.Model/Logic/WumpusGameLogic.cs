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
	    public int PlayerPoints { get; private set; }


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
		    PlayerPoints = 1000;

			//player start at left bottom
			PlayerCord = new Tuple<int, int>(0, 0);
			//init the cave, with empty fields
			InitEmptyCave();
			//set start pos to player
		    var startPos = _cave.First(x => x.Coordinates.Equals(PlayerCord));
			//startPos.FieldType = FieldType.Player;
		    startPos.Visible = true;

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
		/// <summary>
		/// Gets the <see cref="WumpusField"/> with the specified x and y.
		/// </summary>
		/// <value>
		/// The <see cref="WumpusField"/>.
		/// </value>
		/// <param name="x">The i.</param>
		/// <param name="y">The j.</param>
		/// <returns></returns>
		public WumpusField this[int x, int y]
	    {
		    get { return _cave != null ? _cave.FirstOrDefault(f => f.Coordinates.Equals(new Pair(x, y))) : null; }
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
            _cave.Where(f => FieldTrapSafe(f.Coordinates) && f.FieldType == FieldType.Empty)
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

		    if (_cave != null)
		    {
                _cave.Clear();		        
		    }
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

	    public bool Step(Direction direction)
	    {
		    if (!IsStarted) return false;
		    var goalPos = PlayerCord;
		    switch (direction)
		    {
				case Direction.Up:
				    goalPos = new Pair(goalPos.Item1, goalPos.Item2 + 1);
					break;

				case Direction.Down:
					goalPos = new Pair(goalPos.Item1, goalPos.Item2 - 1);

					break;
				case Direction.Left:
				    goalPos = new Pair(goalPos.Item1 - 1, goalPos.Item2);

					break;
				case Direction.Right:
				    goalPos = new Pair(goalPos.Item1 + 1, goalPos.Item2);
				    break;
		    }
		    var dest = _cave.FirstOrDefault(f => f.Coordinates.Equals(goalPos));
		    if (dest == null)
		    {
			    return false;
		    }
	        PlayerCord = dest.Coordinates;
		    switch (dest.FieldType)
		    {
				case FieldType.Gold:
				case FieldType.Empty:
					//user can step here
		            PlayerPoints--;
                    //todo trigger success move event
					break;
				case FieldType.Wumpus:
				case FieldType.Trap:
					//game over, trigger type specific game over event 
                    //todo
		            PlayerPoints -= 1000;
					break;

			}

			return true;
		}

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Wumpus.Common.Enums;
using Wumpus.Model.Presistence;
using Wumpus.Model.Settings;
using Pair = System.Tuple<int, int>;


namespace Wumpus.Model.Logic
{
	[Serializable]
    public class WumpusGameLogic
    {
        #region private fields

		[XmlIgnore]
	    private IWumpusDataAccess _dataAccess;

		[XmlIgnore]

		private List<WumpusField> _cave;
        
        #endregion

        #region Public fields

	    public WumpusSetting Setting { get; private set; }
	    public Tuple<int,int> PlayerCord { get; private set; }
	    public int PlayerArrows { get; private set; }
	    public int PlayerPoints { get; private set; }
        public bool IsStarted { get; private set; }
	    public bool ShowFieldEnd { get; set; }
	    public bool  ShowFieldDebug { get; set; }
		#endregion

		#region Events



		[XmlIgnore]
		public EventHandler OutOfFieldEvent;

        private void OnOutOfFieldEvent()
        {
            if (OutOfFieldEvent != null)
            {
                OutOfFieldEvent(this, null);
            }
        }
		[XmlIgnore]

		public EventHandler SucceccStepEvent;

        private void OnSuccessStepEvent()
        {
            if (SucceccStepEvent != null)
            {
                SucceccStepEvent(this, null);
            }
        }
		[XmlIgnore]

		public EventHandler<WumpusGameOverEventArgs> GameOverEvent;

        private void OnGameOverEvent(WumpusGameOverEventArgs args)
        {
            IsStarted = false;
            if (ShowFieldEnd)
            {
                _cave.ForEach(x => x.Visible = true);
            }
            if (GameOverEvent != null)
            {
                GameOverEvent(this, args);
            }
            
        }

        #endregion


        #region Constructors

	    public WumpusGameLogic() : this(Levels.GetSetting(0), null)
	    {}
		public WumpusGameLogic(WumpusSetting setting, IWumpusDataAccess dataAccess)
		{
			_dataAccess = dataAccess;
			Setting = setting;
			IsStarted = false;
            ShowFieldEnd = true;
            ShowFieldDebug = false;
		}

        #endregion

        #region Public methods

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
        /// Gets the <see cref="WumpusField"/> with the specified x and y. (if visible)
        /// </summary>
        public WumpusField this[int x, int y]
        {
            get { return this[new Pair(x, y), ShowFieldDebug]; }
        }

        /// <summary>
        /// Get the field info for the given position
        /// </summary>
        /// <param name="position">Needed position (int pair)</param>
        /// <param name="priv">if true, the field will not be need to be visible, otherwise it will</param>
        /// <returns>WumpusField info</returns>
        private WumpusField this[Pair position, bool priv = true]
        {
            get { return _cave != null ? _cave.FirstOrDefault(f => f.Coordinates.Equals(position) && (priv || f.Visible)) : null; }
        }

        public bool Step(Direction direction)
        {
            if (!IsStarted) return false;
            PlayerPoints--;

            var goalPos = DirectionToPair(direction);

            var destField = this[goalPos];
            if (destField == null)
            {
                //out of field
                OnOutOfFieldEvent();
                return false;
            }
            PlayerCord = destField.Coordinates;
            switch (destField.FieldType)
            {
                case FieldType.Gold:
                case FieldType.Empty:
                    //user can step here
                    destField.Visible = true;
                    OnSuccessStepEvent();
                    //todo trigger success move event
                    break;
                case FieldType.Wumpus:
                case FieldType.Trap:
                    //game over, trigger type specific game over event 
                    destField.Visible = true;
                    PlayerPoints -= 1000;
                    OnGameOverEvent(new WumpusGameOverEventArgs
                    {
                        IsGameWin = false,
                        GameOverType = destField.FieldType,
                        Points = PlayerPoints
                    });
                    break;

            }

            return true;
        }

        public bool GrabGold()
        {
            if (!IsStarted) return false;
            PlayerPoints--;
            //get player actual filed
            var destField = this[PlayerCord];
            if (destField == null || destField.FieldType != FieldType.Gold) return false;
            //gold is grabbed, player winned the game
            PlayerPoints += 1000;
            OnGameOverEvent(new WumpusGameOverEventArgs
            {
                GameOverType = FieldType.Gold,
                Points = PlayerPoints,
                IsGameWin = true
            });
            return true;
        }

        public bool ShootArrow(Direction direction)
        {
            if (!IsStarted || PlayerArrows <= 0) return false;
            PlayerPoints--;
            PlayerArrows--;
            var goalPos = DirectionToPair(direction);
            var destField = this[goalPos];
	        var wumpusFound = destField != null && destField.FieldType == FieldType.Wumpus;
            while (destField != null && !wumpusFound)
            {
	            destField = this[DirectionToPair(direction, destField.Coordinates)];
                wumpusFound = destField != null && destField.FieldType == FieldType.Wumpus;
            }
            if (!wumpusFound)
            {
                //no wumpus in this direction
                OnOutOfFieldEvent();
                return false;
            }
            //wumpus killed
            PlayerPoints += 1000;
            OnGameOverEvent(new WumpusGameOverEventArgs
            {
                Points =  PlayerPoints,
                IsGameWin = true,
                GameOverType = FieldType.Wumpus
            });
            return true;

        }

        #endregion

        #region Private methods

        /// <summary>
        /// Compute the given direction to position. Starting pos is optional, if not provided, actual player position will be used
        /// </summary>
        /// <param name="direction">The direction</param>
        /// <param name="startPos">Optional starting pos. Default is player actual position</param>
        /// <returns>Computed position - depending on the given direction</returns>
        private Pair DirectionToPair(Direction direction, Pair startPos = null)
        {
            //default start pos is the current player pos.
            var goalPos = startPos ?? PlayerCord;
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
            return goalPos;
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
            if (IsStarted) return;
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

        #endregion

	    public void Save(string fileName)
	    {
		    if (IsStarted && _dataAccess != null)
		    {
			    _dataAccess.Save(fileName, this);
		    }
	    }

    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Wumpus.Common.Enums;
using Wumpus.Model.Logic;
using Wumpus.Model.Presistence;
using Wumpus.Model.Settings;
using Pair = System.Tuple<int, int>;

namespace Wumpus.View.ViewModel
{
	public class WumpusViewModel : ViewModelBase
	{
	    private IWumpusDataAccess _dataAccess;
	    private WumpusGameLogic _gameLogic;
	    private int _diffIndex;
	    public int ActSize { get; private set; }
	    private bool _first = true;
	    private bool _isShooting = false;

	    private Pair _actPos
	    {
	        get { return _gameLogic != null ? _gameLogic.PlayerCord : null; }
	    }

	    private WumpusSetting _setting;

        public DelegateCommand StartGameCommand { get; set; }
        public DelegateCommand SaveGameCommand { get; set; }
        public DelegateCommand LoadGameCommand { get; set; }
        public DelegateCommand ExitGameCommand { get; set; }
        public DelegateCommand StepCommand { get; set; }
        public DelegateCommand ShootCommand { get; set; }
        public DelegateCommand GrabCommand { get; set; }
        public DelegateCommand ShowEndCommand { get; set; }
        public DelegateCommand ShowFieldCommand { get; set; }


        //events
	    public EventHandler QuitEvent { get; set; }

	    private void OnQuitEvent()
	    {
	        if (QuitEvent != null)
	        {
	            QuitEvent(this, new EventArgs());
	        }
	    }
            

        public int SelSize { get; set; }
        public int SelArrow { get; set; }
        public int SelMinTrap { get; set; }
        public int SelMaxTrap { get; set; }

	    public string ArrowShootText
	    {
	        get { return _isShooting ? "Mégse" : "Íj kilövése"; }
            set { OnPropertyChanged("ArrowShootText"); }
	    }

        private string _info;

        public string Info
        {
            get { return _info; }
            set { _info = value; OnPropertyChanged("Info"); }
        }


        private int _actArrow;

        public int ActArrow
        {
            get { return _actArrow; }
            set
            {
                _actArrow = value;
                OnPropertyChanged("ActArrow");
            }
        }
        private string _actPosText;

        public string ActPosText
        {
            get { return _actPosText; }
            set
            {
                _actPosText = value; 
                OnPropertyChanged("ActPosText");
            }
        }

        private string _actSenseText;

        public string ActSenseText
        {
            get { return _actSenseText; }
            set
            {
                _actSenseText = value;
                OnPropertyChanged("ActSenseText");
            }
        }
	    public bool IsStarted { get { return _gameLogic != null && _gameLogic.IsStarted; } }

        private bool _isShowEndChecked;

        public bool IsShowEndChecked
        {
            get { return _isShowEndChecked; }
            set
            {
                _isShowEndChecked = value;
                OnPropertyChanged("IsShowEndChecked");
            }
        }

        private bool _isShowFieldChecked;

        public bool IsShowFieldChecked
        {
            get { return _isShowFieldChecked; }
            set
            {
                _isShowFieldChecked = value;
                OnPropertyChanged("IsShowFieldChecked");
            }
        }
	    public int DiffIndex
	    {
	        get { return _diffIndex; }
	        set
	        {
                
	            //save current selection
                if (!_first) { 
                Levels.SetSettings(GetCurrSetting(),_diffIndex);
                }
                //load selected settings
                _diffIndex = value;
	            _first = false;
	            
                _setting = Levels.GetSetting(_diffIndex);
	            SelSize = _setting.Size;
	            SelArrow = _setting.ArrowNumber;
	            SelMinTrap = _setting.TrapNumberMin;
	            SelMaxTrap = _setting.TrapNumberMax;

                OnPropertyChanged("DiffIndex");
                OnPropertyChanged("SelSize");
                OnPropertyChanged("SelArrow");
                OnPropertyChanged("SelMinTrap");
                OnPropertyChanged("SelMaxTrap");
	        }
	    }

	    public ObservableCollection<WumpusElement> Elements { get; set; }


	    public WumpusViewModel(WumpusGameLogic gameLogic, IWumpusDataAccess dataAccess)
	    {
	        _dataAccess = dataAccess;
	        _gameLogic = gameLogic;
            SetGameEventHandlers();

	        DiffIndex = 0;
	        Info = "Játék indításához válassz nehézséget, és kattints a Játék kezdése gombra!";
            OnPropertyChanged("Info");

	        IsShowEndChecked = true;
	        IsShowFieldChecked = false;

            StartGameCommand = new DelegateCommand(o => StartGame());
            //SaveGameCommand = new DelegateCommand(SavingGame);
            //LoadGameCommand = new DelegateCommand(LoadingGame);
            ExitGameCommand = new DelegateCommand(o => OnQuitEvent());
            StepCommand = new DelegateCommand(o => Step((Direction)Convert.ToInt32(o)));
            ShootCommand = new DelegateCommand(o => ShootArrow());
            GrabCommand = new DelegateCommand(o => GrabGold());
            ShowEndCommand = new DelegateCommand(o => { IsShowEndChecked = !IsShowEndChecked; SetExtraSettings(); UpdateField();});
            ShowFieldCommand = new DelegateCommand(o => { IsShowFieldChecked = !IsShowFieldChecked; SetExtraSettings(); UpdateField();});
	    }

	    private void ShootArrow()
	    {
	        if (!IsStarted) return;
            ActArrow = _gameLogic.PlayerArrows;
	        if (ActArrow == 0)
	        {
	            MessageBox.Show("Nincs több nyilad, így nem lőhetsz!", "Hiba!");
                return;
	        }
            _isShooting = !_isShooting;
	        ArrowShootText = "change!";
	        Info = _isShooting ? "Add meg milyen irányba lövöd a nyilat!" : "Mit cselekszel?";

            
	    }

	    private void GrabGold()
	    {
	        if(!IsStarted) return;
	        if (!_gameLogic.GrabGold())
	        {
                MessageBox.Show("Nincs itt az arany. Nem csillog semmi!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
	        }
	    }

	    private void SetExtraSettings()
	    {
            //set the extra settings.
            _gameLogic.ShowFieldDebug = IsShowFieldChecked;
            _gameLogic.ShowFieldEnd = IsShowEndChecked;
	    }
	    private void StartGame()
        {
            _setting = GetCurrSetting();
            //save current setting
            Levels.SetSettings(_setting, _diffIndex);

            _gameLogic = new WumpusGameLogic(_setting, _dataAccess);
            SetGameEventHandlers();
            try
            {
                if (_setting.Size > 50)
                {
                    throw new Exception();
                }
                SetExtraSettings();

                //start the game
                _gameLogic.StartGame();
                
                ActSize = _setting.Size;
                ActArrow = _gameLogic.PlayerArrows;
                OnPropertyChanged("ActSize");

                UpdateField();
                Info = "Mit cselekszel? Mozogni a nyíl gombokkal illetve a nyíl billentyűkkel is tudsz!";
                ActPosText = "Beléptél a barlangba, a bal alsó sarkában vagy.";
            }
            catch (Exception)
            {
                MessageBox.Show("Hibás játékparaméterek! Kérlek Figyelj, hogy értelmes adatokat adj meg!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                _gameLogic = new WumpusGameLogic();
            }

        }

        private void Step(Direction dir)
        {
            if (!IsStarted)
            {
                return;
            }
            if (_isShooting)
            {
                if (!_gameLogic.ShootArrow(dir))
                {
                    //some error happend
                }
                ActArrow = _gameLogic.PlayerArrows;
                _isShooting = false;
                ArrowShootText = "Change!";
            }
            else if (!_gameLogic.Step(dir))
            {
                //some error, try to catch.. (game is not running or things like this)
            }
        }
	    private void SetGameEventHandlers()
	    {
	        if (_gameLogic == null)
	        {
	            return;
	        }
            _gameLogic.GameOverEvent += WumpusGameOverEvent;
            _gameLogic.OutOfFieldEvent += OutOfFieldEvent;
            _gameLogic.SucceccStepEvent += SucceccStepEvent;
	    }
	    private WumpusSetting GetCurrSetting()
	    {
	        return new WumpusSetting
	        {
                Size = SelSize,
                ArrowNumber = SelArrow,
                TrapNumberType = TrapNumberType.MinMaxNumber,
                TrapNumberMin = SelMinTrap,
                TrapNumberMax = SelMaxTrap
	        };
	    }

	    

        //private void LoadingGame(object o)
        //{
        //    throw new NotImplementedException();
        //}

        //private void SavingGame(object o)
        //{
        //    throw new NotImplementedException();
        //}


	    private void UpdateField()
	    {
	        if (Elements != null)
	        {
	            Elements.Clear();
	        }
            Elements = new ObservableCollection<WumpusElement>();
	        for (int i = ActSize - 1; i >= 0 ; i--)
	        {
	            for (int j = 0 ; j < ActSize ; j++)
	            {
                    //get the i,j field. if not visible, add a disabled elem
	                var field = _gameLogic[j, i];

	                Elements.Add(new WumpusElement
	                {
	                    IsEnabled = field != null,
	                    X = j,
	                    Y = i,
	                    Id = i*ActSize + j,
	                    Text = GetFieldText(field),
                        //StepCommand = new DelegateCommand(o => Step(Convert.ToInt32(o)))
	                });

	            }
	        }
            OnPropertyChanged("Elements");
	    }

	    private string GetFieldText(WumpusField field)
	    {
	        if (field == null)
	        {
	            return String.Empty;
	        }
            //var text = field.Coordinates.Item1 + ":" + field.Coordinates.Item2 + "\n";
	        var text = "";
	        switch (field.FieldType)
	        {
                case FieldType.Gold:
	                text += "Arany\n";
                    break;
                case FieldType.Trap:
	                text += "Csapda\n";
	                break;
                case FieldType.Wumpus:
	                text += "Wumpus\n";
	                break;
	        }
	        if (field.SenseTypes.Count > 0)
	        {
	            var s = new List<String>();
	            foreach (var sens in field.SenseTypes)
	            {
	                switch (sens)
	                {
	                    case SenseType.Breeze:
	                        //szellő
	                        s.Add("Szél");
	                        break;
	                    case SenseType.Glitter:
	                        //ragyogás
	                        s.Add("Ragyog");
	                        break;
	                    case SenseType.Smell:
	                        //bűz
	                        s.Add("Bűz");
	                        break;
	                }
	            }
	            text += String.Join(", ", s) + "\n";
	        }
	        if (_actPos.Equals(field.Coordinates))
	        {
	            text += "Játékos";
	        }
	        return text;
	    }

	    private void SucceccStepEvent(object sender, EventArgs eventArgs)
	    {
            UpdateField();
            ActArrow = _gameLogic.PlayerArrows;

	        var field = _gameLogic[_actPos.Item1, _actPos.Item2];
	        ActPosText = "A(z) " + _actPos.Item1 + (_actPos.Item2 + 1) + "-es terembe érsz";
	        Info = "Sikeres lépés!";
	        ActSenseText = GetSensesText(field);
	    }

	    private string GetSensesText(WumpusField field)
	    {
            var text = String.Empty;
            if (field.SenseTypes.Count > 0)
            {
                var s = new List<String>();
                foreach (var sens in field.SenseTypes)
                {
                    switch (sens)
                    {
                        case SenseType.Breeze:
                            //szellő
                            s.Add("Szellő csap meg. Valamely szomszédos mezőn csapda van!");
                            break;
                        case SenseType.Glitter:
                            //ragyogás
                            s.Add("Vakító ragyogás csap meg, ez csak az arany lehet!");
                            break;
                        case SenseType.Smell:
                            //bűz
                            s.Add("Orrfacsaró bűzt érzel. A wumpusnak közel kell lennie!");
                            break;
                    }
                }
                text += String.Join("\n", s);
            }
	        return text;
	    }

	    private void OutOfFieldEvent(object sender, EventArgs eventArgs)
	    {
	        if (_isShooting)
	        {
	            _isShooting = false;
	            ArrowShootText = "Change!";
	            Info = "A nyíl koppant a barlang falán, nem találtad el a wumpust!";
	        }
	        else
	        {
                Info = "Koppantál a falon, erre nem tudsz menni!";	            
	        }
	    }

	    private void WumpusGameOverEvent(object sender, WumpusGameOverEventArgs args)
	    {
            UpdateField();
            ActSenseText = GetSensesText(_gameLogic[_actPos.Item1, _actPos.Item2]);
	        var res = String.Empty;
            //game over...
            if (args.IsGameWin)
            {
                //winner case
                switch (args.GameOverType)
                {
                    case FieldType.Gold:
                        res = "Aranycsörgést hallasz! Megkaparintottad a wumpus féltve őrzött aranyát!";
                        MessageBox.Show(res + "\n" + "Pontszámod: " + args.Points, "Győztél!", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    case FieldType.Wumpus:
                        res = "Velőtrázó sikoly szeli át a barlangot! A wumpus elpusztult, megmenekültél!";
                        MessageBox.Show(res + "\n" + "Pontszámod: " + args.Points, "Győztél!", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    default:
                        MessageBox.Show("Vége a játéknak, valahogy győztél. Ez lehetetlen ág, te kis csaló!");
                        break;
                }
                Info = "Gratulálok!";
            }
            else
            {
                //lose case
                switch (args.GameOverType)
                {
                    case FieldType.Wumpus:
                        res = "Ajjaj, fogcsattogtatást hallasz, aztán elsötétül a kép!  Te lettél a wumpus vacsorája!";
                        MessageBox.Show(res, "Vesztettél!", MessageBoxButton.OK, MessageBoxImage.Stop);
                        break;
                    case FieldType.Trap:
                        res = "PUFF! Eltűnik a lábad alól a talaj, és már zuhansz is a mélybe. A csapda örökre elnyelt!";
                        MessageBox.Show(res, "Vesztettél!", MessageBoxButton.OK, MessageBoxImage.Stop);
                        break;
                    default:
                        break;
                }
                Info = "A játéknak vége! Pontszámod: " + args.Points;
            }
	        ActPosText = res;

	    }
	}
}

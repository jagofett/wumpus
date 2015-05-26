using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Wumpus.Common.Enums;
using Wumpus.Model.Logic;
using Wumpus.Model.Presistence;
using Wumpus.Model.Settings;
using Wumpus.Toplist;
using Pair = System.Tuple<int, int>;

namespace Wumpus.Console
{
	class Program
	{
		private static WumpusFileDataAccess _dataAccess = new WumpusFileDataAccess();
	    private static WumpusGameLogic _game;
	    private static WumpusSetting _lastSetting;
	    private static bool _lastArrow;

	    private static void ReadSafeInput(out int number)
	    {
            while (!Int32.TryParse(System.Console.ReadLine(), out number))
            { }
	    }

	    private static void Main(string[] args)
	    {
	        System.Console.WriteLine("Üdv a Wumpus világban! \n \n" +
	                                 "A kaland kezdetét veszi, amint belépsz a vérszomjas wumpus barlangjába.\n" +
	                                 "Célod, hogy megtaláld az egyik szobában elrejtett aranyat, vagy leszámolj a wumpusszal!\n" +
	                                 "De vigyázz, a barlangban különféle halálos csapdák is vannak, kerüld el őket!");
			
            NewGame();
	    }

		private static void NewGame()
		{
		    _lastArrow = false;
			System.Console.WriteLine("\n\n Válassz nehézségi szintet: \n" +
			                         "1 - Könnyű \n" +
			                         "2 - Közepes \n" +
			                         "3 - Nehéz \n" +
			                         "100 - Egyéni \n" +
                                     (_lastSetting != null ? "101 - Előző beállítások \n" : "") +
			                         "0 - kilépés\n");

			int levelNumber;
			ReadSafeInput(out levelNumber);
			if (levelNumber <= 0 || levelNumber > 3 && levelNumber != 100 && levelNumber != 101)
			{
				return;
			}
		    if (levelNumber == 101 && _lastSetting == null)
		    {
		        return;
		    }
			//zero indexing
			WumpusSetting level;
		    if (levelNumber == 101)
		    {
		        level = _lastSetting;
		    }
			else if (levelNumber == 100)
			{
				level = GetCustomSettings();
			}
			else
			{
				levelNumber--;
				level = Levels.GetSetting(levelNumber);
			}
		    try
		    {
		        if (level.Size >= 50)
		        {
		            throw new Exception("Hibás méret!");
		        }
                SetGame(new WumpusGameLogic(level, _dataAccess));
		    }
		    catch (Exception e)
		    {
                Debug.WriteLine(e.Message);
		        System.Console.WriteLine("Hibás bemenő adatok!");
                NewGame();
		    }
		}
		private static void SetGame(WumpusGameLogic game)
		{

			_game = game;

            _game.GameOverEvent += WumpusGameOverEvent;
            _game.OutOfFieldEvent += WumpusOutOfFieldEvent;
            _game.SucceccStepEvent += WumpusSucceccStepEvent;
			if (!_game.IsStarted)
			{
				_game.StartGame();
				System.Console.WriteLine("\n\nBeléptél a barlangba, a bal alsó sarkában vagy.\n" +
										 "A barlang mérete: " + _game.Setting.Size + " x " + _game.Setting.Size);
                _lastSetting = _game.Setting;
			}
			//System.Console.WriteLine("Csapdák száma: " + game.Setting.TrapNumberMin + ".." + game.Setting.TrapNumberMax);
			WriteSenses();
		    Game();
		    //System.Console.ReadKey();
		}

		private static WumpusSetting GetCustomSettings()
		{
            int size, arrows, minTraps, maxTraps;
            System.Console.WriteLine("Barlang mérete:");
            ReadSafeInput(out size);
            System.Console.WriteLine("Nyílvesszők száma:");
            ReadSafeInput(out arrows);
            System.Console.WriteLine("Csapdák minimális száma:");
            ReadSafeInput(out minTraps);
            System.Console.WriteLine("Csapdák maximális száma:");
            ReadSafeInput(out maxTraps);


			return new WumpusSetting
			{
			    Size = size,
                ArrowNumber = arrows,
                TrapNumberType = TrapNumberType.MinMaxNumber,
                TrapNumberMin = minTraps,
                TrapNumberMax = maxTraps
			};
		}

		private static void WriteSenses()
	    {
            System.Console.WriteLine("\nNyílvesszőid száma: " + _game.PlayerArrows);


	        var actPos = _game.PlayerCord;

			System.Console.WriteLine("A(z) " + actPos.Item1 + (actPos.Item2+1) +"-es szobába érsz");

			var senses = _game[actPos.Item1, actPos.Item2].SenseTypes;
	        if (senses.Count > 0)
	        {
	            System.Console.WriteLine("Az alábbiakat érzékeled:");
	        }
	        else
	        {
                System.Console.WriteLine("Nem érzékelsz semmit!");	            
	        }
	        foreach (var sens in senses)
	        {
	            switch (sens)
	            {
	                case SenseType.Breeze:
	                    //szellő
                        System.Console.WriteLine("Szellő csap meg. Valamely szomszédos mezőn csapda van!");
	                    break;
	                case SenseType.Glitter:
	                    //ragyogás
                        System.Console.WriteLine("Vakító ragyogás csap meg, ez csak az arany lehet!");
	                    break;
	                case SenseType.Smell:
	                    //bűz
                        System.Console.WriteLine("Orrfacsaró bűzt érzel. A wumpusnak közel kell lennie!");
	                    break;
	            }
	        }
			System.Console.WriteLine("");
	    }

	    private static void Game()
	    {
            System.Console.WriteLine("Mit cselekszel?\n");
            System.Console.WriteLine("1_ - Lépés szomszédos mezőre\n" +
                                     "\t11 - Fel\n" +
                                     "\t12 - Le\n" +
                                     "\t13 - Jobbra\n" +
                                     "\t14 - Balra\n" +
                                     "2_ - Nyíl kilövése\n" +
                                     "\t21 - Fel\n" +
                                     "\t22 - Le\n" +
                                     "\t23 - Jobbra\n" +
                                     "\t24 - Balra\n" +
                                     "3 - Arany felvétele\n" +
                                     //"4 - Mentés\n" +
                                     //"5 - Betöltés\n" +
									 "0 - Kilépés");
	        int dirInt;

	        ReadSafeInput(out dirInt);
	        switch (dirInt)
	        {
                case 11:
                case 12:
                case 13:
                case 14:
	                var dirStep = GetDirection(dirInt - 10);
                    if (dirStep == null) { return; }
                    if (!_game.Step((Direction) dirStep))
                    {
                        //some error, try to catch.. (game is not running or things like this)
                    }
	                break;
                case 21:
                case 22:
                case 23:
                case 24:
	                var dirArrow = GetDirection(dirInt - 20);
                    if (dirArrow == null) { return; }
	                _lastArrow = true;
	                if (!_game.ShootArrow((Direction) dirArrow))
	                {
	                    //some error, try to catch.. (game is not running or things like this)
	                }

	                break;
                case 3:
	                if (!_game.GrabGold())
	                {
	                    //no gold in actual field
                        System.Console.WriteLine("Nincs itt az arany. Nem csillog semmi!");
	                    Game();
	                }
                    break;
				case 4:
					//save
					if (!_game.IsStarted) {return;}
					System.Console.WriteLine("\nAdd meg a fájl nevét: ");
					var fileName = System.Console.ReadLine();
			        if (!String.IsNullOrWhiteSpace(fileName))
			        {
				        try
				        {
					        fileName = "./" + fileName;
							_game.Save(fileName);
							System.Console.WriteLine("Sikeres mentés! \n");
						}
						catch (Exception e)
				        {
					        System.Console.WriteLine("Sikertelen mentés: " + e.Message);
				        }
			        }
					Game();
					break;

				case 5:
					//load
					System.Console.WriteLine("\nAdd meg a fájl nevét: ");
					var loadFileName = System.Console.ReadLine();
			        if (!String.IsNullOrWhiteSpace(loadFileName) && File.Exists(loadFileName))
			        {
				        try
				        {
					        var loadGame = _dataAccess.LoadGame(loadFileName);
							System.Console.WriteLine("Sikeres betöltés!\n");
                            SetGame(loadGame);
						}
						catch (Exception e)
				        {
							System.Console.WriteLine("Sikertelen betöltés: " + e.Message);
							Game();
						}
			        }
			        else
			        {
				        Game();
			        }
			        break;

                //case 5:
                //    //load
                //    System.Console.WriteLine("\nAdd meg a fájl nevét: ");
                //    var loadFileName = System.Console.ReadLine();
                //    if (!String.IsNullOrWhiteSpace(loadFileName) && File.Exists(loadFileName))
                //    {
                //        try
                //        {
                //            SetGame(_dataAccess.LoadGame(loadFileName));
                //        }
                //        catch (Exception e)
                //        {
                //            System.Console.WriteLine("Sikertelen mentés: " + e.Message);
                //            Game();
                //        }
                //    }
                //    else
                //    {
                //        Game();
                //    }
                //    break;



				case 0:
					return;
					break;
                default:
					Game();
                    break;
	        }
		    System.Console.WriteLine("");
	    }

		private static Direction? GetDirection(int? dir = null)
	    {
            int dirInt;
	        if (dir == null)
	        {


	            System.Console.WriteLine("Milyen irányba végzed a tevékenységet?\n");
	            System.Console.WriteLine("1 - Fel\n" +
	                                     "2 - Le\n" +
	                                     "3 - Jobbra\n" +
	                                     "4 - Balra\n" +
	                                     "0 - Kilépés");
	            ReadSafeInput(out dirInt);
	        }
	        else
	        {
	            dirInt = (int) dir;
	        }
	        switch (dirInt)
	        {
                case 1:
                    return Direction.Up;
                    break;
                case 2:
                    return Direction.Down;
                    break;
                case 3:
                    return Direction.Right;
                    break;
                case 4:
                    return Direction.Left;
                    break;
                default:
	                return null;
                    break;
	        }
	    }


	    private static void WumpusSucceccStepEvent(object sender, EventArgs eventArgs)
	    {
            //akt szoba számának kíírása(?)

	        WriteSenses();
            Game();
	    }

	    private static void WumpusOutOfFieldEvent(object sender, EventArgs eventArgs)
	    {
	        System.Console.WriteLine(_lastArrow ? "A nyíl koppant a falon!" : "Koppantál a falon! Erre nincs út!");
	        _lastArrow = false;
	        WriteSenses();
            Game();
	    }

	    private static void WumpusGameOverEvent(object sender, WumpusGameOverEventArgs args)
	    {
	        //game over...
	        if (args.IsGameWin)
	        {
                //winner case
	            switch (args.GameOverType)
	            {
                    case FieldType.Gold:
                        System.Console.WriteLine("Aranycsörgést hallasz! Megkaparintottad a wumpus féltve őrzött aranyát! \n" +
                                                 "Pontszámod: " + args.Points);
                        break;
                    case FieldType.Wumpus:
                        System.Console.WriteLine("Velőtrázó sikoly szeli át a barlangot! A wumpus elpusztult, megmenekültél!\n" +
                                                 "Pontszámod: " + args.Points);
                        break;
                        break;
                    default:
                        System.Console.WriteLine("Vége a játéknak, győztél. De csaltál is, ez lehetetlen ág!");
                        break;
	            }
                System.Console.WriteLine("Gratulálok!");
	        }
	        else
	        {
	            //lose case
	            switch (args.GameOverType)
	            {
                    case FieldType.Wumpus:
                        System.Console.WriteLine("Ajjaj, fogcsattogtatást hallasz, aztán elsötétül a kép!  Te lettél a wumpus vacsorája!");
                        break;
                    case FieldType.Trap:
                        System.Console.WriteLine("PUFF! Eltűnik a lábad alól a talaj, és már zuhansz is a mélybe. A csapda örökre elnyelt!");
	                    break;
                    default:
                        break;
	            }
	            System.Console.WriteLine("A játéknak vége!\n" +
	                                     "Pontszámod: " + args.Points);
	        }
		    System.Console.WriteLine("Add meg a neved:\n");
		    ToplistManager.AddPlayer(System.Console.ReadLine(), args.Points);

		    NewGame();
	    }
	}
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Wumpus.Model.Logic;
using Wumpus.Model.Presistence;
using Wumpus.Model.Settings;
using Wumpus.View.Properties;
using Wumpus.View.View;
using Wumpus.View.ViewModel;

namespace Wumpus.View
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
	    private WumpusGameLogic _gameLogic;
	    private WumpusViewModel _viewModel;
        private IWumpusDataAccess _dataAccess = new WumpusFileDataAccess();
	    private MainWindow _view;

		protected override void OnStartup(StartupEventArgs e)
		{
            base.OnStartup(e);
            Application.Current.Exit += Current_Exit;
		 	_gameLogic = new WumpusGameLogic();
		    _viewModel = new WumpusViewModel(_gameLogic, _dataAccess);
            _viewModel.QuitEvent += QuitEvent;

		    _view = new MainWindow
		    {
		        DataContext = _viewModel
                
		    };
		    _view.Closing += QuitEvent;
            _view.Show();

		}

	    private void QuitEvent(object sender, EventArgs e)
	    {
            if (MessageBox.Show("Biztosan kilépsz? Az aktuális eredmények elvesznek!", "Biztos?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _view.Closing -= QuitEvent;
                Application.Current.Shutdown();
            }
            else if (e is System.ComponentModel.CancelEventArgs)
            {
                var canc = (System.ComponentModel.CancelEventArgs) e;
                canc.Cancel = true;
            }
	    }

	    private void Current_Exit(object sender, ExitEventArgs e)
	    {
            Settings.Default.Save();
	    }

	    private void Application_Exit(object sender, ExitEventArgs e)
        {
            Settings.Default.Save();

        }
	}
}

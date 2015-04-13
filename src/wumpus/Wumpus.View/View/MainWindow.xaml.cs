using System.Windows;
using Wumpus.Model.Logic;
using Wumpus.Model.Settings;

namespace Wumpus.View.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Wumpus.Model.Logic.WumpusGameLogic _model;
		public MainWindow()
		{
			InitializeComponent();
			_model = new WumpusGameLogic(Levels.GetSetting(1));

		}

		private void button_Click(object sender, RoutedEventArgs e)
		{
			_model.StartGame();
		}
	}
}

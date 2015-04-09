using System.Windows;
using Wumpus.Model.Logic;

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
			_model = new WumpusGameLogic();

		}

		private void button_Click(object sender, RoutedEventArgs e)
		{
			_model.StartGame();
		}
	}
}

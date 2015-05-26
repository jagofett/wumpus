using System;
using System.Text.RegularExpressions;
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
		public MainWindow()
		{
			InitializeComponent();

		}

        private void TextToInt(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

	    private void PastingHandler(object sender, DataObjectPastingEventArgs e)
	    {
	        if (e.DataObject.GetDataPresent(typeof (String)))
	        {
	            var text = (String) e.DataObject.GetData(typeof (String));
	            if (!IsTextAllowed(text))
	            {
	                e.CancelCommand();
	            }
	        }
	        else
	        {
	            e.CancelCommand();
	        } 
	    }
    }
}

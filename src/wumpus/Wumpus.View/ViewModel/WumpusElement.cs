using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wumpus.View.ViewModel
{
    public class WumpusElement : ViewModelBase
    {
        private Boolean _isEnabled;
        private String _text;
        public String Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }
        public int X { get; set; }
        public int Y { get; set; }
        public int Id { get; set; }
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }
        public DelegateCommand StepCommand { get; set; }
    }
}

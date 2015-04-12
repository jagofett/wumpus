using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wumpus.Common.Enums;

namespace Wumpus.Model.Logic
{
    public class WumpusGameOverEventArgs :EventArgs
    {
        public bool IsGameWin { get; set; }
        public int Points { get; set; }

        public FieldType GameOverType { get; set; }
    }
}

using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wumpus.Common.Enums;

namespace Wumpus.Model.Logic
{
	public class WumpusField
	{
		public bool Visible { get; set; }
		public FieldType FieldType { get; set; }
		public List<SenseType> SenseTypes { get; set; }
		public Tuple<int,int> Coordinates { get; set; }


	}
}

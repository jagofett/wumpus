using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wumpus.Common.Enums
{
	public enum TrapNumberType
	{
		/// <summary>
		/// The fixed number
		/// </summary>
		FixedNumber = 0,
		/// <summary>
		/// The minimum maximum number tpye
		/// </summary>
		MinMaxNumber = 1
	}

	public enum Direction
	{
		Up,
		Down,
		Left,
		Right
	}
}

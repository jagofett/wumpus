using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Wumpus.Common.Enums;

namespace Wumpus.Model.Settings
{
	public class WumpusSetting
	{
		
		private TrapNumberType _trapNumberType;
		private int _trapNumberMax;
		private int _trapNumberMin;

		public int Size { get; set; }
		public int ArrowNumber { get; set; }
		public string SettingName { get; set; }
		public TrapNumberType TrapNumberType {
			get { return _trapNumberType; }
			set
			{
				_trapNumberType = value;
				CheckFixed(value);
			}
		}
		public int TrapNumberMax {
			get { return _trapNumberMax; }
			set
			{
				_trapNumberMax = value;
				CheckFixed(_trapNumberType);
			}
		}
		/// <summary>
		/// Gets or sets the trap number minimum. If trap number type is fixed, it should be the same as TrapNumberMax
		/// </summary>
		/// <value>
		/// The trap number minimum.
		/// </value>
		public int TrapNumberMin
		{
			get { return _trapNumberMin; }
			set
			{
				_trapNumberMin = value;
				CheckFixed(_trapNumberType);
			}
		}

		private void CheckFixed(TrapNumberType type)
		{
			if (type == TrapNumberType.FixedNumber)
			{
				_trapNumberMin = _trapNumberMax;
			}
		}
	}
}

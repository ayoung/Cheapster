using System;
using System.Linq;
using System.Collections.Generic;
using Cheaper.Data;
using Cheaper.Data.Models;

namespace Cheaper.Rules
{
	public static class ComparableExtensions
	{
		private static Dictionary<int, UnitModel> _units;
		
		public static double GetPricePerBaseUnit(this ComparableModel comparable, int baseUnitId)
		{
			if(_units == null)
			{
				_units = (from u in DataService.GetUnits()
					select u).ToDictionary(u => u.Id, u => u);
			}
			
			return ((comparable.Price / comparable.Quantity) / _units[comparable.UnitId].Multiplier) * _units[baseUnitId].Multiplier;
		}
	}
}


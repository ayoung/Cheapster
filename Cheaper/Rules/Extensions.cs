using System;
using System.Linq;
using System.Collections.Generic;
using Cheaper.Data;
using Cheaper.Data.Models;

namespace Cheaper.Rules
{
	public static class Extensions
	{
		public static double GetPricePerBaseUnit(this ComparableModel comparable, int baseUnitId)
		{
			return CalculatePrice(comparable.Price, comparable.Quantity, comparable.UnitId, baseUnitId);
		}

		public static double GetPricePerBaseUnit(this ComparisonModel comparison)
		{
			return CalculatePrice(comparison.CheapestPrice.Value, comparison.CheapestQuantity.Value, comparison.CheapestUnitId.Value, comparison.UnitId);
		}
		
		private static double CalculatePrice(double price, double quantity, int unitId, int baseUnitId)
		{
			var units = DataService.GetUnitsAsDictionary();
			return ((price / quantity) / units[unitId].Multiplier) * units[baseUnitId].Multiplier;
		}
	}
}


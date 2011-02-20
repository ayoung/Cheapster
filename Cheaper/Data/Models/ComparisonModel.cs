using System;
namespace Cheaper.Data.Models
{
	public class ComparisonModel
	{
		public int Id { get; set; }
		public int UnitTypeId { get; set; }
		public int UnitId { get; set; }
		public int CategoryId { get; set; }
		public string Name { get; set; }
		public string CheapestStore { get; set; }
		public double? CheapestPrice { get; set; }
		public double? CheapestQuantity { get; set; }
		public int? CheapestUnitId { get; set; }
		public int? CheapestComparableId { get; set; }
	}
}


using System;
namespace Cheapster.Data.Models
{
	public class ComparableModel
	{
		public int Id { get; set; }
		public int ComparisonId { get; set; }
		public int UnitId { get; set; }
		public string Store { get; set; }
		public string Product { get; set; }
		public double Price { get; set; }
		public double Quantity { get; set; }
		public DateTime ModifiedOn { get; set; }
	}
}


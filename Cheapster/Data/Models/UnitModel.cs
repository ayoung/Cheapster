using System;
namespace Cheapster.Data.Models
{
	public class UnitModel
	{
		public int Id { get; set; }
		public int UnitTypeId { get; set; }
		public string Name { get; set; }
		public string FullName { get; set; }
		public double Multiplier { get; set; }
	}
}


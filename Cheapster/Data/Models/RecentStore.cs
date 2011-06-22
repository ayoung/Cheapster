using System;
namespace Cheapster
{
	public class RecentStore
	{
		public RecentStore()
		{
		}

		public string Name { get; set; }
		public DateTime LastModifiedOn { get; set; }
		public int UsedCount { get; set; }
	}
}


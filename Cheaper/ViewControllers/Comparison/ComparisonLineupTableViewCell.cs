using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.UIKit;
using Cheaper.Data;
using Cheaper.Data.Models;

namespace Cheaper.ViewControllers.Comparison
{
	public class ComparisonLineupTableViewCell : UITableViewCell
	{
		public ComparisonLineupTableViewCell(UITableViewCellStyle style, string cellIdentifier) : base(style, cellIdentifier)
		{
		
		}
		
		public ComparableModel Comparable { get; set; }
	}
}


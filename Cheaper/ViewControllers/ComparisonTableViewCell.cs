using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.UIKit;
using Cheaper.Data;
using Cheaper.Data.Models;

namespace Cheaper.ViewControllers
{
	public class ComparisonTableViewCell : UITableViewCell
	{
		public ComparisonTableViewCell(UITableViewCellStyle style, string cellIdentifier) : base(style, cellIdentifier)
		{
		
		}
		
		public ComparisonModel Comparison { get; set; }
	}
}


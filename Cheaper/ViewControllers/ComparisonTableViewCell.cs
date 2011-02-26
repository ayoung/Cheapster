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
		
		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			TextLabel.BackgroundColor = UIColor.Clear;
			DetailTextLabel.BackgroundColor = UIColor.Clear;
			BackgroundColor = UIColor.Clear;
		}
		
		public ComparisonModel Comparison { get; set; }
	}
}


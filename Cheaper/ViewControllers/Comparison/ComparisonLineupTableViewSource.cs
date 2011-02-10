using System;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.UIKit;

using Cheaper.ViewControllers.Comparison;

namespace Cheaper
{
	public class ComparisonLineupTableViewSource : UITableViewSource
	{
		private ComparisonLineupTableView _tableView;
		
		public ComparisonLineupTableViewSource(ComparisonLineupTableView tableView)
		{
			_tableView = tableView;
		}
		
		public override int NumberOfSections(UITableView tableView)
		{
			return 1;
		}
		
		public override UITableViewCell GetCell(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			if(_tableView.Comparables == null || _tableView.Comparables.Count == 0)
			{
				var placeholderCell = new UITableViewCell();
				var placeholderLabel = new UILabel(new RectangleF(0, 0, _tableView.Frame.Width, 20));
				placeholderLabel.Center = new PointF(_tableView.Frame.Width / 2, placeholderCell.Frame.Height / 2);
				placeholderLabel.Text = "Nothing here! Add a product item.";
				placeholderLabel.Font = UIFont.FromName("Helvetica", 12);
				placeholderLabel.TextAlignment = UITextAlignment.Center;
				placeholderLabel.TextColor = UIColor.LightGray;
				placeholderCell.ContentView.AddSubview(placeholderLabel);
				return placeholderCell;
			}
			
			string cellIdentifier = "";
			
			var cell = tableView.DequeueReusableCell(cellIdentifier) as ComparisonLineupTableViewCell;
			if(cell == null) {
				// No re-usable cell found, create a new one
				cell = new ComparisonLineupTableViewCell(UITableViewCellStyle.Subtitle, cellIdentifier);
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			}

			var comparable = _tableView.Comparables[indexPath.Row];
			
			cell.TextLabel.Text = comparable.Product;
			cell.Comparable = comparable;
			
			return cell;
		}
		
		public override int RowsInSection(UITableView tableview, int section)
		{
			if(_tableView.Comparables == null || _tableView.Comparables.Count == 0)
			{
				return 1;
			}
			
			return _tableView.Comparables.Count;
		}
	}
}


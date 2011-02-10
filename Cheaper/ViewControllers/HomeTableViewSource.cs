using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.UIKit;
using Cheaper.Data;
using Cheaper.Data.Models;

namespace Cheaper.ViewControllers
{
	public class HomeTableViewSource : UITableViewSource
	{
		private HomeTableView _tableView;
		
		public event EventHandler OnComparisonSelected;
		
		public HomeTableViewSource(HomeTableView tableView)
		{
			_tableView = tableView;
		}

		public override int NumberOfSections(UITableView tableView)
		{
			return 1;
		}
		
		public override int RowsInSection(UITableView tableview, int section)
		{
			if(_tableView.Comparisons == null || _tableView.Comparisons.Count == 0) {
				return 1;
			}
			
			return _tableView.Comparisons.Count;
		}
		
//		public override string TitleForDeleteConfirmation(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
//		{
//			// TODO: Implement - see: http://go-mono.com/docs/index.aspx?link=T%3aMonoTouch.Foundation.ModelAttribute
//		}
		
		public override UITableViewCell GetCell(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			if(_tableView.Comparisons == null || _tableView.Comparisons.Count == 0) {
				var placeholderCell = new UITableViewCell();
				var placeholderLabel = new UILabel(new RectangleF(0, 0, _tableView.Frame.Width, 20));
				placeholderLabel.Center = new PointF(_tableView.Frame.Width / 2, placeholderCell.Frame.Height / 2);
				placeholderLabel.Text = "Start by adding a new comparison";
				placeholderLabel.Font = UIFont.FromName("Helvetica", 12);
				placeholderLabel.TextAlignment = UITextAlignment.Center;
				placeholderLabel.TextColor = UIColor.LightGray;
				placeholderCell.ContentView.AddSubview(placeholderLabel);
				return placeholderCell;
			}
			
			string cellIdentifier = "";
			
			var cell = (ComparisonTableViewCell)tableView.DequeueReusableCell(cellIdentifier);
			if(cell == null) {
				// No re-usable cell found, create a new one
				cell = new ComparisonTableViewCell(UITableViewCellStyle.Subtitle, cellIdentifier);
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			}
			var comparison = _tableView.Comparisons[indexPath.Row];
			
			cell.TextLabel.Text = comparison.Name;
			cell.Comparison = comparison;
			return cell;
		}
				
		public override void RowSelected(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			if(OnComparisonSelected != null) {
				OnComparisonSelected(this, EventArgs.Empty);
			}
		}
	}
}
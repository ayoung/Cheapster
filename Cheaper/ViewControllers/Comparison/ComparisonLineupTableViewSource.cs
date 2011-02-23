using System;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Cheaper.ViewControllers.Shared;
using Cheaper.Data;
using Cheaper.Rules;

using Cheaper.ViewControllers.Comparison;

namespace Cheaper
{
	public class ComparisonLineupTableViewSource : UITableViewSource
	{
		private ComparisonLineupTableView _tableView;
		public event EventHandler OnComparableSelected;
		private bool _deletingLastRow;
		private const string _detailTextWithStore = "${0}/{1} @ {2}";
		private const string _detailTextNoStore = "${0}/{1}";
		
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
			if(_tableView.Comparables.Count == 0)
			{
				var placeholderCell = new UITableViewCell();
				var placeholderLabel = new UILabel(new RectangleF(0, 0, _tableView.Frame.Width, 20));
				placeholderLabel.Center = new PointF(_tableView.Frame.Width / 2, placeholderCell.Frame.Height / 2);
				placeholderLabel.Text = "Nothing here! Add a product item.";
				placeholderLabel.Font = UIFont.FromName("Helvetica", 14);
				placeholderLabel.TextAlignment = UITextAlignment.Center;
				placeholderLabel.TextColor = UIColor.LightGray;
				placeholderCell.ContentView.AddSubview(placeholderLabel);
				return placeholderCell;
			}
			
			string cellIdentifier = "ComparisonLineupCell";
			
			var cell = tableView.DequeueReusableCell(cellIdentifier) as ComparisonLineupTableViewCell;
			if(cell == null) {
				// No re-usable cell found, create a new one
				cell = new ComparisonLineupTableViewCell(UITableViewCellStyle.Subtitle, cellIdentifier);
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			}

			var comparable = _tableView.Comparables[indexPath.Row];
			
			cell.TextLabel.Text = comparable.Product;
			cell.DetailTextLabel.Text = string.Format(string.IsNullOrEmpty(comparable.Store) ? _detailTextNoStore : _detailTextWithStore, 
				comparable.GetPricePerBaseUnit(_tableView.Comparison.UnitId).ToString("0.00#"),
				_tableView.Unit.Name,
				comparable.Store);
		
			return cell;
		}
		
		public override int RowsInSection(UITableView tableview, int section)
		{
			if(!_deletingLastRow && (_tableView.Comparables == null || _tableView.Comparables.Count == 0))
			{
				return 1;
			}
			
			_deletingLastRow = false;
			return _tableView.Comparables.Count;
		}
		
		public override void RowSelected(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			OnComparableSelected.Fire(this, EventArgs.Empty);
		}
		
		public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			if(editingStyle != UITableViewCellEditingStyle.Delete)
			{
				return;
			}
			
			var comparable = _tableView.Comparables[indexPath.Row];
			if(!DataService.DeleteComparable(comparable.Id))
			{
				new UIAlertView("Info", "Comparable was not found. Could not delete.", null, "Dismiss").Show();
			}
			
			if(!_tableView.Comparables.Remove(comparable))
			{
				new UIAlertView("Info", "Comparable was not found in the list. Could not delete.", null, "Dismiss").Show();
			}
			
			if(_tableView.Comparables.Count == 0)
			{
				_deletingLastRow = true;
			}
			
			_tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
		}
	}
}


using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Cheaper.Data;
using Cheaper.Data.Models;
using Cheaper.Rules;

namespace Cheaper.ViewControllers
{
	public class HomeTableViewSource : UITableViewSource
	{
		private HomeTableView _tableView;
		private bool _deletingLastRow;
		
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
			if(!_deletingLastRow && (_tableView.Comparisons == null || _tableView.Comparisons.Count == 0))
			{
				return 1;
			}
			
			_deletingLastRow = false;
			return _tableView.Comparisons.Count;
		}
		
		public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			if(editingStyle != UITableViewCellEditingStyle.Delete)
			{
				return;
			}
			
			var comparison = _tableView.Comparisons[indexPath.Row];
			if(!DataService.DeleteComparison(comparison.Id))
			{
				new UIAlertView("Info", "Comparison was not found. Could not delete.", null, "Dismiss").Show();
			}
			
			if(!_tableView.Comparisons.Remove(comparison))
			{
				new UIAlertView("Info", "Comparison was not found in the list. Could not delete.", null, "Dismiss").Show();
			}
			
			if(_tableView.Comparisons.Count == 0)
			{
				_deletingLastRow = true;
			}
			
			_tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
		}
		
		public override UITableViewCell GetCell(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			if(_tableView.Comparisons == null || _tableView.Comparisons.Count == 0) {
				var placeholderCell = new UITableViewCell();
				var placeholderLabel = new UILabel(new RectangleF(0, 0, _tableView.Frame.Width, 20));
				placeholderLabel.Center = new PointF(_tableView.Frame.Width / 2, placeholderCell.Frame.Height / 2);
				placeholderLabel.Text = "Start by adding a new comparison";
				placeholderLabel.Font = UIFont.FromName("Helvetica", 14);
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
			//Comparison.Summary = 
			//Comparison.Summary = string.Format("Cheaper at {0} for {1}/{2}", comparable.Store, comparable.GetPricePerBaseUnit(Comparison.UnitId).ToString("0.000"), Unit.Name);
			Console.WriteLine(comparison.CheapestComparableId.HasValue);
			Console.WriteLine(comparison.Name);
			if(comparison.CheapestComparableId != null)
			{
				if(!string.IsNullOrEmpty(comparison.CheapestStore))
				{
					
					cell.DetailTextLabel.Text = string.Format("Best buy: ${0}/{1} @ {2}", comparison.GetPricePerBaseUnit().ToString("0.00#"), DataService.GetUnitsAsDictionary()[comparison.UnitId].Name, comparison.CheapestStore);
				}
				else
				{
					cell.DetailTextLabel.Text = string.Format("Best buy: ${0}/{1}", comparison.GetPricePerBaseUnit().ToString("0.00#"), DataService.GetUnitsAsDictionary()[comparison.UnitId].Name);
				}
			}
			else
			{
				cell.DetailTextLabel.Text = null;
			}
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
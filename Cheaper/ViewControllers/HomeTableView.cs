using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Cheaper.Data;
using Cheaper.Data.Models;
using Cheaper.ViewControllers.Shared;

namespace Cheaper.ViewControllers
{
	public class HomeTableView : UITableView
	{
		private HomeTableViewSource _source;
		public event EventHandler OnComparisonSelected;
		public event EventHandler OnComparisonDeleted;
		public List<ComparisonModel> Comparisons { get; private set; }
		
		public HomeTableView(RectangleF frame, UITableViewStyle style) : base(frame, style)
		{
			Reset();
			_source = new HomeTableViewSource(this);
			_source.OnComparisonSelected += (sender, args) =>
			{
				OnComparisonSelected.Fire(this, EventArgs.Empty);
			};
			Source = _source;
		}

		public void ReloadRowForComparison(int comparisonId)
		{
			var comparison = (from c in Comparisons
				where c.Id == comparisonId
				select c).FirstOrDefault();
			
			if(comparison == null)
			{
				return;
			}
			
			var index = Comparisons.IndexOf(comparison);
			Comparisons[index] = DataService.GetComparison(comparisonId);
			var indexPaths = new NSIndexPath[] { NSIndexPath.FromRowSection(index, 0) };
			ReloadRows(indexPaths, UITableViewRowAnimation.None);
			SelectRow(indexPaths[0], false, UITableViewScrollPosition.None);
		}
		
		private void Reset()
		{
			Comparisons = DataService.GetComparisons();
			SetScrollAndSelection();
		}
		
		private void SetScrollAndSelection()
		{
			if(Comparisons.Count == 0)
			{
				AllowsSelection = false;
				ScrollEnabled = false;
			}
			else
			{
				AllowsSelection = true;
				ScrollEnabled = true;
			}
		}
		
		public override void ReloadData()
		{
			Reset();
			base.ReloadData();
		}
		
		public override void DeleteRows(NSIndexPath[] atIndexPaths, UITableViewRowAnimation withRowAnimation)
		{
			base.DeleteRows(atIndexPaths, withRowAnimation);
			
			if(Comparisons.Count == 0)
			{
				InsertRows(atIndexPaths, withRowAnimation);
				SetScrollAndSelection();
			}
			
			OnComparisonDeleted.Fire(this, EventArgs.Empty);
		}
		
		public ComparisonModel GetSelectedComparison()
		{
			return Comparisons[IndexPathForSelectedRow.Row];
		}
		
		public void SelectComparison(int comparisonId)
		{
			var comparison = (from c in Comparisons
				where c.Id == comparisonId
				select c).FirstOrDefault();
			
			if(comparison == null)
			{
				return;
			}
			
			var indexPath = NSIndexPath.FromRowSection(Comparisons.IndexOf(comparison), 0);
			SelectRow(indexPath, true, UITableViewScrollPosition.Top);
			DeselectRow(indexPath, true);
		}
		
		public void DeselectSelectedRow()
		{
			if(IndexPathForSelectedRow == null)
			{
				return;
			}
			DeselectRow(IndexPathForSelectedRow, true);
		}
		
		public void AddComparison(ComparisonModel comparison)
		{
			if(Comparisons.Count == 0)
			{
				Comparisons.Add(comparison);
				ReloadRows(new NSIndexPath[] { NSIndexPath.FromRowSection(0, 0) }, UITableViewRowAnimation.Fade);
			}
			else
			{
				Comparisons.Add(comparison);
				BeginUpdates();
				InsertRows(new NSIndexPath[] { NSIndexPath.FromRowSection(Comparisons.Count - 1, 0) }, UITableViewRowAnimation.Fade);
				EndUpdates();
			}
			SetScrollAndSelection();
		}
	}
}
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
		}
		
		private void Reset()
		{
			Comparisons = DataService.GetComparisons();
			if(Comparisons.Count == 0) {
				AllowsSelection = false;
				ScrollEnabled = false;
			}

			else {
				AllowsSelection = true;
				ScrollEnabled = true;
			}
		}
		
		public override void ReloadData()
		{
			Reset();
			base.ReloadData();
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
			
			SelectRow(NSIndexPath.FromRowSection(Comparisons.IndexOf(comparison), 0), true, UITableViewScrollPosition.Top);
			OnComparisonSelected.Fire(this, EventArgs.Empty);
		}
		
		public void DeselectSelectedRow()
		{
			if(IndexPathForSelectedRow == null) 
			{
				return;
			}
			DeselectRow(IndexPathForSelectedRow, true);
		}
	}
}
using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Cheaper.Data;
using Cheaper.Data.Models;
using Cheaper.ViewControllers.Shared;

namespace Cheaper.ViewControllers.Comparison
{
	public class ComparisonLineupTableView : UITableView
	{
		public List<ComparableModel> Comparables { get; private set; }
		public event EventHandler OnComparableSelected;
		private ComparisonLineupTableViewSource _source;
		private int _comparisonId;
		
		public ComparisonLineupTableView(int comparisonId, RectangleF frame, UITableViewStyle style) : base(frame, style)
		{
			_comparisonId = comparisonId;
			Reset();
			_source = new ComparisonLineupTableViewSource(this);
			_source.OnComparableSelected += (sender, args) =>
			{
				OnComparableSelected.Fire(this, EventArgs.Empty);
			};
			Source = _source;
		}
		
		public ComparableModel GetSelectedComparable()
		{
			return Comparables[IndexPathForSelectedRow.Row];
		}
		
		public void AddComparable(ComparableModel comparable)
		{
			if(Comparables.Count == 0)
			{
				Comparables.Add(comparable);
				ReloadRows(new NSIndexPath[] { NSIndexPath.FromRowSection(0, 0) }, UITableViewRowAnimation.Fade);
			}
			else
			{
				Comparables.Add(comparable);
				BeginUpdates();
				InsertRows(new NSIndexPath[] { NSIndexPath.FromRowSection(Comparables.Count - 1, 0) }, UITableViewRowAnimation.Fade);
				EndUpdates();
			}
		}
		
		public void Reset()
		{
			Comparables = DataService.GetComparables(_comparisonId);
			if(Comparables.Count == 0) {
				ScrollEnabled = false;
				AllowsSelection = false;
			}
			else 
			{
				ScrollEnabled = true;
				AllowsSelection = true;
			}
			ReloadData();
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
using System;
using System.Linq;
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
		public ComparisonModel Comparison { get; private set; }
		public Dictionary<int, UnitModel> UnitMap { get; private set; }
		public UnitModel Unit { get; private set; }
		public event EventHandler OnComparableSelected;
		public event EventHandler OnComparableDeleted;
		private ComparisonLineupTableViewSource _source;
		
		public ComparisonLineupTableView(ComparisonModel comparison, RectangleF frame, UITableViewStyle style) : base(frame, style)
		{
			Reset(comparison);
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
			SetScrollAndSelection();
		}
		
		public override void DeleteRows(NSIndexPath[] atIndexPaths, UITableViewRowAnimation withRowAnimation)
		{
			base.DeleteRows(atIndexPaths, withRowAnimation);
			
			if(Comparables.Count == 0)
			{
				InsertRows(atIndexPaths, withRowAnimation);
				SetScrollAndSelection();
			}
			
			OnComparableDeleted.Fire(this, EventArgs.Empty);
		}

		public void ReloadRowForComparable(int comparableId)
		{
			var comparable = (from c in Comparables
				where c.Id == comparableId
				select c).FirstOrDefault();
		
			if(comparable == null)
			{
				return;
			}
		
			var index = Comparables.IndexOf(comparable);
			Comparables[index] = DataService.GetComparable(comparableId);
			var indexPaths = new NSIndexPath[] { NSIndexPath.FromRowSection(index, 0) };
			ReloadRows(indexPaths, UITableViewRowAnimation.None);
			SelectRow(indexPaths[0], false, UITableViewScrollPosition.None);
		}
		
		public void Reset(ComparisonModel comparison)
		{
			Comparison = comparison;
			Comparables = DataService.GetComparables(Comparison.Id);
			SetScrollAndSelection();
			UnitMap = (from u in DataService.GetUnits(Comparison.UnitTypeId)
				select u).ToDictionary(u => u.Id, u => u);
			Unit = UnitMap[Comparison.UnitId];
			ReloadData();
		}

		private void SetScrollAndSelection()
		{
			if(Comparables.Count == 0)
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
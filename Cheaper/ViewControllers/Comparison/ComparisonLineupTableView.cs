using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Cheaper.Data;
using Cheaper.Data.Models;
using Cheaper.ViewControllers.Shared;
using Cheaper.Rules;

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
		public event EventHandler OnNewCheaper;
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
				UpdateCheapestComparable(comparable);
				ReloadRows(new NSIndexPath[] { NSIndexPath.FromRowSection(0, 0) }, UITableViewRowAnimation.Fade);
			}
			else
			{
				Comparables.Add(comparable);
				Comparables = Comparables.OrderBy(c => c.GetPricePerBaseUnit(Comparison.UnitId)).ToList();
				var addedAt = Comparables.IndexOf(comparable);
				if(addedAt == 0)
				{
					UpdateCheapestComparable(comparable);
				}
				BeginUpdates();
				InsertRows(new NSIndexPath[] { NSIndexPath.FromRowSection(addedAt, 0) }, UITableViewRowAnimation.Fade);
				EndUpdates();
			}
			SetScrollAndSelection();
		}
		
		public void UpdateCheapestComparable(ComparableModel comparable)
		{
			Comparison.CheapestComparableId = comparable == null ? (int?)null : comparable.Id;
			DataService.UpdateComparison(Comparison);
			OnNewCheaper.Fire(this, EventArgs.Empty);
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
			
			if(atIndexPaths[0].Row == 0)
			{
				UpdateCheapestComparable(Comparables.Count == 0 ? null : Comparables[0]);
			}
		}
		
		public void RepositionRowForComparable(int comparableId)
		{
			var comparable = (from c in Comparables
				where c.Id == comparableId
				select c).FirstOrDefault();
		
			if(comparable == null)
			{
				return;
			}
			
			// refresh the comparable object
			var index = Comparables.IndexOf(comparable);
			var refreshedComparable = DataService.GetComparable(comparableId);
			Comparables[index] = refreshedComparable;
			
			// reorder the list again
			Comparables = Comparables.OrderBy(c => c.GetPricePerBaseUnit(Comparison.UnitId)).ToList();
			
			// get the new index of the comparable
			var newIndex = Comparables.IndexOf(refreshedComparable);
			
			if(newIndex == index)
			{
				var indexPaths = new NSIndexPath[] { NSIndexPath.FromRowSection(index, 0) };
				ReloadRows(indexPaths, UITableViewRowAnimation.None);
				SelectRow(indexPaths[0], false, UITableViewScrollPosition.None);
			}
			else
			{
				BeginUpdates();
				InsertRows(new NSIndexPath[] { NSIndexPath.FromRowSection(newIndex, 0) }, UITableViewRowAnimation.Fade);
				DeleteRows(new NSIndexPath[] { NSIndexPath.FromRowSection(index, 0) }, UITableViewRowAnimation.Fade);
				EndUpdates();
				if(newIndex == 0)
				{
					UpdateCheapestComparable(refreshedComparable);
				}
			}
		}
		
		public void Reset(ComparisonModel comparison)
		{
			Comparison = comparison;
			Comparables = DataService.GetComparables(Comparison.Id).OrderBy(c => c.GetPricePerBaseUnit(comparison.UnitId)).ToList();
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
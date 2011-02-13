using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.UIKit;
using Cheaper.Data;
using Cheaper.Data.Models;
using Cheaper.ViewControllers.Shared;

namespace Cheaper.ViewControllers.Comparison
{
	public class ComparisonLineupViewController : UIViewController
	{
		public event EventHandler OnAddComparable;
		public event EventHandler OnComparableSelected;
		public event EventHandler OnModify;
		private ComparisonLineupTableView _tableView;
		private UIToolbar _toolbar;
		private ComparisonModel _comparison;
		
		public ComparisonLineupViewController(int comparisonId)
		{
			_comparison = DataService.GetComparison(comparisonId);
			Title = _comparison.Name;
			NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add, (sender, args) =>
			{
				OnAddComparable.Fire(this, new EventArgs());
			});
			
			_tableView = new ComparisonLineupTableView(comparisonId, new RectangleF(0, 0, View.Frame.Width, View.Frame.Height - 88), UITableViewStyle.Plain);
			_tableView.OnComparableSelected += (sender, args) =>
			{
				OnComparableSelected.Fire(this, EventArgs.Empty);
			};
			
			View.AddSubview(_tableView);
			
			_toolbar = new UIToolbar(new RectangleF(0, View.Frame.Height - 88, View.Frame.Width, 44));
			_toolbar.TintColor = UIColor.DarkGray;
			var toolbarItems = new List<UIBarButtonItem>();
			var buttonItem = new UIBarButtonItem("Edit", UIBarButtonItemStyle.Bordered, (sender, args) => 
			{ 
				OnModify.Fire(this, new EventArgs());
			});
			toolbarItems.Add(new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace));
			toolbarItems.Add(buttonItem);
			_toolbar.SetItems(toolbarItems.ToArray(), false);
			View.AddSubview(_toolbar);
		}
		
		public void ReloadRowForComparable(int comparableId)
		{
			
		}
		
		public void Reload()
		{
			// refresh comparison from DB
			_comparison = DataService.GetComparison(_comparison.Id);
			Title = _comparison.Name;

			_tableView.Reset();
		}
		
		public ComparableModel GetSelectedComparable()
		{
			return _tableView.GetSelectedComparable();
		}
		
		public int ComparisonId { get { return _comparison.Id; } }
	}
}

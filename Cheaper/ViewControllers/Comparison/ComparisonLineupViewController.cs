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
	public class ComparisonLineupViewController : UIViewController
	{
		public event EventHandler OnAddComparable;
		public event EventHandler OnComparableSelected;
		public event EventHandler OnModify;
		public event EventHandler OnNewCheaper;
		private ComparisonLineupTableView _tableView;
		private UIToolbar _toolbar;
		private ComparisonModel _comparison;
		private UIBarButtonItem _trashButton;
		private ComparableModel _comparableToAdd;
		private bool _reloadOnAppeared;
		private int? _comparableIdToReposition;
		
		public ComparisonLineupViewController(ComparisonModel comparison)
		{
			_comparison = comparison;
		}
		
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			
			var whiteBackground = new UIView(new RectangleF(0, 0, View.Frame.Width, View.Frame.Height));
			whiteBackground.BackgroundColor = UIColor.White;
			View.AddSubview(whiteBackground);
			
			Title = _comparison.Name;
			NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add, (sender, args) => { OnAddComparable.Fire(this, new EventArgs()); });
			
			_tableView = new ComparisonLineupTableView(_comparison, new RectangleF(0, 0, View.Frame.Width, View.Frame.Height - 88), UITableViewStyle.Plain);
			_tableView.OnComparableSelected += (sender, args) => { OnComparableSelected.Fire(this, EventArgs.Empty); };
			_tableView.OnComparableDeleted += (sender, args) =>
			{
				if(_tableView.Comparables.Count > 0)
				{
					return;
				}
				
				_trashButton.Enabled = false;
				
				if(_tableView.Editing)
				{
					NavigationItem.RightBarButtonItem.Enabled = true;
					_tableView.SetEditing(false, true);
				}
			};
			_tableView.OnNewCheaper += (sender, args) => 
			{
				OnNewCheaper.Fire(this, EventArgs.Empty);
			};
			
			View.AddSubview(_tableView);
			
			_toolbar = new UIToolbar(new RectangleF(0, View.Frame.Height - 88, View.Frame.Width, 44));
			_toolbar.TintColor = UIColor.DarkGray;
			var toolbarItems = new List<UIBarButtonItem>();
			var editButtonItem = new UIBarButtonItem("Change Base Unit", UIBarButtonItemStyle.Bordered, (sender, args) => { OnModify.Fire(this, new EventArgs()); });
			
			_trashButton = new UIBarButtonItem(UIBarButtonSystemItem.Trash, (sender, args) =>
			{
				NavigationItem.RightBarButtonItem.Enabled = _tableView.Editing;
				_tableView.SetEditing(!_tableView.Editing, true);
			});
			
			toolbarItems.Add(editButtonItem);
			toolbarItems.Add(new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace));
			toolbarItems.Add(_trashButton);
			_toolbar.SetItems(toolbarItems.ToArray(), false);
			View.AddSubview(_toolbar);
			
			if(_tableView.Comparables.Count == 0)
			{
				_trashButton.Enabled = false;
			}
		}
		
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			if(_reloadOnAppeared)
			{
				_reloadOnAppeared = false;
				if(_tableView.Comparables.Count == 0)
				{
					return;
				}
				
				// fade table out, reload data and fade back in
				_tableView.Reset(_comparison);
				UIView.Animate(0.2, () => { _tableView.Alpha = 1; }, () => { _tableView.Opaque = true; });
			}
			else if(_comparableToAdd != null)
			{
				_tableView.AddComparable(_comparableToAdd);
				_comparableToAdd = null;
			}
			
			if(_comparableIdToReposition.HasValue)
			{
				_tableView.RepositionRowForComparable(_comparableIdToReposition.Value);
				_comparableIdToReposition = null;
			}
			
			_tableView.DeselectSelectedRow();
		}
		
		public void RepositionRowForComparable(int comparableId)
		{
			_comparableIdToReposition = comparableId;
		}
		
		public void AddComparable(ComparableModel comparable)
		{
			_comparableToAdd = comparable;
		}
		
		public void ReloadOnAppeared()
		{
			_comparison = DataService.GetComparison(_comparison.Id);
			Title = _comparison.Name;
			_reloadOnAppeared = true;
			_tableView.Opaque = false;
			_tableView.Alpha = 0;
		}
		
		public void EnableTrashButton()
		{
			_trashButton.Enabled = true;
		}
		
		public ComparableModel GetSelectedComparable()
		{
			return _tableView.GetSelectedComparable();
		}
		
		public int ComparisonId { get { return _comparison.Id; } }
	}
}

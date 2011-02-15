using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.UIKit;
using Cheaper.Data;
using Cheaper.ViewControllers.Shared;

namespace Cheaper.ViewControllers
{
	public class HomeListViewController : UIViewController
	{
		public event EventHandler OnAddComparison;
		public event EventHandler OnComparisonSelected;
		private bool _shouldSelectComparisonOnViewDidAppear;
		private HomeTableView _tableView;
		private UIToolbar _toolbar;
		private UIBarButtonItem _trashButton;
		
		public HomeListViewController()
		{
		}
		
		public void ReloadRowForComparison(int comparisonId)
		{
			_tableView.ReloadRowForComparison(comparisonId);
		}
		
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			Title = "Cheaper";

			NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add, (sender, args) =>
			{
				OnAddComparison.Fire(this, EventArgs.Empty);
			});
			
			_tableView = new HomeTableView(new RectangleF(0, 0, View.Frame.Width, View.Frame.Height - 88), UITableViewStyle.Plain);
			_tableView.OnComparisonSelected += (sender, args) =>
			{
				NavigationItem.BackBarButtonItem = new UIBarButtonItem("Home", UIBarButtonItemStyle.Bordered, null);
				SelectedComparisonId = _tableView.GetSelectedComparison().Id;
				OnComparisonSelected.Fire(this, EventArgs.Empty);
			};
			_tableView.OnComparisonDeleted += (sender, args) =>
			{
				if(_tableView.Comparisons.Count > 0)
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
			View.AddSubview(_tableView);
			
			_toolbar = new UIToolbar(new RectangleF(0, View.Frame.Height - 88, View.Frame.Width, 44));
			_toolbar.TintColor = UIColor.DarkGray;
			var toolbarItems = new List<UIBarButtonItem>();
			_trashButton = new UIBarButtonItem(UIBarButtonSystemItem.Trash, (sender, args) =>
			{
				NavigationItem.RightBarButtonItem.Enabled = _tableView.Editing;
				_tableView.SetEditing(!_tableView.Editing, true);
			});

			var infoButton = UIButton.FromType(UIButtonType.Info);
			infoButton.TouchUpInside += (sender, args) =>
			{
				
			};
			
			toolbarItems.Add(new UIBarButtonItem(infoButton));
			toolbarItems.Add(new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace));
			toolbarItems.Add(_trashButton);
			_toolbar.SetItems(toolbarItems.ToArray(), false);
			View.AddSubview(_toolbar);

			if(_tableView.Comparisons.Count == 0) {
				_trashButton.Enabled = false;
			}
		}
		
		public void EnableTrashButton()
		{
			_trashButton.Enabled = true;
		}
		
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			if(_shouldSelectComparisonOnViewDidAppear)
			{
				// reset the flag
				_shouldSelectComparisonOnViewDidAppear = false;
				
				// reload table data
				_tableView.ReloadData();
				_tableView.SelectComparison(SelectedComparisonId.Value);
				
				return;
			}
			
			_tableView.DeselectSelectedRow();
		}
		
		public int? SelectedComparisonId { get; set; }
		
		public void Reload() 
		{
			// todo: implement table reload here
		}
		
		/// <summary>
		/// Causes this controller to redirect to the given 
		/// </summary>
		public void SelectComparisonOnViewDidAppear (int comparisonToSelect)
		{
			_shouldSelectComparisonOnViewDidAppear = true;
			SelectedComparisonId = comparisonToSelect;
		}
	}
}


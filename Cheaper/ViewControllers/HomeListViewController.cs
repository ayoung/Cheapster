using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.UIKit;
using Cheaper.Data;

namespace Cheaper.ViewControllers
{
	public class HomeListViewController : UIViewController
	{
		public event EventHandler OnAddComparison;
		public event EventHandler OnComparisonSelected;
		private bool _shouldSelectComparisonOnViewDidAppear;
		private HomeTableView _tableView;
		private UIToolbar _toolbar;
		
		public HomeListViewController()
		{
		}
		
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			Title = "Cheaper";

			NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add, (sender, args) =>
			{
				if(OnAddComparison != null)
				{
					OnAddComparison(this, EventArgs.Empty);
				}
			});
			
			_tableView = new HomeTableView(new RectangleF(0, 0, View.Frame.Width, View.Frame.Height - 88), UITableViewStyle.Plain);
			_tableView.OnComparisonSelected += (sender, args) =>
			{
				NavigationItem.BackBarButtonItem = new UIBarButtonItem("Home", UIBarButtonItemStyle.Bordered, null);
				SelectedComparisonId = _tableView.GetSelectedComparison().Id;
				if(OnComparisonSelected != null) {
					OnComparisonSelected(this, EventArgs.Empty);
				}
			};
			View.AddSubview(_tableView);
			
			_toolbar = new UIToolbar(new RectangleF(0, View.Frame.Height - 88, View.Frame.Width, 44));
			_toolbar.TintColor = UIColor.DarkGray;
			var toolbarItems = new List<UIBarButtonItem>();
			var buttonItem = new UIBarButtonItem(UIBarButtonSystemItem.Trash, (sender, args) =>
			{
				_tableView.SetEditing(!_tableView.Editing, true);
			});
			toolbarItems.Add(new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace));
			toolbarItems.Add(buttonItem);
			_toolbar.SetItems(toolbarItems.ToArray(), false);
			View.AddSubview(_toolbar);
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
				
//				if(OnComparisonSelected != null) {
//					NavigationItem.BackBarButtonItem = new UIBarButtonItem("Comparison List", UIBarButtonItemStyle.Bordered, null);
//					OnComparisonSelected(this, EventArgs.Empty);
//				}
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


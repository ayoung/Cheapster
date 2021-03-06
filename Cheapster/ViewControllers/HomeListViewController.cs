using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.UIKit;
using Cheapster.Data;
using Cheapster.ViewControllers.Shared;
using Cheapster.Data.Models;
using System.IO;

namespace Cheapster.ViewControllers
{
	public class HomeListViewController : UIViewController
	{
		public event EventHandler OnAddComparison;
		public event EventHandler OnComparisonSelected;
		public event EventHandler OnInfoButton;
		private HomeTableView _tableView;
		private UIToolbar _toolbar;
		private UIBarButtonItem _trashButton;
		private ComparisonModel _comparisonToAdd;
		private int? _comparisonToReposition;
		private Action<Action> _prepareForRestore;
		
		public HomeListViewController()
		{
		}
		
		public void PrepareForRestore(Action<Action> preparedCallback)
		{
			if(NavigationController.TopViewController == this)
			{
				FadeOutTable(preparedCallback);
				return;
			}
			_prepareForRestore = preparedCallback;
		}
		
		public void ReloadRowForComparison(int comparisonId)
		{
			_tableView.ReloadRowForComparison(comparisonId);
		}
		
		public void RepositionRowForComparison(int comparisonId)
		{
			_comparisonToReposition = comparisonId;
		}
		
		public ComparisonModel SelectedComparison { get; private set; }
		
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			Title = "Cheapster";

			NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add, (sender, args) =>
			{
				OnAddComparison.Fire(this, EventArgs.Empty);
			});
			
			_tableView = new HomeTableView(new RectangleF(0, 0, View.Frame.Width, View.Frame.Height - 88), UITableViewStyle.Plain);
			
			_tableView.Opaque = false;
			_tableView.Alpha = 0;
			
			_tableView.OnComparisonSelected += (sender, args) =>
			{
				NavigationItem.BackBarButtonItem = new UIBarButtonItem("Home", UIBarButtonItemStyle.Bordered, null);
				SelectedComparison = _tableView.GetSelectedComparison();
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

			var infoButton = UIButton.FromType(UIButtonType.InfoLight);
			infoButton.TouchUpInside += (sender, args) =>
			{
				OnInfoButton.Fire(this, EventArgs.Empty);
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
		
		private void FadeInTable()
		{
			UIView.Animate(0.4, () => 
			{ 
				_tableView.Alpha = 1; 
			}, () =>
			{
				_tableView.Opaque = true;
			});
		}
		
		private void FadeOutTable(Action<Action> callback)
		{
			_tableView.Opaque = false;
			UIView.Animate(0.4, () => 
			{ 
				_tableView.Alpha = 0; 
			}, () =>
			{
				if(callback != null)
				{
					callback(() =>
					{
						_tableView.Reset();
						_tableView.ReloadData();
						FadeInTable();
					});
				}
			});
		}
		
		public override void ViewDidAppear(bool animated)
		{
			Console.WriteLine("HomeListViewController ViewDidAppear");
			base.ViewDidAppear(animated);
			
			if(_prepareForRestore != null)
			{
				FadeOutTable(_prepareForRestore);
				_prepareForRestore = null;
				return;
			}
			
			if(!_tableView.Opaque)
			{
				FadeInTable();
				return;
			}
			
			if(_comparisonToAdd != null)
			{
				_tableView.AddComparison(_comparisonToAdd);
				_comparisonToAdd = null;
			}
			
			if(_comparisonToReposition.HasValue)
			{
				_tableView.RepositionRowForComparison(_comparisonToReposition.Value);
				_comparisonToReposition = null;
			}
			else
			{
				_tableView.DeselectSelectedRow();
			}
			
		}
		
		public void AddComparison(ComparisonModel comparison)
		{
			_comparisonToAdd = comparison;
		}
		
		public void Reload() 
		{
			// todo: implement table reload here
		}
	}
}


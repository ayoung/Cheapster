using System;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Cheapster.Data;
using Cheapster.Data.Models;
using Cheapster.ViewControllers.Shared;

namespace Cheapster.ViewControllers.Comparison
{
	public class ComparisonViewController : UIViewController
	{
		private UnitPicker _unitPicker;
		public event EventHandler OnFinished;
		public event EventHandler OnCanceled;
		public ComparisonTableView _tableView;
		private bool _initialized;
		
		public ComparisonViewController(ComparisonModel comparison)
		{
			Comparison = comparison;
			Initialize();
		}

		public ComparisonViewController()
		{
			Initialize();
		}

		public ComparisonModel Comparison { get; private set; }

		private void Initialize()
		{
			var view = new EventedView(View.Frame);
			view.OnTouchesEnded += (sender, args) =>
			{
				_tableView.ResignTextFieldAsFirstResponder();
			};
			
			View = view;
			
			// add navigation item
			View.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
			var navigationBar = new UINavigationBar(new RectangleF(0, 0, View.Frame.Width, 44));
			//navigationBar.TintColor = UIColor.DarkGray;
			var navigationItem = new UINavigationItem(Comparison == null ? "New Comparison" : Comparison.Name);
			var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (sender, args) =>
			{
				if(_tableView.ComparisonName == null || _tableView.ComparisonName.Trim() == string.Empty)
				{
					new UIAlertView("Warning", "Please provide a comparison name", null, "Dismiss").Show();
					return;
				}
				
				if(Comparison == null)
				{
					Comparison = new ComparisonModel() {
						Name = _tableView.ComparisonName.Trim(),
						UnitId = _unitPicker.SelectedUnit.Id,
						UnitTypeId = _tableView.UnitTypeId
					};
					Comparison.Id = DataService.SaveComparison(Comparison);
				}
				else
				{
					Comparison.Name = _tableView.ComparisonName.Trim();
					Comparison.UnitId = _unitPicker.SelectedUnit.Id;
					Comparison.UnitTypeId = _tableView.UnitTypeId;
					DataService.UpdateComparison(Comparison);
				}

				if(OnFinished != null) {
					OnFinished(this, EventArgs.Empty);
				}
			});
			
			var cancelButton = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, (sender, args) =>
			{
				if(Comparison != null)
				{
					navigationItem.Title = Comparison.Name;
				}
				
				if(OnCanceled != null) {
					OnCanceled(this, EventArgs.Empty);
				}
			});
			
			navigationItem.SetRightBarButtonItem(doneButton, false);
			navigationItem.SetLeftBarButtonItem(cancelButton, false);
			navigationBar.PushNavigationItem(navigationItem, false);
			View.AddSubview(navigationBar);
			
			_tableView = new ComparisonTableView(new RectangleF(0, 44, View.Frame.Width, View.Frame.Height - 44), UITableViewStyle.Grouped);
			_tableView.OnUnitTypeChanged += (sender, args) =>
			{
				_unitPicker.SetUnitType(_tableView.UnitTypeId);
				_tableView.SetUnitText(_unitPicker.SelectedUnit.FullName);
			};
			
			_tableView.OnTouchesEnded += (sender, args) =>
			{
				_tableView.ResignTextFieldAsFirstResponder();
			};
			
			_tableView.OnKeyboardDone += (sender, args) =>
			{
				_tableView.ResignTextFieldAsFirstResponder();
			};
			
			if(Comparison != null)
			{
				_tableView.OnNameChanged += (sender, args) =>
				{
					navigationItem.Title = _tableView.ComparisonName;
				};
			}
			
			View.AddSubview(_tableView);
			
			if(Comparison == null)
			{
				_unitPicker = new UnitPicker(1, new RectangleF(0, View.Frame.Height - 216, 320, 216));
			}
			else
			{
				_unitPicker = new UnitPicker(Comparison.UnitTypeId, new RectangleF(0, View.Frame.Height - 216, 320, 216));
			}
			
			_unitPicker.OnSelectionChanged += (sender, args) =>
			{
				_tableView.SetUnitText(_unitPicker.SelectedUnit.FullName);
			};
			
			View.AddSubview(_unitPicker);
		}
		
		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			
			if(Comparison != null && !_initialized)
			{
				_tableView.ComparisonName = Comparison.Name;
				_tableView.UnitTypeId = Comparison.UnitTypeId;
				_tableView.DisableUnitTypesExcept(Comparison.UnitTypeId);
				_unitPicker.SetSelectedUnit(Comparison.UnitId);
				_tableView.SetUnitText(_unitPicker.SelectedUnit.FullName);
			}
			
			_initialized = true;
		}
	
		private class EventedView : UIView
		{
			public event EventHandler OnTouchesEnded;
			public EventedView(RectangleF frame) : base(frame)
			{
			
			}
			
			public override void TouchesEnded(MonoTouch.Foundation.NSSet touches, UIEvent evt)
			{
				OnTouchesEnded.Fire(this, EventArgs.Empty);
				base.TouchesEnded(touches, evt);
			}
		}
	}
}

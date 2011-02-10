using System;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Cheaper.Data;
using Cheaper.Data.Models;
using Cheaper.ViewControllers.Shared;

namespace Cheaper.ViewControllers.Comparison
{
	public class ComparisonViewController : UIViewController
	{
		private UnitPicker _unitPicker;
		private ComparisonModel _comparison;
		public event EventHandler OnFinished;
		public event EventHandler OnCanceled;
		public ComparisonTableView _tableView;
		private bool _pickerVisible;
		private bool _keyboardVisible;
		private NSObject _keyboardShowObserver;
		private NSObject _keyboardHideObserver;
		private bool _initialized;
		
		public ComparisonViewController(int comparisonId)
		{
			_comparison = DataService.GetComparison(comparisonId);
			Initialize();
		}

		public ComparisonViewController()
		{
			Initialize();
		}

		public int? NewComparisonId { get; private set; }

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
			navigationBar.TintColor = UIColor.DarkGray;
			var navigationItem = new UINavigationItem(_comparison == null ? "New Comparison" : _comparison.Name);
			var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (sender, args) =>
			{
				if(_tableView.ComparisonName == null || _tableView.ComparisonName.Trim() == string.Empty)
				{
					new UIAlertView("Info", "Please provide a comparison name", null, "Dismiss");
					return;
				}
				
				NewComparisonId = DataService.SaveComparison(new ComparisonModel() {
					Name = _tableView.ComparisonName.Trim(),
					UnitId = _unitPicker.SelectedUnit.Id,
					UnitTypeId = _tableView.UnitTypeId
				});
				
				if(OnFinished != null) {
					OnFinished(this, EventArgs.Empty);
				}
			});
			
			var cancelButton = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, (sender, args) =>
			{
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
			
			_tableView.OnEditUnit += (sender, args) =>
			{
				_pickerVisible = true;
				if(_keyboardVisible)
				{
					_unitPicker.Frame = new RectangleF(0, View.Frame.Height - _unitPicker.Frame.Height, View.Frame.Width, 216);
				}
				else {
					UIView.Animate(0.3, () =>
					{
						_unitPicker.Frame = new RectangleF(0, View.Frame.Height - _unitPicker.Frame.Height, View.Frame.Width, 216);
					});
				}
			};
			
			_tableView.OnTouchesEnded += (sender, args) =>
			{
				if(_pickerVisible) {
					_pickerVisible = false;
					UIView.Animate(0.3, () =>{
						_unitPicker.Frame = new RectangleF(0, 460, 320, 216);
					});
				}
				_tableView.ResignTextFieldAsFirstResponder();
			};
			
			_tableView.OnKeyboardDone += (sender, args) =>
			{
				if(_pickerVisible) {
					_pickerVisible = false;
					UIView.Animate(0.3, () =>{
						_unitPicker.Frame = new RectangleF(0, 460, 320, 216);
					});
				}
				_tableView.ResignTextFieldAsFirstResponder();
			};
			
			View.AddSubview(_tableView);
			
			if(_comparison == null)
			{
				_unitPicker = new UnitPicker(1, new RectangleF(0, 460, 320, 216));
			}
			else
			{
				_unitPicker = new UnitPicker(_comparison.UnitTypeId, new RectangleF(0, 460, 320, 216));
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
			
			if(_comparison != null && !_initialized)
			{
				_tableView.ComparisonName = _comparison.Name;
				_tableView.UnitTypeId = _comparison.UnitTypeId;
				_tableView.DisableUnitTypesExcept(_comparison.UnitTypeId);
				_unitPicker.SetSelectedUnit(_comparison.UnitId);
				_tableView.SetUnitText(_unitPicker.SelectedUnit.FullName);
			}
			
			_initialized = true;
		}
		
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
		
			_keyboardShowObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.DidShowNotification, (notification) => {
				_keyboardVisible = true;
			});

			_keyboardHideObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, (notification) => {
				_keyboardVisible = false;
			});
		}
		
		public override void ViewDidUnload()
		{
			base.ViewDidUnload();
			NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardHideObserver);
			NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardShowObserver);
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

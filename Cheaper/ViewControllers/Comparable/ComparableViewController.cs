using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Cheaper.Data;
using Cheaper.Data.Models;
using Cheaper.ViewControllers.Shared;

namespace Cheaper.ViewControllers.Comparable
{
	public class ComparableViewController : UIViewController
	{
		public event EventHandler OnFinished;
		public event EventHandler OnCanceled;
		private UnitPicker _unitPicker;
		private ComparableTableView _tableView;
		private NSObject _keyboardShowObserver;
		private NSObject _keyboardHideObserver;
		private bool _pickerVisible;
		private bool _keyboardVisible;
		private static Regex _moneyRegex = new Regex(@"^\$?([1-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?)$");
		
		/// <summary>
		/// Controller that adds/edits a comparable
		/// </summary>
		/// <param name="comparisonId">
		/// The comparison that this comparable belongs to
		/// </param>
		public ComparableViewController(int comparisonId)
		{
			ComparisonId = comparisonId;
			Comparison = DataService.GetComparison(comparisonId);
			View.BackgroundColor = UIColor.White;
			
			// add tableview
			_tableView = new ComparableTableView(new RectangleF(0, 44, View.Frame.Width, View.Frame.Height - 44), UITableViewStyle.Grouped);
			_tableView.OnEditUnit += (sender, args) =>
			{
				_pickerVisible = true;
				if(_keyboardVisible)
				{
					_unitPicker.Frame = new RectangleF(0, View.Frame.Height - _unitPicker.Frame.Height, View.Frame.Width, 216);
					_tableView.Frame = new RectangleF(0, 44, View.Frame.Width, View.Frame.Height - _unitPicker.Frame.Height - 44);
				}
				else {
					UIView.Animate(0.3, () =>
					{
						_unitPicker.Frame = new RectangleF(0, View.Frame.Height - _unitPicker.Frame.Height, View.Frame.Width, 216);
						_tableView.Frame = new RectangleF(0, 44, View.Frame.Width, View.Frame.Height - _unitPicker.Frame.Height - 44);
					}, () => {
						_tableView.ScrollToActiveRow();
					});
				}
			};
			_tableView.OnKeyboardDone += (sender, args) =>
			{
				if(_pickerVisible) {
					_pickerVisible = false;
					_unitPicker.Frame = new RectangleF(0, 460, 320, 216);
				}
				_tableView.ResignTextFieldAsFirstResponder();
			};
			
			View.AddSubview(_tableView);
			
			// add unit picker
			_unitPicker = new UnitPicker(Comparison.UnitTypeId, new RectangleF(0, 460, 320, 216));
			_unitPicker.OnSelectionChanged += (sender, args) =>
			{
				_tableView.SetUnitText(_unitPicker.SelectedUnit.FullName);
			};
			View.AddSubview(_unitPicker);

			var navigationBar = new UINavigationBar(new RectangleF(0, 0, View.Frame.Width, 44));
			var navigationItem = new UINavigationItem("New Comparison");
			var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (sender, args) =>
			{
				if(!ValidateData())
				{
					return;
				}
				
				var comparable = new ComparableModel(){
					ComparisonId = ComparisonId,
					Price = Convert.ToDouble(_tableView.Price),
					Product = _tableView.Product,
					Quantity = Convert.ToDouble(_tableView.Quantity),
					Store = _tableView.Store,
					UnitId = _unitPicker.SelectedUnit.Id
				};
				
				NewComparableId = DataService.SaveComparable(comparable);
				OnFinished.Fire (this, new EventArgs ());
			});
			
			var cancelButton = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, (sender, args) =>
			{
				OnCanceled.Fire(this, EventArgs.Empty);
			});

			navigationItem.SetRightBarButtonItem (doneButton, false);
			navigationItem.SetLeftBarButtonItem(cancelButton, false);
			navigationBar.PushNavigationItem(navigationItem, false);
			View.AddSubview(navigationBar);
		}
		
		public bool ValidateData()
		{
			if(_tableView.Price == null || !_moneyRegex.IsMatch(_tableView.Price))
			{
				new UIAlertView("Invalid Price", "Enter a proper monetary amount", null, "ok").Show();
				return false;
			}

			double d;
			if(_tableView.Quantity == null || !double.TryParse(_tableView.Quantity, out d))
			{
				new UIAlertView("Invalid Quantity", "Enter a numeric quantatative value", null, "ok").Show();
				return false;
			}
			
			return true;
		}
		
		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			_tableView.SetUnitText(_unitPicker.SelectedUnit.FullName);
		}
		
		private void SetTextFieldStyle (UITextField textField)
		{
			textField.BorderStyle = UITextBorderStyle.RoundedRect;
			textField.Font = UIFont.FromName ("Helvetica", 12);
			textField.VerticalAlignment = UIControlContentVerticalAlignment.Center;
		}
				
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			_keyboardShowObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.DidShowNotification, (notification) => {
				var keyboardBounds = (NSValue)notification.UserInfo.ObjectForKey(UIKeyboard.BoundsUserInfoKey);
				var keyboardSize = keyboardBounds.RectangleFValue;
				_keyboardVisible = true;
				
				if(_pickerVisible)
				{
					_tableView.ScrollToActiveRow();
					return;
				}

				UIView.Animate(0.3, () => {
					_tableView.Frame = new RectangleF(0, 44, View.Frame.Width, View.Frame.Height - keyboardSize.Height - 44);
				}, () => {
					_tableView.ScrollToActiveRow();
				});
			});

			_keyboardHideObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, (notification) => {
				_keyboardVisible = false;
				if(_pickerVisible)
				{
					return;
				}
				_tableView.Frame = new RectangleF(0, 44 - _tableView.ContentOffset.Y, View.Frame.Width, View.Frame.Height - 44);
				UIView.Animate(0.3, () => {
					_tableView.Frame = new RectangleF(0, 44, View.Frame.Width, _tableView.Frame.Height);
				}, null);
			});
		}
		
		public override void ViewDidUnload()
		{
			base.ViewDidUnload();
			NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardHideObserver);
			NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardShowObserver);
		}
		
		public int? NewComparableId { get; private set; }
		public int ComparisonId { get; private set; }
		public ComparisonModel Comparison { get; private set; }
	}
}


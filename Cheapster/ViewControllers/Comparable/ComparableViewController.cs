using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Cheapster.Data;
using Cheapster.Data.Models;
using Cheapster.ViewControllers.Shared;

namespace Cheapster.ViewControllers.Comparable
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
		private UINavigationItem _navigationItem;
		private static Regex _moneyRegex = new Regex(@"^\$?([1-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?)$");
		
		public ComparableViewController(ComparableModel comparable)
		{
			Comparable = comparable;
			Initialize(comparable.ComparisonId);
		}
		
		/// <summary>
		/// Controller that adds/edits a comparable
		/// </summary>
		/// <param name="comparisonId">
		/// The comparison that this comparable belongs to
		/// </param>
		public ComparableViewController(int comparisonId)
		{
			Initialize(comparisonId);
		}
		
		private void Initialize(int comparisonId)
		{
			ComparisonId = comparisonId;
			Comparison = DataService.GetComparison(comparisonId);
			View.BackgroundColor = UIColor.White;
			
			// add tableview
			_tableView = new ComparableTableView(new RectangleF(0, 0, View.Frame.Width, View.Frame.Height), UITableViewStyle.Grouped, Comparable, this);
			_tableView.OnEditUnit += (sender, args) =>
			{
				_pickerVisible = true;
				if(_keyboardVisible)
				{
					_unitPicker.Frame = new RectangleF(0, View.Frame.Height - _unitPicker.Frame.Height, View.Frame.Width, 216);
					_tableView.Frame = new RectangleF(0, 0, View.Frame.Width, View.Frame.Height - _unitPicker.Frame.Height);
				}
				else
				{
					UIView.Animate(0.3, () =>
					{
						_unitPicker.Frame = new RectangleF(0, View.Frame.Height - _unitPicker.Frame.Height, View.Frame.Width, 216);
						_tableView.Frame = new RectangleF(0, 0, View.Frame.Width, View.Frame.Height - _unitPicker.Frame.Height);
					}, () => {
						_tableView.ScrollToActiveRow();
					});
				}
			};
			_tableView.OnProductNameChanged += (sender, args) =>
			{
				_navigationItem.Title = string.IsNullOrEmpty(_tableView.Product) ? "Product" : _tableView.Product;
			};
			_tableView.OnKeyboardDone += (sender, args) =>
			{
				if(_pickerVisible)
				{
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
		}
		
		private UIBarButtonItem GetDoneButton()
		{
			return new UIBarButtonItem(UIBarButtonSystemItem.Save, (sender, args) =>
			{
				if(!ValidateData())
				{
					return;
				}
				
				if(Comparable == null)
				{
					Comparable = new ComparableModel(){
						ComparisonId = ComparisonId,
						Price = Convert.ToDouble(_tableView.Price),
						Product = _tableView.Product,
						Quantity = Convert.ToDouble(_tableView.Quantity),
						Store = _tableView.Store,
						UnitId = _unitPicker.SelectedUnit.Id
					};
					
					Comparable.Id = DataService.SaveComparable(Comparable);
				}
				else
				{
					Comparable.Store = _tableView.Store;
					Comparable.Product = _tableView.Product;
					Comparable.Quantity = Convert.ToDouble(_tableView.Quantity);
					Comparable.Price = Convert.ToDouble(_tableView.Price);
					Comparable.UnitId = _unitPicker.SelectedUnit.Id;
					DataService.UpdateComparable(Comparable);
				}
				
				OnFinished.Fire (this, new EventArgs ());
			});
		}
		
		public bool ValidateData()
		{
			
			if(string.IsNullOrEmpty(_tableView.Product))
			{
				new UIAlertView("Product blank", "Enter a product/brand name", null, "ok").Show();
				return false;
			}
			
			if(string.IsNullOrEmpty(_tableView.Price) || !_moneyRegex.IsMatch(_tableView.Price))
			{
				new UIAlertView("Invalid Price", "Enter a proper monetary amount", null, "ok").Show();
				return false;
			}

			double d;
			if(string.IsNullOrEmpty(_tableView.Quantity) || !double.TryParse(_tableView.Quantity, out d))
			{
				new UIAlertView("Invalid Quantity", "Enter a numeric quantatative value", null, "ok").Show();
				return false;
			}
			
			return true;
		}
		
		private void ConfigureNavigationBar()
		{
			_navigationItem = NavigationItem;

			if(Comparable == null)
			{
				Title = "Product";
				var cancelButton = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, (sender, args) =>
				{
					OnCanceled.Fire(this, EventArgs.Empty);
				});
				
				_navigationItem.SetRightBarButtonItem(GetDoneButton(), false);
				_navigationItem.SetLeftBarButtonItem(cancelButton, false);
			}
			else
			{
				NavigationController.SetNavigationBarHidden(false, false);
				NavigationItem.RightBarButtonItem = GetDoneButton();
				Title = string.IsNullOrEmpty(Comparable.Product) ? "Product" : Comparable.Product;
			}
		}
		
		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			ConfigureNavigationBar();
			
			if(Comparable != null)
			{
				_unitPicker.SetSelectedUnit(Comparable.UnitId);
			}
			else
			{
				_unitPicker.SetSelectedUnit(Comparison.UnitId);
			}
			
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
					_tableView.Frame = new RectangleF(0, 0, View.Frame.Width, View.Frame.Height - keyboardSize.Height);
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
				_tableView.Frame = new RectangleF(0, _tableView.ContentOffset.Y, View.Frame.Width, View.Frame.Height);
				UIView.Animate(0.3, () => {
					_tableView.Frame = new RectangleF(0, 0, View.Frame.Width, _tableView.Frame.Height);
				}, null);
			});
		}
		
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			_tableView.SetUnitText(_unitPicker.SelectedUnit.FullName);
		}
		
		private void SetTextFieldStyle(UITextField textField)
		{
			textField.BorderStyle = UITextBorderStyle.RoundedRect;
			textField.Font = UIFont.FromName("Helvetica", 12);
			textField.VerticalAlignment = UIControlContentVerticalAlignment.Center;
		}
		
		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardHideObserver);
			NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardShowObserver);
		}
		
		public int ComparisonId { get; private set; }
		public ComparisonModel Comparison { get; private set; }
		public ComparableModel Comparable { get; private set; }
	}
}


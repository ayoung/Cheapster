using System;
using System.Drawing;
using MonoTouch.UIKit;
using Cheaper.Data;
using Cheaper.Data.Models;
using Cheaper.ViewControllers.Shared;

namespace Cheaper.ViewControllers.Comparison
{
	public class ComparisonViewController_ : UIViewController
	{
		private UITextField _comparisonNameText;
		private UISegmentedControl _segmentedControl;
		private UnitPicker _unitPicker;
		private ComparisonModel _comparison;
		public event EventHandler OnFinished;
		public event EventHandler OnCanceled;
		
		public ComparisonViewController_(int comparisonId)
		{
			_comparison = DataService.GetComparison(comparisonId);
			Initialize();
		}

		public ComparisonViewController_()
		{
			Initialize();
		}

		private void Initialize()
		{
			var view = new EventedView(View.Frame);
			view.OnTouchesEnded += (sender, args) =>
			{
				if(_comparisonNameText.IsFirstResponder)
				{
					_comparisonNameText.ResignFirstResponder();
				}
			};
			
			View = view;
			
			// add navigation item
			View.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
			var navigationBar = new UINavigationBar(new RectangleF(0, 0, View.Frame.Width, 44));
			navigationBar.TintColor = UIColor.DarkGray;
			var navigationItem = new UINavigationItem("New Comparison");
			var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (sender, args) =>
			{
				NewComparisonId = DataService.SaveComparison(new ComparisonModel() {
					Name = _comparisonNameText.Text,
					UnitId = _unitPicker.SelectedUnit.Id,
					UnitTypeId = _segmentedControl.SelectedSegment + 1
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
			
			// add label
			var label = new UILabel(new RectangleF(20, 55, 210, 40));
			label.Lines = 2;
			label.Font = UIFont.FromName("Helvetica", 14);
			label.Text = "What type of product do you want to start comparing?";
			label.TextAlignment = UITextAlignment.Left;
			label.BackgroundColor = UIColor.Clear;
			View.AddSubview(label);
			
			// add comparison text field
			_comparisonNameText = new UITextField(new RectangleF(20, 100, 280, 31));
			_comparisonNameText.Placeholder = "product category";
			_comparisonNameText.TextAlignment = UITextAlignment.Left;
			_comparisonNameText.BorderStyle = UITextBorderStyle.RoundedRect;
			_comparisonNameText.Font = UIFont.FromName("Helvetica", 12);
			_comparisonNameText.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			_comparisonNameText.ReturnKeyType = UIReturnKeyType.Done;
			_comparisonNameText.Delegate = new TextFieldDelegate();

			if(_comparison != null)
			{
				_comparisonNameText.Text = _comparison.Name;
			}

			View.AddSubview(_comparisonNameText);
		
			// add label
			label = new UILabel(new RectangleF(20, 135, 280, 25));
			label.Lines = 2;
			label.Font = UIFont.FromName("Helvetica", 14);
			label.Text = _comparison == null ? "Compare by:" : "Comparing by " + GetUnitTypeName();
			label.TextAlignment = UITextAlignment.Left;
			label.BackgroundColor = UIColor.Clear;
			View.AddSubview(label);
			
			if(_comparison == null)
			{
				// add segmented control
				_segmentedControl = new UISegmentedControl(new RectangleF(20, 163, 280, 40));
				_segmentedControl.InsertSegment("Weight", 0, false);
				_segmentedControl.InsertSegment("Volume", 1, false);
				_segmentedControl.InsertSegment("Each", 2, false);
				
				if(_comparison != null)
				{
					_segmentedControl.SelectedSegment = _comparison.UnitTypeId - 1;
				}
				else
				{
					_segmentedControl.SelectedSegment = 0;
				}
				
				_segmentedControl.ValueChanged += (sender, args) =>
				{
					if(_comparisonNameText.IsFirstResponder)
					{
						_comparisonNameText.ResignFirstResponder();
					}
					_unitPicker.SetUnitType(_segmentedControl.SelectedSegment + 1);
				};

				View.AddSubview(_segmentedControl);
			}
			
			// add unit picker
			_unitPicker = new UnitPicker(1, new RectangleF(0, View.Frame.Height - 216, 320, 216));
			if(_comparison != null)
			{
				_unitPicker.SetUnitType(_comparison.UnitTypeId);
				_unitPicker.SetSelectedUnit(_comparison.UnitId);
			}
			View.AddSubview(_unitPicker);
			
			// add label
			label = new UILabel(new RectangleF(20, 214, 280, 25));
			label.Lines = 2;
			label.Font = UIFont.FromName("Helvetica", 14);
			label.Text = "Use this base unit for comparisons:";
			label.TextAlignment = UITextAlignment.Left;
			label.BackgroundColor = UIColor.Clear;
			View.AddSubview(label);
		}
		
		private string GetUnitTypeName()
		{
			switch(_comparison.UnitTypeId)
			{
				case 1:
					return "weight";
				case 2:
					return "volume";
				case 3:
					return "each";
			}
			return "";
		}
		
		public int? NewComparisonId { get; private set; }
		
		private class TextFieldDelegate : UITextFieldDelegate
		{
			public override bool ShouldReturn(UITextField textField)
			{
				textField.ResignFirstResponder();
				return true;
			}
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
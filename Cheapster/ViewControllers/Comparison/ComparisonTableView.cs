using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Cheaper.ViewControllers.Shared;

namespace Cheaper.ViewControllers.Comparison
{
	public class ComparisonTableView : UITableView
	{
		private ComparisonTableViewSource _source;
		public event EventHandler OnEditUnit;
		public event EventHandler OnKeyboardDone;
		public event EventHandler OnTouchesEnded;
		public event EventHandler OnUnitTypeChanged;
		public event EventHandler OnNameChanged;
		
		public ComparisonTableView(RectangleF frame, UITableViewStyle style) : base(frame, style)
		{
			_source = new ComparisonTableViewSource(this);
			_source.OnEditUnit += (sender, args) =>
			{
				OnEditUnit.Fire(this, args);
				ResignTextFieldAsFirstResponder();
			};
			_source.OnKeyboardDone += (sender, args) =>
			{
				OnKeyboardDone.Fire(this, EventArgs.Empty);
			};
			_source.OnNameChanged += (sender, args) =>
			{
				OnNameChanged.Fire(this, EventArgs.Empty);
			};
			Source = _source;
			ScrollEnabled = false;
			AllowsSelection = false;
			
			var footerView = new UIView(new RectangleF(0, 0, Frame.Width, 34));
			footerView.Center = new PointF(Frame.Width / 2, 17);
			var footerLabel = new UILabel(new RectangleF(0, 0, footerView.Frame.Width - 100, 34));
			footerLabel.Center = new PointF(footerView.Frame.Width / 2, 17);
			footerLabel.TextAlignment = UITextAlignment.Center;
			footerLabel.Text = "Comparisons will be performed using this base unit";
			footerLabel.Lines = 2;
			footerLabel.Font = UIFont.FromName("Helvetica", 14);
			footerLabel.BackgroundColor = UIColor.Clear;
			footerLabel.TextColor = UIColor.DarkGray;
			footerLabel.ShadowColor = UIColor.White;
			footerLabel.ShadowOffset = new SizeF(0, 1);
			footerView.AddSubview(footerLabel);
			
			TableFooterView = footerView;
		}
		
		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			OnTouchesEnded.Fire(this, EventArgs.Empty);
		}
		
		public void ResignTextFieldAsFirstResponder()
		{
			var firstResponder = GetFirstResponder();
			if(firstResponder != null)
			{
				firstResponder.ResignFirstResponder();
			}
		}
		
		public UITextField GetFirstResponder()
		{
			return _source.ComparisonNameText.IsFirstResponder ? _source.ComparisonNameText : null;
		}
		
		public void SetUnitText(string text)
		{
			_source.UnitLabel.Text = text;
		}
		
		public void DisableUnitTypesExcept(int unitTypeId)
		{
			if(unitTypeId != 1)
			{
				_source.UnitTypeSegmented.SetEnabled(false, 0);
			}
			
			if(unitTypeId != 2)
			{
				_source.UnitTypeSegmented.SetEnabled(false, 1);
			}

			if(unitTypeId != 3)
			{
				_source.UnitTypeSegmented.SetEnabled(false, 2);
			}
		}
		
		public bool UnitHasFocus
		{
			get { return _source.UnitLabel.IsFirstResponder; }
		}
		
		public string ComparisonName 
		{
			get 
			{
				return _source.ComparisonNameText.Text;
			}
			set
			{
				_source.ComparisonNameText.Text = value;
			}
		}
		
		public int UnitTypeId
		{
			get
			{
				return _source.UnitTypeSegmented.SelectedSegment + 1;
			}
			set
			{
				_source.UnitTypeSegmented.SelectedSegment = value - 1;
			}
		}
		
		public void FireOnUnitTypeChanged()
		{
			OnUnitTypeChanged.Fire(this, EventArgs.Empty);
		}
	}
}


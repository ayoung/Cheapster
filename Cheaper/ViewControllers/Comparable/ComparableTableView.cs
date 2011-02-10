using System;
using System.Drawing;
using System.Linq;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Cheaper.ViewControllers.Shared;

namespace Cheaper.ViewControllers.Comparable
{
	public class ComparableTableView : UITableView
	{
		public event EventHandler OnTouchesEnded;
		public event EventHandler OnEditUnit;
		public event EventHandler OnKeyboardDone;
		private ComparableTableViewSource _tableViewSource;
		
		public ComparableTableView(RectangleF frame, UITableViewStyle style) : base(frame, style)
		{
			_tableViewSource = new ComparableTableViewSource(this);
			_tableViewSource.OnEditUnit += (sender, args) =>
			{
				OnEditUnit.Fire(this, args);
				ResignTextFieldAsFirstResponder();
			};
			_tableViewSource.OnKeyboardDone += (sender, args) =>
			{
				OnKeyboardDone.Fire(this, EventArgs.Empty);
			};
			Source = _tableViewSource;
			AllowsSelection = false;
		}
		
		public void ScrollToActiveRow()
		{
			ScrollToRow(_tableViewSource.ActiveIndexPath, UITableViewScrollPosition.Top, true);
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
			return (from t in _tableViewSource.TextFields
				where t.IsFirstResponder
				select t).FirstOrDefault();
		}
		
		public void SetUnitText(string text)
		{
			_tableViewSource.UnitLabel.Text = text;
		}
		
		public bool UnitHasFocus
		{
			get { return _tableViewSource.UnitLabel.IsFirstResponder; }
		}
		
		public string Store 
		{
			get { return _tableViewSource.StoreText.Text; }
		}
		
		public string Product
		{
			get { return _tableViewSource.ProductText.Text; }
		}
		
		public string Price
		{
			get { return _tableViewSource.PriceText.Text; }
		}
		
		public string Quantity
		{
			get { return _tableViewSource.QuantityText.Text; }
		}
	}
}
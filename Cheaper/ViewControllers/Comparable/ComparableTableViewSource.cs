using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Cheaper.ViewControllers.Shared;
using Cheaper.Data.Models;

namespace Cheaper.ViewControllers.Comparable
{
	public class ComparableTableViewSource : UITableViewSource
	{
		public UITextField StoreText { get; private set; }
		public UITextField ProductText { get; private set; }
		public UITextField PriceText { get; private set; }
		public UITextField QuantityText { get; private set; }
		public UILabel UnitLabel { get; private set; }
		public List<UITextField> TextFields { get; private set; }
		private ComparableTableView _tableView;
		private ComparableModel _comparable;
		private EventedTableViewCell _productCell;
		private EventedTableViewCell _storeCell;
		private EventedTableViewCell _priceCell;
		private EventedTableViewCell _quantityCell;
		private EventedTableViewCell _unitCell;
		public event EventHandler OnEditUnit;
		public event EventHandler OnKeyboardDone;
		
		public ComparableTableViewSource(ComparableTableView tableView) : this(tableView, null)
		{
		}
		
		public ComparableTableViewSource(ComparableTableView tableView, ComparableModel comparable)
		{
			_comparable = comparable;
			_tableView = tableView;
			TextFields = new List<UITextField>();
		}

		public override int NumberOfSections(UITableView tableView)
		{
			return 1;
		}
		
		public override int RowsInSection(UITableView tableview, int section)
		{
			return 5;
		}
		
		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			switch(indexPath.Row) {
				case (0):
					if(_productCell == null)
					{
						_productCell = new EventedTableViewCell();
						AddLabel(_productCell, "Product");
						ProductText = AddTextField(_productCell, "or brand name (optional)", indexPath, () => { return PriceText; });
						ProductText.Text = _comparable == null ? null : _comparable.Product;
						ProductText.AutocapitalizationType = UITextAutocapitalizationType.Words;
						ProductText.EditingChanged += (sender, args) => 
						{
							_tableView.FireOnProductNameChanged();
						};
					}
					return _productCell;
				case (1):
					if(_storeCell == null)
					{
						_storeCell = new EventedTableViewCell();
						AddLabel(_storeCell, "Store");
						StoreText = AddTextField(_storeCell, "store name (optional)", indexPath, () => { return ProductText; });
						StoreText.Text = _comparable == null ? null : _comparable.Store;
						StoreText.AutocapitalizationType = UITextAutocapitalizationType.Words;
					}
					return _storeCell;
				case (2):
					if(_priceCell == null)
					{
						_priceCell = new EventedTableViewCell();
						AddLabel(_priceCell, "Price");
						PriceText = AddTextField(_priceCell, "0.00", indexPath, () => { return QuantityText; });
						PriceText.KeyboardType = UIKeyboardType.DecimalPad;
						var label = new UILabel(new RectangleF(0, 10, 9, 18));
						label.TextColor = UIColor.LightGray;
						label.Font = UIFont.FromName("Helvetica", 14);
						label.Text = "$";
						PriceText.LeftView = label;
						PriceText.LeftViewMode = UITextFieldViewMode.Always;
						PriceText.Text = _comparable == null ? null : _comparable.Price.ToString();
					}
					return _priceCell;
				case (3):
					if(_quantityCell == null)
					{
						_quantityCell = new EventedTableViewCell();
						AddLabel(_quantityCell, "Quantity");
						QuantityText = AddTextField(_quantityCell, "quantity", indexPath, () => 
						{
							OnKeyboardDone.Fire(this, EventArgs.Empty);
							return null;
						});
						QuantityText.KeyboardType = UIKeyboardType.DecimalPad;
						QuantityText.ReturnKeyType = UIReturnKeyType.Done;
						QuantityText.Text = _comparable == null ? null : _comparable.Quantity.ToString();
					}
					return _quantityCell;
				case (4):
					if(_unitCell == null)
					{
						_unitCell = new EventedTableViewCell();
						AddLabel(_unitCell, "Unit");
						UnitLabel = new UILabel(new RectangleF(0, 0, _unitCell.Frame.Width - 115, 20));
						UnitLabel.Center = new PointF(200, (_unitCell.Frame.Height / 2));
						UnitLabel.Font = UIFont.FromName("Helvetica", 14);
						UnitLabel.TextColor = UIColor.DarkTextColor;
						_unitCell.AddSubview(UnitLabel);
						_unitCell.OnTouchesEnded += (sender, args) => 
						{
							ActiveIndexPath = indexPath;
							OnEditUnit.Fire(this, EventArgs.Empty);
						};
					}
					return _unitCell;
				default:
					throw new ArgumentException("Invalid row index: " + indexPath.Row);
			}
		}
		
		private UILabel AddLabel(EventedTableViewCell cell, string text)
		{
			var label = new UILabel(new RectangleF(0, 0, 80, 20));
			label.Center = new PointF(60, (cell.Frame.Height / 2) + 1);
			label.Text = text;
			label.TextAlignment = UITextAlignment.Left;
			label.Font = UIFont.FromName("Helvetica-Bold", 14);
			cell.AddSubview(label);
			return label;
		}
		
		private UITextField AddTextField(EventedTableViewCell cell, string placeholder, NSIndexPath indexPath, Func<UIResponder> getNextResponder)
		{
			var textFieldDelegate = new TextFieldDelegate(_tableView, indexPath, getNextResponder);
			textFieldDelegate.OnStartedEditing += (sender, args) =>
			{
				ActiveIndexPath = indexPath;
//				if(sender as UITextField == UnitButton) {
//					OnUnitBecomesFirstResponder.Fire(this, EventArgs.Empty);
//				}
			};

			var textField = new UITextField(new RectangleF(0, 0, cell.Frame.Width - 110, 18));
			textField.AutocapitalizationType = UITextAutocapitalizationType.None;
			textField.AutocorrectionType = UITextAutocorrectionType.No;
			textField.Center = new PointF(200, (cell.Frame.Height / 2) + 1);
			textField.Placeholder = placeholder;
			textField.Font = UIFont.FromName("Helvetica", 14);
			textField.ReturnKeyType = UIReturnKeyType.Next;
			textField.Delegate = textFieldDelegate;
			cell.AddSubview(textField);
			cell.OnTouchesEnded += (sender, args) => { textField.BecomeFirstResponder(); };
			TextFields.Add(textField);
			return textField;
		}
		
		public NSIndexPath ActiveIndexPath { get; private set; }
		
		private class TextFieldDelegate : UITextFieldDelegate
		{
			private Func<UIResponder> _getNextResponder;
			private UITableView _tableView;
			private NSIndexPath _indexPath;
			public event EventHandler OnStartedEditing;

			public TextFieldDelegate(UITableView tableView, NSIndexPath indexPath, Func<UIResponder> getNextResponder)
			{
				_getNextResponder = getNextResponder;
				_tableView = tableView;
				_indexPath = indexPath;
			}

			public override void EditingStarted(UITextField textField)
			{
				OnStartedEditing.Fire(this, EventArgs.Empty);
				_tableView.ScrollToRow(_indexPath, UITableViewScrollPosition.Top, true);
			}

			public override bool ShouldReturn(UITextField textField)
			{
				var nextResponder = _getNextResponder();
				
				if(nextResponder != null) {
					nextResponder.BecomeFirstResponder();
				}
				
				return true;
			}
		}
		
		private class EventedTableViewCell : UITableViewCell
		{
			public event EventHandler OnTouchesEnded;

			public EventedTableViewCell()
			{
			}

			public override void TouchesEnded(MonoTouch.Foundation.NSSet touches, UIEvent evt)
			{
				OnTouchesEnded.Fire(this, EventArgs.Empty);
			}
		}
	}
}
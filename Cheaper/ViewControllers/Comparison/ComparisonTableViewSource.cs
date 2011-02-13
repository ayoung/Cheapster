using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Cheaper.ViewControllers.Shared;

namespace Cheaper.ViewControllers.Comparison
{
	public class ComparisonTableViewSource : UITableViewSource
	{
		public event EventHandler OnEditUnit;
		public event EventHandler OnKeyboardDone;
		public UITextField ComparisonNameText { get; private set; }
		public EventedSegmentedControl UnitTypeSegmented { get; private set; }
		public UILabel UnitLabel { get; private set; }
		private ComparisonTableView _tableView;
		
		public ComparisonTableViewSource(ComparisonTableView tableView)
		{
			_tableView = tableView;
		}
		
		public override int NumberOfSections(UITableView tableView)
		{
			return 1;
		}
		
		public override int RowsInSection(UITableView tableview, int section)
		{
			return 3;
		}
		
		public override float GetHeightForFooter(UITableView tableView, int section)
		{
			return 24;
		}
		
		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			EventedTableViewCell cell;
			switch(indexPath.Row) {
				case (0):
					cell = new EventedTableViewCell();
					AddLabel(cell, "Name");
					ComparisonNameText = AddTextField(cell, "comparison name (i.e. Apples)", indexPath, () =>
					{
						OnKeyboardDone.Fire(this, EventArgs.Empty);
						return null;
					});
					ComparisonNameText.ReturnKeyType = UIReturnKeyType.Done;
					break;
				case (1):
					cell = new EventedTableViewCell();
					AddLabel(cell, "Compare by");
					// add segmented control
					UnitTypeSegmented = new EventedSegmentedControl(new RectangleF(120, 7, 180, 30));
					UnitTypeSegmented.ControlStyle = UISegmentedControlStyle.Bar;
					UnitTypeSegmented.InsertSegment("Weight", 0, false);
					UnitTypeSegmented.InsertSegment("Volume", 1, false);
					UnitTypeSegmented.InsertSegment("Each", 2, false);
					UnitTypeSegmented.SelectedSegment = 0;
					
					UnitTypeSegmented.ValueChanged += (sender, args) =>
					{
						_tableView.FireOnUnitTypeChanged();
					};
					
					UnitTypeSegmented.OnTouchesEnded += (sender, args) =>
					{
						if(ComparisonNameText.IsFirstResponder) {
							ComparisonNameText.ResignFirstResponder();
						}
					};
					
					cell.AddSubview(UnitTypeSegmented);
					break;
				case (2):
					cell = new EventedTableViewCell();
					AddLabel(cell, "Base unit");
					UnitLabel = new UILabel(new RectangleF(0, 0, cell.Frame.Width - 115, 20));
					UnitLabel.Center = new PointF(200, (cell.Frame.Height / 2) + 1);
					UnitLabel.Text = "ounces";
					UnitLabel.Font = UIFont.FromName("Helvetica", 14);
					UnitLabel.TextColor = UIColor.DarkTextColor;
					cell.AddSubview(UnitLabel);
					cell.OnTouchesEnded += (sender, args) =>
					{
						ActiveIndexPath = indexPath;
						OnEditUnit.Fire(this, EventArgs.Empty);
					};
					
					break;
				default:
					throw new ArgumentException("Invalid row index: " + indexPath.Row);
			}
			return cell;
		}
		
		private UILabel AddLabel(EventedTableViewCell cell, string text)
		{
			var label = new UILabel(new RectangleF(0, 0, 100, 20));
			label.Center = new PointF(70, (cell.Frame.Height / 2) + 1);
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
			cell.OnTouchesEnded += (sender, args) =>
			{
				ActiveIndexPath = indexPath;
				OnEditUnit.Fire(this, EventArgs.Empty);
			};
			cell.OnTouchesEnded += (sender, args) => { textField.BecomeFirstResponder(); };
			return textField;
		}
		
		public NSIndexPath ActiveIndexPath { get; private set; }
		
		private class TextFieldDelegate : UITextFieldDelegate
		{
			private UITableView _tableView;
			private NSIndexPath _indexPath;
			public event EventHandler OnStartedEditing;

			public TextFieldDelegate(UITableView tableView, NSIndexPath indexPath, Func<UIResponder> getNextResponder)
			{
				
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
				textField.ResignFirstResponder();
				return true;
			}
		}
		
		private class UnitTextFieldDelegate : TextFieldDelegate
		{
			public UnitTextFieldDelegate(UITableView tableView, NSIndexPath indexPath, Func<UIResponder> getNextResponder)
				: base(tableView, indexPath, getNextResponder)
			{
			}
			
			public override bool ShouldBeginEditing(UITextField textField)
			{
				return false;
			}
		}
		
		private class EventedTableViewCell : UITableViewCell
		{
			public event EventHandler OnTouchesEnded;
			
			public EventedTableViewCell()
			{
			}
			
			public EventedTableViewCell(UITableViewCellStyle style) : base(style, "")
			{
			}

			public override void TouchesEnded(MonoTouch.Foundation.NSSet touches, UIEvent evt)
			{
				OnTouchesEnded.Fire(this, EventArgs.Empty);
			}
		}
		
		public class EventedSegmentedControl : UISegmentedControl
		{
			public event EventHandler OnTouchesEnded;
			
			public EventedSegmentedControl(RectangleF frame) : base(frame)
			{
			
			}
			
			public override void TouchesEnded(NSSet touches, UIEvent evt)
			{
				base.TouchesEnded(touches, evt);
				OnTouchesEnded.Fire(this, EventArgs.Empty);
			}
		}
	}
}


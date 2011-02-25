using System;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace Cheaper.ViewControllers
{
	public class MoreTableView : UITableView
	{
		private AboutViewController _controller;

		public MoreTableView(AboutViewController controller, RectangleF frame) : base(frame, UITableViewStyle.Grouped)
		{
			_controller = controller;
			Source = new TableViewSource();
			BackgroundColor = UIColor.Clear;
			ScrollEnabled = false;
		}

		public void FireOnRateThisApp()
		{
			_controller.FireOnRateThisApp();
		}

		public void FireOnFeedback()
		{
			_controller.FireOnFeedback();
		}

		public void FireOnTwitter()
		{
			_controller.FireOnTwitter();
		}

		private class TableViewSource : UITableViewSource
		{
			public override int RowsInSection(UITableView tableview, int section)
			{
				return 1;
			}

			public override int NumberOfSections(UITableView tableView)
			{
				return 3;
			}

			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				var cell = new UITableViewCell(UITableViewCellStyle.Subtitle, "more");
				switch(indexPath.Section)
				{
					case 0:
						cell.TextLabel.Text = "Rate this app";
						cell.DetailTextLabel.Text = "Rate it on the App Store.";
						cell.DetailTextLabel.TextAlignment = UITextAlignment.Left;
						break;
					case 1:
						cell.TextLabel.Text = "Submit feedback & bugs";
						cell.DetailTextLabel.Text = "Tell us what you think.";
						cell.DetailTextLabel.TextAlignment = UITextAlignment.Left;
						break;
					case 2:
						cell.TextLabel.Text = "@cheaperapp";
						cell.DetailTextLabel.Text = "Follow. Tell us how you got it cheaper.";
						cell.DetailTextLabel.TextAlignment = UITextAlignment.Left;
						cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
						break;
					default:
						throw new ArgumentException("Invalid section");
				}
				
				return cell;
			}
			
			public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
			{
				return 60;
			}

			public override UIView GetViewForHeader(UITableView tableView, int section)
			{
				return new UIView(new RectangleF(0, 0, tableView.Frame.Width, 1));
			}

			public override float GetHeightForHeader(UITableView tableView, int section)
			{
				if(section == 0)
					return 10;
				return 6;
			}

			public override UIView GetViewForFooter(UITableView tableView, int section)
			{
				return new UIView(new RectangleF(0, 0, tableView.Frame.Width, 1));
			}

			public override float GetHeightForFooter(UITableView tableView, int section)
			{
				return 6;
			}

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				var moreTable = (MoreTableView)tableView;
				
				switch(indexPath.Section)
				{
					case 0:
						moreTable.FireOnRateThisApp();
						tableView.DeselectRow(tableView.IndexPathForSelectedRow, true);
						break;
					case 1:
						moreTable.FireOnFeedback();
						break;
					case 2:
						moreTable.FireOnTwitter();
						break;
					default:
						throw new ArgumentException("Invalid section");
				}
			}
		}
	}
}


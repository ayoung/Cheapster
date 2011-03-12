using System;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace Cheapster.ViewControllers
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

		public void FireOnBackupData()
		{
			_controller.FireOnBackupData();
		}

		public void FireOnTwitter()
		{
			_controller.FireOnTwitter();
		}

		private class TableViewSource : UITableViewSource
		{
			public override int RowsInSection(UITableView tableview, int section)
			{
				if(section == 0)
				{
					return 3;
				}
				return 1;
			}

			public override int NumberOfSections(UITableView tableView)
			{
				return 2;
			}

			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				var cell = new UITableViewCell(UITableViewCellStyle.Subtitle, "more");
				if(indexPath.Section == 0)
				{
					switch(indexPath.Row)
					{
						case 0:
							cell.TextLabel.Text = "Rate this app";
							cell.DetailTextLabel.Text = "Rate it on the App Store.";
							cell.DetailTextLabel.TextAlignment = UITextAlignment.Left;
							cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
							break;
						case 1:
							cell.TextLabel.Text = "heycheapster@gmail.com";
							cell.DetailTextLabel.Text = "Tell us what you think - feedback, bugs";
							cell.DetailTextLabel.TextAlignment = UITextAlignment.Left;
							cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
							break;
						case 2:
							cell.TextLabel.Text = "@cheapsterapp";
							cell.DetailTextLabel.Text = "Follow. Tell us how you got it cheaper.";
							cell.DetailTextLabel.TextAlignment = UITextAlignment.Left;
							cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
							break;
						default:
							throw new ArgumentException("Invalid section");
					}
				}
				else
				{
					switch(indexPath.Row)
					{
						case 0:
							cell.TextLabel.Text = "Email your data";
							cell.DetailTextLabel.Text = "Back it up. Send it to yourself or share it with a friend.";
							cell.DetailTextLabel.TextAlignment = UITextAlignment.Left;
							var frame = cell.DetailTextLabel.Frame;
							cell.DetailTextLabel.Frame = new RectangleF(frame.X, frame.Y, frame.Width, frame.Height + 15);
							cell.DetailTextLabel.Lines = 2;
							cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
							break;
						default:
							throw new ArgumentException("Invalid section");
					}
				}
				
				return cell;
			}
			
			public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
			{
				if(indexPath.Section == 0)
				{
					return 60;
				}
				
				return 75;
			}
			
			public override UIView GetViewForHeader(UITableView tableView, int section)
			{
				var headerView = new UIView(new RectangleF(0, 0, tableView.Frame.Width, 30));
				var headerLabel = new UILabel(new RectangleF(14, section == 0 ? 10 : 0, tableView.Frame.Width - 20, 25));
				headerLabel.BackgroundColor = UIColor.Clear;
				headerLabel.Font = UIFont.FromName("Helvetica-Bold", 16);
				headerLabel.TextColor = UIColor.White;
				headerLabel.ShadowColor = UIColor.Black;
				headerLabel.ShadowOffset = new SizeF(0, 1);
				
				if(section == 0)
				{
					headerLabel.Text = "Hey Cheapster!";
				}
				else
				{
					headerLabel.Text = "Data transfer";
				}
				
				headerView.AddSubview(headerLabel);
				return headerView;
			}
			
			public override float GetHeightForHeader(UITableView tableView, int section)
			{
				if(section == 0)
				{
					return 40;
				}
				return 30;
			}

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				var moreTable = (MoreTableView)tableView;
				if(indexPath.Section == 0)
				{
					switch(indexPath.Row)
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
							tableView.DeselectRow(tableView.IndexPathForSelectedRow, true);
							break;
						default:
							throw new ArgumentException("Invalid section");
					}
				}
				else
				{
					switch(indexPath.Row)
					{
						case 0:
							moreTable.FireOnBackupData();
							break;
						default:
							throw new ArgumentException("Invalid section");
					}
				}
			}
		}
	}
}


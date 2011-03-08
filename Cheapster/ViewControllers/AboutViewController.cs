using System;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Cheapster.ViewControllers.Shared;

namespace Cheapster.ViewControllers
{
	public class AboutViewController : UIViewController
	{
		public event EventHandler OnDone;
		private MoreTableView _tableView;
		public event EventHandler OnRateThisApp;
		public event EventHandler OnFeedback;
		public event EventHandler OnTwitter;
		public event EventHandler OnBackupData;

		public AboutViewController()
		{
		}

		public void FireOnRateThisApp()
		{
			OnRateThisApp.Fire(this, EventArgs.Empty);
		}
		
		public void FireOnBackupData()
		{
			OnBackupData.Fire(this, EventArgs.Empty);
		}

		public void FireOnFeedback()
		{
			OnFeedback.Fire(this, EventArgs.Empty);
		}

		public void FireOnTwitter()
		{
			OnTwitter.Fire(this, EventArgs.Empty);
		}
		
		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			NavigationController.SetNavigationBarHidden(true, false);
		}
		
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			_tableView.DeselectRow(_tableView.IndexPathForSelectedRow, true);
		}
		
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			
			View.BackgroundColor = UIColor.ScrollViewTexturedBackgroundColor;
			var navigationBar = new UINavigationBar(new RectangleF(0, 0, View.Frame.Width, 44));
			navigationBar.BarStyle = UIBarStyle.Black;
			//navigationBar.TintColor = UIColor.DarkGray;
			var navigationItem = new UINavigationItem("Extras");
			var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (sender, args) =>
			{
				OnDone.Fire(this, EventArgs.Empty);
			});
			
			navigationItem.SetRightBarButtonItem(doneButton, false);
			navigationBar.PushNavigationItem(navigationItem, false);
			View.AddSubview(navigationBar);
			
			_tableView = new MoreTableView(this, new RectangleF(0, 44, View.Frame.Width, View.Frame.Height - 44));
			View.AddSubview(_tableView);
		}
	}
}


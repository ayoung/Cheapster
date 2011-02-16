using System;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Cheaper.ViewControllers.Shared;

namespace Cheaper.ViewControllers
{
	public class AboutViewController : UIViewController
	{
		public event EventHandler OnDone;
		public AboutViewController()
		{
		}
		
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			View.BackgroundColor = UIColor.ViewFlipsideBackgroundColor;
			var navigationBar = new UINavigationBar(new RectangleF(0, 0, View.Frame.Width, 44));
			navigationBar.BarStyle = UIBarStyle.Black;
			//navigationBar.TintColor = UIColor.DarkGray;
			var navigationItem = new UINavigationItem("Cheaper");
			var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (sender, args) =>
			{
				OnDone.Fire(this, EventArgs.Empty);
			});
			
			navigationItem.SetRightBarButtonItem(doneButton, false);
			navigationBar.PushNavigationItem(navigationItem, false);
			View.AddSubview(navigationBar);
		}
	}
}


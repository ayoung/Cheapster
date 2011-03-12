using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Cheapster.Support;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Cheapster.ViewControllers;
using Cheapster.ViewControllers.Shared;

namespace Cheapster
{
	public class Application
	{
		static void Main (string[] args)
		{
			UIApplication.Main (args);
		}
	}

	// The name AppDelegate is referenced in the MainWindow.xib file.
	public partial class AppDelegate : UIApplicationDelegate
	{
		private HomeListNavigationController _homeListNavigationController;
		private UIAlertView _restoreAlertView;
				
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching(UIApplication application, NSDictionary options)
		{
			Installation.MigrateDb();
			_homeListNavigationController = new HomeListNavigationController();
			window.AddSubview(_homeListNavigationController.View);
			window.BackgroundColor = UIColor.Black;
			window.MakeKeyAndVisible();
			Window = window;
			Console.WriteLine("FinishedLaunching");
			return true;
		}
		
		public override void OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			Console.WriteLine("OpenUrl");
			var fileName = Path.GetFileName(url.AbsoluteString);
			
			_restoreAlertView = new UIAlertView("Restore Backup", 
				"Restoring will wipe all of your existing data. Continue?",
				new RestoreAlertViewDelegate(url, _homeListNavigationController), "Cancel", "Restore");
			_restoreAlertView.Show();
			AppState.Current.RestoreDbPath = url.AbsoluteString;
		}
		
		public override void HandleOpenURL(UIApplication application, NSUrl url)
		{
			Console.WriteLine("HandleOpenUrl");
			Console.WriteLine(url.AbsoluteString);
		}
			
		public static UIWindow Window { get; private set; }

		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
		}
	}
}


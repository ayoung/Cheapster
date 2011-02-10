using System.Collections.Generic;
using System.Linq;
using Cheaper.Support;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Cheaper.ViewControllers;

namespace Cheaper
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
		private HomeListNavigationController _shoppingListNavigationController;
				
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			Installation.MigrateDb();
			
			_shoppingListNavigationController = new HomeListNavigationController();
			window.AddSubview(_shoppingListNavigationController.View);
			window.MakeKeyAndVisible();
			Window = window;
			return true;
		}
		
		public static UIWindow Window { get; private set; }

		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
		}
	}
}


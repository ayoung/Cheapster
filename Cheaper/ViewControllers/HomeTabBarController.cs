using System;
using MonoTouch.UIKit;

namespace Cheaper.ViewControllers
{
	public class HomeTabBarController : UITabBarController
	{
		private HomeListViewController _shoppingListController;
		
		public HomeTabBarController ()
		{
		}
		
		public override void ViewDidLoad ()
		{
			_shoppingListController = new HomeListViewController ();
			
			ViewControllers = new UIViewController[] {
				_shoppingListController
			};
			
			base.ViewDidLoad ();
		}
	
	}
}


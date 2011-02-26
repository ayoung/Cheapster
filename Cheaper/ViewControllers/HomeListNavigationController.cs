using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.MessageUI;
using Cheaper.ViewControllers.Comparable;
using Cheaper.ViewControllers.Comparison;
using Cheaper.ViewControllers.Shared;

namespace Cheaper.ViewControllers
{
	public class HomeListNavigationController : UINavigationController
	{
		private HomeListViewController _homeListViewController;
		private ComparableViewController _comparableViewController;
		private ComparisonViewController _comparisonViewController;
		private ComparisonLineupViewController _comparisonLineupViewController;
		private UINavigationController _aboutNavigationController;
		private AboutViewController _aboutViewController;
		private MFMailComposeViewController _emailController;
		private WebViewController _webViewController;
		private const string _urlFormat = "itms-apps://ax.itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id={0}";
		
		public override void ViewDidLoad()
		{
			_homeListViewController = new HomeListViewController();
			
			NavigationBar.TintColor = UIColor.DarkGray;
			
			// go to new comparison view if add button is touched
			_homeListViewController.OnAddComparison += (sender, args) =>
			{
				_comparisonViewController = new ComparisonViewController();
				
				// when a new comparison is added
				// select the created comparison in the shopping list controller
				_comparisonViewController.OnFinished += (sender_, args_) =>
				{
					if(_comparisonViewController.Comparison == null)
					{
						throw new Exception("New comparison id was not set.");
					}
					
					_homeListViewController.EnableTrashButton();
					_homeListViewController.AddComparison(_comparisonViewController.Comparison);
					DismissModalViewControllerAnimated(true);
					_comparableViewController = null;
				};
				
				_comparisonViewController.OnCanceled += (sender_, args_) =>
				{
					DismissModalViewControllerAnimated(true);
					_comparisonViewController = null;
				};
				
				PresentModalViewController(_comparisonViewController, true);
			};
			
			// go to the created comparison's lineup view when a comparison is selected
			_homeListViewController.OnComparisonSelected += (sender, args) =>
			{
				_comparisonLineupViewController = new ComparisonLineupViewController(_homeListViewController.SelectedComparison);
				
				// new comparable
				_comparisonLineupViewController.OnAddComparable += (sender_, args_) =>
				{
					_comparableViewController = new ComparableViewController(_comparisonLineupViewController.ComparisonId);
					_comparableViewController.OnFinished += (sender__, args__) =>
					{
						_comparisonLineupViewController.EnableTrashButton();
						_comparisonLineupViewController.AddComparable(_comparableViewController.Comparable);
						DismissModalViewControllerAnimated(true);
						_comparableViewController = null;
					};
					_comparableViewController.OnCanceled += (sender__, args__) =>
					{
						DismissModalViewControllerAnimated(true);
						_comparableViewController = null;
					};
					PresentModalViewController(_comparableViewController, true);
				};
				
				// edit comparable
				_comparisonLineupViewController.OnComparableSelected += (sender_, args_) =>
				{
					_comparableViewController = new ComparableViewController(_comparisonLineupViewController.GetSelectedComparable());
					_comparableViewController.OnFinished += (sender__, args__) =>
					{
						// this fixes the bug where changing a comparison name
						_homeListViewController.ReloadRowForComparison(_comparisonLineupViewController.ComparisonId);
						
						_comparisonLineupViewController.RepositionRowForComparable(_comparisonLineupViewController.GetSelectedComparable().Id);
						PopViewControllerAnimated(true);
						_comparableViewController = null;
					};
					_comparableViewController.OnCanceled += (sender__, args__) =>
					{
						PopViewControllerAnimated(true);
						_comparableViewController = null;
					};
					PushViewController(_comparableViewController, true);
				};
				
				// edit comparison
				_comparisonLineupViewController.OnModify += (sender_, args_) =>
				{
					_comparisonViewController = new ComparisonViewController(_homeListViewController.SelectedComparison);
					_comparisonViewController.OnCanceled += (sender__, args__) =>
					{
						DismissModalViewControllerAnimated(true);
					};
					
					_comparisonViewController.OnFinished += (sender__, args__) =>
					{
						_comparisonLineupViewController.ReloadOnAppeared();
						_homeListViewController.ReloadRowForComparison(_comparisonLineupViewController.ComparisonId);
						DismissModalViewControllerAnimated(true);
					};
					PresentModalViewController(_comparisonViewController, true);
				};
				
				_comparisonLineupViewController.OnNewCheaper += (sender__, args__) =>
				{
					_homeListViewController.ReloadRowForComparison(_comparisonLineupViewController.ComparisonId);
				};
				
				PushViewController(_comparisonLineupViewController, true);
			};
			
			_homeListViewController.OnInfoButton += (sender, args) =>
			{
				_aboutViewController = new AboutViewController();
				_aboutViewController.OnDone += (sender_, args_) =>
				{
					DismissModalViewControllerAnimated(true);
					_aboutViewController = null;
					_aboutNavigationController = null;
					_emailController = null;
					_webViewController = null;
				};
				_aboutViewController.OnFeedback += (sender_, args_) =>
				{
					_emailController = new MFMailComposeViewController();
					_emailController.SetToRecipients(new string[] { "cheaperapp@gmail.com" });
					_emailController.SetSubject("Feedback and bugs");
					_emailController.Finished += (sender__, args__) =>
					{
						if(args__.Result == MFMailComposeResult.Sent)
						{
							new UIAlertView("Thank you", "We appreciate your feedback and you'll hear back from us soon!", null, "Ok").Show();
						}
						_aboutViewController.DismissModalViewControllerAnimated(true);
					};

					_aboutViewController.PresentModalViewController(_emailController, true);
				};
				_aboutViewController.OnRateThisApp += (sender__, args__) =>
				{
					var url = string.Format(_urlFormat, "375611783");
					UIApplication.SharedApplication.OpenUrl(NSUrl.FromString(url));
				};
				_aboutViewController.OnTwitter += (sender__, args__) =>
				{
					UIApplication.SharedApplication.OpenUrl(NSUrl.FromString("http://twitter.com/cheaperapp"));
				};
				_aboutViewController.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;
				
				_aboutNavigationController = new UINavigationController();
				_aboutNavigationController.PushViewController(_aboutViewController, false);
				
				PresentModalViewController(_aboutNavigationController, true);
			};

			PushViewController (_homeListViewController, true);
			base.ViewDidLoad ();
		}
	}
}


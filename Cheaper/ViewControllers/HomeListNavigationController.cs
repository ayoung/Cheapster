using System;
using MonoTouch.UIKit;
using Cheaper.ViewControllers.Comparable;
using Cheaper.ViewControllers.Comparison;

namespace Cheaper.ViewControllers
{
	public class HomeListNavigationController : UINavigationController
	{
		private HomeListViewController _homeListViewController;
		private ComparableViewController _comparableViewController;
		private ComparisonViewController _comparisonViewController;
		private ComparisonLineupViewController _comparisonLineupViewController;
		private AboutViewController _aboutViewController;
		
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
				
				// go to new comparable view if add button is touched
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
				
				// go to edit comparable view when a comparable is selected
				_comparisonLineupViewController.OnComparableSelected += (sender_, args_) =>
				{
					_comparableViewController = new ComparableViewController(_comparisonLineupViewController.GetSelectedComparable());
					_comparableViewController.OnFinished += (sender__, args__) =>
					{
						//_comparisonLineupViewController.ReloadOnAppeared();
						_comparisonLineupViewController.ReloadRowForComparable(_comparisonLineupViewController.GetSelectedComparable().Id);
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
				
				PushViewController(_comparisonLineupViewController, true);
			};
			
			_homeListViewController.OnInfoButton += (sender, args) =>
			{
				_aboutViewController = new AboutViewController();
				_aboutViewController.OnDone += (sender_, args_) =>
				{
					DismissModalViewControllerAnimated(true);
					_aboutViewController = null;
				};
				_aboutViewController.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;
				PresentModalViewController(_aboutViewController, true);
			};

			PushViewController (_homeListViewController, true);
			base.ViewDidLoad ();
		}
	}
}


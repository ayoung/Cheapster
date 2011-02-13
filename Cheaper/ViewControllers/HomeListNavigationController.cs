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
					if(!_comparisonViewController.NewComparisonId.HasValue)
					{
						throw new Exception("New comparison id was not set.");
					}
					
					_homeListViewController.SelectComparisonOnViewDidAppear(_comparisonViewController.NewComparisonId.Value);
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
				_comparisonLineupViewController = new ComparisonLineupViewController(_homeListViewController.SelectedComparisonId.Value);
				
				// go to new comparable view if add button is touched
				_comparisonLineupViewController.OnAddComparable += (sender_, args_) =>
				{
					_comparableViewController = new ComparableViewController(_comparisonLineupViewController.ComparisonId);
					_comparableViewController.OnFinished += (sender__, args__) =>
					{
						_comparisonLineupViewController.Reload();
						DismissModalViewControllerAnimated(true);
					};
					_comparableViewController.OnCanceled += (sender__, args__) =>
					{
						DismissModalViewControllerAnimated(true);
					};
					PresentModalViewController (_comparableViewController, true);
				};
				
				// go to edit comparable view if a comparable is selected
				_comparisonLineupViewController.OnComparableSelected += (sender_, args_) =>
				{
					// todo
				};
				
				_comparisonLineupViewController.OnModify += (sender_, args_) =>
				{
					_comparisonViewController = new ComparisonViewController(_homeListViewController.SelectedComparisonId.Value);
					_comparisonViewController.OnCanceled += (sender__, args__) =>
					{
						DismissModalViewControllerAnimated(true);
					};
					
					_comparisonViewController.OnFinished += (sender__, args__) =>
					{
						_comparisonLineupViewController.Reload();
						
						DismissModalViewControllerAnimated(true);
					};
					PresentModalViewController(_comparisonViewController, true);
				};
				
				PushViewController (_comparisonLineupViewController, true);
			};

			PushViewController (_homeListViewController, true);
			base.ViewDidLoad ();
		}
	}
}


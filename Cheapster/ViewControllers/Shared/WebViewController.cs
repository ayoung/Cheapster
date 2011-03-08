using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Cheapster.ViewControllers.Shared
{
	public class WebViewController : UIViewController
	{
		private UIWebView _webView;
		private Uri _uri;
		
		public WebViewController(Uri uri)
		{
			_uri = uri;
		}
		
		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			NavigationController.SetNavigationBarHidden(false, true);
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			
			_webView = new UIWebView(new RectangleF(0, 0, View.Frame.Width, View.Frame.Height - 44));
			_webView.LoadStarted += (sender, args) => { };
			_webView.LoadFinished += (sender, args) => { };
			_webView.LoadError += (sender, args) => { };
			View.AddSubview(_webView);
			_webView.LoadRequest(new NSUrlRequest(NSUrl.FromString(_uri.ToString())));
		}
	}
}

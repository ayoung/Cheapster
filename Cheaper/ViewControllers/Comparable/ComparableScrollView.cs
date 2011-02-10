using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Cheaper.ViewControllers.Comparable
{
	public class ComparableScrollView : UIScrollView
	{
		public ComparableScrollView (RectangleF frame) : base (frame)
		{
		
		}
		
		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			Superview.TouchesEnded (touches, evt);
		}
	}
}


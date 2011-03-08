
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Cheaper
{
	public partial class TestViewController : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public TestViewController (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public TestViewController (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public TestViewController () : base("TestViewController", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		#endregion
	}
}


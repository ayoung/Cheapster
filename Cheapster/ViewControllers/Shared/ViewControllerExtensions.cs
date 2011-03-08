using System;
namespace Cheapster.ViewControllers.Shared
{
	public static class ViewControllerExtensions
	{
		public static void Fire(this EventHandler handler, object sender, EventArgs args)
		{
			if(handler != null) {
				handler(sender, args);
			}
		}
	}
}


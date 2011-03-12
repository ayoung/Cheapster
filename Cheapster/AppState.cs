using System;

namespace Cheapster
{
	public class AppState
	{
		public static AppState Current { get; private set; }
		
		static AppState()
		{
			Current = new AppState();
		}
		
		public AppState()
		{
		}
		
		public string RestoreDbPath { get; set; }
	}
}


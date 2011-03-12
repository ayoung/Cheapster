using System;
using System.IO;

namespace Cheapster.Support
{
	public static class Installation
	{
		public static void MigrateDb()
		{
			if(!File.Exists(Configuration.USER_DB_INSTALLED_PATH))
			{
				File.Copy(Configuration.USER_DB_ORIGINAL_PATH, Configuration.USER_DB_INSTALLED_PATH, true);
			}
		}
		
		public static void ResetData()
		{
			File.Copy(Configuration.USER_DB_ORIGINAL_PATH, Configuration.USER_DB_INSTALLED_PATH, true);
		}
	}
}


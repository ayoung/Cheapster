using System;
using System.IO;

namespace Cheaper.Support
{
	public static class Installation
	{
		public static void MigrateDb()
		{
			if(!File.Exists(Configuration.DB_INSTALLED_PATH)) {
				File.Copy(Configuration.DB_ORIGINAL_PATH, Configuration.DB_INSTALLED_PATH, true);
			}
		}
	}
}


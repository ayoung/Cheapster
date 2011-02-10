using System;
using System.IO;
namespace Cheaper
{
	public static class Configuration
	{
		private const string _dbFilename = "Cheaper.db3";
		private const string _dbPath = "Content/Database";
		public static readonly string DB_ORIGINAL_PATH;
		public static readonly string DB_INSTALLED_PATH;

		static Configuration()
		{
			DB_ORIGINAL_PATH = Path.Combine(ToAbsolutePath(_dbPath), Configuration._dbFilename);
			DB_INSTALLED_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), _dbFilename);
		}

		/// <summary>
		/// Returns an absolute path given a path relative to the project root
		/// </summary>
		/// <param name="relativePath">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		public static string ToAbsolutePath(string relativePath)
		{
			return Path.Combine (Environment.CurrentDirectory, relativePath);
		}
	}
}


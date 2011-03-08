using System;
using System.IO;
namespace Cheapster
{
	public static class Configuration
	{
		private const string _dbPath = "Content/Database";
		public const string DB_FILENAME = "Cheapster.db3";
		public const string DB_TEMP_BACKUP_FILENAME = "backup.zip";
		public static readonly string DB_ORIGINAL_PATH;
		public static readonly string DB_INSTALLED_PATH;
		public static readonly string DB_TEMP_BACKUP_PATH;

		static Configuration()
		{
			DB_ORIGINAL_PATH = Path.Combine(ToAbsolutePath(_dbPath), Configuration.DB_FILENAME);
			DB_INSTALLED_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), DB_FILENAME);
			DB_TEMP_BACKUP_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), DB_TEMP_BACKUP_FILENAME);
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


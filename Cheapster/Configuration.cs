using System;
using System.IO;
namespace Cheapster
{
	public static class Configuration
	{
		private const string _dbPath = "Content/Database";
		public const string USER_DB_FILENAME = "UserData.db3";
		public const string APP_DB_FILENAME = "AppData.db3";
		public const string DB_TEMP_BACKUP_FILENAME = "backup.zip";
		public const string DB_TEMP_RESTORE_FILENAME = "restore.zip";
		public static readonly string USER_DB_ORIGINAL_PATH;
		public static readonly string USER_DB_INSTALLED_PATH;
		public static readonly string USER_DB_TEMP_BACKUP_PATH;
		public static readonly string TEMP_FOLDER;
		public static readonly string APP_DB_PATH;

		static Configuration()
		{
			TEMP_FOLDER = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "temp");
			USER_DB_ORIGINAL_PATH = Path.Combine(ToAbsolutePath(_dbPath), Configuration.USER_DB_FILENAME);
			USER_DB_INSTALLED_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), USER_DB_FILENAME);
			USER_DB_TEMP_BACKUP_PATH = Path.Combine(TEMP_FOLDER, DB_TEMP_BACKUP_FILENAME);
			APP_DB_PATH = Path.Combine(ToAbsolutePath(_dbPath), Configuration.APP_DB_FILENAME);
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


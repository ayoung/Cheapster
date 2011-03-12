using System;
using System.Collections.Generic;
using System.IO;
using Mono.Data.Sqlite;
using Cheapster;

namespace Cheapster.Data
{
	static internal class SqlConnection
	{
		private static readonly string ATTACH_COMMAND;
		
		static SqlConnection()
		{
			ATTACH_COMMAND = string.Format("attach database '{0}' as app;", Configuration.APP_DB_PATH);
		}

		public static void ReaderWithCommand(string commandString, Action<SqliteDataReader> block)
		{
			ReaderWithCommand(commandString, null, block);
		}
		
		public static void ReaderWithCommand(string commandString, Dictionary<string, object> parameters, Action<SqliteDataReader> block)
		{
			using(var connection = SqlConnection.NewConnection())
			{
				connection.Open();
				using(var attachCommand = connection.CreateCommand())
				{
					attachCommand.CommandText = ATTACH_COMMAND;
					attachCommand.ExecuteNonQuery();
				}
				
				using(var command = connection.CreateCommand()) {
					command.CommandText = commandString;
					FillParameters(parameters, command);
					
					using(var reader = command.ExecuteReader()) {
						block(reader);
					}
				}
			}
		}
		
		public static int ExecuteNonQuery(string commandString, Dictionary<string, object> parameters)
		{
			using(var connection = SqlConnection.NewConnection())
			{
				connection.Open();
				using(var attachCommand = connection.CreateCommand())
				{
					attachCommand.CommandText = ATTACH_COMMAND;
					attachCommand.ExecuteNonQuery();
				}
				
				using(var command = connection.CreateCommand())
				{
					command.CommandText = commandString;
					FillParameters(parameters, command);
					return command.ExecuteNonQuery();
				}
			}
		}

		public static SqliteConnection NewConnection ()
		{
			bool exists = File.Exists (Configuration.USER_DB_INSTALLED_PATH);
			if (!exists) {
				throw new Exception ("Sqlite file was not found");
			}
			
			return new SqliteConnection ("Data Source=" + Configuration.USER_DB_INSTALLED_PATH);
		}

		private static void FillParameters(Dictionary<string, object> parameters, SqliteCommand command)
		{
			if(parameters != null && parameters.Count != 0) {
				foreach(var key in parameters.Keys) {
					command.Parameters.AddWithValue(key, parameters[key]);
				}
			}
		}
	}
}


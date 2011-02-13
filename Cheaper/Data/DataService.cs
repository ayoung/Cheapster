using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using Cheaper.Data.Models;
using Cheaper.Data;

namespace Cheaper.Data
{
	public static class DataService
	{
		private const string _lastRowId = "select last_insert_rowid();";
		
		#region Comparison
		
		public static List<ComparisonModel> GetComparisons()
		{
			var comparisons = new List<ComparisonModel>();
			var commandText = "select Id, UnitTypeId, UnitId, CategoryId, Name from Comparison order by Name;";
			SqlConnection.ReaderWithCommand(commandText, (reader) =>
			{
				while(reader.Read()) {
					comparisons.Add(CreateComparison(reader));
				}
			});
			
			return comparisons;
		}
		
		public static ComparisonModel GetComparison(int id)
		{
			var commandText = "select Id, UnitTypeId, UnitId, CategoryId, Name from Comparison where Id = @Id;";
			var parameters = new Dictionary<string, object>();
			parameters.Add("@Id", id);
			ComparisonModel comparison = null;
			SqlConnection.ReaderWithCommand(commandText, parameters, (reader) =>
			{
				if(reader.Read())
				{
					comparison = CreateComparison(reader);
				}
			});
			
			return comparison;
		}
		
		public static int SaveComparison(ComparisonModel comparison)
		{
			var commandText = string.Empty;
			
			if(comparison.Id == 0)
			{
				commandText = "insert into Comparison (UnitTypeId, UnitId, Name) values (@UnitTypeId, @UnitId, @Name);";
				commandText += "select last_insert_rowid();";
			}
			else 
			{
				commandText = "update Comparison set UnitTypeId=@UnitTypeId, UnitId=@UnitId, Name=@Name where Id=@Id;";
			}
			
			var parameters = new Dictionary<string, object>();
			parameters.Add("@UnitTypeId", comparison.UnitTypeId);
			parameters.Add("@Name", comparison.Name);
			parameters.Add("@UnitId", comparison.UnitId);
			parameters.Add("@Id", comparison.Id);
			
			int newId = 0;
			
			if(comparison.Id == 0)
			{
				SqlConnection.ReaderWithCommand(commandText, parameters, (reader) => {
					newId = reader.Read() ? reader.GetInt32(0) : 0;
				});
				return newId;
			}
			else
			{
				SqlConnection.ExecuteNonQuery(commandText, parameters);
				return comparison.Id;
			}
		}
		
		private static ComparisonModel CreateComparison(SqliteDataReader reader)
		{
			return new ComparisonModel { 
						Id = reader.GetInt32(0),
						UnitTypeId = reader.GetInt32(1),
						UnitId = reader.GetInt32(2),
						//CategoryId = reader.GetInt32(3),
						Name = reader.IsDBNull(4) ? null : reader.GetString(4)
					};
		}
		
		#endregion
		
		#region Comparable 
		
		public static List<ComparableModel> GetComparables(int comparisonId)
		{
			var comparables = new List<ComparableModel>();
			var commandText = "select Id, ComparisonId, UnitId, Store, Product, Price, Quantity from Comparable where ComparisonId = @ComparisonId;";
			var parameters = new Dictionary<string, object>();
			parameters.Add("@ComparisonId", comparisonId);
			SqlConnection.ReaderWithCommand(commandText, parameters, (reader) =>
			{
				while(reader.Read()) {
					comparables.Add(CreateComparable(reader));
				}
			});
			
			return comparables;
		}
		
		public static ComparableModel GetComparable(int id)
		{
			var commandText = "select Id, ComparisonId, UnitId, Store, Product, Price, Quantity from Comparable where Id=@Id;";
			var parameters = new Dictionary<string, object>();
			parameters.Add("@Id", id);
			ComparableModel comparable = null;
			SqlConnection.ReaderWithCommand(commandText, parameters, (reader) =>
			{
				if(reader.Read())
				{
					comparable = CreateComparable(reader);
				}
			});
			
			return comparable;
		}
		
		public static int SaveComparable(ComparableModel comparable)
		{
			var commandText = "insert into Comparable (ComparisonId, UnitId, Store, Product, Price, Quantity) values (@ComparisonId, @UnitId, @Store, @Product, @Price, @Quantity);";
			commandText += "select last_insert_rowid();";
			var parameters = new Dictionary<string, object>();
			parameters.Add("@ComparisonId", comparable.ComparisonId);
			parameters.Add("@UnitId", comparable.UnitId);
			parameters.Add("@Store", comparable.Store);
			parameters.Add("@Product", comparable.Product);
			parameters.Add("@Price", comparable.Price);
			parameters.Add("@Quantity", comparable.Quantity);
			
			int newId = 0;
			SqlConnection.ReaderWithCommand(commandText, parameters, (reader) => {
				newId = reader.Read() ? reader.GetInt32(0) : 0;
			});
			return newId;
		}
		
		private static ComparableModel CreateComparable(SqliteDataReader reader)
		{
			return new ComparableModel() {
						Id = reader.GetInt32(0),
						ComparisonId = reader.GetInt32(1),
						UnitId = reader.GetInt32(2),
						Store = reader.GetString(3),
						Product = reader.GetString(4),
						Price = reader.GetDouble(5),
						Quantity = reader.GetDouble(6)
					};
		}
		
		#endregion
		
		#region Unit
		
		public static List<UnitModel> GetUnits(int unitTypeId)
		{
			var units = new List<UnitModel>();
			var commandText = "select Id, UnitTypeId, Name, FullName from Unit where UnitTypeId = @UnitTypeId;";
			var parameters = new Dictionary<string, object>();
			parameters.Add("@UnitTypeId", unitTypeId);
			SqlConnection.ReaderWithCommand(commandText, parameters, (reader) =>
			{
				while(reader.Read()) {
					units.Add(new UnitModel() {
						Id = reader.GetInt32(0),
						UnitTypeId = reader.GetInt32(1),
						Name = reader.GetString(2),
						FullName = reader.GetString(3)
					});
				}
			});
			
			return units;
		}
		
		#endregion
	}
}


using System;
using System.Linq;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using Cheapster.Data.Models;
using Cheapster.Data;

namespace Cheapster.Data
{
	public static class DataService
	{
		private const string _lastRowId = "select last_insert_rowid();";
		private const string _selectComparison = "select c.Id, c.UnitTypeId, c.UnitId, c.CategoryId, c.Name, cc.Store, cc.Price, cc.Quantity, cc.UnitId, cc.Id, cc.Product from Comparison c left outer join Comparable cc on c.CheapestComparableId = cc.Id";
		
		#region Comparison
		
		public static List<ComparisonModel> GetComparisons()
		{
			var comparisons = new List<ComparisonModel>();
			var commandText = _selectComparison + ";";
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
			var commandText = _selectComparison + " where c.Id = @Id;";
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
		
		/// <summary>
		/// Deletes a comparison with the given id.
		/// </summary>
		/// <param name="id">
		/// The comparison to delete.
		/// </param>
		/// <returns>
		/// A value whether a comparison has been deleted. False if nothing was deleted.
		/// </returns>
		public static bool DeleteComparison(int id)
		{
			var commandText = "delete from Comparison where Id = @Id;";
			var parameters = new Dictionary<string, object>();
			parameters.Add("@Id", id);
			return SqlConnection.ExecuteNonQuery(commandText, parameters) > 0;
		}
		
		public static int SaveComparison(ComparisonModel comparison)
		{
			if(comparison.Id != 0)
			{
				throw new ArgumentException("Comparison with non-zero id cannot be saved.");
			}
			
			var commandText = "insert into Comparison (UnitTypeId, UnitId, Name, CheapestComparableId) values (@UnitTypeId, @UnitId, @Name, @CheapestComparableId);";
			commandText += "select last_insert_rowid();";
			
			var parameters = new Dictionary<string, object>();
			parameters.Add("@UnitTypeId", comparison.UnitTypeId);
			parameters.Add("@Name", comparison.Name);
			parameters.Add("@UnitId", comparison.UnitId);
			parameters.Add("@CheapestComparableId", comparison.CheapestComparableId);
			
			int newId = 0;

			SqlConnection.ReaderWithCommand(commandText, parameters, (reader) => {
				newId = reader.Read() ? reader.GetInt32(0) : 0;
			});
			return newId;
		}
		
		public static bool UpdateComparison(ComparisonModel comparison)
		{
			var commandText = "update Comparison set UnitTypeId=@UnitTypeId, UnitId=@UnitId, CheapestComparableId=@CheapestComparableId, Name=@Name where Id=@Id;";
			var parameters = new Dictionary<string, object>();
			parameters.Add("@UnitTypeId", comparison.UnitTypeId);
			parameters.Add("@Name", comparison.Name);
			parameters.Add("@UnitId", comparison.UnitId);
			parameters.Add("@Id", comparison.Id);
			parameters.Add("@CheapestComparableId", comparison.CheapestComparableId);
			
			return SqlConnection.ExecuteNonQuery(commandText, parameters) > 0;
		}
		
		private static ComparisonModel CreateComparison(SqliteDataReader reader)
		{
			return new ComparisonModel { 
				Id = reader.GetInt32(0),
				UnitTypeId = reader.GetInt32(1),
				UnitId = reader.GetInt32(2),
				//CategoryId = reader.GetInt32(3),
				Name = reader.IsDBNull(4) ? null : reader.GetString(4),
				CheapestStore = reader.IsDBNull(5) ? null : reader.GetString(5),
				CheapestPrice = reader.IsDBNull(6) ? (double?)null : reader.GetDouble(6),
				CheapestQuantity = reader.IsDBNull(7) ? (double?)null : reader.GetDouble(7),
				CheapestUnitId = reader.IsDBNull(8) ? (int?)null : reader.GetInt32(8),
				CheapestComparableId = reader.IsDBNull(9) ? (int?)null : reader.GetInt32(9),
				CheapestProduct = reader.IsDBNull(10) ? null : reader.GetString(10)
			};
		}
		
		#endregion
		
		#region Comparable 
		
		public static List<ComparableModel> GetComparables(int comparisonId)
		{
			var comparables = new List<ComparableModel>();
			var commandText = "select Id, ComparisonId, UnitId, Store, Product, Price, Quantity, ModifiedOn from Comparable where ComparisonId = @ComparisonId;";
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
			var commandText = "select Id, ComparisonId, UnitId, Store, Product, Price, Quantity, ModifiedOn from Comparable where Id=@Id;";
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
			if(comparable.Id != 0)
			{
				throw new ArgumentException("Comparable with non-zero id cannot be saved.");
			}
			
			comparable.ModifiedOn = DateTime.Now;
			var commandText = "insert into Comparable (ComparisonId, UnitId, Store, Product, Price, Quantity, ModifiedOn) values (@ComparisonId, @UnitId, @Store, @Product, @Price, @Quantity, @ModifiedOn);";
			commandText += "select last_insert_rowid();";
			var parameters = new Dictionary<string, object>();
			parameters.Add("@ComparisonId", comparable.ComparisonId);
			parameters.Add("@UnitId", comparable.UnitId);
			parameters.Add("@Store", comparable.Store);
			parameters.Add("@Product", comparable.Product);
			parameters.Add("@Price", comparable.Price);
			parameters.Add("@Quantity", comparable.Quantity);
			parameters.Add("@ModifiedOn", comparable.ModifiedOn);
			
			int newId = 0;
			SqlConnection.ReaderWithCommand(commandText, parameters, (reader) => {
				newId = reader.Read() ? reader.GetInt32(0) : 0;
			});
			return newId;
		}
		
		/// <summary>
		/// Updates all properties of a comparable except for the ComparisonId.
		/// </summary>
		/// <param name="comparable">
		/// A <see cref="ComparableModel"/>
		/// </param>
		/// <returns>
		/// True if rows were updated.
		/// </returns>
		public static bool UpdateComparable(ComparableModel comparable)
		{
			comparable.ModifiedOn = DateTime.Now;
			var commandText = "update Comparable set UnitId=@UnitId, Store=@Store, Product=@Product, Price=@Price, Quantity=@Quantity, ModifiedOn=@ModifiedOn where Id=@Id;";
			var parameters = new Dictionary<string, object>();
			parameters.Add("@Id", comparable.Id);
			parameters.Add("@UnitId", comparable.UnitId);
			parameters.Add("@Store", comparable.Store);
			parameters.Add("@Product", comparable.Product);
			parameters.Add("@Price", comparable.Price);
			parameters.Add("@Quantity", comparable.Quantity);
			parameters.Add("@ModifiedOn", comparable.ModifiedOn);
			return SqlConnection.ExecuteNonQuery(commandText, parameters) > 0;
		}
		
		/// <summary>
		/// Deletes a comparable with the given id.
		/// </summary>
		/// <param name="id">
		/// The comparable to delete.
		/// </param>
		/// <returns>
		/// A value whether a comparable has been deleted. False if nothing was deleted.
		/// </returns>
		public static bool DeleteComparable(int id)
		{
			var commandText = "delete from Comparable where Id = @Id;";
			var parameters = new Dictionary<string, object>();
			parameters.Add("@Id", id);
			return SqlConnection.ExecuteNonQuery(commandText, parameters) > 0;
		}
		
		private static ComparableModel CreateComparable(SqliteDataReader reader)
		{
			return new ComparableModel() {
						Id = reader.GetInt32(0),
						ComparisonId = reader.GetInt32(1),
						UnitId = reader.GetInt32(2),
						Store = reader.IsDBNull(3) ? null : reader.GetString(3),
						Product = reader.IsDBNull(4) ? null : reader.GetString(4),
						Price = reader.GetDouble(5),
						Quantity = reader.GetDouble(6),
						ModifiedOn = reader.GetDateTime(7)
					};
		}
		
		#endregion
		
		#region Unit
		
		public static List<UnitModel> GetUnits(int unitTypeId)
		{
			var units = new List<UnitModel>();
			var commandText = "select Id, UnitTypeId, Name, FullName, Multiplier from Unit where UnitTypeId = @UnitTypeId;";
			var parameters = new Dictionary<string, object>();
			parameters.Add("@UnitTypeId", unitTypeId);
			SqlConnection.ReaderWithCommand(commandText, parameters, (reader) =>
			{
				while(reader.Read()) {
					units.Add(new UnitModel() {
						Id = reader.GetInt32(0),
						UnitTypeId = reader.GetInt32(1),
						Name = reader.GetString(2),
						FullName = reader.GetString(3),
						Multiplier = reader.GetDouble(4)
					});
				}
			});
			
			return units;
		}
		
		private static List<UnitModel> _units;
		
		public static List<UnitModel> GetUnits()
		{
			if(_units != null)
			{
				return _units;
			}	
			
			_units = new List<UnitModel>();
			var commandText = "select Id, UnitTypeId, Name, FullName, Multiplier from Unit;";
			SqlConnection.ReaderWithCommand(commandText, (reader) =>
			{
				while(reader.Read()) {
					_units.Add(new UnitModel() {
						Id = reader.GetInt32(0),
						UnitTypeId = reader.GetInt32(1),
						Name = reader.GetString(2),
						FullName = reader.GetString(3),
						Multiplier = reader.GetDouble(4)
					});
				}
			});
			
			return _units;
		}
		
		public static Dictionary<int, UnitModel> GetUnitsAsDictionary()
		{
			return (from u in DataService.GetUnits() select u).ToDictionary(u => u.Id, u => u);
		}
		
		#endregion
		
		#region Store Names
		
		public static List<RecentStore> GetRecentStoreNames()
		{
			var stores = new List<RecentStore>();
			var commandText = "select Store, max(ModifiedOn) as ModifiedOn, count(Store) as Count from Comparable group by Store;";
			SqlConnection.ReaderWithCommand(commandText, (reader) =>
			{
				while(reader.Read()) {
					stores.Add(new RecentStore() {
						Name = reader.GetString(0),
						LastModifiedOn = reader.GetDateTime(1),
						UsedCount = reader.GetInt32(2)
					});
				}
			});
			
			return stores;
		} 
		
		#endregion
		
		#region Database
		
		public static double GetDbVersion(string pathToDb)
		{
			var commandText = "select DbVersion from Meta;";
			double version = 0;

			using(var connection = SqlConnection.NewConnection(pathToDb))
			using(var command = connection.CreateCommand())
			{
				command.CommandText = commandText;
				connection.Open();
				using(var reader = command.ExecuteReader())
				{
					while(reader.Read())
					{
						version = reader.GetFloat(0);
					}
				}
			}
			
			return version;
		}		
		
		#endregion
	}
}


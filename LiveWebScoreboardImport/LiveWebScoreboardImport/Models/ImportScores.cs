using System.Data;
using System.Text;

using LiveWebScoreboardImport.Common;
using LiveWebScoreboardImport.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Primitives;

#pragma warning disable CS8603
#pragma warning disable CS8604
#pragma warning disable CS8600
namespace LiveWebScoreboardImport.Models {

	public class ImportScores {
		private readonly ILogger myLogger;
		private readonly String myModuleName = "ImportScores: ";

		public ImportScores( ILogger inLogger ) {
			myLogger = inLogger;
			JsonSerializerOptions options = new() {
				ReferenceHandler = ReferenceHandler.Preserve
				, WriteIndented = true
			};
		}

		public DataTable getTournament( String inSanctionId ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select[Name], Class, Federation, SanctionEditCode" );
			curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds" );
			curSqlStmt.Append( ", Rules, EventDates, EventLocation " );
			curSqlStmt.Append( "From Tournament " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}'", inSanctionId ) );
			return DataAccess.getDataTable( curSqlStmt.ToString(), myLogger );
		}

		public DataTable getTournamentList() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select[Name], Class, Federation, SanctionEditCode" );
			curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds" );
			curSqlStmt.Append( ", Rules, EventDates, EventLocation " );
			curSqlStmt.Append( "From Tournament " );
			return DataAccess.getDataTable( curSqlStmt.ToString(), myLogger );
		}

		public string importEventScores( JsonDocument inJsonDoc ) {
			String curMethodName = myModuleName + "importEventScores: ";
			String curReturnMsg = "";

			try {
				HelperFunctions.writeLogger( myLogger, "Debug", curMethodName, "Begin" );

				JsonElement curElementRoot = inJsonDoc.RootElement;
				JsonElement curElementRequest = curElementRoot.GetProperty( "LiveWebRequest" );
				JsonElement curElementTable = curElementRequest.GetProperty( "Tables" );
				JsonValueKind curElementType = curElementTable.ValueKind;
				if ( !( curElementType ).ToString().Equals( "Array" ) ) {
					curReturnMsg = "Error: Tables element must be an array";
					HelperFunctions.writeLogger( myLogger, "Error", curMethodName, curReturnMsg );
					return curReturnMsg;
				}
				List<Dictionary<string, object>> curListTables = JsonSerializer.Deserialize<List < Dictionary<string, object> >> ( curElementTable );
				if ( curListTables == null || curListTables.Count == 0 ) {
					curReturnMsg = "Error: Tables element array is empty";
					HelperFunctions.writeLogger( myLogger, "Error", curMethodName, curReturnMsg );
					return curReturnMsg;
				}
				return importAllTables( curListTables );

			} catch ( Exception ex ) {
				HelperFunctions.writeLogger( myLogger, "Error", curMethodName, "Exception encountered serializing input JSON object: Message: " + ex.Message );
				curReturnMsg = "Error: Exception encountered serializing input JSON object: Message: " + ex.Message;
				HelperFunctions.writeLogger( myLogger, "Error", curMethodName, curReturnMsg );
				return curReturnMsg;
			}
		}

		private string importAllTables( List<Dictionary<string, object>> curListTables ) {
			String curMethodName = myModuleName + "importAllTables: ";
			HelperFunctions.writeLogger( myLogger, "Debug", curMethodName, "Import request contains " + curListTables.Count + " tables" );

			bool curErrorsFound = false;
			String curResultMsg = "";
			StringBuilder curReturnMsg = new StringBuilder( "" );
			List<String> curListKeys;
			List<String> curListColumns;
			List<Dictionary<string, object>> curListRows;

			foreach ( Dictionary<string, object> curTable in curListTables) {
				if ( curTable == null ) continue;
				String curTableName = HelperFunctions.getAttributeValue( curTable, "name" );
				String curCommand = HelperFunctions.getAttributeValue( curTable, "command" );

				JsonElement curElement = (JsonElement)curTable["Keys"];
				JsonValueKind curElementType = curElement.ValueKind;
				if ( curElementType.ToString().Equals( "Array" ) ) {
					curListKeys = JsonSerializer.Deserialize<List<String>>( curElement );
				
				} else {
					String curMsg = String.Format( "Table import {0} failed: Table element must contain an array of keys", curTableName );
					curReturnMsg.Append( curMsg + "\n" );
					HelperFunctions.writeLogger( myLogger, "Error", curMethodName, curMsg );
					curErrorsFound = true;
					continue;
				}

				curElement = (JsonElement)curTable["Columns"];
				curElementType = curElement.ValueKind;
				if ( curElementType.ToString().Equals( "Array" ) ) {
					curListColumns = JsonSerializer.Deserialize<List<String>>( curElement );

				} else {
					String curMsg = String.Format( "Table import {0} failed: Table element must contain an array of column names", curTableName );
					curReturnMsg.Append( curMsg + "\n" );
					HelperFunctions.writeLogger( myLogger, "Error", curMethodName, curMsg );
					curErrorsFound = true;
					continue;
				}

				curElement = (JsonElement)curTable["Rows"];
				curElementType = curElement.ValueKind;
				if ( curElementType.ToString().Equals( "Array" ) ) {
					curListRows = JsonSerializer.Deserialize<List<Dictionary<string, object>>>( curElement );

				} else {
					String curMsg = String.Format( "Table import {0} failed: Table element must contain an array of data row values", curTableName );
					curReturnMsg.Append( curMsg + "\n" );
					HelperFunctions.writeLogger( myLogger, "Error", curMethodName, curMsg );
					curErrorsFound = true;
					continue;
				}

				curResultMsg = updateTableData( curTableName, curCommand, curListKeys, curListColumns, curListRows );
				if ( curResultMsg.StartsWith( "OK" ) ) {
					String curMsg = String.Format( "Table import {0} completed: {1}", curTableName, curResultMsg );
					curReturnMsg.Append( curMsg + "\n" );
					HelperFunctions.writeLogger( myLogger, "Debug", curMethodName, curMsg );
				
				} else {
					String curMsg = String.Format( "Table import {0} failed: {1}", curTableName, curResultMsg );
					curReturnMsg.Append( curMsg + "\n" );
					HelperFunctions.writeLogger( myLogger, "Warn", curMethodName, curMsg );
					curErrorsFound = true;
				}
			}

			if ( curErrorsFound ) {
				return "Error: " + curReturnMsg.ToString();
			} else {
				return "OK: " + curReturnMsg.ToString();
			}
		}

		private string updateTableData( String curTableName, String curCommand, List<String> curListKeys, List<String> curListColumns, List<Dictionary<string, object>> curListRows ) {
			String curMethodName = myModuleName + "updateTableData: ";
			StringBuilder curReturnMsg = new StringBuilder("" );

			int curRowsDeleted = 0, curRowsUpdated = 0, curRowsInserted = 0, curRowsError = 0, curResult = 0;
			HelperFunctions.writeLogger( myLogger, "Debug", curMethodName, String.Format( "Import {0} {1} request contains {2} keys, {3} column names, {4} data rows"
				, curCommand, curTableName, curListKeys.Count, curListColumns.Count, curListRows.Count ) );
			if ( curListKeys.Count == 0 || curListColumns.Count == 0 || curListRows.Count == 0 ) {
				curReturnMsg.Append(String.Format( "Error: Import request must contain values for keys, columns and rows: {0} keys, {1} column names, {2} data columns", curListKeys.Count, curListColumns.Count, curListRows.Count ) );
				HelperFunctions.writeLogger( myLogger, "Error", curMethodName, curReturnMsg.ToString() );
				return curReturnMsg.ToString();
			}

			foreach ( Dictionary<string, object> curRow in curListRows ) {
				if ( curCommand.Equals( "Delete" ) ) {
					curResult = deleteRow( curTableName, curListKeys, curListColumns, curRow );
					if ( curResult >= 0 ) {
						curRowsDeleted += curResult;
					} else {
						curRowsError += 1;
					}

				} else if ( isRowFound( curTableName, curListKeys, curListColumns, curRow ) ) {
					curResult = updateRow( curTableName, curListKeys, curListColumns, curRow );
					if ( curResult >= 0 ) {
						curRowsUpdated += curResult;
					} else {
						curRowsError += 1;
					}

				} else {
					curResult = insertRow( curTableName, curListKeys, curListColumns, curRow );
					if ( curResult >= 0 ) {
						curRowsInserted += curResult;
					} else {
						curRowsError += 1;
					}
				}
			}

			curReturnMsg.Append( String.Format( "Request complete.  Records Inserted {0}, Updated {1}, Deleted {2}, Errors {3}"
				, curRowsInserted, curRowsUpdated, curRowsDeleted, curRowsError ) );
			if ( curRowsError > 0 ) return "Error: " + curReturnMsg.ToString();
			return "OK: " + curReturnMsg.ToString();
		}

		private bool isRowFound( String curTableName, List<String> curListKeys, List<String> curListColumns, Dictionary<string, object> curRow ) {
			String curMethodName = myModuleName + "isRowFound: ";
			int curIdx = 0;
			String curColValue = "";

			StringBuilder curSqlStmt = new StringBuilder( "Select " );
			try {
				foreach ( String curKey in curListKeys ) {
					curIdx++;
					if ( curIdx > 1 ) curSqlStmt.Append( ", " );
					curSqlStmt.Append( curKey );
				}
				curSqlStmt.Append( String.Format( " From {0} Where ", curTableName ) );

				curIdx = 0;
				foreach ( String curKey in curListKeys ) {
					curIdx++;
					if ( curIdx > 1 ) curSqlStmt.Append( " AND " );
					curColValue = HelperFunctions.getAttributeValue( curRow, curKey );
					curSqlStmt.Append( String.Format( "{0} = '{1}'", curKey, curColValue ) );
				}

				DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString(), myLogger );
				if ( curDataTable == null || curDataTable.Rows.Count == 0 ) {
					HelperFunctions.writeLogger( myLogger, "Debug", curMethodName, String.Format( "Row not found for {0}", curSqlStmt.ToString() ) );
					return false;
				}

				return true;

			} catch ( Exception ex ) {
				HelperFunctions.writeLogger( myLogger, "Error", curMethodName, "Exception encountered checking for data row: Message: " + ex.Message + " SQL=" + curSqlStmt.ToString() );
				return false;
			}
		}

		private int insertRow( String curTableName, List<String> curListKeys, List<String> curListColumns, Dictionary<string, object> curRow ) {
			String curMethodName = myModuleName + "insertRow: ";
			int curIdx = 0;
			String curColValue;

			StringBuilder curSqlStmt = new StringBuilder( "" );
			try {
				curSqlStmt.Append( String.Format( "Insert INTO  {0} ( ", curTableName ) );
				foreach ( String curColumn in curListColumns ) {
					curIdx++;
					if ( curIdx > 1 ) curSqlStmt.Append( ", " );
					curSqlStmt.Append( curColumn );
				}

				curSqlStmt.Append( " ) VALUES ( " );

				curIdx = 0;
				foreach ( String curColumn in curListColumns ) {
					curIdx++;
					curColValue = HelperFunctions.getAttributeValue( curRow, curColumn );
					if ( curIdx > 1 ) curSqlStmt.Append( ", " );
					if ( HelperFunctions.isObjectEmpty( curColValue ) ) {
						curSqlStmt.Append( "null" );

					} else if ( curColValue.Equals( "False" ) ) {
						curSqlStmt.Append( "0" );

					} else if ( curColValue.Equals( "True" ) ) {
						curSqlStmt.Append( "1" );

					} else {
						curSqlStmt.Append( String.Format( "'{0}'", curColValue ) );
					}
				}
				curSqlStmt.Append( " )" );

				int curRowsInserted = DataAccess.ExecuteCommand( curSqlStmt.ToString(), myLogger );
				if ( curRowsInserted <= 0 ) {
					HelperFunctions.writeLogger( myLogger, "Error", curMethodName, String.Format( "Insert failed SQL={0} ", curSqlStmt.ToString() ) );
					return -1;
				}

				HelperFunctions.writeLogger( myLogger, "Info", curMethodName, String.Format( "{0} Rows successfully inserted {1}", curRowsInserted, curSqlStmt.ToString() ) );
				return curRowsInserted;

			} catch ( Exception ex ) {
				HelperFunctions.writeLogger( myLogger, "Error", curMethodName, "Exception encountered inserting record: Message: " + ex.Message + " SQL=" + curSqlStmt.ToString() );
				return -1;
			}
		}

		private int updateRow( String curTableName, List<String> curListKeys, List<String> curListColumns, Dictionary<string, object> curRow ) {
			String curMethodName = myModuleName + "updateRow: ";
			String curColValue = "";
			int curIdx = 0;

			StringBuilder curSqlStmt = new StringBuilder( String.Format( "Update {0} Set ", curTableName ) );
			try {
				curIdx = 0;
				foreach ( String curColumn in curListColumns ) {
					curIdx++;
					curColValue = HelperFunctions.getAttributeValue( curRow, curColumn );
					if ( curIdx > 1 ) curSqlStmt.Append( ", " );
					curColValue = HelperFunctions.getAttributeValue( curRow, curColumn );
					if ( curColValue.Equals( "False" ) ) {
						curSqlStmt.Append( String.Format( "{0} = 0", curColumn ) );

					} else if ( curColValue.Equals( "True" ) ) {
						curSqlStmt.Append( String.Format( "{0} = 1", curColumn ) );

					} else if ( HelperFunctions.isObjectEmpty( curColValue ) ) {
						curSqlStmt.Append( String.Format( "{0} = null", curColumn ) );

					} else {
						curSqlStmt.Append( String.Format( "{0} = '{1}'", curColumn, curColValue ) );
					}
				}

				curIdx = 0;
				curSqlStmt.Append( " Where " );
				foreach ( String curKey in curListKeys ) {
					curIdx++;
					if ( curIdx > 1 ) curSqlStmt.Append( " AND " );
					curColValue = HelperFunctions.getAttributeValue( curRow, curKey );
					curSqlStmt.Append( String.Format( "{0} = '{1}'", curKey, curColValue ) );
				}

				int curRowsUpdated = DataAccess.ExecuteCommand( curSqlStmt.ToString(), myLogger );
				if ( curRowsUpdated <= 0 ) {
					HelperFunctions.writeLogger( myLogger, "Error", curMethodName, String.Format( "Update failed SQL={0} ", curSqlStmt.ToString() ) );
					return -1;
				}

				HelperFunctions.writeLogger( myLogger, "Info", curMethodName, String.Format( "{0} Rows successfully updated {1}", curRowsUpdated, curSqlStmt.ToString() ) );
				return curRowsUpdated;

			} catch ( Exception ex ) {
				HelperFunctions.writeLogger( myLogger, "Error", curMethodName, "Exception encountered updating record: Message: " + ex.Message + " SQL=" + curSqlStmt.ToString() );
				return -1;
			}
		}

		private int deleteRow( String curTableName, List<String> curListKeys, List<String> curListColumns, Dictionary<string, object> curRow ) {
			String curMethodName = myModuleName + "deleteRow: ";

			int curIdx = 0;
			String curColValue = "";

			StringBuilder curSqlStmt = new StringBuilder( String.Format( "Delete From {0} Where ", curTableName ) );
			try {
				foreach ( String curKey in curListKeys ) {
					curIdx++;
					if ( curIdx > 1 ) curSqlStmt.Append( " AND " );
					curColValue = HelperFunctions.getAttributeValue( curRow, curKey );
					curSqlStmt.Append( String.Format( "{0} = '{1}'", curKey, curColValue ) );
				}

				int curRowsDeleted = DataAccess.ExecuteCommand( curSqlStmt.ToString(), myLogger );
				if ( curRowsDeleted < 0 ) {
					HelperFunctions.writeLogger( myLogger, "Error", curMethodName, String.Format( "Delete failed SQL={0} ", curSqlStmt.ToString() ) );
					return -1;
				}

				HelperFunctions.writeLogger( myLogger, "Info", curMethodName, String.Format( "{0} Rows successfully deleted {1}", curRowsDeleted, curSqlStmt.ToString() ) );
				return curRowsDeleted;

			} catch ( Exception ex ) {
				HelperFunctions.writeLogger( myLogger, "Error", curMethodName, "Exception encountered deleting record: Message: " + ex.Message + " SQL=" + curSqlStmt.ToString() );
				return -1;
			}
		}

	}
}

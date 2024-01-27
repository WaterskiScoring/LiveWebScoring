using System.Data;
using LiveWebScoreboardImport.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;

#pragma warning disable CA1416 // Validate platform compatibility
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.
namespace LiveWebScoreboardImport.Services {
	public class DataAccess {
		private static int DataAccessOpenCount = 0;
		private static int DataAccessEmbeddedTransactionCount = 0;
		//private readonly ILogger<DataAccess> myLogger;

		private static SqlConnection? DataAccessConnection = null;
		private static SqlTransaction? DataAccessTransaction = null;
		private static SqlDataAdapter? DataAccessDataAdapter = null;
		private static SqlCommand? DataAccessCommand = null;

		public DataAccess() {
		}

		public static bool DataAccessOpen( ILogger inLogger ) {
			string curMethodName = "DataAccess:DataAccessOpen: ";

			try {
				if ( DataAccessConnection == null ) {
					string curAppConnectString = getConnectionString( inLogger );
					if ( HelperFunctions.isObjectEmpty( curAppConnectString ) ) return false;

					DataAccessOpenCount = 1;
					DataAccessConnection = new SqlConnection( curAppConnectString );
					DataAccessConnection.Open();
					HelperFunctions.writeLogger( inLogger, "Info", curMethodName, "Database opened" );

				} else {
					DataAccessOpenCount++;
				}
				return true;

			} catch ( Exception ex ) {
				string curMsg = string.Format( "Exception encountered opening data source {0}", ex.Message );
				HelperFunctions.writeLogger( inLogger, "Error", curMethodName, curMsg );
				return false;
			}
		}

		public static bool DataAccessClose() {
			return DataAccessClose( false );
		}
		public static bool DataAccessClose( bool inCloseAll ) {
			string curMethodName = "DataAccess:DataAccessClose";

			try {
				if ( DataAccessConnection == null ) {
					DataAccessOpenCount = 0;
				} else {
					if ( inCloseAll ) {
						DataAccessOpenCount = 0;
						DataAccessConnection.Close();
						DataAccessConnection.Dispose();
						DataAccessConnection = null;
						DataAccessDataAdapter = null;
						DataAccessTransaction = null;
						DataAccessCommand = null;
					} else {
						if ( DataAccessOpenCount > 1 ) {
							DataAccessOpenCount--;
						} else {
							DataAccessOpenCount = 0;
							DataAccessConnection.Close();
							DataAccessConnection.Dispose();
							DataAccessConnection = null;
							DataAccessDataAdapter = null;
							DataAccessTransaction = null;
							DataAccessCommand = null;
						}
					}
				}
				return true;

			} catch ( Exception ex ) {
				string curMsg = string.Format( "{0}: Exception closing database", curMethodName, ex.Message );
				return false;
			}
		}

		public static DataTable? getDataTable( string inSelectStmt, ILogger inLogger ) {
			string curMethodName = "DataAccess:getDataTable";
			DataTable curDataTable = new DataTable();

			try {
				// Open connection if needed
				if ( DataAccessConnection == null ) DataAccessOpen( inLogger );
				if ( DataAccessConnection == null ) return null;

				// Create data adapter
				DataAccessDataAdapter = new SqlDataAdapter( inSelectStmt, DataAccessConnection );
				DataAccessDataAdapter.Fill( curDataTable );

			} catch ( Exception ex ) {
				string curExcpMsg = ex.Message;
				if ( inSelectStmt != null ) { curExcpMsg += "\nSQL=" + inSelectStmt; }
				string curMsg = string.Format( "Exception executing SQL operation {0} {1}", ex.Message, curExcpMsg );
				HelperFunctions.writeLogger( inLogger, "Error", curMethodName, curMsg );
				return null;
			}

			return curDataTable;
		}

		public static int ExecuteCommand( string inSelectStmt, ILogger inLogger ) {
			string curMethodName = "DataAccess:ExecuteCommand";
			try {
				// Open connection if needed
				if ( DataAccessConnection == null ) DataAccessOpen( inLogger );
				if ( DataAccessConnection == null ) return -1;

				// Create data adapter
				DataAccessCommand = new SqlCommand( inSelectStmt, DataAccessConnection );
				return DataAccessCommand.ExecuteNonQuery();

			} catch ( Exception ex ) {
				string curExcpMsg = ex.Message;
				if ( inSelectStmt != null ) { curExcpMsg += "\nSQL=" + inSelectStmt; }
				string curMsg = string.Format( "Exception executing SQL operation {0} {1}", ex.Message, curExcpMsg );
				HelperFunctions.writeLogger( inLogger, "Error", curMethodName, curMsg );
				return -1;
			}
		}

		public static object? ExecuteScalarCommand( string inSelectStmt, ILogger inLogger ) {
			string curMethodName = "DataAccess:ExecuteScalarCommand";
			try {
				// Open connection if needed
				if ( DataAccessConnection == null ) DataAccessOpen( inLogger );
				if ( DataAccessConnection == null ) return null;

				// Create data adapter
				DataAccessCommand = new SqlCommand( inSelectStmt, DataAccessConnection );
				return DataAccessCommand.ExecuteScalar();

			} catch ( Exception ex ) {
				string curExcpMsg = ex.Message;
				if ( inSelectStmt != null ) { curExcpMsg += "\nSQL=" + inSelectStmt; }
				string curMsg = string.Format( "Exception executing SQL operation {0} {1}", ex.Message, curExcpMsg );
				HelperFunctions.writeLogger( inLogger, "Error", curMethodName, curMsg );
				return null;
			}
		}

		public static bool BeginTransaction( string inSelectStmt, ILogger inLogger ) {
			string curMethodName = "DataAccess:BeginTransaction";

			try {
				// Open connection if needed
				if ( DataAccessConnection == null ) DataAccessOpen( inLogger );
				if ( DataAccessConnection == null ) return false;

				// Create transaction if needed
				if ( DataAccessTransaction == null ) DataAccessTransaction = DataAccessConnection.BeginTransaction();
				DataAccessEmbeddedTransactionCount++;
				return true;

			} catch ( Exception ex ) {
				string curExcpMsg = ex.Message;
				if ( inSelectStmt != null ) { curExcpMsg += "\nSQL=" + inSelectStmt; }
				string curMsg = string.Format( "Exception executing SQL operation {0} {1}", ex.Message, curExcpMsg );
				HelperFunctions.writeLogger( inLogger, "Error", curMethodName, curMsg );
				return false;
			}
		}

		public static bool CommitTransaction( string inSelectStmt, ILogger inLogger ) {
			string curMethodName = "DataAccess:CommitTransaction";

			try {
				if ( DataAccessTransaction == null ) {
					DataAccessEmbeddedTransactionCount = 0;
					string curExcpMsg = "";
					if ( inSelectStmt != null ) { curExcpMsg += "\nSQL=" + inSelectStmt; }
					string curMsg = string.Format( "No active transaction, Commit bypassed {0}", curExcpMsg );
					HelperFunctions.writeLogger( inLogger, "Warn", curMethodName, curMsg );
					return false;

				} else {
					// if we have more than 1 in the count, then we have embedded Transactions and should not commit yet
					if ( DataAccessEmbeddedTransactionCount > 1 ) {
						DataAccessEmbeddedTransactionCount--;

					} else {
						DataAccessTransaction.Commit();
						DataAccessEmbeddedTransactionCount = 0;
						DataAccessTransaction = null;
					}
					return true;
				}

			} catch ( Exception ex ) {
				string curExcpMsg = ex.Message;
				if ( inSelectStmt != null ) { curExcpMsg += "\nSQL=" + inSelectStmt; }
				string curMsg = string.Format( "Exception executing SQL operation {0} {1}", ex.Message, curExcpMsg );
				HelperFunctions.writeLogger( inLogger, "Error", curMethodName, curMsg );
				return false;
			}
		}

		public static bool RollbackTransaction( string inSelectStmt, ILogger inLogger ) {
			string curMethodName = "DataAccess:RollbackTransaction";

			try {
				if ( DataAccessTransaction == null ) {
					DataAccessEmbeddedTransactionCount = 0;
					string curExcpMsg = "";
					if ( inSelectStmt != null ) { curExcpMsg += "\nSQL=" + inSelectStmt; }
					string curMsg = string.Format( "No active transaction, Unable to Rollback Transaction {0}", curExcpMsg );
					HelperFunctions.writeLogger( inLogger, "Warn", curMethodName, curMsg );
					return false;

				} else {
					// if we have more than 1 in the count, then we have embedded Transactions and should not commit yet
					if ( DataAccessEmbeddedTransactionCount > 1 ) {
						DataAccessEmbeddedTransactionCount--;
					} else {
						DataAccessTransaction.Rollback();
						DataAccessEmbeddedTransactionCount = 0;
						DataAccessTransaction = null;
					}
					return true;
				}
			} catch ( Exception ex ) {
				string curExcpMsg = ex.Message;
				if ( inSelectStmt != null ) { curExcpMsg += "\nSQL=" + inSelectStmt; }
				string curMsg = string.Format( "Exception executing SQL operation {0} {1}", ex.Message, curExcpMsg );
				HelperFunctions.writeLogger( inLogger, "Error", curMethodName, curMsg );
				return false;
			}
		}

		public static string getConnectionString( ILogger inLogger ) {
			string curMethodName = "DataAccess:getConnectionString: ";
			string curAppRegName = "Software\\LiveWebScoreboard";
			RegistryKey curAppRegKey;
			try {
				curAppRegKey = Registry.LocalMachine.OpenSubKey( curAppRegName, false );
			} catch ( Exception ex ) {
				curAppRegKey = null;
				string curMsg = string.Format( "Exception accessing LocalMachine for registry key {0} : {1}", curAppRegName, ex.Message );
				HelperFunctions.writeLogger( inLogger, "Error", curMethodName, curMsg );
			}
			if ( curAppRegKey == null ) curAppRegKey = Registry.CurrentUser.OpenSubKey( curAppRegName, true );
			if ( curAppRegKey == null ) {
				string curMsg = string.Format( "Registry key {0} was not found and is required", curAppRegName );
				HelperFunctions.writeLogger( inLogger, "Error", curMethodName, curMsg );
				return "";

			} else if ( curAppRegKey.GetValue( "DataSource" ) == null ) {
				string curMsg = string.Format( "Registry key {0} was not found and is required", curAppRegName );
				HelperFunctions.writeLogger( inLogger, "Error", curMethodName, curMsg );
				return "";

			} else {
				HelperFunctions.writeLogger( inLogger, "Info", curMethodName, "Registry " + curAppRegName.ToString() + " contains DataSource: " + obscurreConnectionString( curAppRegKey.GetValue( "DataSource" ).ToString() ) );
				return curAppRegKey.GetValue( "DataSource" ).ToString();
			}
		}

		/*
		usawaterski-prod.cgmg2itmb6sa.us-east-1.rds.amazonaws.com; 
		Initial Catalog=LiveWebScoreboard; 
		Persist Security Info=True; 
		User ID=WaterskiResults; 
		Password=Live#Web$Score@board2024#; 
		Trust Server Certificate=True (581757a7)
		*/
		public static string obscurreConnectionString( string inConnectionString ) {
			int curDelimStart = inConnectionString.IndexOf( "Password=" );
			int curDelimEnd = inConnectionString.IndexOf( ";", curDelimStart + 9 );
			return inConnectionString.Substring( 0, curDelimStart + 9 ) + "**********" + inConnectionString.Substring( curDelimEnd );
		}
	}
}

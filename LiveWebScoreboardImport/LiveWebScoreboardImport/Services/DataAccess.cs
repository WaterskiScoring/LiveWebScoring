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
		private static int DataAccessEmbeddedTransactionCount = 0;
        private static string DataAccessConnnectString = null;

		public DataAccess() {
		}

        public static DataTable? getDataTable( string inSqlStmt ) {
			string curMethodName = "DataAccess:getDataTable: ";
			DataTable curDataTable = new DataTable();

            try {
                using (SqlConnection curDataAccessConnection = new SqlConnection( getConnectionString() )) {
                    curDataAccessConnection.Open();

                    using (SqlDataAdapter curDataAdapter = new SqlDataAdapter( inSqlStmt, curDataAccessConnection )) {
                        curDataAdapter.Fill( curDataTable );
                        return curDataTable;
                    }
                }
            
            } catch (Exception ex) {
                string curExcpMsg = ex.Message;
                if (inSqlStmt != null) { curExcpMsg += "\nSQL=" + inSqlStmt; }
                throw new Exception( string.Format( "Exception executing SQL operation with message: {0}", curExcpMsg ) );
            }
        }

        public static int ExecuteCommand( string inSqlStmt ) {
            try {
                using (SqlConnection curDataAccessConnection = new SqlConnection( getConnectionString() )) {
                    curDataAccessConnection.Open();
                    
                    using (SqlCommand curDataAccessCommand = new SqlCommand( inSqlStmt, curDataAccessConnection )) {
                        return curDataAccessCommand.ExecuteNonQuery();
                    }
                }

            } catch ( Exception ex ) {
				string curExcpMsg = ex.Message;
				if (inSqlStmt != null ) { curExcpMsg += "\nSQL=" + inSqlStmt; }
                throw new Exception( string.Format( "Exception executing SQL operation with message: {0}", curExcpMsg ) );
			}
		}

		private static string getConnectionString() {
            if (DataAccessConnnectString != null) return DataAccessConnnectString;

            string curAppRegName = "Software\\LiveWebScoreboard";
			RegistryKey curAppRegKey;
            curAppRegKey = Registry.LocalMachine.OpenSubKey( curAppRegName, false );

            if (curAppRegKey == null) curAppRegKey = Registry.CurrentUser.OpenSubKey( curAppRegName, true );
            if (curAppRegKey == null) {
                throw new Exception( string.Format( "Registry key {0} was not found and is required", curAppRegName ) );

            } else if (curAppRegKey.GetValue( "DataSource" ) == null) {
                throw new Exception( string.Format( "Registry key {0} was not found and is required", curAppRegName ) );

            } else {
                return curAppRegKey.GetValue( "DataSource" ).ToString();
            }
        }

	}
}

using System.Data;
using System.Text;
using System.Text.Json;

#pragma warning disable CS8603
namespace LiveWebScoreboardImport.Common {
	public class HelperFunctions {
		#pragma warning disable CS8602 // Dereference of a possibly null reference.
		public static char[] TabDelim = new char[] { '\t' };
		public static char[] SingleQuoteDelim = new char[] { '\'' };
		public static String TabChar = "\t";
		public static readonly String WebDomainUri = "https://www.waterskiresults.com";
		public static readonly String PublishWebFolder = "D:\\waterskiresults.com";
		public static readonly String PublishImportFolderUri = "/publish/tournament/";
		//public static readonly String WebDomainUri = "https://localhost:7007";
		//public static readonly String PublishWebFolder = "C:\\WaterskiScoring\\LiveWebScoreboard";


		//private static String myEnv = Environment.GetEnvironmentVariable( "ASPNETCORE_ENVIRONMENT" );

		public static bool writeLogger( ILogger inLogger, String inLogType, String inMethodName, String inMsg) {
			String dateString = "";
			//if ( myEnv.Equals( "Development" ) ) dateString = DateTime.Now.ToString( "MMM dd yyyy hh:mm:ss tt" );
			if ( inLogType.Equals( "Info" ) ) inLogger.LogInformation( dateString + " " + inMethodName + inMsg );
			if ( inLogType.Equals( "Warn" ) ) inLogger.LogWarning( dateString + " " + inMethodName + inMsg );
			if ( inLogType.Equals( "Error" ) ) inLogger.LogError( dateString + " " + inMethodName + inMsg );
			if ( inLogType.Equals( "Critical" ) ) inLogger.LogCritical( dateString + " " + inMethodName + inMsg );
			if ( inLogType.Equals( "Debug" ) ) inLogger.LogDebug( dateString + " " + inMethodName + inMsg );
			return true;
		}

		public static bool isObjectEmpty( object inObject ) {
			if ( inObject == null ) return true;
			else if ( inObject == System.DBNull.Value ) return true;
			else if ( inObject.ToString().Length > 0 ) return false;
			return true;
		}
		public static bool isObjectPopulated( object inObject ) {
			if ( inObject == null ) return false;
			else if ( inObject == System.DBNull.Value ) return false;
			else if ( inObject.ToString().Length > 0 ) return true;
			return false;
		}

		public static bool IsObjectDecimal( object inObject ) {
			if ( inObject == null ) return false;
			else if ( inObject == System.DBNull.Value ) return false;
			else if ( inObject.ToString().Length > 0 ) {
				decimal curNum;
				return decimal.TryParse( inObject.ToString(), out curNum );
			}
			return false;
		}
		public static bool IsObjectLong( object inObject ) {
			if ( inObject == null ) return false;
			else if ( inObject == System.DBNull.Value ) return false;
			else if ( inObject.ToString().Length > 0 ) {
				long curNum;
				return long.TryParse( inObject.ToString(), out curNum );
			}
			return false;
		}
		public static bool IsObjectInt( object inObject ) {
			if ( inObject == null ) return false;
			else if ( inObject == System.DBNull.Value ) return false;
			else if ( inObject.ToString().Length > 0 ) {
				int curNum;
				return int.TryParse( inObject.ToString(), out curNum );
			}
			return false;
		}

		public static bool isValueTrue( String inValue ) {
			String checkValue = inValue.Trim().ToLower();
			if ( checkValue.Equals( "true" ) ) return true;
			else if ( checkValue.Equals( "false" ) ) return false;
			else if ( checkValue.Equals( "y" ) ) return true;
			else if ( checkValue.Equals( "n" ) ) return false;
			else if ( checkValue.Equals( "yes" ) ) return true;
			else if ( checkValue.Equals( "no" ) ) return false;
			else if ( checkValue.Equals( "1" ) ) return true;
			else if ( checkValue.Equals( "0" ) ) return false;
			else return false;
		}

		public static String stringReplace( String inValue, char[] inCurValue, String inReplValue ) {
			StringBuilder curNewValue = new StringBuilder( "" );

			String[] curValues = inValue.Split( inCurValue );
			if ( curValues.Length > 1 ) {
				int curCount = 0;
				foreach ( String curValue in curValues ) {
					curCount++;
					if ( curCount < curValues.Length ) {
						curNewValue.Append( curValue + inReplValue );
					} else {
						curNewValue.Append( curValue );
					}
				}

			} else {
				curNewValue.Append( inValue );
			}

			return curNewValue.ToString();
		}
		
		public static String escapeString( String inValue ) {
			String curReturnValue = "";
			char[] singleQuoteDelim = new char[] { '\'' };
			curReturnValue = HelperFunctions.stringReplace( inValue, singleQuoteDelim, "''" );
			return curReturnValue;
		}

		public static String getDataRowColValue( DataRow dataRow, String colName, String defaultValue ) {
			try {
				if ( dataRow == null ) return defaultValue;
				if ( !( dataRow.Table.Columns.Contains( colName ) ) ) return defaultValue;
				if ( dataRow[colName] == System.DBNull.Value ) return defaultValue;
				if ( dataRow[colName].GetType().Equals( typeof( String ) ) ) return ( (String)dataRow[colName] ).ToString().Trim();
				if ( dataRow[colName].GetType().Equals( typeof( int ) ) ) return ( (int)dataRow[colName] ).ToString();
				if ( dataRow[colName].GetType().Equals( typeof( Int16 ) ) ) return ( (Int16)dataRow[colName] ).ToString();
				if ( dataRow[colName].GetType().Equals( typeof( Int64 ) ) ) return ( (Int64)dataRow[colName] ).ToString();
				if ( dataRow[colName].GetType().Equals( typeof( byte ) ) ) return ( (byte)dataRow[colName] ).ToString();
				if ( dataRow[colName].GetType().Equals( typeof( bool ) ) ) return ( (bool)dataRow[colName] ).ToString();
				if ( dataRow[colName].GetType().Equals( typeof( decimal ) ) ) return ( (decimal)dataRow[colName] ).ToString( "##,###0.00" );
				if ( dataRow[colName].GetType().Equals( typeof( DateTime ) ) ) return ( (DateTime)dataRow[colName] ).ToString( "yyyy/MM/dd HH:mm:ss" );

				return ( (String)dataRow[colName] ).ToString();

			} catch {
				return defaultValue;
			}
		}

		public static String getDataRowColValueDecimal( DataRow dataRow, String colName, String defaultValue, int numDecimals ) {
			try {
				if ( dataRow == null ) return defaultValue;
				if ( !( dataRow.Table.Columns.Contains( colName ) ) ) return defaultValue;
				if ( dataRow[colName] == System.DBNull.Value ) return defaultValue;
				if ( dataRow[colName].GetType().Equals( typeof( decimal ) ) && numDecimals == 0 ) return ( (decimal)dataRow[colName] ).ToString( "##,###0" );
				if ( dataRow[colName].GetType().Equals( typeof( decimal ) ) && numDecimals == 1 ) return ( (decimal)dataRow[colName] ).ToString( "##,###0.0" );
				if ( dataRow[colName].GetType().Equals( typeof( decimal ) ) && numDecimals == 2 ) return ( (decimal)dataRow[colName] ).ToString( "##,###0.00" );
				if ( dataRow[colName].GetType().Equals( typeof( decimal ) ) && numDecimals == -1 ) return ( (decimal)dataRow[colName] ).ToString( "##,####.#" );
				if ( dataRow[colName].GetType().Equals( typeof( decimal ) ) && numDecimals == -2 ) return ( (decimal)dataRow[colName] ).ToString( "##,####.##" );
				if ( dataRow[colName].GetType().Equals( typeof( decimal ) ) ) return ( (decimal)dataRow[colName] ).ToString( "##,###0.00" );
				return getDataRowColValue( dataRow, colName, defaultValue );

			} catch {
				return "";
			}
		}
		public static decimal getDataRowColValueDecimal( DataRow dataRow, String colName, decimal defaultValue ) {
			try {
				if ( dataRow == null ) return defaultValue;
				if ( !( dataRow.Table.Columns.Contains( colName ) ) ) return defaultValue;
				if ( dataRow[colName] == System.DBNull.Value ) return defaultValue;
				if ( dataRow[colName].GetType().Equals( typeof( decimal ) ) ) return (decimal)dataRow[colName];
				return Convert.ToDecimal( getDataRowColValue( dataRow, colName, defaultValue.ToString( "##,###0.00" ) ) );

			} catch {
				return defaultValue;
			}
		}

		public static Dictionary<string, object> getAttributeList( Dictionary<string, object> msgAttributeList, String keyName ) {
			if ( !( msgAttributeList.ContainsKey( keyName ) ) ) return null;
			return (Dictionary<string, object>)msgAttributeList[keyName];
		}

		public static decimal getAttributeValueNum( Dictionary<string, object> msgAttributeList, String keyName ) {
			if ( !( msgAttributeList.ContainsKey( keyName ) ) ) return 0;

			Type curAttrType = msgAttributeList[keyName].GetType();
			if ( curAttrType == System.Type.GetType( "System.Int32" ) ) {
				if ( Decimal.TryParse( ( (int)msgAttributeList[keyName] ).ToString(), out decimal returnValue ) ) {
					return returnValue;
				}

			} else if ( curAttrType == System.Type.GetType( "System.Decimal" ) ) {
				if ( Decimal.TryParse( ( (decimal)msgAttributeList[keyName] ).ToString(), out decimal returnValue ) ) {
					return returnValue;
				}

			} else if ( curAttrType == System.Type.GetType( "System.String" ) ) {
				if ( Decimal.TryParse( (String)msgAttributeList[keyName], out decimal returnValue ) ) {
					return returnValue;
				}
			}

			return 0;
		}

		public static String getAttributeValue( Dictionary<string, object> msgAttributeList, String keyName ) {
			if ( !( msgAttributeList.ContainsKey( keyName ) ) ) return "";

			Type curAttrType = msgAttributeList[keyName].GetType();

			if ( curAttrType == System.Type.GetType( "System.Int32" ) ) {
				return ( (int)msgAttributeList[keyName] ).ToString();

			} else if ( curAttrType == System.Type.GetType( "System.Decimal" ) ) {
				return ( (decimal)msgAttributeList[keyName] ).ToString();

			} else if ( curAttrType == System.Type.GetType( "System.String" ) ) {
				return ( (String)msgAttributeList[keyName] ).Trim();
			
			} else if ( curAttrType.Name.Equals("JsonElement") ) {
				JsonElement curElement = (JsonElement)msgAttributeList[keyName];
				JsonValueKind curElementType = curElement.ValueKind;
				if ( curElementType.ToString().Equals( "Null" ) || curElementType.ToString().Equals( "Object" ) || curElementType.ToString().Equals( "Undefined" ) ) {
					return "";

				} else if ( curElementType.ToString().Equals( "String" ) || curElementType.ToString().Equals( "Number" ) ) {
					return curElement.GetString();

				} else if ( curElementType.ToString().Equals( "True" ) ) {
					return "True";

				} else if ( curElementType.ToString().Equals( "False" ) ) {
					return "False";

				} else {
					return "";
				}

			} else if ( curAttrType.Name.Equals( "JsonArray" ) ) {
				return "JsonArray";

			}

			return "";
		}

	}
}

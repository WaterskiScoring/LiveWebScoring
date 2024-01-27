using LiveWebScoreboardImport.Common;
using LiveWebScoreboardImport.Services;
using System.Data;
using System.Text;

#pragma warning disable CS8603
#pragma warning disable CS8600
namespace LiveWebScoreboardImport.Models {

	public class ImportFiles {
		private readonly ILogger myLogger;
		private readonly String myModuleName = "ImportFiles: ";

		public ImportFiles( ILogger inLogger ) {
			myLogger = inLogger;
		}

		public DataTable getPublishedFileList( String inSanctionId, String inReportType ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select PK, Event, SanctionId, ReportType, ReportTitle, ReportFilePath, LastUpdateDate" );
			curSqlStmt.Append( ", '" + HelperFunctions.WebDomainUri + "' + ReportFilePath AS ReportFileUri " );
			curSqlStmt.Append( "From PublishReport " );
			if ( inReportType.Equals( "Export" ) ) {
				curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND ReportType = '{1}' ", inSanctionId, inReportType ) );

			} else if ( inReportType.Equals( "All" ) ) {
				curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' ", inSanctionId ) );

			} else {
				curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND ReportType != 'Export' ", inSanctionId ) );
			}
			curSqlStmt.Append( "Order By ReportType, Event, LastUpdateDate Desc " );
			return DataAccess.getDataTable( curSqlStmt.ToString(), myLogger );
		}

		public string deleteFile( String inPK ) {
			String curMethodName = myModuleName + "deleteFile: ";
			String curFilePath = "";
			bool curUpdateStatus = false;

			String curReportFilePath = getFileRef( inPK );
			if ( HelperFunctions.isObjectPopulated( curReportFilePath ) ) {
				try {
					curFilePath = Path.Combine( HelperFunctions.PublishWebFolder, curReportFilePath );
					File.Delete( curFilePath );
				
				} catch ( Exception ex ) {
					HelperFunctions.writeLogger( myLogger, "Error", curMethodName, String.Format("Exception encountered deleting file {0}: Message: {1}", curFilePath, ex.Message ) );
				}
			}

			curUpdateStatus = deleteRecord( inPK );
			if ( curUpdateStatus ) {
				return String.Format( "OK. Successfully deleted {0}", inPK );
			}
			return String.Format( "OK. Request completed but record not found to be deleted {0}", inPK ); ;
		}

		public string uploadFile( String inReportType, String inSkiEvent, String inSanctionId, String inReportTitle, ImportFileUploadForm inForm ) {
			String curMethodName = myModuleName + "uploadFile: ";
			bool curUpdateStatus = false;
			String curImportFileUri = "";
			String curUploadfilePath = "";

			try {
				curImportFileUri = HelperFunctions.PublishImportFolderUri + inSanctionId + "/" + inForm.PublishFilenameBase;
				String curReportFileRelFolder = HelperFunctions.PublishImportFolderUri.Substring( 1 );
				String curSanctionReportFolder = Path.Combine( HelperFunctions.PublishWebFolder, curReportFileRelFolder.Replace( "/", "\\" ), inSanctionId );
				Directory.CreateDirectory( curSanctionReportFolder );
				curUploadfilePath = Path.Combine( curSanctionReportFolder, inForm.PublishFilenameBase );

				using ( Stream fileStream = new FileStream( curUploadfilePath, FileMode.Create, FileAccess.Write ) ) {
					inForm.PublishFile.CopyTo( fileStream );
				}

				if ( isRowFound( inReportType, inSkiEvent, inSanctionId, inReportTitle ) ) {
					curUpdateStatus = updateRecord( inReportType, inSkiEvent, inSanctionId, inReportTitle, curImportFileUri );
				} else {
					curUpdateStatus = insertRecord( inReportType, inSkiEvent, inSanctionId, inReportTitle, curImportFileUri );
				}

				if ( curUpdateStatus ) return String.Format( "OK. Successfully published {0} ({1})", inReportTitle, inForm.PublishFile.FileName );
				return String.Format( "ERROR. Failed to published {0} ({1})", inReportTitle, inForm.PublishFile.FileName ); ;
			
			} catch ( Exception ex ) {
				HelperFunctions.writeLogger( myLogger, "Error", curMethodName, String.Format( "Exception encountered uploading file {0}: Message: {1}", curUploadfilePath, ex.Message ) );
				return String.Format( "ERROR. Failed to published {0} ({1})", inReportTitle, inForm.PublishFile.FileName ); ;
			}
		}

		private bool isRowFound( String inReportType, String inSkiEvent, String inSanctionId, String inReportTitle ) {
			String curMethodName = myModuleName + "isRowFound: ";
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select PK From PublishReport " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND ReportType = '{1}' AND Event = '{2}' AND ReportTitle = '{3}' ", inSanctionId, inReportType, inSkiEvent, inReportTitle ) );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString(), myLogger );
			if ( curDataTable == null || curDataTable.Rows.Count == 0 ) return false;
			return true;
		}

		private string getFileRef( String inPK ) {
			String curMethodName = myModuleName + "getFileRef: ";
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( String.Format( "Select ReportFilePath From PublishReport Where PK = '{0}'", inPK ) );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString(), myLogger );
			if ( curDataTable == null || curDataTable.Rows.Count == 0 ) return "";

			String curReportFileUri = HelperFunctions.getDataRowColValue( curDataTable.Rows[0], "ReportFilePath", "" );
			if ( HelperFunctions.isObjectEmpty( curReportFileUri ) ) return "";
			String curReportFileRef = curReportFileUri.Substring( 1 );
			return Path.Combine( HelperFunctions.PublishWebFolder, curReportFileRef.Replace( "/", "\\" ) );
		}

		private bool insertRecord( String inReportType, String inSkiEvent, String inSanctionId, String inReportTitle, String inImportFileUri ) {
			String curMethodName = myModuleName + "insertRecord: ";
			StringBuilder curSqlStmt = new StringBuilder( "" );

			try {
				curSqlStmt.Append( "Insert INTO PublishReport ( " );
				curSqlStmt.Append( "SanctionId, ReportType, Event, ReportTitle, ReportFilePath, LastUpdateDate" );
				curSqlStmt.Append( " ) VALUES ( " );
				curSqlStmt.Append( String.Format( "'{0}', '{1}', '{2}', '{3}', '{4}', GetDate() ) ", inSanctionId, inReportType, inSkiEvent, inReportTitle, inImportFileUri ) );
				int curRowsInserted = DataAccess.ExecuteCommand( curSqlStmt.ToString(), myLogger );
				if ( curRowsInserted <= 0 ) {
					HelperFunctions.writeLogger( myLogger, "Error", curMethodName, String.Format( "Insert failed SQL={0} ", curSqlStmt.ToString() ) );
					return false;
				}

				HelperFunctions.writeLogger( myLogger, "Debug", curMethodName, String.Format( "{0} Rows successfully inserted {1}", curRowsInserted, curSqlStmt.ToString() ) );
				return true;

			} catch ( Exception ex ) {
				HelperFunctions.writeLogger( myLogger, "Error", curMethodName, "Exception encountered inserting record: Message: " + ex.Message );
				return false;
			}
		}
		private bool updateRecord( String inReportType, String inSkiEvent, String inSanctionId, String inReportTitle, String inImportFileUri ) {
			String curMethodName = myModuleName + "updateRecord: ";
			StringBuilder curSqlStmt = new StringBuilder( "" );

			try {
				curSqlStmt.Append( "Update PublishReport " );
				curSqlStmt.Append( String.Format( "Set ReportFilePath = '{0}'", inImportFileUri ) );
				curSqlStmt.Append( ", LastUpdateDate = GetDate() " );
				curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND ReportType = '{1}' AND Event = '{2}' AND ReportTitle = '{3}' ", inSanctionId, inReportType, inSkiEvent, inReportTitle ) );
				int curRowsUpdated = DataAccess.ExecuteCommand( curSqlStmt.ToString(), myLogger );
				if ( curRowsUpdated <= 0 ) {
					HelperFunctions.writeLogger( myLogger, "Error", curMethodName, String.Format( "Update failed SQL={0} ", curSqlStmt.ToString() ) );
					return false;
				}

				HelperFunctions.writeLogger( myLogger, "Debug", curMethodName, String.Format( "{0} Rows successfully updated {1}", curRowsUpdated, curSqlStmt.ToString() ) );
				return true;

			} catch ( Exception ex ) {
				HelperFunctions.writeLogger( myLogger, "Error", curMethodName, "Exception encountered updating record: Message: " + ex.Message );
				return false;
			}
		}

		private bool deleteRecord( String inPK ) {
			String curMethodName = myModuleName + "deleteRecord: ";
			StringBuilder curSqlStmt = new StringBuilder( "" );

			try {
				curSqlStmt.Append( "Delete PublishReport " );
				curSqlStmt.Append( String.Format( "Where PK = '{0}' ", inPK ) );
				int curRowsUpdated = DataAccess.ExecuteCommand( curSqlStmt.ToString(), myLogger );
				if ( curRowsUpdated <= 0 ) {
					HelperFunctions.writeLogger( myLogger, "Error", curMethodName, String.Format( "Delete failed SQL={0} ", curSqlStmt.ToString() ) );
					return false;
				}

				HelperFunctions.writeLogger( myLogger, "Debug", curMethodName, String.Format( "{0} Rows successfully deleted {1}", curRowsUpdated, curSqlStmt.ToString() ) );
				return true;

			} catch ( Exception ex ) {
				HelperFunctions.writeLogger( myLogger, "Error", curMethodName, "Exception encountered updating record: Message: " + ex.Message );
				return false;
			}
		}

	}
}

using Microsoft.AspNetCore.Mvc;

using System.Data;

using Newtonsoft.Json;

using LiveWebScoreboardImport.Common;
using LiveWebScoreboardImport.Models;
using Microsoft.AspNetCore.Http.Extensions;

namespace LiveWebScoreboardImport.Controllers {
	[Route( "api/[controller]" )]
	[ApiController]

#pragma warning disable CS8602
	public class ImportFilesController : ControllerBase {
		private readonly ILogger<ImportFilesController> myLogger;
		private readonly IConfiguration myAppCconfig;
		private readonly String myModuleName = "ImportFilesController: ";
		private ImportFiles myImportFiles;

		public ImportFilesController( ILogger<ImportFilesController> inLogger, IConfiguration inAppCconfig ) {
			myLogger = inLogger;
			myAppCconfig = inAppCconfig;
			myImportFiles = new ImportFiles( myLogger );
		}
		/*
		 * 200 OK - The service ran successfully and returned the requested information
		 * 404 NotFound - Invalid input or missing input data
		 */
		[HttpGet( Name = "ImportFilesControllerGet" )]
		public ActionResult<String> Get( String SanctionId, String ReportType ) {
			String curMethodName = myModuleName + "Get: ";

			if ( HelperFunctions.isObjectEmpty( SanctionId ) || SanctionId.Equals( "" ) ) {
				HelperFunctions.writeLogger( myLogger, "Error", curMethodName, "Input variable SanctionId not provided" );
				return BadRequest();
			}
			if ( HelperFunctions.isObjectEmpty( ReportType )
				|| !( ReportType.Equals( "Export" ) || ReportType.Equals( "Report" ) || ReportType.Equals( "All" ) ) ) {
				HelperFunctions.writeLogger( myLogger, "Error", curMethodName, "Input variable ReportType invalid or not provided" );
				return BadRequest();
			}
			HelperFunctions.writeLogger( myLogger, "Info", curMethodName, String.Format( "Using input SanctionId={0}, ReportType={1}", SanctionId, ReportType ) );
			DataTable curDataTable = myImportFiles.getPublishedFileList( SanctionId, ReportType );
			if ( curDataTable == null ) NotFound();

			HelperFunctions.writeLogger( myLogger, "Info", curMethodName, String.Format( "{0} records retrieved ", curDataTable.Rows.Count ) );
			return JsonConvert.SerializeObject( curDataTable );
		}

		/*
		 * 201 CreatedAtAction - The service ran successfully and updated the database for the data provided
		 * 400 BadRequest  - Invalid or missing input data
		 * ReportType (Results, Other, RunOrder, Export)
		 * Event (Overall, Tour, Trick, Slalom, Export, Jump)
		 */
		[HttpPost( Name = "ImportFilesControllerPost" )]
		[Consumes( "multipart/form-data" )]
		public ActionResult<String> Post( String ReportType, String SkiEvent, String SanctionId, String ReportTitle, [FromForm] ImportFileUploadForm inForm ) {
			String curMethodName = myModuleName + "Post: ";
			String curMsg;

			if ( HelperFunctions.isObjectEmpty( Request.GetMultipartBoundary() ) ) {
				return BadRequest( handleErrorCondition(curMethodName, "Multipart boundary not found" ) );
			}
			if ( HelperFunctions.isObjectEmpty( ReportType ) ) {
				return BadRequest( handleErrorCondition( curMethodName, "Required ReportType is empty or not provided" ) );
			}
			if ( HelperFunctions.isObjectEmpty( SkiEvent ) ) {
				return BadRequest( handleErrorCondition( curMethodName, "Required SkiEvent is empty or not provided" ) );
			}
			if ( HelperFunctions.isObjectEmpty( SanctionId ) ) {
				return BadRequest( handleErrorCondition( curMethodName, "Required SanctionId is empty or not provided" ) );
			}
			if ( HelperFunctions.isObjectEmpty( ReportTitle ) ) {
				return BadRequest( handleErrorCondition( curMethodName, "Required ReportTitle is empty or not provided" ) );
			}
			if ( inForm == null ) {
				return BadRequest( handleErrorCondition( curMethodName, "Required input form with file attachment is empty or not provided" ) );
			}
			if ( HelperFunctions.isObjectEmpty( inForm.PublishFilename ) ) {
				return BadRequest( handleErrorCondition( curMethodName, "Required PublishFilename is empty or not provided" ) );
			}
			if ( HelperFunctions.isObjectEmpty( inForm.PublishFilenameBase ) ) {
				return BadRequest( handleErrorCondition( curMethodName, "Required PublishFilenameBase is empty or not provided" ) );
			}
			if ( HelperFunctions.isObjectEmpty( inForm.PublishFile ) ) {
				return BadRequest( handleErrorCondition( curMethodName, "Required PublishFile is empty or not provided" ) );
			}
			if ( HelperFunctions.isObjectEmpty( inForm.PublishFile.Length == 0 ) ) {
				return BadRequest( handleErrorCondition( curMethodName, "Required PublishFile is empty or not provided" ) );
			}

			curMsg = String.Format( "ReportType={0}, SkiEvent={1}, SanctionId={2}, ReportTitle={3}, inForm.PublishFile.FileName=={4}, FileLength={5}, PublishFilenameBase={6}, PublishFilename={7}"
				, ReportType, SkiEvent, SanctionId, ReportTitle, inForm.PublishFile.FileName, inForm.PublishFile.Length, inForm.PublishFilenameBase, inForm.PublishFilename );
			HelperFunctions.writeLogger( myLogger, "Info", curMethodName, curMsg );

			curMsg = myImportFiles.uploadFile( ReportType, SkiEvent, SanctionId, ReportTitle, inForm );
			if ( curMsg.Substring( 0, 2).Equals("OK" ) ) return Ok( formatResponseMsg( curMsg ) );
			return BadRequest( formatResponseMsg( curMsg ) );
		}

		/*
		 * 201 CreatedAtAction - The service ran successfully and updated the database for the data provided
		 * 400 BadRequest  - Invalid or missing input data
		 * ReportType (Results, Other, RunOrder, Export)
		 * Event (Overall, Tour, Trick, Slalom, Export, Jump)
		 */
		[HttpDelete( Name = "ImportFilesControllerDelete" )]
		public ActionResult<String> Delete( String PK ) {
			String curMethodName = myModuleName + "Delete: ";
			String curMsg;

			if ( HelperFunctions.isObjectEmpty( PK ) ) {
				return BadRequest( handleErrorCondition( curMethodName, "Required PK is empty or not provided" ) );
			}
			curMsg = String.Format( "PK={0}", PK );
			HelperFunctions.writeLogger( myLogger, "Info", curMethodName, curMsg );

			curMsg = myImportFiles.deleteFile( PK );
			if ( curMsg.Substring( 0, 2 ).Equals( "OK" ) ) return Ok( formatResponseMsg( curMsg ) );
			return BadRequest( curMsg );
		}

		private string handleErrorCondition( String inMethodName, String inMsg) {
			HelperFunctions.writeLogger( myLogger, "Error", inMethodName, inMsg );
			formatResponseMsg( inMsg );
			return inMsg;

		}
		private string formatResponseMsg( String inMsg ) {
			String curValue = String.Format( "{{Message: \"{0}\"}}", inMsg );
			curValue = String.Format( "{{\"Message\": \"{0}\"}}", inMsg );
			return String.Format( "{{\"Message\": \"{0}\"}}", inMsg );
		}


	}
}

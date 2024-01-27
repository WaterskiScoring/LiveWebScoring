using Microsoft.AspNetCore.Mvc;

using System.Data;
using System.Text.Json;

using Newtonsoft.Json;

using LiveWebScoreboardImport.Common;
using LiveWebScoreboardImport.Models;

namespace LiveWebScoreboardImport.Controllers {
    [Route( "api/[controller]" )]
	[ApiController]
	public class ImportScoresController : ControllerBase {
		private readonly ILogger<ImportScoresController> myLogger;
		private readonly IConfiguration myAppCconfig;
		private readonly String myModuleName = "ImportScoresController: ";
		private ImportScores myImportScores;

		public ImportScoresController( ILogger<ImportScoresController> inLogger, IConfiguration inAppCconfig ) {
			myLogger = inLogger;
			myAppCconfig = inAppCconfig;
			myImportScores = new ImportScores( myLogger );
		}
		/*
		 * 200 OK - The service ran successfully and returned the requested information
		 * 404 NotFound - Invalid input or missing input data
		 */
		[HttpGet( Name = "ImportScoresControllerGet" )]
		public ActionResult<String> Get(String SanctionId) {
			String curMethodName = myModuleName + "Get: ";

			if ( HelperFunctions.isObjectEmpty( SanctionId ) || SanctionId.Equals( "xxxxxx" ) ) {
				HelperFunctions.writeLogger( myLogger, "Error", curMethodName, "Input variable SanctionId not provided" );
				return BadRequest();
			}
			HelperFunctions.writeLogger( myLogger, "Info", curMethodName, String.Format( "Using input SanctionId={0}", SanctionId ) );
			DataTable curDataTable = myImportScores.getTournament( SanctionId );
			if ( curDataTable == null )  NotFound();

			return JsonConvert.SerializeObject( curDataTable );
		}

		/*
		 * 201 CreatedAtAction - The service ran successfully and updated the database for the data provided
		 * 400 BadRequest  - Invalid or missing input data
		 */
		[HttpPost( Name = "ImportScoresControllerPost" )]
		[Consumes( "application/json; charset=utf-8" )]
		public ActionResult<String> Post( [FromBody] JsonDocument JsonDoc ) {
			String curMethodName = myModuleName + "Post: ";
			HelperFunctions.writeLogger( myLogger, "Debug", curMethodName, "JsonDoc=" + JsonDoc );

			String curReturnMsg = myImportScores.importEventScores( JsonDoc );
			if ( curReturnMsg.StartsWith( "OK" ) ) return new CreatedResult( "api/[controller]", curReturnMsg );
			return BadRequest( curReturnMsg );
		}

	}
}

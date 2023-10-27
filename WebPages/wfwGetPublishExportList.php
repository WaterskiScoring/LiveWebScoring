<?php
error_reporting(E_ALL);
//phpinfo();
ini_set("display_errors", 1);
include_once( "WfwInit.php" );

/* ****************************************
Process input requests
**************************************** */
if ( !isset($_GET['sanctionNum']) ) {
	echo "{\"Message\": \"All required values not provided!!!\"};";
	exit(400);
}

$sanctionNum = $_GET['sanctionNum'];
$mySqlStmt = "Select PK, Event, SanctionId, ReportType, ReportTitle, ReportFilePath, LastUpdateDate From PublishReport "
	. "Where SanctionId = '" . $sanctionNum . "' AND ReportType = 'Export' Order By LastUpdateDate Desc";
$queryResult = $dbConnect->query($mySqlStmt);
if ($dbConnect->error) {
	echo "[{\"Message\": \"Errors detected on SQL request: " . $dbConnect->error . "\"}]";
	exit(400);

} else if ( $queryResult && $queryResult->num_rows > 0 ) {
	$returnArray = array();
	while ($SqlRow = $queryResult->fetch_assoc()) {
		array_push($returnArray, $SqlRow); 
	}

	echo json_encode($returnArray);
	include_once( "WfwTerm.php" );
	exit(200);

} else {
	echo "[{ \"Message\": \"No data found\"}]";
	include_once( "WfwTerm.php" );
	exit(400);
}

?>

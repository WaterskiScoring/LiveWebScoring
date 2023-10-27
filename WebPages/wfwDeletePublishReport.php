<?php
error_reporting(E_ALL);
//phpinfo();
ini_set("display_errors", 1);
include_once( "WfwInit.php" );

/* ****************************************
Process input requests
**************************************** */
if ( !isset($_GET['PK']) ) {
	echo "{\"Message\": \"All required values not provided!!!\"};";
	exit(400);
}

$reportPK = $_GET['PK'];
$deleteMsg =  "";

$mySqlStmt = "Select PK, Event, SanctionId, ReportType, ReportTitle, ReportFilePath From PublishReport Where PK = " . $reportPK;
$queryResult = $dbConnect->query($mySqlStmt);
if ($dbConnect->error) {
	echo "{\"Message\": \"Errors detected on SQL request: " . $dbConnect->error . "\"}";
	exit(400);

} else if ( $queryResult && $queryResult->num_rows > 0 ) {
	$returnArray = array();
	$SqlRow = $queryResult->fetch_assoc();
	
	$reportFilePath = $SqlRow["ReportFilePath"];
	$reportTitle = $SqlRow["ReportTitle"];
	if (unlink($reportFilePath)) {
		$deleteMsg = $reportTitle . " has been deleted";
	} else {
		$deleteMsg = $reportTitle . " unable to delete";
	}	

	$mySqlStmt = "Delete From PublishReport Where PK = " . $reportPK ;
	$queryResult = $dbConnect->query($mySqlStmt);
	if ($dbConnect->error) {
		echo "{\"Message\": \"Errors detected on SQL request: " . $dbConnect->error . "\"}";
		exit(400);

	} else {
		echo "{ \"Message\": \"Report deleted, " . $deleteMsg . "\"}";
		include_once( "WfwTerm.php" );
		exit(200);
	}

	include_once( "WfwTerm.php" );
	exit(200);

} else {
	echo "{ \"Message\": \"No data found\"}";
	include_once( "WfwTerm.php" );
	exit(400);
}


?>

<?php
error_reporting(E_ALL);
//phpinfo();
ini_set("display_errors", 1);
include_once( "WfwInit.php" );

/* ****************************************
**************************************** */
function isRowFound($dbConnect, $sanctionNum, $reportType, $skiEvent, $reportTitle) {
	$mySqlStmt = "Select PK From PublishReport ";
	$mySqlStmt .= "Where SanctionId = '" . $sanctionNum . "'";
	$mySqlStmt .= "  AND ReportType = '" . $reportType . "'";
	$mySqlStmt .= "  AND Event = '" . $skiEvent . "'";
	$mySqlStmt .= "  AND ReportTitle = '" . $reportTitle . "'";
	$result = $dbConnect->query($mySqlStmt);
	if ($dbConnect->error) {
		echo ", \"Message\": \"Errors detected on SQL request: " . $dbConnect->error . "\"";
		return false;
	} else {
		if ( $result ) {
			$num_rows = $result->num_rows;
			if ( $num_rows > 0 ) {
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}
	}
}

function updateRow($dbConnect, $sanctionNum, $reportType, $skiEvent, $reportTitle, $uploadFilePath) {
	$mySqlStmt = "Update PublishReport ";
	$mySqlStmt .= "Set ReportFilePath = '" . $uploadFilePath . "' ";
	$mySqlStmt .= ", LastUpdateDate = CURRENT_TIMESTAMP() ";
	$mySqlStmt .= "Where SanctionId = '" . $sanctionNum . "'";
	$mySqlStmt .= "  AND ReportType = '" . $reportType . "'";
	$mySqlStmt .= "  AND Event = '" . $skiEvent . "'";
	$mySqlStmt .= "  AND ReportTitle = '" . $reportTitle . "'";
	$result = $dbConnect->query($mySqlStmt);
	if ($dbConnect->error) {
		echo ", \"Message\": \"Errors detected on SQL request: " . $dbConnect->error . "\"";
		return false;
	} else {
		return true;
	}
}

function insertRow($dbConnect, $sanctionNum, $reportType, $skiEvent, $reportTitle, $uploadFilePath) {
	$mySqlStmt = "Insert INTO PublishReport ( ";
	$mySqlStmt .= "SanctionId, ReportType, Event, ReportTitle, ReportFilePath, LastUpdateDate";
	$mySqlStmt .= " ) VALUES (";
	$mySqlStmt .= " '" . $sanctionNum . "'";
	$mySqlStmt .= ", '" . $reportType . "'";
	$mySqlStmt .= ", '" . $skiEvent . "'";
	$mySqlStmt .= ", '" . $reportTitle . "'";
	$mySqlStmt .= ", '" . $uploadFilePath . "'";
	$mySqlStmt .= ", CURRENT_TIMESTAMP() ";
	$mySqlStmt .= " );";
	$result = $dbConnect->query($mySqlStmt);
	if ($dbConnect->error) {
		echo ", \"Message\": \"Errors detected on SQL request: " . $dbConnect->error . "\"";
		return false;
	} else {
		return true;
	}
}


$currentDir = getcwd();
$publishDir = "/home/awsaeast/public_html/scoring/Tournament";

$skiEvent = "";
$sanctionNum = "";
$reportTitle = "";

/* ****************************************
Process input requests
**************************************** */
if ( !isset($_GET['sanctionNum'],$_GET['skiEvent'], $_GET['reportTitle']) ) {
	echo "{\"Message\": \"All required values not provided!!!\"}";
	exit(400);
}

$sanctionNum = $_GET['sanctionNum'];
$reportType = $_GET['reportType'];
$skiEvent = $_GET['skiEvent'];
$reportTitle = $_GET['reportTitle'];


if ( !is_dir($publishDir) ) {
	mkdir($publishDir, 0770, true);
}
echo "{ \"sanctionNum\": \"$sanctionNum\"";
echo ", \"reportType\": \"$reportType\"";
echo ", \"skiEvent\": \"$skiEvent\"";
echo ", \"reportTitle\": \"$reportTitle\"";

$reportFilename = $_POST['reportFilename'];
$reportFilenameBase = $_POST['reportFilenameBase'];
$inputFilename = $_FILES['PublishReport']['name'];
$inputFiletype = $_FILES['PublishReport']['type'];
$inputFileSize = $_FILES['PublishReport']['size'];
$inputFilenameTmp  = $_FILES['PublishReport']['tmp_name'];
echo ", \"reportFilenameBase\": \"$reportFilenameBase\"";
echo ", \"inputFilename\": \"$inputFilename\"";
echo ", \"inputFileSize\": \"$inputFileSize\"";
echo ", \"inputFiletype\": \"$inputFiletype\"";

$uploadFolder = $publishDir . "/" . $sanctionNum;
if ( !is_dir($uploadFolder) ) {
	mkdir($uploadFolder, 0755, true);
}
$uploadFilePath = $uploadFolder . "/" . $inputFilename;

if (move_uploaded_file($inputFilenameTmp, $uploadFilePath)) {
	if ( isRowFound($dbConnect, $sanctionNum, $reportType, $skiEvent, $reportTitle)) {
		$rowStatus = updateRow($dbConnect, $sanctionNum, $reportType, $skiEvent, $reportTitle, $uploadFilePath);
	} else {
		$rowStatus = insertRow($dbConnect, $sanctionNum, $reportType, $skiEvent, $reportTitle, $uploadFilePath);
	}
	if ( $rowStatus ) {
		echo ", \"Message\": \"Successfully published $reportTitle ($reportFilenameBase)\"";
	}

} else {
	echo ", \"Message\": \"Error encountered uploading report $reportTitle ($reportFilenameBase)\"";
}
echo "}";
include_once( "WfwTerm.php" );
?>

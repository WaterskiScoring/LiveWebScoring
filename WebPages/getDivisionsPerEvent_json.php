<?php
error_reporting(E_ALL);
ini_set("display_errors", 1);
include_once( "WfwInit.php" );

$eventDivisions=array();

if (isset($_POST['sanctionID']) && isset($_POST['skiEvent'])) {
	$curSanctionID = $_POST['sanctionID'];
	$curSkiEvent = $_POST['skiEvent'];

	// Retrieve divisions currently scored for the specified tournament and event
	if ( $curSkiEvent = 'Overall' ) {
		$QueryCmd = "SELECT DISTINCT AgeGroup FROM EventReg WHERE SanctionID='" . $curSanctionID . "' ORDER BY AgeGroup ASC";
	} else {
		$QueryCmd = "SELECT DISTINCT AgeGroup FROM EventReg WHERE SanctionID='" . $curSanctionID . "' AND Event='" . $curSkiEvent . "' ORDER BY AgeGroup ASC";
	}
	$QueryResult = mysql_query($QueryCmd) or die (mysql_error());
	$curRowCount = mysql_num_rows($QueryResult);
	if ( $curRowCount != 0 ) {
		while ($curRow = mysql_fetch_assoc($QueryResult)) {
			$eventDivisions[] = $curRow['AgeGroup'];
		}
		$eventDivisions[] = 'All';
		$eventDivisions[] = 'Recent';
	} else {
		echo "<span class='noScores'>No Divisions have been scored yet.</span>" . $_POST['sanctionID'] . " - " . $_POST['skiEvent'];
	}
    /* close statement */
	mysql_free_result($QueryResult);

	echo json_encode($eventDivisions);
} else {
	echo "Tournament sanction and event information not provided: SanctionId=" . $_POST['sanctionID'] . " - Evemt=" . $_POST['skiEvent'];
}
?>
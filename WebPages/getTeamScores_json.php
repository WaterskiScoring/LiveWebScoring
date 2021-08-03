<?php
error_reporting(E_ALL);
ini_set("display_errors", 1);
session_start(); // base page pulling this file in already starts session
include_once( "WfwInit.php" );

function checkReqVars() {
  return isset($_POST['sanctionID']);
}

$QueryCmd = "";
if (checkReqVars()) {
	$thisSanc = $_POST['sanctionID'];
	$curRegion = strtoupper(substr($thisSanc, 2, 1));

	$SortOrder = "S.AgeGroup, S.OverallPlcmt, SD.SkierCategory, SD.LineNum ";

	$QueryCmd = "Select S.SanctionId, S.TeamCode, S.AgeGroup, Name, ReportFormat, SD.SkierCategory, LineNum"
		. ", S.OverallPlcmt AS OverallPlcmtTeam, S.SlalomPlcmt AS SlalomPlcmtTeam, S.TrickPlcmt AS TrickPlcmtTeam, S.JumpPlcmt AS JumpPlcmtTeam"
		. ", S.OverallScore AS OverallScoreTeam, S.SlalomScore AS SlalomScoreTeam, S.TrickScore AS TrickScoreTeam, S.JumpScore AS JumpScoreTeam"
		. ", SlalomSkierName, SD.SlalomPlcmt, SD.SlalomScore, SlalomNops, SlalomPoints"
		. ", TrickSkierName, SD.TrickPlcmt, SD.TrickScore, TrickNops, TrickPoints"
		. ", JumpSkierName, SD.JumpPlcmt, SD.JumpScore, JumpNops, JumpPoints "
		. "From TeamScore S "
		. "Inner Join TeamScoreDetail SD on S.SanctionId = SD.SanctionId AND S.TeamCode = SD.TeamCode AND S.AgeGroup = SD.AgeGroup ";
	$QueryCmd = $QueryCmd . "Order by " . $SortOrder;

	$QueryResult = $dbConnect->query($QueryCmd) or die ($dbConnect->error);

	$curRowCount = $QueryResult->num_rows;
?>
<style>
.TeamScore {
	Border: 5px #DDDDDD outset;
	Background: #396B9E;
	Color: White;
	font-size:1.1em;
	margin: auto;
	text-align:center;
}
.LastSkied {
	float:right;
	display:inline-block;
}

</style>
<?php
	if ( $curRowCount > 0 ) {
		$prevEventGroup = '';

		echo "\r\n<h2><p class='centeredItalic'>These are unofficial, repeat <span class='alertNotice'>UNOFFICIAL</span></p></h2>";
		echo "\r\n<ul data-role='listview' id='TeamScoreID'>";
		while ($curDataRow = $QueryResult->fetch_assoc()) {
			$curEventGroup = $curDataRow['TeamCode'] . "-" . $curDataRow['AgeGroup'];
			if ( $curEventGroup == $prevEventGroup ) {
			} else {
				echo "\r\n<div data-role='header' class='RunOrderGroup'>Event Group: " . $curDataRow['EventGroup'] . "</div>";
			}

			echo "\r\n<li>";
			echo $curDataRow['SlalomSkierName']
				. $curDataRow['TrickSkierName']
				. $curDataRow['JumpSkierName'];
				. "</li>";

			$prevEventGroup = $curEventGroup;
		}

		echo "\r\n</ul><!-- /listview -->";
		$QueryResult->free();
	} else {
		echo "<span class='noScores'>No team scores available yet</span>";
	}

} else {
	echo "All required values not sent!!!";
}

?>
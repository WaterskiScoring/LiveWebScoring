<?php
error_reporting(E_ALL);
ini_set("display_errors", 99);
session_start();
if(!isset($_SESSION['sanctionID']) || empty($_SESSION['sanctionID'])) {
   header('Location: ../index.php');
}
include_once( "WfwInit.php" );

$thisSanctionId = $_SESSION['sanctionID'];
?>

<!DOCTYPE html>
<html>
<head>
	<title>Tournament Team Scores</title>
	<link rel="stylesheet" href="https://code.jquery.com/mobile/1.3.1/jquery.mobile-1.3.1.min.css" />
	<link rel="stylesheet" href="wfwMobile.css">

	<script src="https://code.jquery.com/jquery-1.9.1.min.js"></script>
	<script src="https://code.jquery.com/mobile/1.3.1/jquery.mobile-1.3.1.min.js"></script>
	<script type="text/javascript" src="wfwMobile.js"></script>

	<link rel="apple-touch-icon" href="customIcon.png" />
	<meta name="apple-mobile-web-app-capable" content="yes" />
	<meta name="apple-mobile-web-app-status-bar-style" content="black" />

	<style>
	.TeamChange {
		Background: Silver;
		font-size: 25%;
	}
	</style>
</head>

<body>
<!--here-->
<div data-role="page" id="getRunOrder">

	<div data-role="header">
    	<a href='wfwShowTourScores.php' class='ui-btn-left' data-role='button' data-icon='back' data-mini='true' data-ajax='false'></a>
		<h1><?php echo "Sanction: " . $thisSanctionId; ?></h1>
        <a id="ui-header-refresh" href='#' onclick='reloadPage()' class='ui-btn-right RefreshLink' data-role='button' data-icon='refresh' data-mini='true' data-ajax='true'>Refresh</a>

		<hr />
        <p class="centeredItalic">These team scores <span class="alertNotice">UNOFFICIAL</span></p>
	</div><!-- /header -->

    <div data-role="content" data-theme="c">

		<Table>
			<tr>
				<th>Format</th>
				<th>Div</th>
				<th>Team</th>
				<th>Name</th>
				<th>Overall<br/>Plcmt</th>
				<th>Overall<br/>Score</th>
				<th>Slalom<br/>Plcmt</th>
				<th>Slalom<br/>Score</th>
				<th>Trick<br/>Plcmt</th>
				<th>Trick<br/>Score</th>
				<th>Jump<br/>Plcmt</th>
				<th>Jump<br/>Score</th>
			</tr>
		<?php
			$curRegion = strtoupper(substr($thisSanctionId, 2, 1));
			$QueryCmd = "Select S.SanctionId, S.TeamCode, S.AgeGroup, Name, ReportFormat"
				. ", S.OverallPlcmt AS OverallPlcmtTeam, S.SlalomPlcmt AS SlalomPlcmtTeam, S.TrickPlcmt AS TrickPlcmtTeam, S.JumpPlcmt AS JumpPlcmtTeam"
				. ", S.OverallScore AS OverallScoreTeam, S.SlalomScore AS SlalomScoreTeam, S.TrickScore AS TrickScoreTeam, S.JumpScore AS JumpScoreTeam "
				. "From TeamScore S "
				. "Where S.SanctionId = '" . $thisSanctionId . "' "
				. "Order by S.AgeGroup, S.OverallPlcmt ";

			$QueryResult = $dbConnect->query($QueryCmd) or die ($dbConnect->error);
			$curRowCount = $QueryResult->num_rows;

			if ( $curRowCount > 0 ) {
				$curAgeGroup = '';
				$prevAgeGroup = '';

				while ($curDataRow = $QueryResult->fetch_assoc()) {
					$curAgeGroup = $curDataRow['AgeGroup'];

					if ( $curAgeGroup != $prevAgeGroup ) {
						echo "\r\n<tr Class='TeamChange'><td colspan='12'>&nbsp;</td></tr>";
					}

					echo "\r\n<tr>";
					echo "\r\n<td>" . $curDataRow['ReportFormat'] . "</td>";
					echo "\r\n<td>" . $curDataRow['AgeGroup'] . "</td>";
					echo "\r\n<td>" . $curDataRow['TeamCode'] . "</td>";
					echo "\r\n<td>" . $curDataRow['Name'] . "</td>";

					echo "\r\n<td>" . $curDataRow['OverallPlcmtTeam'] . "</td>";
					echo "\r\n<td>" . $curDataRow['OverallScoreTeam'] . "</td>";
					echo "\r\n<td>" . $curDataRow['SlalomPlcmtTeam'] . "</td>";
					echo "\r\n<td>" . $curDataRow['SlalomScoreTeam'] . "</td>";
					echo "\r\n<td>" . $curDataRow['TrickPlcmtTeam'] . "</td>";
					echo "\r\n<td>" . $curDataRow['TrickScoreTeam'] . "</td>";
					echo "\r\n<td>" . $curDataRow['JumpPlcmtTeam'] . "</td>";
					echo "\r\n<td>" . $curDataRow['JumpScoreTeam'] . "</td>";
					echo "\r\n</tr>\r\n";

					$prevAgeGroup = $curDataRow['AgeGroup'];
				}
			} else {
				echo "<span class='noScores'>No running orders available yet for " . $_SESSION['divisionID'] . ".</span>";
			}
		?>
		</Table><!-- /listview -->
		<br/>this is a test

	</div><!-- /content -->

</div><!-- /page -->

<div id="bgDiv" style="position:absolute; top:0; left:0; width:100%; height:100%; margin:0; padding:0; background-image:url('Images/unofficial.jpg');background-size:cover; opacity:1; z-index:-1"></div>

<script type="text/javascript">
$('body').hide();
$('body').fadeIn(1500);
</script>

</body>
</html>


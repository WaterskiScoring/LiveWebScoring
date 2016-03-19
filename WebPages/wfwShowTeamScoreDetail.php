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
	<link rel="stylesheet" href="http://code.jquery.com/mobile/1.3.1/jquery.mobile-1.3.1.min.css" />
	<link rel="stylesheet" href="wfwMobile.css">

	<script src="http://code.jquery.com/jquery-1.9.1.min.js"></script>
	<script src="http://code.jquery.com/mobile/1.3.1/jquery.mobile-1.3.1.min.js"></script>
	<script type="text/javascript" src="wfwMobile.js"></script>

	<link rel="apple-touch-icon" href="customIcon.png" />
	<meta name="apple-mobile-web-app-capable" content="yes" />
	<meta name="apple-mobile-web-app-status-bar-style" content="black" />
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
				<th>ReportFormat</th>
				<th>TeamCode</th>
				<th>Name</th>
				<th>SkierCategory</th>
				<th>AgeGroup</th>
				<th>LineNum</th>
				<th>SlalomSkierName</th>
				<th>TrickSkierName</th>
				<th>JumpSkierName</th>
			</tr>
		<?php
			$curRegion = strtoupper(substr($thisSanctionId, 2, 1));

			$QueryCmd = "Select S.SanctionId, S.TeamCode, S.AgeGroup, Name, ReportFormat, SD.SkierCategory, LineNum"
				. ", S.OverallPlcmt AS OverallPlcmtTeam, S.SlalomPlcmt AS SlalomPlcmtTeam, S.TrickPlcmt AS TrickPlcmtTeam, S.JumpPlcmt AS JumpPlcmtTeam"
				. ", S.OverallScore AS OverallScoreTeam, S.SlalomScore AS SlalomScoreTeam, S.TrickScore AS TrickScoreTeam, S.JumpScore AS JumpScoreTeam"
				. ", SlalomSkierName, SD.SlalomPlcmt, SD.SlalomScore, SlalomNops, SlalomPoints"
				. ", TrickSkierName, SD.TrickPlcmt, SD.TrickScore, TrickNops, TrickPoints"
				. ", JumpSkierName, SD.JumpPlcmt, SD.JumpScore, JumpNops, JumpPoints "
				. "From TeamScore S "
				. "Inner Join TeamScoreDetail SD on S.SanctionId = SD.SanctionId AND S.TeamCode = SD.TeamCode AND S.AgeGroup = SD.AgeGroup "
				. "Where S.SanctionId = '" . $thisSanctionId . "'"
				. "Order by S.AgeGroup, S.OverallPlcmt, SD.SkierCategory, SD.LineNum ";

			$QueryResult = mysql_query($QueryCmd) or die (mysql_error());
			$curDataRow = mysql_num_rows($QueryResult);
			if ( $curDataRow != 0 ) {
				$curTeam = '';
				$prevTeam = '';
				
				while ($curDataRow = mysql_fetch_assoc($QueryResult)) {
					$curTeam = $curDataRow['TeamCode'];
					
					if ( $curTeam == $prevTeam ) {
						echo "\r\n<tr><td>-----------------</td></tr>";
						
						echo "\r\n<tr>";
						echo "\r\n<td>.</td>";
						echo "\r\n<td>.</td>";
						echo "\r\n<td>.</td>";
						echo "\r\n<td>.</td>";
						echo "\r\n<td>.</td>";
						echo "\r\n<td>.</td>";
					} else {
						echo "\r\n<tr>";
						echo "\r\n<td>" . $curDataRow['ReportFormat'] . "</td>";
						echo "\r\n<td>" . $curDataRow['TeamCode'] . "</td>";
						echo "\r\n<td>" . $curDataRow['Name'] . "</td>";
						echo "\r\n<td>" . $curDataRow['LineNum'] . "</td>";
						echo "\r\n<td>" . $curDataRow['SkierCategory'] . "</td>";
						echo "\r\n<td>" . $curDataRow['AgeGroup'] . "</td>";
					}
					
					echo "\r\n<td>" . $curDataRow['SlalomSkierName'] . "</td>";
					echo "\r\n<td>" . $curDataRow['TrickSkierName'] . "</td>";
					echo "\r\n<td>" . $curDataRow['JumpSkierName'] . "</td>";
					echo "\r\n</tr>\r\n";
					
					$prevTeam = $curDataRow['TeamCode'];
				}
			} else {
				echo "<span class='noScores'>No running orders available yet for " . $_SESSION['divisionID'] . ".</span>";
			}
		?>
		</Table><!-- /listview -->
		<br/>this is a test

	</div><!-- /content -->

</div><!-- /page -->

<div id="bgDiv" style="position:absolute; top:0; left:0; width:100%; height:100%; margin:0; padding:0; background-image:url('unofficial.jpg');background-size:cover; opacity:1; z-index:-1"></div>

<script type="text/javascript">
$('body').hide();
$('body').fadeIn(1500);
</script>

</body>
</html>


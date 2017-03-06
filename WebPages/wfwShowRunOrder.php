<?php
error_reporting(E_ALL);
ini_set("display_errors", 99);
session_start();
if(!isset($_SESSION['sanctionID']) || empty($_SESSION['sanctionID'])) {
   header('Location: index.php');
}
include_once( "WfwInit.php" );


?>

<!DOCTYPE html>
<html>
<head>
	<title>Tournament Running Order</title>
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
    	<a href='wfwShowTourScores.php' class='ui-btn-left' data-role='button' data-icon='back' data-mini='true' data-ajax='false'>Scores</a>
		<h1><?php echo "Sanction: " . $_SESSION['sanctionID'] . " Event: " . $_SESSION['skiEvent'];?></h1>
        <a id="ui-header-refresh" href='#' onclick='reloadPage()' class='ui-btn-right RefreshLink' data-role='button' data-icon='refresh' data-mini='true' data-ajax='true'>Refresh</a>

		<hr />
        <p class="centeredItalic">These running orders are <span class="alertNotice">UNOFFICIAL</span></p>
	</div><!-- /header -->

    <div data-role="content" data-theme="c">

		<Table>
			<tr>
			<th>Skier</th>
			<th>Group</th>
			<th>Division</th>
			<th>Class</th>
			<th>Order</th>
			<th>RankingScore</th>
			<th>Team</th>
			</tr>
		<?php
			$SqlCmd = "Select TR.SanctionId, TR.MemberId, TR.SkierName, TR.AgeGroup "
			. ", ER.Event, ER.EventGroup, ER.EventClass, ER.RunOrder, ER.RankingScore, ER.TeamCode "
			. "From TourReg TR "
			. "Inner Join EventReg ER on ER.SanctionId = TR.SanctionId AND ER.MemberId = TR.MemberId AND TR.AgeGroup = ER.AgeGroup "
			. "Where TR.SanctionId = '" .  $_SESSION['sanctionID'] . "' AND ER.Event = '" .  $_SESSION['skiEvent'] . "' "
			. "Order by TR.SanctionId, ER.Event, ER.EventGroup, TR.MemberId, ER.RunOrder, ER.RankingScore ";

			$QueryResult = $dbConnect->query($SqlCmd) or die ($dbConnect->error);
			$curRowCount = $QueryResult->num_rows;

			if ( $curRowCount > 0 ) {
				while ($curDataRow = $QueryResult->fetch_assoc()) {
					echo "\r\n<tr>";
					echo "\r\n<td>" . $curDataRow['SkierName'] . "</td>";
					echo "\r\n<td>" . $curDataRow['EventGroup'] . "</td>";
					echo "\r\n<td>" . $curDataRow['AgeGroup'] . "</td>";
					echo "\r\n<td>" . $curDataRow['EventClass'] . "</td>";
					echo "\r\n<td>" . $curDataRow['RunOrder'] . "</td>";
					echo "\r\n<td>" . $curDataRow['RankingScore'] . "</td>";
					echo "\r\n<td>" . $curDataRow['TeamCode'] . "</td>";
					echo "\r\n</tr>\r\n";
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


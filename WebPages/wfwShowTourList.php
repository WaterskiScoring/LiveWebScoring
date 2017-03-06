<?php
error_reporting(E_ALL);
ini_set("display_errors", 1);
session_start();
include_once( "WfwInit.php" );
?>

<!DOCTYPE html>
<html>
<head>
	<title>Tournament List</title>
    <meta name="apple-mobile-web-app-capable" content="yes" />
	<meta name="viewport" content="width=device-width, initial-scale=1">

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
<div data-role="page" id="tourneyList">

	<div data-role="header">
		<a href='wfwShowScoreboard.php' class='ui-btn-left' data-role='button' data-icon='home' data-mini='true' data-ajax='false'>Home</a>
		<h1>Tournament List</h1>
	</div><!-- /header -->

    <div data-role="content" data-theme="c">
		<p class="centeredItalic">Most recent 10 scores from around the country</p>
        <a href="wfwShowRecentScores.php" data-role="button" data-theme="b" data-mini="true" data-ajax="true">AWSA Recent Skier Scores</a>
		<hr />
        <p class="centeredItalic">Tournaments sorted by activity date
        <br/><sup class='videoNote'>V</sup>Indicates trick videos are available
		<br/>These scores are unofficial, repeat <span class="alertNotice">UNOFFICIAL</span></p>

		<?php
		$TourneyQry = "Select SanctionId, Name, Class, EventLocation"
			. ", SlalomRounds, TrickRounds, JumpRounds"
			. ", STR_TO_DATE(EventDates, '%m/%d/%Y') as EventDate "
			. ", (Select count(*) from TrickVideo V Where V.SanctionId = T.SanctionId "
			. "And (V.Pass1VideoUrl is not null or V.Pass2VideoUrl is not null)) as TrickVideoCount "
			. "from Tournament T "
			. "Order By STR_TO_DATE(EventDates, '%m/%d/%Y') DESC";
		$TourneyResult = $dbConnect->query($TourneyQry);
		if ( $TourneyResult->num_rows > 0 ) {
			echo "<ul data-role='listview' data-theme='b'>\n\r";
			while ($TourneyRow = $TourneyResult->fetch_assoc()) {
				$thisSanctionID = $TourneyRow['SanctionId'];
				echo "\r\n<li>\r\n<a href='javascript:void(0)' onclick='javascript:getTourneyBySanctionID(\"" . $thisSanctionID . "\", \""
				. $TourneyRow['Name'] . "\")';>";
				if ( $TourneyRow['TrickVideoCount'] > 0 ) {
					echo "<sup class='videoNote'>V</sup>" ;
				}
				echo $TourneyRow['Name']
				. "<span class='eventDate'>"
				. $TourneyRow['SanctionId'] . " " . $TourneyRow['EventLocation'] . " " . $TourneyRow['EventDate']
				. "</a>\r\n</li>";

			}
			$TourneyResult->free_result();
			echo "\r\n</ul>";

		} else {
			echo "<span class='noScores'>No Tournaments have been uploaded.  Please check back later.</span>";
		}

		?>
	</div><!-- /content -->

	<form method="post" name="tourneyList" action="wfwShowTourScores.php" id="ShowTourScores" data-ajax="true">
	<input type="hidden" name="sanctionID" value="" />
	<input type="hidden" name="EventName" value="" />
	</form>

</div><!-- /page -->


<script type="text/javascript">
$('body').hide();
$('body').fadeIn(1000);
</script>

</body>
</html>


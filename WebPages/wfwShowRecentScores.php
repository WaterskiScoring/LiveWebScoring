<?php
error_reporting(E_ALL);
ini_set("display_errors", 1);
session_start();
include_once( "WfwInit.php" );
?>

<!DOCTYPE html>
<html>
<head>
	<title>Most recent 10 scores from around the country</title>
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
<div data-role="page" id="recentPasses">

<script>
function cookieReload(val) {
	if (val=="Now") {
		setCookie("refreshRate","Now","2");
		reloadPage();
	} else {
		setCookie("refreshRate",val,"2");
		reloadPage();
	}
}

var cookieVal = getCookie("refreshRate");

if (cookieVal != null && cookieVal !="" && cookieVal !="Now") {
	setTimeout("reloadPage()",cookieVal);
}
</script>

	<div data-role="header">
    	<a href='wfwShowTourList.php' class='ui-btn-left' data-role='button' data-direction='reverse' data-icon='back' data-mini='true' data-ajax='true'>Tournys</a>
		<h1>Most recent 10 scores from around the country</h1>
        <span class="ui-btn-right ui-btn ui-shadow ui-btn-corner-all ui-btn-up-a">
			<select name="select-choice-a" id="select-choice-a" data-native-menu="false" data-mini='true' data-inline='true' onChange="javascript:cookieReload(this.value);">
    			<option>Refresh</option>
                <option value="Now">Now</option>
    			<option value="30000">1 min.</option>
   				<option value="120000">2 min.</option>
    			<option value="300000">5 min.</option>
			</select>
            </span>
	</div><!-- /header -->

    <div data-role="content">
		<?php
		$RecentSlalomQry = "SELECT ssi.SanctionId, ssi.MemberId, tour.Name, tour.EventDates, tour.EventLocation, tri.SkierName, ssi.AgeGroup, ssi.Round
			, ssi.Score, ssi.FinalSpeedMph, ssi.FinalSpeedKph, ssi.FinalLen, ssi.FinalLenOff, ssi.FinalPassScore, ssi.LastUpdateDate
			from TourReg tri
			JOIN SlalomScore ssi on ssi.SanctionId = tri.SanctionId AND ssi.MemberId=tri.MemberId AND ssi.AgeGroup = tri.AgeGroup
			JOIN Tournament tour on tour.SanctionId = tri.SanctionId
			order by ssi.LastUpdateDate DESC LIMIT 5";

		$RecentTrickQry = "SELECT ssi.SanctionId, ssi.MemberId, tour.Name, tour.EventDates, tour.EventLocation, tri.SkierName, ssi.AgeGroup, ssi.Round
			, ssi.Score, ssi.ScorePass1, ssi.ScorePass2, ssi.LastUpdateDate
			from TourReg tri
			JOIN TrickScore ssi on ssi.SanctionId = tri.SanctionId AND ssi.MemberId=tri.MemberId AND ssi.AgeGroup = tri.AgeGroup
			JOIN Tournament tour on tour.SanctionId = tri.SanctionId
			order by ssi.LastUpdateDate DESC LIMIT 5";

		$RecentJumpQry = "SELECT ssi.SanctionId, ssi.MemberId, tour.Name, tour.EventDates, tour.EventLocation, tri.SkierName, ssi.AgeGroup, ssi.Round
			, ssi.ScoreFeet, ssi.ScoreMeters, ssi.LastUpdateDate
			from TourReg tri
			JOIN JumpScore ssi on ssi.SanctionId = tri.SanctionId AND ssi.MemberId=tri.MemberId AND ssi.AgeGroup = tri.AgeGroup
			JOIN Tournament tour on tour.SanctionId = tri.SanctionId
			order by ssi.LastUpdateDate DESC LIMIT 5";

		//Empty variable to check for any scores in all events
		$noSlalomScores='';
		$noJumpScores='';
		$noJumpScores='';

		echo "<div data-role='collapsible-set' data-theme='b' data-content-theme='d' data-inset='false'>";
		$RecentSlalomResult = mysql_query($RecentSlalomQry) or die (mysql_error());
		$RecentSlalomRow = mysql_num_rows($RecentSlalomResult);

		if ( $RecentSlalomRow != 0 ) {
			echo "<div data-role='collapsible' data-collapsed='false'>\r\n";
			echo "<h2>Slalom</h2>\r\n";
			echo "<ul data-role='listview' data-role='collapsible'>\r\n";

			while ($RecentSlalomRow = mysql_fetch_assoc($RecentSlalomResult)) {
				echo "<li><a href='javascript:void(0)' onclick='javascript:getScoresFromRecent(\""
				. $RecentSlalomRow['AgeGroup'] . "\",\"" . $RecentSlalomRow['Round'] . "\",\"Slalom\");'>"
				. $RecentSlalomRow['AgeGroup'] . " " . $RecentSlalomRow['SkierName'] . " on " . $RecentSlalomRow['LastUpdateDate']
				. "<span class='score'>" . $RecentSlalomRow['Score']
				. " " . $RecentSlalomRow['Name'] . " " . $RecentSlalomRow['EventDates']
				. "</span></a></li>\r\n";
			}
			echo "</ul>";
			echo "</div>";
		} else {
			$noSlalomScores=true;
		}

		$RecentTrickResult = mysql_query($RecentTrickQry) or die (mysql_error());
		$RecentTrickRow = mysql_num_rows($RecentTrickResult);

		if ( $RecentTrickRow != 0 ) {
			echo "<div data-role='collapsible'>";
			echo "<h2>Trick</h2>";
			echo "<ul data-role='listview' data-role='collapsible'>";

			while ($RecentTrickRow = mysql_fetch_assoc($RecentTrickResult)) {
				echo "<li><a href='javascript:void(0)' onclick='javascript:getScoresFromRecent(\"" . $RecentTrickRow['AgeGroup'] . "\",\"" . $RecentTrickRow['Round'] . "\",\"Trick\");'>"
				. $RecentTrickRow['AgeGroup'] . " " . $RecentTrickRow['SkierName'] . " on " . $RecentSlalomRow['LastUpdateDate']
				. "<span class='score'>" . $RecentTrickRow['Score']
				. " " . $RecentTrickRow['Name'] . " " . $RecentTrickRow['EventDates']
				. "</span></a></li>\r\n";
			}
			echo "</ul>";
			echo "</div>";
		} else {
			$noTrickScores=true;
		}

		$RecentJumpResult = mysql_query($RecentJumpQry) or die (mysql_error());
		$RecentJumpRow = mysql_num_rows($RecentJumpResult);

		if ( $RecentJumpRow != 0 ) {
			 echo "<div data-role='collapsible'>";
			echo "<h2>Jump</h2>";
			echo "<ul data-role='listview' data-role='collapsible'>";

			while ($RecentJumpRow = mysql_fetch_assoc($RecentJumpResult)) {
				echo "<li><a href='javascript:void(0)' onclick='javascript:getScoresFromRecent(\"" . $RecentJumpRow['AgeGroup'] . "\",\"" . $RecentJumpRow['Round'] . "\",\"Jump\");'>"
				. $RecentJumpRow['AgeGroup'] . " " . $RecentJumpRow['SkierName'] . " on " . $RecentSlalomRow['LastUpdateDate']
				. "<span class='score'>" . $RecentJumpRow['ScoreFeet'] . " (" . $RecentJumpRow['ScoreMeters'] . ")"
				. " " . $RecentJumpRow['Name'] . " " . $RecentJumpRow['EventDates']
				. "</span></a></li>\r\n";
			}
			echo "</ul>";
			echo "</div>";
		} else {
			$noJumpScores=true;
		}

		if ($noSlalomScores && $noJumpScores && $noJumpScores)
		echo "<p class='centeredItalic'>No scores are currently available for the most recent tournament.  Please check back later.</p>
		<a href='tournyList.php' data-direction='reverse' data-theme='b' data-role='button' data-icon='back' data-mini='true' data-ajax='true'>Back to Tournaments</a>
		";
	?>
	</div><!-- /collapsable set -->

	</div><!-- /content -->

	<form method="post" name="getRecentScoresForm" action="wfwShowScores.php" id="getRecentScoresFormID" data-ajax="false">
	<input type="hidden" name="divisionID" value="" />
	<input type="hidden" name="eventRoundID" value="" />
	<input type="hidden" name="eventTypeID" value="" />
	<input type="hidden" name="formID" value="recentPassesForm" />
	</form>
	<script>
	//setTimeout("reloadPage()",60000);
	if (cookieVal != null && cookieVal != "Now") {
			$("#select-choice-a").val(cookieVal);
		} else {
		$('select option:first-child').attr("selected", "selected");
	}
	</script>

</div><!-- /page -->

<script type="text/javascript">
$('body').hide();
$('body').fadeIn(1000);
</script>

</body>
</html>


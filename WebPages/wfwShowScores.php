<?php
error_reporting(E_ALL);
ini_set("display_errors", 1);
session_start();
if(!isset($_SESSION['sanctionID']) || empty($_SESSION['sanctionID'])) {
   header('Location: index.php');
}
include_once( "WfwInit.php" );

function checkPostSet() {
	if ($_POST['formID']=='getDivisionsForm') {
		return isset($_POST['divisionID']);
	}
	if ($_POST['formID']=='recentPassesForm') {
		return isset($_POST['divisionID'], $_POST['eventRoundID'], $_POST['eventTypeID']);
	}
}

function checkSessionSet() {
  return isset($_SESSION['sanctionID'],$_SESSION['divisionID'], $_SESSION['skiEvent'], $_SESSION['skiRound'], $_SESSION['eventTable']);
}

// SET SESSION DATA WITH POST DATA
$sanctionID = '';
$divisionID = '';
$skiRound = '';
$skiEvent = '';
$eventRoundID = '';
$eventTypeID = '';
$eventTable = '';
$formID = '';
$scoreField = '';

if (checkPostSet() != FALSE || checkSessionSet() != FALSE) {
	if ( isset($_POST['formID']) ) {
		if ( isset($_SESSION['sanctionID']) ) $sanctionID = $_SESSION['sanctionID'];
		if ( isset($_SESSION['divisionID']) ) $divisionID = $_SESSION['divisionID'];
		if ( isset($_SESSION['skiRound']) ) $skiRound = $_SESSION['skiRound'];
		if ( isset($_SESSION['skiEvent']) ) $skiEvent = $_SESSION['skiEvent'];
		if ( isset($_SESSION['eventTable']) ) $eventTable = $_SESSION['eventTable'];

		$formID = $_POST['formID'];
		if ($formID == 'getDivisionsForm') {
			if (isset($_POST['divisionID'])) {
				$divisionID = $_POST['divisionID'];
				$_SESSION['divisionID'] = $divisionID;
			}

		} else if ($formID == 'recentPassesForm') {
			if (isset($_POST['divisionID'])) {
				$divisionID = $_POST['divisionID'];
				$_SESSION['divisionID'] = $divisionID;
			}
			if (isset($_POST['eventRoundID'])) {
				$skiRound = $_POST['eventRoundID'];
				$_SESSION['skiRound'] = $skiRound;
			}
			if (isset($_POST['eventTypeID'])) {
				$eventTypeID = $_POST['eventTypeID'];
				$_SESSION['skiEvent'] = $skiEvent;
				$eventTable = $eventTypeID . "Score";
				$_SESSION['eventTable'] = $eventTable;
			}
		}

		if ( $skiEvent == "Jump") {
			$scoreField="ScoreFeet";
		} else {
			$scoreField="Score";
		}

	} else {
		$errorMsg = "<span class='noScores'>Input form not available.  Return to previous.</span>";
	}

} else {
	$errorMsg = "<span class='noScores'>Data not set to display scores.  Return to previous.</span>";
}

?>

<!DOCTYPE html>
<html>
<head>
	<title>Tournament Scores</title>
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
<div data-role="page" id="getScores">

	<div data-role="header">
    	<a href='wfwShowTourDiv.php' class='ui-btn-left' data-role='button' data-icon='back' data-mini='true' data-ajax='true'>Divisions</a>
		<h1><?php echo $divisionID . " " . $skiEvent;?></h1>
        <a id="ui-header-refresh" href='#' onclick='reloadPage()' class='ui-btn-right RefreshLink' data-role='button' data-icon='refresh' data-mini='true' data-ajax='true'>Refresh</a>

		<hr />
	</div><!-- /header -->

        <p class="centeredItalic">These scores are unofficial, repeat <span class="alertNotice">UNOFFICIAL</span></p>
    <div data-role="content" data-theme="c">

		<ul data-role="listview">
		<?php
			if ($skiEvent == "Jump") {
				$ScoresQry = "SELECT ssi.SanctionID, tri.SkierName, tri.MemberId, ssi.MemberId, ssi.AgeGroup, ssi.Round
					, ssi.ScoreFeet, ssi.ScoreMeters
				, (SELECT IFNULL((ssi2.ScoreFeet),0) from TourReg tri2
					JOIN JumpScore ssi2 on ssi2.MemberId=tri2.MemberId AND ssi2.SanctionId=tri2.SanctionId AND ssi2.AgeGroup=tri2.AgeGroup
					WHERE ssi2.AgeGroup = ssi.AgeGroup AND ssi2.SanctionID = ssi.SanctionID AND ssi2.memberid = ssi.memberid AND ssi2.Round = '25'
					) AS RunOffScore
				from TourReg tri
				JOIN JumpScore ssi on ssi.MemberId=tri.MemberId AND ssi.SanctionId=tri.SanctionId AND ssi.AgeGroup=tri.AgeGroup
				WHERE ssi.SanctionID='" .  $sanctionID . "'  AND ssi.AgeGroup='" .  $divisionID . "' AND Round = '" . $skiRound . "'
				ORDER BY ssi.ScoreFeet DESC, ssi.ScoreMeters, RunOffScore DESC";
			}
			if ($skiEvent == "Trick") {
				$ScoresQry = "SELECT ssi.SanctionID, tri.SkierName, tri.MemberId, ssi.MemberId, ssi.AgeGroup, ssi.Round
					, ssi.Score, ssi.ScorePass1, ssi.ScorePass2
				, (SELECT IFNULL(ssi2.Score,0) from TourReg tri2
					JOIN TrickScore ssi2 on ssi2.MemberId=tri2.MemberId AND ssi2.SanctionId=tri2.SanctionId AND ssi2.AgeGroup=tri2.AgeGroup
					WHERE ssi2.AgeGroup = ssi.AgeGroup AND ssi2.SanctionID = ssi.SanctionID AND ssi2.memberid = ssi.memberid AND ssi2.Round = '25'
					) AS RunOffScore
				from TourReg tri
				JOIN TrickScore ssi on ssi.MemberId=tri.MemberId AND ssi.SanctionId=tri.SanctionId AND ssi.AgeGroup=tri.AgeGroup
				WHERE ssi.SanctionID='" .  $sanctionID . "'  AND ssi.AgeGroup='" .  $divisionID . "' AND Round = '" . $skiRound . "'
				ORDER BY ssi.Score DESC, RunOffScore DESC";
			}
			if ($skiEvent == "Slalom") {
				$ScoresQry = "SELECT ssi.SanctionID, tri.SkierName, tri.MemberId, ssi.MemberId, ssi.AgeGroup, ssi.Round
					, ssi.Score, ssi.FinalSpeedMph, ssi.FinalSpeedKph, ssi.FinalLen, ssi.FinalLenOff, ssi.FinalPassScore
				, (SELECT IFNULL(ssi2.Score,0) from TourReg tri2
					JOIN SlalomScore ssi2 on ssi2.MemberId=tri2.MemberId AND ssi2.SanctionId=tri2.SanctionId AND ssi2.AgeGroup=tri2.AgeGroup
					WHERE ssi2.AgeGroup = ssi.AgeGroup AND ssi2.SanctionID = ssi.SanctionID AND ssi2.memberid = ssi.memberid AND ssi2.Round = '25'
					) AS RunOffScore
				from TourReg tri
				JOIN SlalomScore ssi on ssi.MemberId=tri.MemberId AND ssi.SanctionId=tri.SanctionId AND ssi.AgeGroup=tri.AgeGroup
				WHERE ssi.SanctionID='" .  $sanctionID . "'  AND ssi.AgeGroup='" .  $divisionID . "' AND Round = '" . $skiRound . "'
				ORDER BY ssi.Score DESC, RunOffScore DESC";
			}

			$ScoresResult = $dbConnect->query($ScoresQry) or die ($dbConnect->error);
			$curRowCount = $ScoresResult->num_rows;

			if ( $curRowCount > 0 ) {
				while ($ScoresRow = $ScoresResult->fetch_assoc()) {
					echo "<li>\r\n";
					echo "<a href='wfwShowScoreRecap.php?MemberId=" . $ScoresRow['MemberId'] . "&SkierName=" . $ScoresRow['SkierName'] . "' data-rel='dialog' data-transition='pop'>" . $ScoresRow['SkierName'] . "\r\n";
					if ($skiEvent == "Jump") {
						echo "<span class='scoreLine'>" . $ScoresRow['ScoreFeet']  . " feet (" . $ScoresRow['ScoreMeters'] . "M)</span>";
					}
					if ($skiEvent == "Trick") {
						echo "<span class='scoreLine'>" . $ScoresRow['Score']  . " points (" . $ScoresRow['ScorePass1'] . ", " . $ScoresRow['ScorePass2'] . ")</span>";
					}
					if ($skiEvent == "Slalom") {
						echo "<span class='scoreLine'>" . $ScoresRow['Score']  . " buoys " . $ScoresRow['FinalPassScore'];
						if ($ScoresRow['FinalLenOff'] == "Long") {
							echo "@" . $ScoresRow['FinalSpeedMph'] . "mph (" . $ScoresRow['FinalSpeedKph'] . "kph)";
						} else {
 							echo "@" . $ScoresRow['FinalLenOff']  . "(" . $ScoresRow['FinalLen'] . "M)";
 						}
						echo "</span>";
					}
					echo "</a></li>\r\n";
				}
			} else {
				echo "<span class='noScores'>No scores available yet for " . $divisionID . ".</span>";
			}
		?>
		</ul><!-- /listview -->

	</div><!-- /content -->

</div><!-- /page -->

<div id="bgDiv" style="position:absolute; top:0; left:0; width:100%; height:100%; margin:0; padding:0; background-image:url('unofficial.jpg');background-size:cover; opacity:1; z-index:-1">xxxxxxxxxxxxxxxx</div>

<script type="text/javascript">
$('body').hide();
$('body').fadeIn(1500);
</script>

</body>
</html>


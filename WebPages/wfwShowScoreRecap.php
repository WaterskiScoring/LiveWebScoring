<?php
error_reporting(E_ALL);
ini_set("display_errors", 1);
session_start();

if(!isset($_SESSION['sanctionID']) || empty($_SESSION['sanctionID'])) {
   header('Location: index.php');
}
include_once( "WfwInit.php" );

$MemberId = $_GET['MemberId'];
$SkierName = $_GET['SkierName'];
$SkierRound = $_GET['SkierRound'];
$SkierEvent = $_GET['SkierEvent'];

// PAUL NOTE: FIX to make usre all sessions and post data is avialble before running query below
// PAUL NOTE: Add ability to display run-off scores.
?>

<!DOCTYPE html>
<html>
<head>
	<title>Tournament Scoring Recap</title>
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
<div data-role="dialog" id="scoringRecap">

	<div data-role="header" data-theme="b">
		<h1>Score Recap</h1>
	</div>

	<div data-role="content" data-theme="d">
		<h1><?php echo $SkierName ?> - Round: <?php echo $SkierRound  ?></h1>

		<?php
		if ($SkierEvent == "Slalom") {
			$RecapQry = "Select Score, PassLineLength, Note, Reride, ScoreProt, RerideReason "
				. "FROM `SlalomRecap` "
				. "WHERE SanctionId='" . $_SESSION['sanctionID'] . "' AND MemberId='". $MemberId . "' AND Round='" . $SkierRound
				. "' Order By SkierRunNum ASC";
		}

		if ($SkierEvent == "Trick") {
			$RecapQry = "Select P.PassNum, P.Seq, P.Skis, P.Score, P.Code, P.Results, S.ScorePass1, S.ScorePass2, S.Score as TotalScore"
				. ", V.Pass1VideoUrl, V.Pass2VideoUrl, S.Pass1VideoUrl as Pass1VideoUrlX, S.Pass2VideoUrl as Pass2VideoUrlX "
				. "FROM `TrickPass` P "
				. "Join TrickScore S on S.SanctionId = P.SanctionId AND S.MemberId = P.MemberId AND S.AgeGroup = P.AgeGroup AND S.Round = P.Round "
				. "Left Outer Join TrickVideo V ON V.SanctionId = P.SanctionId AND V.MemberId = P.MemberId AND V.AgeGroup = P.AgeGroup AND V.Round = P.Round "
				. "WHERE P.SanctionId='" . $_SESSION['sanctionID'] . "' AND P.MemberId='". $MemberId . "' AND P.Round='" . $SkierRound
				. "' Order By P.PassNum ASC, P.Seq ASC";
		}

		if ($SkierEvent == "Jump") {
			$RecapQry = "Select ScoreFeet, ScoreMeters, PassNum, Results, BoatSpeed, RampHeight, ScoreProt, Reride, RerideReason "
				. "FROM `JumpRecap` "
				. "WHERE SanctionId='" . $_SESSION['sanctionID'] . "' AND MemberId='". $MemberId . "' AND Round='" . $SkierRound
				. "' Order By PassNum ASC";
		}

		if ($SkierEvent == "Overall") {
			$RecapQry = "SELECT TR.SkierName, TR.MemberId, TR.AgeGroup, 'Overall' as Event"
				. ", COALESCE(SS.NopsScore,0) + COALESCE(TS.NopsScore,0) + COALESCE(JS.NopsScore,0) as OverallScore"
				. ", SS.NopsScore as SlalomNopsScore, TS.NopsScore as TrickNopsScore, JS.NopsScore as JumpNopsScore"
				. ", COALESCE(SS.Round,COALESCE(TS.Round,COALESCE(JS.Round,0))) as Round"
				. ", SS.Score as SlalomScore, FinalPassScore, FinalSpeedMph, FinalSpeedKph, FinalLen, FinalLenOff"
				. ", TS.Score as TrickScore, ScorePass1, ScorePass2"
				. ", JS.ScoreFeet, JS.ScoreMeters "
				. "FROM TourReg TR "
				. "LEFT OUTER JOIN SlalomScore SS on SS.MemberId=TR.MemberId AND SS.SanctionId=TR.SanctionId AND SS.AgeGroup = TR.AgeGroup AND SS.Round = '" . $SkierRound . "' "
				. "LEFT OUTER JOIN TrickScore TS on TS.MemberId=TR.MemberId AND TS.SanctionId=TR.SanctionId AND TS.AgeGroup = TR.AgeGroup AND TS.Round = '" . $SkierRound . "' "
				. "LEFT OUTER JOIN JumpScore JS on JS.MemberId=TR.MemberId AND JS.SanctionId=TR.SanctionId AND JS.AgeGroup = TR.AgeGroup AND JS.Round = '" . $SkierRound . "' "
				. "WHERE TR.SanctionID='" .  $_SESSION['sanctionID'] . "' AND TR.MemberId='". $MemberId . "' "
				. "AND COALESCE(SS.Round,COALESCE(TS.Round,COALESCE(JS.Round,0))) > 0 "
				. "AND ((Select count(*) from EventReg ER Where ER.MemberId=TR.MemberId AND ER.SanctionId=TR.SanctionId AND ER.AgeGroup = TR.AgeGroup ) > 2 "
				. "	OR (Select count(*) from EventReg ER Where ER.MemberId=TR.MemberId AND ER.SanctionId=TR.SanctionId AND ER.AgeGroup = TR.AgeGroup ) >= 2 "
				. "		AND TR.AgeGroup in ('B1', 'G1', 'W8', 'W9', 'WA', 'WB', 'M8', 'M9', 'MA', 'MB')) "
				. "Order By TR.AgeGroup, OverallScore DESC, TR.SkierName ";

		}

		$RecapResult = mysql_query($RecapQry) or die (mysql_error());
		$RecapRow = mysql_num_rows($RecapResult);

		if ( $RecapRow != 0 ) {

			if ($SkierEvent == "Overall") { // OUTPUT OVERALL DATA
				while ($RecapRow = mysql_fetch_assoc($RecapResult)) {
					echo "<p>Slalom: NOPS " . $RecapRow['SlalomNopsScore']
						. "<span class='score'>" . $RecapRow['SlalomScore'] . " Buoys ("
						. $RecapRow['FinalPassScore'] . " @ "  . $RecapRow['FinalLenOff'] . " " . $RecapRow['FinalSpeedMph'] . "Mph"
						. " / " . $RecapRow['FinalLen'] . "M " . $RecapRow['FinalSpeedKph'] . "Kph "
						. ")</span></p>\r\n";
					echo "<p>  Trick: NOPS " . $RecapRow['TrickNopsScore']
						. "<span class='score'>" . $RecapRow['TrickScore'] . " Points ("
						. "Pass 1: " . $RecapRow['ScorePass1'] . ", Pass 2: " . $RecapRow['ScorePass2']
						. ")</span></p>\r\n";
					echo "<p>   Jump: NOPS " . $RecapRow['JumpNopsScore']
						. "<span class='score'>" . $RecapRow['ScoreFeet'] . " Feet (" . $RecapRow['ScoreMeters'] . " Meters)"
						. "</span></p>\r\n";
					echo "<p class='PageSubTitle'>Overall: NOPS " . $RecapRow['OverallScore'] . " Points </p>\r\n";
				}
			}

			if ($SkierEvent == "Slalom") { // OUTPUT SLALOM DATA
				while ($RecapRow = mysql_fetch_assoc($RecapResult)) {
					echo "<p>" . $RecapRow['Score'] . " buoys at " . $RecapRow['Note'];
					if ( $RecapRow['Reride'] == "Y" ) {
						echo "<br/>Reride: " . $RecapRow['Reride'] . " Protected: " . $RecapRow['ScoreProt'] . "  " . $RecapRow['RerideReason'];
					}
					echo "</p>\r\n";
				}
			}

			if ($SkierEvent == "Trick") { // OUTPUT TRICK DATA
				?>
				<div class='ui-grid-c' style="margin:0; padding:0; text-align: center; background-image:url('unofficial.jpg');background-size:cover; opacity:1; z-index:-1">
				<?php
				$myPrevPassNum = 0;
				while ($RecapRow = mysql_fetch_assoc($RecapResult)) {
					if ( $myPrevPassNum == 0 ) {
						$Pass1VideoUrl = $RecapRow['Pass1VideoUrl'];
						$Pass2VideoUrl = $RecapRow['Pass2VideoUrl'];
						$Pass1VideoUrlX = $RecapRow['Pass1VideoUrlX'];
						$Pass2VideoUrlX = $RecapRow['Pass2VideoUrlX'];

						echo "<span class='PageSubTitle'>Total Score: {$RecapRow['TotalScore']}</span>";
						echo "<br/><span class='RecapMessage'>For mobile viewing rotate to landscape</span>";
						echo "<table style='border: 4px Silver outset; margin-left: auto; margin-right: auto;'>";
						echo "<tr style='vertical-align: top;'><td><span class='PageSubTitle'>Pass 1 Score {$RecapRow['ScorePass1']}</span>";
						?>
						<table style="border: 2px Silver inset;"><tr>
						<th>Skis</th>
						<th>Trick</th>
						<th>Results</th>
						<th>Score</th>
						</tr>
						<?php
					}
					if ( ($RecapRow['PassNum'] == 2) && ($myPrevPassNum == 1) ) {
						echo "</table></td>";
						echo "<td style='background-color: #CCCCCC; width: 10px;'>&nbsp;</td>";
						echo "<td><span class='PageSubTitle'>Pass 2 Score {$RecapRow['ScorePass2']}</span>";
						?>
						<table style="border: 2px Silver inset;"><tr>
						<th>Skis</th>
						<th>Trick</th>
						<th>Results</th>
						<th>Score</th>
						</tr>
						<?php
						echo "\r\n";
					}
					$myPrevPassNum = $RecapRow['PassNum'];
					echo "<tr>";
					echo "<td>{$RecapRow['Skis']}</td>";
					echo "<td>{$RecapRow['Code']}</td>";
					echo "<td>{$RecapRow['Results']}</td>";
					echo "<td>{$RecapRow['Score']}</td>";
					echo "</tr>\n";
				}
				?>
				</table>
				</td></tr>
				</table>
				<br/><div data-theme='b' style="width='375px'; ">Pass1: <br/>
				<?php
				if ( strlen($Pass1VideoUrl) > 1 ) {
					echo "{$Pass1VideoUrl}";
				} else if ( strlen($Pass1VideoUrlX) > 1 ) {
					echo "{$Pass1VideoUrlX}";
				}
				?>
				</div>
				<br/><div data-theme='b' style="width='375px'; ">Pass2: <br/>
				<?php
				if ( strlen($Pass2VideoUrl) > 1 ) {
					echo "{$Pass2VideoUrl}";
				} else if ( strlen($Pass2VideoUrlX) > 1 ) {
					echo "{$Pass2VideoUrlX}";
				}
				?>
				</div>
				</div>
				<?php
				echo "\r\n";
			}

			if ($SkierEvent == "Jump") { // OUTPUT JUMP DATA
				echo "<div class='ui-grid-b'>";
				while ($RecapRow = mysql_fetch_assoc($RecapResult)) {
					echo "<div class='ui-block-a'>Pass: " . $RecapRow['PassNum'] . "</div>\r\n";
					if ($RecapRow['Results'] != "Jump") {
						echo "<div class='ui-block-b'>" . $RecapRow['Results'];
					} else {
						echo "<div class='ui-block-b'>" . $RecapRow['ScoreFeet']. " (" . $RecapRow['ScoreMeters'] . "m) ";
					}
					echo "</div>\r\n";
					echo "<div class='ui-block-b'> Speed:" . $RecapRow['BoatSpeed'] . "kph Ramp:" . $RecapRow['RampHeight'] . "</div>\r\n";
					if ( $RecapRow['Reride'] == "Y" ) {
						echo "<div style='width: 90%;'><strong>Reride: " . $RecapRow['Reride'] . " Protected:</strong> " . $RecapRow['ScoreProt'] . "<br/>" . $RecapRow['RerideReason'] . "</div>\r\n";
					} else {
						echo "<br/>";
					}
					echo "<br/>\r\n";
				}
				echo "</div>";
			}


		} else {
			echo "<span class='noScores'>No scores available yet for this skier.</span>";
		}

		?>

		<a href='#' data-rel="back" data-role='button' data-mini='false' data-ajax='true' data-theme='b'>Back to Results</a>
	</div>
</div><!-- /page -->

</body>
</html>


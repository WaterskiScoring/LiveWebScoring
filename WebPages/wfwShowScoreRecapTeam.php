<?php
error_reporting(E_ALL);
ini_set("display_errors", 1);
session_start();

include_once( "WfwInit.php" );

$SanctionId = $_GET['SanctionId'];
$TeamCode = $_GET['TeamCode'];
$TeamDiv = $_GET['AgeGroup'];
$ReportFormat = $_GET['ReportFormat'];

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

		<?php
		$RecapQry = "Select S.SanctionId, S.TeamCode, S.AgeGroup, Name, ReportFormat, SD.SkierCategory, LineNum"
			. ", S.OverallPlcmt AS OverallPlcmtTeam, S.SlalomPlcmt AS SlalomPlcmtTeam, S.TrickPlcmt AS TrickPlcmtTeam, S.JumpPlcmt AS JumpPlcmtTeam"
			. ", S.OverallScore AS OverallScoreTeam, S.SlalomScore AS SlalomScoreTeam, S.TrickScore AS TrickScoreTeam, S.JumpScore AS JumpScoreTeam"
			. ", SlalomSkierName, SD.SlalomPlcmt, SD.SlalomScore, SlalomNops, SlalomPoints"
			. ", TrickSkierName, SD.TrickPlcmt, SD.TrickScore, TrickNops, TrickPoints"
			. ", JumpSkierName, SD.JumpPlcmt, SD.JumpScore, JumpNops, JumpPoints "
			. "From TeamScore S "
			. "Inner Join TeamScoreDetail SD on S.SanctionId = SD.SanctionId AND S.TeamCode = SD.TeamCode AND S.AgeGroup = SD.AgeGroup "
			. "Where S.SanctionId = '" . $SanctionId . "'"
			. "AND S.TeamCode = '" . $TeamCode . "'"
			. "AND S.AgeGroup = '" . $TeamDiv . "'"
			. "Order by S.AgeGroup, S.OverallPlcmt, SD.SkierCategory, SD.LineNum ";

		$RecapResult = $dbConnect->query($RecapQry);
		if ($dbConnect->error) {
			echo "An error was encountered running a query: " . $dbConnect->error;
			exit(500);
		} else {
			$curRowCount = $RecapResult->num_rows;
			echo "<span class='noScores'>Query Row Count: " . $curRowCount . "</span>";
			if ( $curRowCount > 0 ) {
				if ($ReportFormat == "ncwsa" || $ReportFormat == "awsa") {
					$RecapRow = $RecapResult->fetch_assoc();
					echo "<h1>" . $TeamCode . "-" . $RecapRow['Name'] . " (" . $TeamDiv . ") Score: " . $RecapRow['OverallScoreTeam'] . "</h1>";

					?>
					<div style="border:0; margin:0 0 0 0; padding:0; text-align: left; background-image:url('unofficial.jpg');background-size:cover; opacity:1; z-index:-1">
					<?php


					$RowCount = 0;
					$curCategory = '';
					$prevCategory = '';

					$RecapResult->data_seek(0);
					while ( $RecapRow = $RecapResult->fetch_assoc() ) {
						$curCategory = $RecapRow['SkierCategory'];
						if ( $RowCount == 0 ) {
							echo "<p style='border:0; margin:0; padding:0;'>"
								. "<span class='PageSubTitle'>"
								. "Slalom: (" . $ReportFormat . ")<br/>"
								. "Team Plcmt: " . $RecapRow['SlalomPlcmtTeam']
								. ", Score: " . $RecapRow['SlalomScoreTeam']
								. "</span></p>\r\n";
							echo "<div style='padding-left:16px;'>";
						}
						if ($ReportFormat == "awsa") {
							if ( $curCategory != $prevCategory ) {
								echo "<p style='border:0; margin:0; padding:0;'>"
									. "<span class='PageSubTitle'>" . "Category: "
									. $RecapRow['SkierCategory'] . "</span></p>\r\n";
							}
						}
						if ( $RecapRow['SlalomSkierName'] != '' ) {
							if ($ReportFormat == "ncwsa") {
								echo "<p style='border:0; margin:.25em 0 .25em 0; padding:0;'><span>"
									. "<strong>" . $RecapRow['SlalomSkierName'] . "</strong>"
									. " <strong>, Pl:</strong>" . $RecapRow['SlalomPlcmt']
									. " <strong>, Pts:</strong>" . $RecapRow['SlalomNops']
									. "</span></p>\r\n";
							}
							if ($ReportFormat == "awsa") {
								echo "<p style='border:0; margin:.25em 0 .25em 0; padding:0;'><span>"
									. "<strong>" . $RecapRow['SlalomSkierName'] . "</strong>"
									. " <span>, Pl:</span>" . $RecapRow['SlalomPlcmt']
									. " <strong>, NOPS:</strong>" . $RecapRow['SlalomNops']
									. " <strong>, Pts:</strong>" . $RecapRow['SlalomPoints']
									. "</span></p>\r\n";
							}
						}

						$RowCount++;
						$prevCategory = $RecapRow['SkierCategory'];
					}
					echo "</div>";

					$RowCount = 0;
					$curCategory = '';
					$prevCategory = '';

					$RecapResult->data_seek(0);
					while ( $RecapRow = $RecapResult->fetch_assoc() ) {
						$curCategory = $RecapRow['SkierCategory'];
						if ( $RowCount == 0 ) {
							echo "<p style='border:0; margin:1em 0 0 0; padding:0;'>"
								. "<span class='PageSubTitle'>"
								. "Trick: (" . $ReportFormat . ")<br/>"
								. "Team Plcmt: " . $RecapRow['TrickPlcmtTeam']
								. ", Score: " . $RecapRow['TrickScoreTeam']
								. "</span></p>\r\n";
							echo "<div style='padding-left:16px;'>";
						}
						if ($ReportFormat == "awsa") {
							if ( $curCategory != $prevCategory ) {
								echo "<p style='border:0; margin:0; padding:0;'>"
									. "<span class='PageSubTitle'>" . "Category: "
									. $RecapRow['SkierCategory'] . "</span></p>\r\n";
							}
						}
						if ( $RecapRow['TrickSkierName'] != '' ) {
							if ($ReportFormat == "ncwsa") {
								echo "<p style='border:0; margin:.25em 0 .25em 0; padding:0;'><span>"
									. "<strong>" . $RecapRow['TrickSkierName'] . "</strong>"
									. " <strong>, Pl:</strong>" . $RecapRow['TrickPlcmt']
									. " <strong>, Pts:</strong>" . $RecapRow['TrickNops']
									. "</span></p>\r\n";
							}
							if ($ReportFormat == "awsa") {
								echo "<p style='border:0; margin:.25em 0 .25em 0; padding:0;'><span>"
									. "<strong>" . $RecapRow['TrickSkierName'] . "</strong>"
									. " <span>, Pl:</span>" . $RecapRow['TrickPlcmt']
									. " <strong>, NOPS:</strong>" . $RecapRow['TrickNops']
									. " <strong>, Pts:</strong>" . $RecapRow['TrickPoints']
									. "</span></p>\r\n";
							}
						}

						$RowCount++;
						$prevCategory = $RecapRow['SkierCategory'];
					}
					echo "</div>";

					$RowCount = 0;
					$curCategory = '';
					$prevCategory = '';

					$RecapResult->data_seek(0);
					while ( $RecapRow = $RecapResult->fetch_assoc() ) {
						$curCategory = $RecapRow['SkierCategory'];
						if ( $RowCount == 0 ) {
							echo "<p style='border:0; margin:1em 0 0 0; padding:0;'>"
								. "<span class='PageSubTitle'>"
								. "Jump: (" . $ReportFormat . ")<br/>"
								. "Team Plcmt: " . $RecapRow['JumpPlcmtTeam']
								. ", Score: " . $RecapRow['JumpScoreTeam']
								. "</span></p>\r\n";
							echo "<div style='padding-left:16px;'>";
						}
						if ($ReportFormat == "awsa") {
							if ( $curCategory != $prevCategory ) {
								echo "<p style='border:0; margin:0; padding:0;'>"
									. "<span class='PageSubTitle'>" . "Category: "
									. $RecapRow['SkierCategory'] . "</span></p>\r\n";
							}
						}
						if ( $RecapRow['JumpSkierName'] != '' ) {
							if ($ReportFormat == "ncwsa") {
								echo "<p style='border:0; margin:.25em 0 .25em 0; padding:0;'><span>"
									. "<strong>" . $RecapRow['JumpSkierName'] . "</strong>"
									. " <strong>, Pl:</strong>" . $RecapRow['JumpPlcmt']
									. " <strong>, Pts:</strong>" . $RecapRow['JumpNops']
									. "</span></p>\r\n";
							}
							if ($ReportFormat == "awsa") {
								echo "<p style='border:0; margin:.25em 0 .25em 0; padding:0;'><span>"
									. "<strong>" . $RecapRow['JumpSkierName'] . "</strong>"
									. " <span>, Pl:</span>" . $RecapRow['JumpPlcmt']
									. " <strong>, NOPS:</strong>" . $RecapRow['JumpNops']
									. " <strong>, Pts:</strong>" . $RecapRow['JumpPoints']
									. "</span></p>\r\n";
							}
						}

						$RowCount++;
						$prevCategory = $RecapRow['SkierCategory'];
					}
					echo "</div>";


					echo "</div>";
				}

			} else {
				echo "<span class='noScores'>No scores available yet for this Team.</span>";
			}
		}
		?>

		<a href='#' data-rel="back" data-role='button' data-mini='false' data-ajax='true' data-theme='b'>Back to Results</a>
	</div>
</div><!-- /page -->

</body>
</html>


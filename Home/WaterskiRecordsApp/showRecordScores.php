<?php
error_reporting(E_ALL);
ini_set("display_errors", 1);
include_once( "showRecordsInit.php" );

$inRecordType = $_POST['RecordType'];
$inRecordSubType = $_POST['RecordSubType'];
$inRecordStatus = $_POST['RecordStatus'];

?>

<!DOCTYPE html>
<html>
<head>
<?php
include( "PageHeaderDef.php" );
?>
</head>

<body>

<div data-role="page"  data-theme="a" id="showRecordScores"	>
	<div data-role="header">
		<h1>Water Ski Records</h1>
		<h2><?php echo $inRecordType . " - " . $inRecordSubType . " (" . $inRecordStatus . ") Records"; ?></h2>
	</div><!-- /header -->

    <div data-role="content" data-theme="c">
		<?php
		$SqlCommand = "Select R.RecordType, R.State, R.Division, Event, Score, SkierName, R.EventClass, R.SanctionId, R.MemberId, S.PK, DATE_FORMAT(RecordDate,'%Y-%m-%d') as RecordDate, EventScoreFK "
			. "from RecordDef R "
			. "Inner Join SlalomScore S on S.SanctionId = R.SanctionId AND S.MemberId = R.MemberId AND S.Division = R.Division "
			. "Where R.Event = 'Slalom' "
			. "AND R.RecordType = '" . $inRecordType . "' "
			. "AND R.State = '" . $inRecordSubType . "' "
			. "AND R.RecordStatus = '" . $inRecordStatus . "' "
			. "Union "
			. "Select R.RecordType, R.State, R.Division, Event, Score, SkierName, R.EventClass, R.SanctionId, R.MemberId, S.PK, DATE_FORMAT(RecordDate,'%Y-%m-%d') as RecordDate, EventScoreFK "
			. "from RecordDef R "
			. "Inner Join TrickScore S on S.SanctionId = R.SanctionId AND S.MemberId = R.MemberId AND S.Division = R.Division "
			. "Where R.Event = 'Trick' "
			. "AND R.RecordType = '" . $inRecordType . "' "
			. "AND R.State = '" . $inRecordSubType . "' "
			. "AND R.RecordStatus = '" . $inRecordStatus . "' "
			. "Union "
			. "Select R.RecordType, R.State, R.Division, Event, ScoreFeet as Score, SkierName, R.EventClass, R.SanctionId, R.MemberId, S.PK, DATE_FORMAT(RecordDate,'%Y-%m-%d') as RecordDate, EventScoreFK "
			. "from RecordDef R "
			. "Inner Join JumpScore S on S.SanctionId = R.SanctionId AND S.MemberId = R.MemberId AND S.Division = R.Division "
			. "Where R.Event = 'Jump' "
			. "AND R.RecordType = '" . $inRecordType . "' "
			. "AND R.State = '" . $inRecordSubType . "' "
			. "AND R.RecordStatus = '" . $inRecordStatus . "' "
			. "Order by RecordType, Division, Event ";
		;
		$SqlResult = mysql_query($SqlCommand) or die (mysql_error());
		$DataRow = mysql_num_rows($SqlResult);
		if ( $DataRow != 0 ) {
			echo "<ul data-role='listview' data-theme='b'>\n\r";
			while ($DataRow = mysql_fetch_assoc($SqlResult)) {
				echo "\r\n<li>" . $DataRow['State']
					. ", " . $DataRow['Division']
					. ", " . $DataRow['Event']
					. ", " . $DataRow['Score']
					. ", " . $DataRow['SkierName']
					. ", " . $DataRow['EventClass']
					. ", " . $DataRow['SanctionId']
					. ", " . $DataRow['RecordDate']
					. "</li>";
			}
			echo "\r\n</ul>";
			mysql_free_result($SqlResult);

		} else {
			echo "<span class='noScores'>No selections available.</span>";
		}

		?>
	</div><!-- /content -->

<?php
include( "PageFooter.php" );
?>
</div><!-- /page -->

</body>
</html>
<?php
include_once( "showRecordsTerm.php" );
?>

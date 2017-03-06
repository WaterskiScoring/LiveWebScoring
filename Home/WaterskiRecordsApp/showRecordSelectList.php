<?php
error_reporting(E_ALL);
ini_set("display_errors", 1);
session_start();
include_once( "showRecordsInit.php" );
?>

<!DOCTYPE html>
<html>
<head>
<?php
include( "PageHeaderDef.php" );
?>
</head>

<body>
<div data-role="page"  data-theme="a" id="selectRecordList"	>

	<div data-role="header">
		<h1>
		Water Ski Records For Eastern Region
		<br/><span class="HeaderNote">Select the record type list to show</span><br/>
		</h1>
	</div><!-- /header -->

    <div class="ui-content" role="main">

		<?php
		$SqlCommand = "Select Distinct RecordType, State as RecordSubType from RecordDef Where RecordType = 'State' "
			. "UNION "
			. "Select Distinct RecordType, EventClass as RecordSubType from RecordDef Where RecordType = 'Region' "
			. "UNION "
			. "Select Distinct RecordType, Club as RecordSubType from RecordDef Where RecordType = 'Club' "
			. "UNION "
			. "Select Distinct RecordType, SiteFk as RecordSubType from RecordDef "
			. "       Inner Join Site on SiteFk = Site.PK "
			. "       Where RecordType = 'Site' "
			. "Order By RecordType DESC";
		$SqlResult = mysql_query($SqlCommand) or die (mysql_error());
		$DataRow = mysql_num_rows($SqlResult);
		if ( $DataRow != 0 ) {

			echo "<ul data-role='listview' data-theme='b'>\n\r";
			while ($DataRow = mysql_fetch_assoc($SqlResult)) {
				$thisRecordType = $DataRow['RecordType'];
				$thisRecordSubType = $DataRow['RecordSubType'];
				echo "\r\n<li>\r\n<a href='javascript:void(0)' "
					. "onclick='javascript:getRecordDataByType("
					. "\"" . $thisRecordType . "\""
					. ", \"" . $thisRecordSubType . "\")';>"
					. $thisRecordType . " - " . $thisRecordSubType
					. "</a>\r\n</li>";
			}
			mysql_free_result($SqlResult);
			echo "\r\n</ul>";

		} else {
			echo "<span class='noScores'>No selections available.</span>";
		}

		?>
	</div><!-- /content -->

	<div data-role="footer" data-position="fixed" data-theme="b">
		<div data-role="navbar">
			<ul>
				<li><a href="http://www.waterskiresults.com" data-ajax="false">WSTIMS For Window Web Scoreboard</a></li>
				<li><a href="http://www.usawaterski.org" data-ajax="false">USA Water Ski</a></li>
			</ul>
		</div><!-- /navbar -->
	</div><!-- /footer -->

	<form method="post" name="selectRecordList" action="showRecordScores.php" id="ShowRecordsScores" data-ajax="true">
	<input type="hidden" name="RecordType" value="" />
	<input type="hidden" name="RecordSubType" value="" />
	<input type="hidden" name="RecordStatus" value="A" />
	</form>

<?php
include( "PageFooter.php" );
?>
</div><!-- /page -->

</body>
</html>
<?php
include_once( "showRecordsTerm.php" );
?>

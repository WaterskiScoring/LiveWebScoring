<?php
error_reporting(E_ALL);
ini_set("display_errors", 1);
session_start();
include_once( "WfwInit.php" );

$sanctionID = '';
$EventName = '';

if ( isset($_GET['sanctionID']) ) {
	$sanctionID = $_GET['sanctionID'];
	$EventName = $_GET['EventName'];
	$_SESSION['sanctionID'] = $sanctionID;
	$_SESSION['EventName'] = $EventName;
} else {
	if ( isset($_SESSION['sanctionID']) ) $sanctionID = $_SESSION['sanctionID'];
	if ( isset($_SESSION['EventName']) ) $EventName = $_SESSION['EventName'];
}
?>

<!DOCTYPE html>
<html>
<head>
	<title><?php echo $EventName ?> (<?php echo $sanctionID ?>) - Tournament Published Reports</title>
    <meta name="apple-mobile-web-app-capable" content="yes" />
	<meta name="viewport" content="width=device-width, initial-scale=1">

	<link rel="stylesheet" href="wfwMobile.css">
	<link rel="stylesheet" href="https://code.jquery.com/mobile/1.3.1/jquery.mobile-1.3.1.min.css" />

	<script type="text/javascript" src="wfwMobile.js"></script>
	<script src="https://code.jquery.com/jquery-1.9.1.min.js"></script>
	<script src="https://code.jquery.com/mobile/1.3.1/jquery.mobile-1.3.1.min.js"></script>

	<link rel="apple-touch-icon" href="customIcon.png" />
	<meta name="apple-mobile-web-app-capable" content="yes" />
	<meta name="apple-mobile-web-app-status-bar-style" content="black" />

	<script>
		var TournamentInfo = new Tournament("<?php echo $sanctionID ?>", "<?php echo $EventName ?>");

		function getURLParameter(name) {
			return decodeURI(
				(RegExp(name + '=' + '(.+?)(&|$)').exec(location.search)||[,null])[1]
			);
		}

		function openReports(reportName) {
		  var i;
		  var x = document.getElementsByClassName("ContainerHide");
		  for (i = 0; i < x.length; i++) {
			x[i].style.display = "none";
		  }
		  document.getElementById(reportName).style.display = "block";
		}
	</script>
</head>

<body>

<div data-role="page" id="PublishResults">

	<div data-role="header">
		<h2><?php echo $EventName?> (<?php echo $sanctionID ?>) - Tournament Published Reports</h2>
	</div><!-- /header -->

	<div data-role="header">
	   	<a href='wfwShowTourScores.php?sanctionID=<?php echo $sanctionID ?>&EventName=<?php echo $EventName?>'
	   	class='ui-btn-left' data-role='button' data-icon='back' data-mini='true' data-ajax='false' data-iconpos="notext">Scores</a>
		<h1>
			<span class="dropNav">
				<div id="toggle">
				Available Published Reports
				</div>
			</span>
        </h1>
		<a id="ui-header-refresh" href='#' onclick='reloadPage()' class='ui-btn-right RefreshLink' data-role='button' data-icon='refresh' data-mini='true' data-ajax='true'>Refresh</a>
	</div><!-- /header -->

	<div data-role="content" data-theme="c" id="resultsID">
		<h2 class="PageSubTitle">Official Event Results</h2>

		<?php
		$SqlQuery = "SELECT ReportType, Event, ReportTitle, ReportFilePath, DATE_FORMAT(LastUpdateDate, '%Y/%m/%d %h:%i %p') as LastUpdateDate "
			. "FROM PublishReport "
			. "WHERE SanctionId = '" . $sanctionID . "' "
			. "AND ReportType = 'Results' "
			. "Order by ReportType, Event, ReportTitle";

		$SqlResult = $dbConnect->query($SqlQuery);
		if ( $SqlResult->num_rows > 0 ) {
			?>
			<ul data-role='listview' data-theme='c'>
		
			<?php
			$prevEvent = "";
			while ($SqlRow = $SqlResult->fetch_assoc()) {
				$ReportFileRef = "/Tournament/" . $sanctionID . "/" . basename($SqlRow['ReportFilePath']);
				$curEvent = $SqlRow['Event'];
				?>
				<?php 
				if ( $curEvent != $prevEvent ) {
					if ( $prevEvent != "" ) {
						echo "</ul><br/>";
					}
					echo "<br/><div data-role='header' class='GroupHeader'>" . $curEvent . "</div><br/>";
					echo "<ul data-role='listview' data-theme='c'>";
					echo "<li>";
				} else {
					echo "<li>";
				}
				?>
				
				<a href="<?php echo $ReportFileRef ?>" target="blank"><?php echo $SqlRow['ReportTitle'] . " " . $SqlRow['LastUpdateDate']; ?></a>
				</li>

				<?php 
				$prevEvent = $curEvent;
			} 
			?>
			
			</ul>
			
		<?php 
		} else {
			echo "<span class='noScores'>No published official event results available</span>";
		}
		?>
	
	</div> <!-- // Content -->

	<div data-role="content" data-theme="c" id="runOrdersID">
		<br/><h2 class="PageSubTitle">Official Running Orders</h2>
		<?php
		$SqlQuery = "SELECT ReportType, Event, ReportTitle, ReportFilePath, DATE_FORMAT(LastUpdateDate, '%Y/%m/%d %h:%i %p') as LastUpdateDate "
			. "FROM PublishReport "
			. "WHERE SanctionId = '" . $sanctionID . "' "
			. "AND ReportType = 'RunOrder' "
			. "Order by ReportType, Event, ReportTitle";

		$SqlResult = $dbConnect->query($SqlQuery);
		if ( $SqlResult->num_rows > 0 ) {
			?>
			<ul data-role='listview' data-theme='d'>
		
			<?php
			$prevEvent = "";
			while ($SqlRow = $SqlResult->fetch_assoc()) {
				$ReportFileRef = "/Tournament/" . $sanctionID . "/" . basename($SqlRow['ReportFilePath']);
				$curEvent = $SqlRow['Event'];
				?>
				<?php 
				if ( $curEvent != $prevEvent ) {
					if ( $prevEvent != "" ) {
						echo "</ul><br/>";
					}
					echo "<br/><div data-role='header' class='GroupHeader'>" . $curEvent . "</div><br/>";
					echo "<ul data-role='listview' data-theme='c'>";
					echo "<li>";
				} else {
					echo "<li>";
				}
				?>
				
				<a href="<?php echo $ReportFileRef ?>" target="blank"><?php echo $SqlRow['ReportTitle'] . " " . $SqlRow['LastUpdateDate']; ?></a>
				</li>

				<?php 
				$prevEvent = $curEvent;
			} 
			?>
			
			</ul>
			
		<?php 
		} else {
			echo "<span class='noScores'>No published running orders available</span>";
		}
		?>
	</div> <!-- // Content -->


	<div data-role="content" data-theme="c" id="runOrdersID">
		<br/><h2 class="PageSubTitle">Official Miscellaneous Reports</h2>
		<?php
		$SqlQuery = "SELECT ReportType, Event, ReportTitle, ReportFilePath, DATE_FORMAT(LastUpdateDate, '%Y/%m/%d %h:%i %p') as LastUpdateDate "
			. "FROM PublishReport "
			. "WHERE SanctionId = '" . $sanctionID . "' "
			. "AND ReportType = 'Other' "
			. "Order by ReportType, Event, ReportTitle";

		$SqlResult = $dbConnect->query($SqlQuery);
		if ( $SqlResult->num_rows > 0 ) {
			?>
			<ul data-role='listview' data-theme='d'>
		
			<?php
			$prevEvent = "";
			while ($SqlRow = $SqlResult->fetch_assoc()) {
				$ReportFileRef = "/Tournament/" . $sanctionID . "/" . basename($SqlRow['ReportFilePath']);
				$curEvent = $SqlRow['Event'];
				?>
				<?php 
				if ( $curEvent != $prevEvent ) {
					if ( $prevEvent != "" ) {
						echo "</ul><br/>";
					}
					echo "<br/><div data-role='header' class='GroupHeader'>" . $curEvent . "</div><br/>";
					echo "<ul data-role='listview' data-theme='c'>";
					echo "<li>";
				} else {
					echo "<li>";
				}
				?>
				
				<a href="<?php echo $ReportFileRef ?>" target="blank"><?php echo $SqlRow['ReportTitle'] . " " . $SqlRow['LastUpdateDate']; ?></a>
				</li>

				<?php 
				$prevEvent = $curEvent;
			} 
			?>
			
			</ul>
			
		<?php 
		} else {
			echo "<span class='noScores'>No published miscellaneous reports available</span>";
		}
		?>
	</div> <!-- // Content -->

</div><!-- /page -->
</body>

</html>

<?php
error_reporting(E_ALL);
ini_set("display_errors", 1);
session_start();
include_once( "WfwInit.php" );

$SanctionID = "";
$EventName = "";
$EventTourClass = "";
$TourneyName = "";
if (isset($_POST['sanctionID'])) {
	$SanctionID = $_POST['sanctionID'];
	$EventName = $_POST['EventName'];
	$EventTourClass = $_POST['EventClass'];
} else if (isset($_GET['sanctionID'])) {
	$SanctionID = $_GET['sanctionID'];
	$EventName = $_GET['EventName'];
} else {
	  header('Location: https://www.waterskiresults.com/WfwWeb/wfwShowTourList.php');
}

?>
<!DOCTYPE html>
<html>
<head>
	<title><?php echo $EventName . " (" . $SanctionID . ") - Tournament Scores"; ?> </title>
    <meta name="apple-mobile-web-app-capable" content="yes" />
	<meta name="viewport" content="width=device-width, initial-scale=1">

	<link rel="stylesheet" type="text/css" href="wfwMobile.css">
	<link rel="stylesheet" type="text/css" href="https://code.jquery.com/mobile/1.3.1/jquery.mobile-1.3.1.min.css" />

	<script src="https://code.jquery.com/jquery-1.9.1.min.js"></script>
	<script src="https://code.jquery.com/mobile/1.3.1/jquery.mobile-1.3.1.min.js"></script>
	<script type="text/javascript" src="wfwMobile.js"></script>

	<link rel="apple-touch-icon" href="customIcon.png" />
	<meta name="apple-mobile-web-app-capable" content="yes" />
	<meta name="apple-mobile-web-app-status-bar-style" content="black" />

	<style>
	#slalomRoundDiv, #trickRoundDiv, #jumpRoundDiv, #overallRoundDiv, #sDivisions, #tDivisions, #jDivisions, #oDivisions {
		display:none;
	}
	</style>
	<script>
		var TournamentInfo = new Tournament(<?php echo "'" . $SanctionID . "', '" . $EventName . "'"; ?>);

		function getURLParameter(name) {
			return decodeURI(
				(RegExp(name + '=' + '(.+?)(&|$)').exec(location.search)||[,null])[1]
			);
		}

		function setDefaults () {
			if (window.location.hash == "#fromRecent") {
				var the_event = getURLParameter('eventTypeID').toLowerCase(); // needs to be lowercase of event.  ie. 'slalom'
				var the_round = the_event + "Round" + getURLParameter('eventRoundID'); // needs to be in format of 'slalomRound1'
				var the_division = the_event + "divisionID" + getURLParameter('divisionID'); // needs to be in format of 'divisionIDMM'

				$("input#"+the_event+", label[for="+the_event+"]").trigger('click');
				$("input#"+the_round+", label[for="+the_round+"]").trigger('click');
				$("input#"+the_division+",label[for="+the_division+"]").trigger('click');
				$("#submitBtn").click();
				//divisionIDMM slalomRound1 slalom
			}
		} // END setDefaults


		$( document ).ajaxStop(function() {
			$("#toggle").click();
			$("#toggle").html("Select...");
			document.title = "<?php echo $EventName . "(" . $SanctionID . ") - Tournament Scores "; ?>";
			buildNavHTML();
		});

	</script>
</head>

<body>

<div data-role="page" id="Scores">

	<div data-role="header">
		<h2>
		<?php
		if ( $EventTourClass == "L" OR $EventTourClass == "R" OR $EventTourClass == "A" OR $EventTourClass == "B" ) {
			echo $EventName
				. " (" . $SanctionID . ")"
				. " <sup class='videoNote'><img src='Images/AWSALogo.png' alt='AWSA Sacntioned Event'></sup>"
				. " <sup class='videoNote'><img src='Images/IwwfLogo.png' alt='IWWF Class L/R Event'></sup>"
 				. " - Tournament Scores";
		} else {
			echo $EventName
				. " (" . $SanctionID . ")"
				. " <sup class='videoNote'><img src='Images/AWSALogo.png' alt='AWSA Sacntioned Event'></sup>"
 				. " - Tournament Scores";
		}
		?>
		</h2>
	    <a id='PublishResultsBtn' class='ui-btn-right' data-role='button' data-icon='grid' data-mini='true' data-ajax='false' data-iconpos="left"
			<?php echo "href='wfwShowPublishResults.php?sanctionID=" . $SanctionID
			. "&EventName=" . str_replace("'","&#039;", $EventName) . "'>Published Reports</a>";
			?>
	</div><!-- /header -->

	<div data-role="header">
	   	<a href='wfwShowTourList.php' class='ui-btn-left' data-role='button' data-icon='back' data-mini='true' data-ajax='true' data-iconpos="notext"></a>
		<h1>
        <div id="navContainer">
        	<div id="panel" style="display:none;"></div>
        	<span class="dropNav">
                <div id="toggle">
                Loading...
                </div>
            </span>
        </div>
        </h1>

			<a id="ui-header-refresh" href='#' onclick='location.replace()' class='ui-btn-right RefreshLink' data-role='button' data-icon='refresh' data-mini='true' data-ajax='true'>Refresh</a>
	</div><!-- /header -->

    <div data-role="content" data-theme="c" id="scoresID"></div> <!-- // Content -->

</div><!-- /page -->

</body>
</html>


<?php
error_reporting(E_ALL);
ini_set("display_errors", 1);
session_start();
include_once( "WfwInit.php" );

if (isset($_POST['sanctionID'])) {
	$_SESSION['sanctionID'] = $_POST['sanctionID'];
	$_SESSION['EventName'] = $_POST['EventName'];
} else if (!isset ($_SESSION['sanctionID'])) {
	  header('Location: http://www.waterskiresults.com/WfwWeb/wfwShowTourList.php');
}

?>

<!DOCTYPE html>
<html>
<head>
	<title><?php echo $_SESSION['EventName'] ?> (<?php echo $_SESSION['sanctionID'] ?>) - Tournament Scores </title>
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

	<script>
		var TournamentInfo = new Tournament("<?php echo $_SESSION['sanctionID'] ?>", "<?php echo $_SESSION['EventName'] ?>");

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
			document.title = "<?php echo $_SESSION['EventName']?> (<?php echo $_SESSION['sanctionID'] ?>) - Tournament Scores ";
			buildNavHTML();
		});

	</script>
</head>

<body>
<!--
<div id="nav">
	<div id="loading">Loading...</div>
</div>
-->

<div data-role="page" id="Scores">

	<div data-role="header">
		<h2><?php echo $_SESSION['EventName']?> (<?php echo $_SESSION['sanctionID'] ?>) - Tournament Scores</h2>
	    <a href='wfwShowTourRunOrder.php' id='RunOrderBtn' class='ui-btn-right' data-role='button' data-icon='grid' data-mini='true' data-ajax='false' data-iconpos="left">Running Orders</a>
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
			<a href='#' id='refreshBtn' class='ui-btn-right' data-role='button' data-icon='refresh' data-mini='true' data-ajax='true' data-iconpos="notext"></a>
	</div><!-- /header -->

    <div data-role="content" data-theme="c" id="scoresID"></div> <!-- // Content -->

</div><!-- /page -->

</body>
</html>


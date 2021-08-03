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
	<title><?php echo $EventName ?> (<?php echo $sanctionID ?>) - Tournament Running Order</title>
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
		var TournamentInfo = new Tournament("<?php echo $sanctionID ?>", "<?php echo $EventName ?>");

		function getURLParameter(name) {
			return decodeURI(
				(RegExp(name + '=' + '(.+?)(&|$)').exec(location.search)||[,null])[1]
			);
		}

		function setDefaults () {
			if (window.location.hash == "#fromRecent") {
				var the_event = getURLParameter('eventTypeID').toLowerCase(); // needs to be lowercase of event.  ie. 'slalom'
				var the_group = the_event + "groupID" + getURLParameter('groupID'); // needs to be in format of 'groupIDMM'

				$("input#"+the_event+", label[for="+the_event+"]").trigger('click');
				$("input#"+the_group+",label[for="+the_group+"]").trigger('click');
				$("#submitBtn").click();
			}
		} // END setDefaults


		$( document ).ajaxStop(function() {
			$("#toggle").click();
			$("#toggle").html("Select...");
			document.title = "<?php echo $EventName?> (<?php echo $sanctionID ?>) - Tournament Running Order";
			buildRunOrderNavHTML();
		});

	</script>
</head>

<body>

<div data-role="page" id="RunOrder">

	<div data-role="header">
		<h2><?php echo $EventName?> (<?php echo $sanctionID ?>) - Tournament Running Orders</h2>
	</div><!-- /header -->

	<div data-role="header">
	   	<a href='wfwShowTourScores.php?sanctionID=<?php echo $sanctionID ?>&EventName=<?php echo $EventName?>'
	   	class='ui-btn-left' data-role='button' data-icon='back' data-mini='true' data-ajax='false' data-iconpos="notext">Scores</a>
		<h1>
        <div id="navContainer">
        	<div id="panel" style="display:none;"></div>
        	<span class="dropNav">
                <div id="toggle">
                Loading Running Orders...
                </div>
            </span>
        </div>
        </h1>
        <a href='#' id='refreshBtn' class='ui-btn-right' data-role='button' data-icon='refresh' data-mini='true' data-ajax='true' data-iconpos="notext"></a>
	</div><!-- /header -->

    <div data-role="content" data-theme="c" id="runOrderID"></div> <!-- // Content -->

</div><!-- /page -->
</body>
</html>


<?php
//error_reporting(E_ALL);
//ini_set("display_errors", 1);
?>

<!DOCTYPE html>
<html>

<head>
	<title>WSTIMS For Window Web Scoreboard</title>
    <meta name="apple-mobile-web-app-capable" content="yes" />
	<meta name="viewport" content="width=device-width, initial-scale=1">

	<link rel="stylesheet" href="http://code.jquery.com/mobile/1.3.1/jquery.mobile-1.3.1.min.css" />
	<link rel="stylesheet"  href="wfwMobile.css">
	<link rel="stylesheet" href="wfw.css" type="text/css" />

	<script src="http://code.jquery.com/jquery-1.9.1.min.js"></script>
	<script src="http://code.jquery.com/mobile/1.3.1/jquery.mobile-1.3.1.min.js"></script>
	<script language="JavaScript" type="text/javascript" src="wfw.js" ></script>

	<link rel="apple-touch-icon" href="customIcon.png" />
	<meta name="apple-mobile-web-app-capable" content="yes" />
	<meta name="apple-mobile-web-app-status-bar-style" content="black" />
</head>

<body>
	<div data-role="page" id="homepage">

		<div data-role="header" style="background:rgba(0, 0, 0, 0.4);" id="homepageHeader">
			<h1>WSTIMS For Window Web Scoreboard</h1>
		</div><!-- /header -->

	    <div data-role="content" data-theme="d" id="content">
		    <div id="introBox">
				<span class="titleCopy">
					<div id="TitleContainer" class="ReportTitle">
						<BR/>
						<div id="SubTitleContainer" class="SubTitle">
						These scores are unofficial, repeat <span class="SubTitleNote">UNOFFICIAL</span>
					</div>
					<p class="introCopy">A running list of all tournament scores uploaded from WSTIMS are viewable.</p>
				</span>
			</div>

		    <a href='wfwShowTourList.php' data-ajax='false' data-role='button' data-theme='b'>Live Scoring</a>

    	</div><!-- /content -->

    	<div data-role="footer" data-position="fixed" data-theme="b">
			<div data-role="navbar">
				<ul>
					<li><a href="http://www.waterskiresults.com/WfwWeb/wfwShowScoreboard.php" data-ajax="false">WSTIMS For Window Web Scoreboard</a></li>
					<li><a href="http://www.usawaterski.org" data-ajax="false">USA Water Ski</a></li>
				</ul>
			</div><!-- /navbar -->
		</div><!-- /footer -->

	</div><!-- /page -->
	<div id="bgDiv" style="position:absolute; top:0; left:0; width:100%; height:100%; margin:0; padding:0; background-image:url('AdamsPondFarm.jpg');background-size:cover; opacity:1; z-index:-1"></div>

	<script type="text/javascript">
		$('body').hide();
		$('body').fadeIn(1200);
	</script>

</body>
</html>

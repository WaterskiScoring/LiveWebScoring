<html>
<head>
	<title>WSTIMS For Window Deployment Files</title>
	<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1">
</head>
<link rel="stylesheet" href="WfwWeb/wfw.css" type="text/css" />
<script language="JavaScript" type="text/javascript" src="WfwWeb/wfw.js" ></script>

<body>
<div>
	<div id="TitleContainer" class="ReportTitle">
	WSTIMS For Windows File List
	</div>

	<div>
	<div style="margin-left: 16px;">
	<a href="http://www.waterskiresults.com">Home</a>
	</div>
	</div>

	<div style="margin-top: 16px;">
	<?php
	ini_set( 'display_errors', true );

	$curFileList = array();
	if ($handle = opendir('.')) {
		while (false !== ($curFile = readdir($handle))) {
			if ($curFile != "." && $curFile != "..") {
				$curFileList[] = $curFile;
			}
		}
		closedir($handle);
	}

	sort($curFileList);
	echo "<table><tr>";
	echo "<th>File</th>";
	echo "<th>Size</th>";
	echo "<th>Modified</th>";
	echo "<th>Updated</th>";
	echo "</tr>\n";
	foreach($curFileList as $curFile) {

 		$curFileExtn = strtolower(substr(strrchr($curFile, '.'), 1));
 		if ( $curFile == "admin_awsaeast"
			|| $curFile == "WaterskiScoringSystem.application"
			|| $curFileExtn == "xxx"
			|| $curFileExtn == "zzz"
 		) {
 		} else {
			echo "<tr><td class=\"DataLeft\"><a href=\"$curFile\">$curFile</a></td>";
			echo "<td class=\"DataRight\">" . friendly_filesize(filesize($curFile)) . "</td>";
			echo "<td class=\"DataLeft\">".date( "D d M Y g:i A", filemtime($curFile)) . "</td>";
			echo "<td class=\"DataLeft\">".date( "D d M Y g:i A", filemtime($curFile)) . "</td>";
			echo "</tr>\n";
		}

	}
	echo "</table>";

	function friendly_filesize($bytes) {
		$measurements = array('TB'=>1099511627776, 'GB'=>1073741824, 'MB'=>1048576, 'kB'=>1024, 'b'=>1);
		foreach($measurements as $key=>$value) {
			$conv = $bytes/$value;
			if($conv > 1) {
				return round($conv, 1)." $key";
			}
		}
	}

	?>
	</div>

</div>
</body>
</html>
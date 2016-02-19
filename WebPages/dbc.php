<?php
	$dbname = 'DB_NAME_HERE';
	$link = mysql_connect("HOSTNAME_LIKELY_LOCALHOST","USERNAME","PASSWORD") or die("Couldn't make connection.");	
    
	$db = mysql_select_db($dbname, $link) or die("Couldn't select Database");
?>
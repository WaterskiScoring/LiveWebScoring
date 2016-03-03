<?php
/* ****************************************
Set error status
**************************************** */
ini_set( 'display_errors', true );
error_reporting(E_ALL | E_STRICT);
//phpinfo();

/* ****************************************
Connect to database
**************************************** */
$dbConnect = mysql_connect( 'localhost', 'waterski', 'scor2007ing', 'AWSAEast' );
if (mysql_error()) {
	echo "\n<br/>Connection failed";
	exit();
} else {
	//echo "\n<br/>AWSAEast connection successful";
	mysql_select_db('AWSAEast') or die(mysql_error());
	if (mysql_error()) {
	} else {
		//echo "\n<br/>AWSAEast selected";
	}
}
?>

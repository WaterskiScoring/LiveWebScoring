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
/* $dbConnect = new mysqli( 'localhost', 'awsaeast_scoring', 'r3g89N&vpS!d1v3', 'awsaeast_scoring' );  */
$dbConnect = new mysqli( 'localhost', 'awsaeast_liveweb', 'Waterski#13', 'awsaeast_scoring' );

if ($dbConnect->connect_error) {
    die('Connect Error (' . $dbConnect->connect_errno . ') ' . $dbConnect->connect_error);
} else {
	/* This statement was added because otherwise MAX_JOIN_SIZE errors occurred on every select statement */
	$dbConnect->query("SET SQL_BIG_SELECTS=1");
}

ini_set( 'display_errors', true );
?>

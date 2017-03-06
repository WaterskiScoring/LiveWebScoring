<?
	$mysql = new mysqli( 'localhost', 'awsaeast_liveweb', 'Waterski#13', 'awsaeast_scoring' );
	if (mysqli_connect_errno()) {
    	printf("Connect failed: %s\n", mysqli_connect_error());
    	exit();
	}
	ini_set( 'display_errors', true );

	include_once( 'admin_awsaeast/classes/image.php' );
	include_once( 'admin_awsaeast/classes/image_public_display.php' );
?>
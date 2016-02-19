<html>
 <head>
  <title>PHP Test</title>
 </head>
 <body>

<?php
/* ****************************************
**************************************** */
ini_set( 'display_errors', true );
error_reporting(E_ALL | E_STRICT);
//phpinfo();

/* ****************************************
printf("Connect failed: %s\n", mysql_connect_error());
printf("\nConnection successful");
**************************************** */
$dbConnect = mysql_connect( 'localhost', 'waterski', 'scor2007ing', 'AWSAEast' );
if (mysql_error()) {
	echo "\nConnection failed";
	exit();
} else {
	printf("\nConnection successful");
	echo "\nConnection successful";
	mysql_select_db('AWSAEast') or die(mysql_error());
	if (mysql_error()) {
	} else {
		echo "\nAWSAEast selected";
	}
}

$curTableName = $_GET['TableName'];;
$mySqlStmt = "Select * from $curTableName";

$result = mysql_query($mySqlStmt);
if (mysql_error()) {
	printf("\nErrors detected for query %s ", mysql_error());
} else {
	if ( $result ) {
		$num_rows = mysql_num_rows($result);
		if ( $num_rows > 0 ) {
			printf("<br/>Select returned %d rows<br/><br/>", $num_rows);

			$fields_num = mysql_num_fields($result);
			echo "<h1>Table: {$curTableName}</h1>";
			echo "<table border='1'><tr>";
			// printing table headers
			for($i=0; $i<$fields_num; $i++)
			{
				$field = mysql_fetch_field($result);
				echo "<td>{$field->name}</td>";
			}
			echo "</tr>\n";
			// printing table rows
			while($row = mysql_fetch_row($result)) {
				echo "<tr>";

				// $row is array... foreach( .. ) puts every element
				// of $row to $cell variable
				foreach($row as $cell)
					echo "<td>$cell</td>";

				echo "</tr>\n";
			}
			mysql_free_result($result);
		} else {
			printf("\n No rows found");
		}
	} else {
		printf("\n No rowsz found");
		return false;
	}
}
?>


</body>
</html>
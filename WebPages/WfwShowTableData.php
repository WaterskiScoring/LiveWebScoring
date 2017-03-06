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
printf("Connect failed: %s\n", $dbConnect->error);
printf("\nConnection successful");
**************************************** */
$dbConnect = new mysqli( 'localhost', 'awsaeast_liveweb', 'Waterski#13', 'awsaeast_scoring' );
if ($dbConnect->error) {
	echo "\nConnection failed";
	exit();
} else {
	printf("\nConnection successful");
	echo "\nConnection successful";
}

$curTableName = $_GET['TableName'];;
$QueryCmd = "Select * from $curTableName";

$QueryResult = $dbConnect->query($Query) or die ($dbConnect->error);
if ($dbConnect->error) {
	printf("\nErrors detected for query %s ", $dbConnect->error);
} else {
	if ( $QueryResult ) {
		$curRowCount = $QueryResult->num_rows;
		if ( $curRowCount > 0 ) {
			printf("<br/>Select returned %d rows<br/><br/>", $curRowCount);

			$fieldCount = $QueryResult->field_count;
			echo "<h1>Table: {$curTableName}</h1>";
			echo "<table border='1'><tr>";
			// printing table headers
			for($i=0; $i<$fieldCount; $i++) {
				$field = $QueryResult->fetch_field()
				echo "<td>{$field->name}</td>";
			}
			echo "</tr>\n";
			// printing table rows
			while($curRow = $QueryResult->fetch_assoc()) {
				echo "<tr>";

				// $row is array... foreach( .. ) puts every element
				// of $curRow to $cell variable
				foreach($curRow as $cell)
					echo "<td>$cell</td>";

				echo "</tr>\n";
			}
			$QueryResult->free();
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
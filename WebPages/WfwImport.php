<?php
include_once( "WfwInit.php" );
/* ****************************************
**************************************** */
function updateTable($inTableNode) {
	$myAttrList = $inTableNode->attributes();
	$myTableName = $myAttrList["name"];
	$myCmd = $myAttrList["command"];
	echo "\n $myCmd table $myTableName";

	$myRows = $inTableNode->xpath('Rows/Row');
	$myRowCount = count($myRows);

	if($myRowCount > 0) {
		foreach($myRows as $myRow) {
			if ( $myCmd == "Delete" ) {
				deleteRow($myTableName, $inTableNode, $myRow);
			} else {
				if ( isRowFound($myTableName, $inTableNode, $myRow)) {
					updateRow($myTableName, $inTableNode, $myRow);
				} else {
					insertRow($myTableName, $inTableNode, $myRow);
				}
			}
		}
	}
	echo "\n------------------------------\n";
}

/* ****************************************
**************************************** */
function isRowFound($inTableName, $inTableNode, $inRowNode) {
	//global $dbConnect;
	$mySep = "";
	$mySqlStmt = "Select ";
	$myKeys = $inTableNode->xpath('Keys/Key');
	foreach($myKeys as $myKey) {
		$mySqlStmt .= $mySep . $myKey;
		$mySep = ", ";
	}
	$mySep = "";
	$mySqlStmt .= " From " . $inTableName . " Where ";
	foreach($myKeys as $myKey) {
		$myDataValue = $inRowNode->xpath($myKey);
		$mySqlStmt .= $mySep . $myKey . " = '" . $myDataValue[0] . "'";
		$mySep = " AND ";
	}

	//echo "\nSQL=$mySqlStmt";
	$result = mysql_query($mySqlStmt);
	if (mysql_error()) {
		printf("\nErrors detected for query %s ", mysql_error());
	} else {
		if ( $result ) {
			$num_rows = mysql_num_rows($result);
			if ( $num_rows > 0 ) {
				printf("\nSelect returned %d rows.", $num_rows);
				return true;
			} else {
				printf("\n No rows found");
				return false;
			}
		} else {
			printf("\n No rowsz found");
			return false;
		}
	}
}

/* ****************************************
**************************************** */
function updateRow($inTableName, $inTableNode, $inRowNode) {
	$mySep = "";
	$myKeys = $inTableNode->xpath('Keys/Key');
	$myColumns = $inTableNode->xpath('Columns/Column');

	$mySqlStmt = "Update " . $inTableName . " Set ";
	foreach($myColumns as $myColumn) {
		$myDataValue = $inRowNode->xpath($myColumn);
		$mySqlStmt .= $mySep . $myColumn . "='" . $myDataValue[0] . "'";
		$mySep = ", ";
	}

	$mySep = "";
	$mySqlStmt .= " Where ";
	foreach($myKeys as $myKey) {
		$myDataValue = $inRowNode->xpath($myKey);
		$mySqlStmt .= $mySep . $myKey . " = '" . $myDataValue[0] . "'";
		$mySep = " AND ";
	}

	$result = mysql_query($mySqlStmt);
	if (mysql_error()) {
		printf("\n Errors detected on update %s ", mysql_error());
		return false;
	} else {
		printf("\n Update successful %d ", $result);
		mysql_info();
		return true;
	}
}

/* ****************************************
**************************************** */
function insertRow($inTableName, $inTableNode, $inRowNode) {
	$mySep = "";
	$mySqlStmt = "Insert INTO " . $inTableName . " ( ";

	$myColumns = $inTableNode->xpath('Columns/Column');
	foreach($myColumns as $myColumn) {
		$mySqlStmt .= $mySep . $myColumn;
		$mySep = ", ";
	}

	$mySep = "";
	$mySqlStmt .= " ) VALUES (";
	foreach($myColumns as $myColumn) {
		$myDataValue = $inRowNode->xpath($myColumn);
		$mySqlStmt .= $mySep . " '" . $myDataValue[0] . "'";
		$mySep = ", ";
	}

	$mySqlStmt .= " );";

	//echo "\nSQL=$mySqlStmt";
	$result = mysql_query($mySqlStmt);
	if (mysql_error()) {
		printf("\n Errors detected on insert %s ", mysql_error());
		return false;
	} else {
		printf("\n Record inserted %d", mysql_insert_id());
		mysql_info();
		return true;
	}
}

/* ****************************************
**************************************** */
function deleteRow($inTableName, $inTableNode, $inRowNode) {
	$mySep = "";
	$myKeys = $inTableNode->xpath('Keys/Key');
	$myColumns = $inTableNode->xpath('Columns/Column');

	$mySqlStmt = "Delete From " . $inTableName ;
	$mySqlStmt .= " Where ";
	foreach($myKeys as $myKey) {
		$myDataValue = $inRowNode->xpath($myKey);
		$mySqlStmt .= $mySep . $myKey . " = '" . $myDataValue[0] . "'";
		$mySep = " AND ";
	}

	$result = mysql_query($mySqlStmt);
	if (mysql_error()) {
		printf("\n Errors detected on update %s ", mysql_error());
		return false;
	} else {
		printf("\n Delete successful %d ", $result);
		mysql_info();
		return true;
	}
}

/* ****************************************
Get input stream
**************************************** */
$xml = file_get_contents('php://input');
$xmlDoc = simplexml_load_string($xml);

/* ****************************************
Process input requests
**************************************** */
echo "\n------------------------------";
$myTables = $xmlDoc->xpath('//Table');
foreach($myTables as $myTable) {
	updateTable($myTable);
}
include_once( "WfwTerm.php" );
?>

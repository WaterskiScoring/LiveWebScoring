<?php
error_reporting(E_ALL);
//phpinfo();
ini_set("display_errors", 1);
//session_start();

include_once( "WfwInit.php" );

/* ****************************************
**************************************** */
function updateTable($dbConnect, $inTableNode) {
	$myAttrList = $inTableNode->attributes();
	$myTableName = $myAttrList["name"];
	$myCmd = $myAttrList["command"];
	echo "\n $myCmd table $myTableName";

	$myRows = $inTableNode->xpath('Rows/Row');
	$myRowCount = count($myRows);

	if($myRowCount > 0) {
		foreach($myRows as $myRow) {
			if ( $myCmd == "Delete" ) {
				deleteRow($dbConnect, $myTableName, $inTableNode, $myRow);
			} else {
				if ( isRowFound($dbConnect, $myTableName, $inTableNode, $myRow)) {
					updateRow($dbConnect, $myTableName, $inTableNode, $myRow);
				} else {
					insertRow($dbConnect, $myTableName, $inTableNode, $myRow);
				}
			}
		}
	}
	echo "\n------------------------------\n";
}

/* ****************************************
**************************************** */
function isRowFound($dbConnect, $inTableName, $inTableNode, $inRowNode) {
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
	$result = $dbConnect->query($mySqlStmt);
	if ($dbConnect->error) {
		printf("\nErrors detected for query %s ", $dbConnect->error);
	} else {
		if ( $result ) {
			$num_rows = $result->num_rows;
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
function updateRow($dbConnect, $inTableName, $inTableNode, $inRowNode) {
	$mySep = "";
	$myKeys = $inTableNode->xpath('Keys/Key');
	$myColumns = $inTableNode->xpath('Columns/Column');

	$mySqlStmt = "Update " . $inTableName . " Set ";
	foreach($myColumns as $myColumn) {
		$myDataValue = $inRowNode->xpath($myColumn);
		if ( strlen($myDataValue[0]) > 0 ) {
			if ( $myDataValue[0] == 'True' ) {
				$mySqlStmt .= $mySep . $myColumn . "=1";
			} else if ( $myDataValue[0] == 'False' ) {
				$mySqlStmt .= $mySep . $myColumn . "=0";
			} else {
				$mySqlStmt .= $mySep . $myColumn . "='" . $myDataValue[0] . "'";
			}
		} else {
			$mySqlStmt .= $mySep . $myColumn . "=null";
		}
		$mySep = ", ";
	}

	$mySep = "";
	$mySqlStmt .= " Where ";
	foreach($myKeys as $myKey) {
		$myDataValue = $inRowNode->xpath($myKey);
		$mySqlStmt .= $mySep . $myKey . " = '" . $myDataValue[0] . "'";
		$mySep = " AND ";
	}

	$result = $dbConnect->query($mySqlStmt);
	if ($dbConnect->error) {
		printf("\n Errors detected on update %s %s", $dbConnect->error, $mySqlStmt);
		return false;
	} else {
		printf("\n Update successful %d ", $result);
		/* mysqli_get_client_info() */
		return true;
	}
}

/* ****************************************
**************************************** */
function insertRow($dbConnect, $inTableName, $inTableNode, $inRowNode) {
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
		if ( strlen($myDataValue[0]) > 0 ) {
			if ( $myDataValue[0] == 'True' ) {
				$mySqlStmt .= $mySep . " 1";
			} else if ( $myDataValue[0] == 'False' ) {
				$mySqlStmt .= $mySep . " 0";
			} else {
				$mySqlStmt .= $mySep . " '" . $myDataValue[0] . "'";
			}
		} else {
			$mySqlStmt .= $mySep . " null";
		}
		$mySep = ", ";
	}

	$mySqlStmt .= " );";

	//echo "\nSQL=$mySqlStmt";
	$result = $dbConnect->query($mySqlStmt);
	if ($dbConnect->error) {
		printf("\n Errors detected on insert %s %s", $dbConnect->error, $mySqlStmt);
		return false;
	} else {
		printf("\n Record inserted %d", $dbConnect->insert_id);
		/* mysqli_get_client_info() */
		return true;
	}
}

/* ****************************************
**************************************** */
function deleteRow($dbConnect, $inTableName, $inTableNode, $inRowNode) {
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

	$result = $dbConnect->query($mySqlStmt);
	if ($dbConnect->error) {
		printf("\n Errors detected on delete %s ", $dbConnect->error);
		return false;
	} else {
		printf("\n Delete successful %d ", $result);
		/* mysqli_get_client_info() */
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
	updateTable($dbConnect, $myTable);
}
include_once( "WfwTerm.php" );
?>

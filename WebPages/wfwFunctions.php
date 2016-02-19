<?php



function printallSanctionIDs() {
  foreach( $_SESSION['allSanctionIDs'] as $key => $value){
      echo "SANCID: $key<br />";
          foreach( $value as $key2 => $value2){
              echo "$key2 $value2<br />";
          }

  }
}

function setEventAndRounds($sancID) {
	 foreach ($_SESSION['allSanctionIDs'] as $sanctions => $sanctionID) {
		 if ($sancID == $sanctions) {
			 $_SESSION["thisSancID"] = $sanctions;
			 $_SESSION["thisTourneyName"] = $sanctionID['tourneyName'];
			 $_SESSION["thisSlalomRounds"] = $sanctionID['slalomRounds'];
			 $_SESSION["thisTrickRounds"] = $sanctionID['trickRounds'];
			 $_SESSION["thisJumpRounds"] = $sanctionID['jumpRounds'];
			 $_SESSION["thisDivisions"] = $sanctionID['allDivisions'];
			 $_SESSION["thisSancID"] = array("tourName"=>$sanctionID['tourneyName']);
		 }
	 }
}

function getEventAndRounds($sancID) { // NOT CURRENTLY USING THIS...not sure we will need it
	 foreach ($_SESSION['allSanctionIDs'] as $sanctions => $sanctionID) {
		 if ($sancID == $sanctions) {
			 echo $sanctions . "<br />";
			 echo $sanctionID['tourneyName'];
			 echo $sanctionID['slalomRounds'];
			 echo $sanctionID['trickRounds'];
			 echo $sanctionID['jumpRounds'];


			 $_SESSION["thisSancID"] = $sanctions;
			 $_SESSION["thisTourneyName"] = $sanctionID['tourneyName'];
			 $_SESSION["thisSlalomRounds"] = $sanctionID['slalomRounds'];
			 $_SESSION["thisTrickRounds"] = $sanctionID['trickRounds'];
			 $_SESSION["thisJumpRounds"] = $sanctionID['jumpRounds'];
		 }
	 }
}


function isMultipleRounds($eventType) {
	if (strtolower($eventType) == strtolower("slalom") && $_SESSION["thisSlalomRounds"] > 1) {
		return $_SESSION["thisSlalomRounds"];
	}
	if (strtolower($eventType) == strtolower("trick") && $_SESSION["thisTrickRounds"] > 1) {
		return $_SESSION["thisTrickRounds"];
	}
	if (strtolower($eventType) == strtolower("jump") && $_SESSION["thisJumpRounds"] > 1) {
		return $_SESSION["thisJumpRounds"];
	}
	else {
		return false;
	}
}

function buildRoundsMenu($numOfRounds) { // NOT GOING TO USE THIS FUNCTION SINCE BUILDING DROP DOWN MENU WITH ALL DATA
	$htmlString = '<span>';
	$htmlString .= '<form method="post" action="changeSessionData.php" style="display:inline-block"><select name="changeRoundSelector" id="changeRoundSelector" data-native-menu="true" data-mini="true" data-inline="true"  onchange="this.form.submit()">';
	for ($i=1;$i<=$numOfRounds;$i++) {
		if ($_SESSION['skiRound'] == $i) {
			$htmlString .= '<option value="' . $i . ' " selected>Rd ' . $i . '</option>';
		} else {
			$htmlString .= '<option value="' . $i . '">Rd ' . $i . '</option>';
		}
	}
	$htmlString .= '</select></form></span>';
	return $htmlString;
}


function buildExistingSelections() {

}

function buildNavMenu() {
	$currentEvent = strtolower($_SESSION['skiEvent']); // GET CURRENTLY SELECTED EVENT TO SET CHECKED
	$currentRound = $_SESSION['skiRound']; // GET CURRENTLY SELECTED DIVISION TO SET CHECKED
	$currentDivision = $_SESSION['divisionID']; // GET CURRENTLY SELECTED DIVISION TO SET CHECKED

	$sRoundsString="";
	$tRoundsString="";
	$jRoundsString="";


	$htmlString = "\n\r<fieldset data-role='controlgroup' data-type='horizontal' data-mini='true' id='eventFieldSet'>\n\r";

	// Build SLALOM events
	$sRounds = $_SESSION["thisSlalomRounds"];
	if ($sRounds > 0) {
			$htmlString .= "<input type='radio' name='skiEvent' id='slalom' value='Slalom'><label for='slalom'>Slalom</label>\n\r";
		$sRoundsString = "<div id='slalomRoundDiv'>\n\r<fieldset data-role='controlgroup' data-type='horizontal' data-mini='true'>\n\r";
		for ($s=1;$s<=$sRounds;$s++) {
			$sRoundsString .= "<input type='radio' name='changeRoundSelector' id='slalomRound" . $s ."' value='" . $s . "'><label for='slalomRound" . $s ."'>Round " . $s . "</label>\n\r";
		}
		$sRoundsString .= "</fieldset>\n\r</div>\n\r";
	}


	// Build TRICK events
	$tRounds = $_SESSION["thisTrickRounds"];
	if ($tRounds > 0) {
			$htmlString .= "<input type='radio' name='skiEvent' id='trick' value='Trick'><label for='trick'>Trick</label>\n\r";
		$tRoundsString = "<div id='trickRoundDiv'>\n\r<fieldset data-role='controlgroup' data-type='horizontal' data-mini='true'>\n\r";
		for ($t=1;$t<=$tRounds;$t++) {
			$tRoundsString .= "<input type='radio' name='changeRoundSelector' id='trickRound" . $t ."' value='" . $t . "'><label for='trickRound" . $t ."'>Round " . $t . "</label>\n\r";
		}
		$tRoundsString .= "</fieldset>\n\r</div>\n\r";
	}


	// Build JUMP events
	$jRounds = $_SESSION["thisJumpRounds"];
	if ($jRounds > 0) {
			$htmlString .= "<input type='radio' name='skiEvent' id='jump' value='Jump'><label for='jump'>Jump</label>\n\r";
		$jRoundsString = "<div id='jumpRoundDiv'>\n\r<fieldset data-role='controlgroup' data-type='horizontal' data-mini='true'>\n\r";
		for ($j=1;$j<=$jRounds;$j++) {
			$jRoundsString .= "<input type='radio' name='changeRoundSelector' id='jumpRound" . $j ."' value='" . $j . "'><label for='jumpRound" . $j ."'>Round " . $j . "</label>\n\r";
		}
		$jRoundsString .= "</fieldset>\n\r</div>\n\r";
	}
	$htmlString .= "</fieldset>\n\r";
	//Build Rounds

	$htmlString .= $sRoundsString;
	$htmlString .= $tRoundsString;
	$htmlString .= $jRoundsString;


	//Build Divisions
	$divisions = explode("|",$_SESSION["thisDivisions"]);
	$numOfDivisions = count($divisions);
	$divisionsString = "<div id='divisions'><fieldset data-role='controlgroup' data-type='horizontal' data-mini='true'>\n\r";
	for ($i=0;$i<$numOfDivisions;$i++) {
		$divisionsString .= "<input type='radio' name='divisionID' id='divisionID" . $divisions[$i] ."' value='" . $divisions[$i] . "'><label for='divisionID" . $divisions[$i] ."'> " . $divisions[$i] . " </label>\n\r";
	}
	$divisionsString .= "</fieldset></div>\n\r";


	$htmlString .= $divisionsString;

	// SHOW CURRENTLY SELECTED EVENT, ROUND, DIVISION when page loads
	$htmlString .= "<script>$('#" .$currentEvent. "').attr('checked',true);</script>\n\r";
	$htmlString .= "<script>$('#" . $currentEvent . "Round" .$currentRound. "').attr('checked',true);</script>\n\r";
	$htmlString .= "<script>$('#divisionID" .$currentDivision. "').attr('checked',true);</script>\n\r";

	$htmlString .= "<script>$('#" .$currentEvent . "RoundDiv').show();</script>\n\r";
	$htmlString .= "<script>$('#divisions').show();</script>\n\r";

	return $htmlString;


}
?>
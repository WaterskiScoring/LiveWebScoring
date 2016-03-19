// JavaScript Document
$("a[data-ajax=false]").click(handleClick);

/* ******************************************************************** */
function handleClick(e) { // AJAX SPECIFIC
    var target = $(e.target).closest('a');
    if( target ) {
        e.preventDefault();
        window.location = target.attr('href');
    }
}

/* ******************************************************************** */
// WHEN DOCUMENT LOADS, SLIDE NAVIGATION PANEL DOWN
$(document).ready(function() { 
    $("#toggle").click(function(){
        $("#panel").slideToggle(400);
    });
});

/* ********************************** BUILDNAVHTML ********************************** */
// GLOBAL FUNCTION to build available options to choose for this.sancID
function buildNavHTML() {
	var eventString="";
	var sRoundsString="";
	var tRoundsString="";
	var jRoundsString="";
	var oRoundsString="";
	var sDivisionsString="";
	var tDivisionsString="";
	var jDivisionsString="";
	var oDivisionsString="";
	var submitString="";

	// BUILD SLALOM EVENTS
	//var formStartString = "<form id='getScoreForm'>";
	eventString += "<div id='eventsDiv'><fieldset data-role='controlgroup' data-type='horizontal' data-mini='true'>";
	
	if (this.slalom>0) {
		// BUILD EVENT
		eventString += "<input type='radio' name='skiEvent' id='slalom' value='Slalom' />"
			+ "<label id='slabel' for='slalom'>Slalom</label>";
		
		// BUILD ROUNDS
		sRoundsString += "<div id='slalomRoundDiv'><fieldset data-role='controlgroup' data-type='horizontal' data-mini='true'>";
		for (s=1;s<=slalom;s++) {
			sRoundsString += "<input type='radio' name='changeRoundSelector' id='slalomRound" + s + "' value='" + s + "'><label for='slalomRound" + s + "'>Round " + s + "</label>";	
		}
		sRoundsString += "</fieldset></div>";

		// BUILD SLALOM DIVISIONS
		sDivisionsString += "<div style='display:none' id='sDivisions'><fieldset data-role='controlgroup' data-type='horizontal' data-mini='true'>";	
		try {
			for (sd=0;sd<slalomDivisions.length;sd++) {
				sDivisionsString += "<input type='radio' name='divisionID' id='slalomdivisionID" + slalomDivisions[sd] 
					+ "' value='" + slalomDivisions[sd] + "'>" 
					+ "<label for='slalomdivisionID" + slalomDivisions[sd] + "'> " + slalomDivisions[sd] + " </label>";
			}
		} catch(e) {
			sDivisionsString += "<input type='radio' name='divisionID' id='jumpdivisionID0" + "' value=''>";
		}
		sDivisionsString += "</fieldset></div>";	
	} // END OF SLALOM
		
	// BUILD TRICK EVENTS
	if (this.trick>0) {
		// BUILD EVENT
		eventString += "<input type='radio' name='skiEvent' id='trick' value='Trick' />"
			+ "<label for='trick'>Trick</label>"; 
		
		// BUILD ROUNDS
		tRoundsString += "<div id='trickRoundDiv'><fieldset data-role='controlgroup' data-type='horizontal' data-mini='true'>"; 
		for (t=1;t<=trick;t++) {
			tRoundsString += "<input type='radio' name='changeRoundSelector' id='trickRound" + t + "' value='" + t + "'><label for='trickRound" + t + "'>Round " + t + "</label>";	
		}
		tRoundsString += "</fieldset></div>";

		// BUILD TRICK DIVISIONS
		tDivisionsString += "<div style='display:none' id='tDivisions'><fieldset data-role='controlgroup' data-type='horizontal' data-mini='true'>";	
		try {
			for (td=0;td<trickDivisions.length;td++) {
				tDivisionsString += "<input type='radio' name='divisionID' id='trickdivisionID" + trickDivisions[td] +"' value='" + trickDivisions[td] + "'><label for='trickdivisionID" + trickDivisions[td] + "'> " + trickDivisions[td] + " </label>";
			}
		} catch(e) {
			tDivisionsString += "<input type='radio' name='divisionID' id='trickdivisionID0" + "' value=''>";
		}
		tDivisionsString += "</fieldset></div>";	
	} // END OF TRICK 
		
		
	// BUILD JUMP EVENTS
	if (this.jump>0) {
		// BUILD EVENT
		eventString += "<input type='radio' name='skiEvent' id='jump' value='Jump' />"
			+ "<label for='jump'>Jump</label>"; 
		
		// BUILD ROUNDS
		jRoundsString += "<div id='jumpRoundDiv'><fieldset data-role='controlgroup' data-type='horizontal' data-mini='true'>"; 
		for (j=1;j<=jump;j++) {
			jRoundsString += "<input type='radio' name='changeRoundSelector' id='jumpRound" + j + "' value='" + j + "'><label for='jumpRound" + j + "'>Round " + j + "</label>";	
		}
		jRoundsString += "</fieldset></div>";

		// BUILD JUMP DIVISIONS
		jDivisionsString += "<div style='display:none' id='jDivisions'><fieldset data-role='controlgroup' data-type='horizontal' data-mini='true'>";	
		try {
			for (jd=0;jd<jumpDivisions.length;jd++) {
				jDivisionsString += "<input type='radio' name='divisionID' id='jumpdivisionID" + jumpDivisions[jd] +"' value='" + jumpDivisions[jd] + "'><label for='jumpdivisionID" + jumpDivisions[jd] + "'> " + jumpDivisions[jd] + " </label>";
			}
		} catch(e) {
			jDivisionsString += "<input type='radio' name='divisionID' id='jumpdivisionID0" + "' value=''>";
		}
		jDivisionsString += "</fieldset></div>";	
	} // END OF JUMP
		 
	if (this.slalom>0 && this.jump>0 && this.trick>0) {
		// BUILD EVENT
		eventString += "<input type='radio' name='skiEvent' id='overall' value='Overall' />"
			+ "<label for='overall'>Overall</label>"; 
		
		// BUILD ROUNDS
		oRoundsString += "<div id='overallRoundDiv'><fieldset data-role='controlgroup' data-type='horizontal' data-mini='true'>"; 
		for (o=1;o<=jump;o++) {
			oRoundsString += "<input type='radio' name='changeRoundSelector' id='overallRound" + o + "' value='" + o + "'><label for='overallRound" + o + "'>Round " + o + "</label>";	
		}
		oRoundsString += "</fieldset></div>";

		// BUILD OVERALL DIVISIONS
		oDivisionsString += "<div style='display:none' id='oDivisions'><fieldset data-role='controlgroup' data-type='horizontal' data-mini='true'>";	
		try {
			for (od=0;od<overallDivisions.length;od++) {
				oDivisionsString += "<input type='radio' name='divisionID' id='overalldivisionID" + overallDivisions[od] +"' value='" + overallDivisions[od] + "'><label for='overalldivisionID" + overallDivisions[od] + "'> " + overallDivisions[od] + " </label>";
			}
		} catch(e) {
			oDivisionsString += "<input type='radio' name='divisionID' id='overalldivisionID0" + "' value=''>";
		}
		oDivisionsString += "</fieldset></div>";	
	}
		
	eventString += "<input type='radio' name='skiEvent' id='recent' value='Recent' />"
		+ "<label for='recent'>Recent</label>";

	eventString += "<input type='radio' name='skiEvent' id='team' value='Team' />"
		+ "<label for='team'>Team</label>";

	eventString += "</fieldset></div>";
	submitString += "<div style='display:none' id='submitBtn'>" 
		+ "<button data-mini='true' data-inline='true' data-theme='b' name='submit' value='submit-value'>" 
		+ "Submit</button></div>";
	//submitString += "<div style='display:none' id='submitBtn'><button type='submit' data-mini='true' data-inline='true' data-theme='b' name='submit' value='submit-value'>Submit</button></div></form>";
	//var formEndString = "</form>";


	//$("#panel").append(formStartString).trigger('create');
	$("#panel").append(eventString).trigger('create');
	$("#panel").append(sRoundsString).trigger('create');
	$("#panel").append(tRoundsString).trigger('create');
	$("#panel").append(jRoundsString).trigger('create');
	$("#panel").append(oRoundsString).trigger('create');
	$("#panel").append(sDivisionsString).trigger('create');
	$("#panel").append(tDivisionsString).trigger('create');
	$("#panel").append(jDivisionsString).trigger('create');
	$("#panel").append(oDivisionsString).trigger('create');
	$("#panel").append(submitString).trigger('create');
	//$("#panel").append(formEndString).trigger('create');
			
			
	$(document).ready(function(){
	  $("#recent").click(function(){
		$("#slalomRoundDiv,#jumpRoundDiv,#trickRoundDiv,#overallRoundDiv").hide(350);
		$("#sDivisions,#jDivisions,#tDivisions,#oDivisions").hide(350);
		$("#submitBtn").show(200);
	  });
	});

	$(document).ready(function(){
	  $("#team").click(function(){
		$("#slalomRoundDiv,#jumpRoundDiv,#trickRoundDiv,#overallRoundDiv").hide(350);
		$("#sDivisions,#jDivisions,#tDivisions,#oDivisions").hide(350);
		$("#submitBtn").show(200);
	  });
	});

	$(document).ready(function(){
	  $("#slalom").click(function(){
		$("#trickRoundDiv,#jumpRoundDiv,#overallRoundDiv").hide(350);
		$("#tDivisions,#jDivisions,#oDivisions").hide(350);
		$("#slalomRoundDiv").show(100);
	  });
	});

	$(document).ready(function(){
	  $("#trick").click(function(){
		$("#slalomRoundDiv,#jumpRoundDiv,#overallRoundDiv").hide(350);
		$("#sDivisions,#jDivisions,#oDivisions").hide(350);
		$("#trickRoundDiv").show(200);
	  });
	});

	$(document).ready(function(){
	  $("#jump").click(function(){
		$("#slalomRoundDiv,#trickRoundDiv,#overallRoundDiv").hide(350);
		$("#sDivisions,#tDivisions,#oDivisions").hide(350);
		$("#jumpRoundDiv").show(200);
	  });
	});

	$(document).ready(function(){
	  $("#overall").click(function(){
		$("#slalomRoundDiv,#trickRoundDiv,#jumpRoundDiv").hide(350);
		$("#sDivisions,#tDivisions,#jDivisions").hide(350);
		$("#overallRoundDiv").show(200);
	  });
	});

	$(document).ready(function(){
	  $("#slalomRoundDiv :input").click(function(){
		$("#tDivisions,#jDivisions,#oDivisions").hide(350);
		$("#sDivisions").show(200);
	  });
	});

	$(document).ready(function(){
	  $("#trickRoundDiv :input").click(function(){
		$("#sDivisions,#jDivisions,#oDivisions").hide(350);
		$("#tDivisions").show(200);
	  });
	});

	$(document).ready(function(){
	  $("#jumpRoundDiv :input").click(function(){
		$("#sDivisions,#tDivisions,#oDivisions").hide(350);
		$("#jDivisions").show(200);
	  });
	});

	$(document).ready(function(){
	  $("#overallRoundDiv :input").click(function(){
		$("#sDivisions,#tDivisions,#jDivisions").hide(350);
		$("#oDivisions").show(200);
	  });
	});

	$(document).ready(function(){
		$("#sDivisions :input,#tDivisions :input,#jDivisions :input,#oDivisions :input").click(function(){
		$("#submitBtn").show(200);
	  });
	});

	$(document).ready(function(){
	$("#submitBtn").click(function() {
		$("#toggle").html("Loading...").trigger('create');
		$("#panel").hide(300);

		fields.push({name: 'sanctionID', value: TournamentInfo.sancID});  
		$.ajax({url:"getScores_json.php",data:fields,type:'POST',success:function(scoresHTML) {
			//$( "#scoresID" ).empty();

			// POPULATE PAGE WITH SCORES
			$("#scoresID").html(scoresHTML).trigger('create'); 
			var selectedString = TournamentInfo.eventName + ': ';
			$.each(fields, function(i, val) {
				if (val.name=="skiEvent") selectedString += val.value + " ";
				if (val.name =="changeRoundSelector") selectedString += "Round " + val.value + " ";
				if (val.name=="divisionID") selectedString += val.value;
			});
			
			document.title = selectedString;
			$("#toggle").html(selectedString).trigger('create'); // PLACE SELECTED EVENT, ROUND, DIVISION IN HEADER 
			$("#panel").die(); // NOT SURE WHY I HAD TO DO THIS BUT IT PREVENTS THE PANEL DIV FROM DUPLICATING AFTER THIS AJAX SUBMISSION
		}}) // END ajax function		*/

	});
	});


	$(document).ready(function(){
	$("#refreshBtn").click(function() {
		$("#toggle").html("Loading...").trigger('create');
		$("#panel").hide(300);

		fields.push({name: 'sanctionID', value: TournamentInfo.sancID});  
		$.ajax({url:"getScores_json.php",data:fields,type:'POST',success:function(scoresHTML) {
			//$( "#scoresID" ).empty();

			// POPULATE PAGE WITH SCORES
			$("#scoresID").html(scoresHTML).trigger('create'); 
			var selectedString = TournamentInfo.eventName + ': ';
			$.each(fields, function(i, val) {
				if (val.name=="skiEvent") selectedString += val.value + " ";
				if (val.name =="changeRoundSelector") selectedString += "Round " + val.value + " ";
				if (val.name=="divisionID") selectedString += val.value;
			});
			
			document.title = selectedString;
			$("#toggle").html(selectedString).trigger('create'); // PLACE SELECTED EVENT, ROUND, DIVISION IN HEADER 
			$("#panel").die(); // NOT SURE WHY I HAD TO DO THIS BUT IT PREVENTS THE PANEL DIV FROM DUPLICATING AFTER THIS AJAX SUBMISSION
		}}) // END ajax function		*/

	});
	});


	// GLOBAL FIELDS OBJECT
	var fields="";
	function buildJsonObj() {
		fields = $( ":input" ).serializeArray();
	}

	$( ":radio" ).click( buildJsonObj );
	buildJsonObj();

	function showValues() {
		var fields = $( ":input" ).serialize();
		$( "#scoresID" ).empty();
		jQuery.each( fields, function( i, field ) {
		  $( "#scoresID" ).append( field.name + ":" + field.value + " " );
		});
	  }

	setDefaults(); // THIS IS FOR DEEP LINKING TO SCORES
} /* ********************************** END BUILDNAVHTML********************************** */


/* ********************************** BUILDRUNORDERNAVHTML ********************************** */
// GLOBAL FUNCTION to build available options to choose for this.sancID
function buildRunOrderNavHTML() {
	var eventString="";
	var sGroupsString="";
	var tGroupsString="";
	var jGroupsString="";
	var submitString="";
	
	//alert('buildRunOrderNavHTML' + '\nSlalom Groups: ' + sGroupsString + '\nTrick Groups: ' + sGroupsString + '\nJump Group: ' + sGroupsString);

	// BUILD SLALOM EVENTS
	//var formStartString = "<form id='getRunOrderForm'>";
	eventString += "<div id='eventsDiv'><fieldset data-role='controlgroup' data-type='horizontal' data-mini='true'>";

	if (this.slalom>0) {
		// BUILD EVENT
		eventString += "<input type='radio' name='skiEvent' id='slalom' value='Slalom'>"
			+ "<label id='slabel' for='slalom'>Slalom</label>";
		
		// BUILD SLALOM Groups
		sGroupsString += "<div style='display:none' id='sGroups'>"
			+ "<fieldset data-role='controlgroup' data-type='horizontal' data-mini='true'>";	
		try {
			for (sd=0;sd<slalomGroups.length;sd++) {
				sGroupsString += "<input type='radio' name='groupID' id='slalomGroupID" 
					+ slalomGroups[sd] 
					+"' value='" + slalomGroups[sd] + "'>"
					+ "<label for='slalomGroupID" + slalomGroups[sd] + "'> " + slalomGroups[sd] + " </label>";
			}
		} catch(e) {
			//alert('Exception encountered building slalom running order groups: \n\n' + e.message);
			sGroupsString += "<input type='radio' name='groupID' id='slalomGroupID0" + "' value=''>";
		}
		sGroupsString += "</fieldset></div>";	
	} // END OF SLALOM
		
	// BUILD TRICK EVENTS
	if (this.trick>0) {
		// BUILD EVENT
		eventString += "<input type='radio' name='skiEvent' id='trick' value='Trick'><label for='trick'>Trick</label>"; 
		
		// BUILD TRICK Groups
		tGroupsString += "<div style='display:none' id='tGroups'>" 
			+ "<fieldset data-role='controlgroup' data-type='horizontal' data-mini='true'>";	
		try {
			for (td=0;td<trickGroups.length;td++) {
				tGroupsString += "<input type='radio' name='groupID' id='trickGroupID" + trickGroups[td] 
					+ "' value='" + trickGroups[td] + "'>" 
					+ "<label for='trickGroupID" + trickGroups[td] + "'> " + trickGroups[td] + " </label>";
			}
		} catch(e) {
			//alert('Exception encountered building trick running order groups: \n\n' + e.message);
			tGroupsString += "<input type='radio' name='groupID' id='trickGroupID0" + "' value=''>";
		}
		tGroupsString += "</fieldset></div>";	
	} // END OF TRICK 
		
		
	// BUILD JUMP EVENTS
	if (this.jump>0) {
		// BUILD EVENT
		eventString += "<input type='radio' name='skiEvent' id='jump' value='Jump'><label for='jump'>Jump</label>"; 
		
		// BUILD JUMP Groups
		jGroupsString += "<div style='display:none' id='jGroups'><fieldset data-role='controlgroup' data-type='horizontal' data-mini='true'>";	
		try {
			for (jd=0;jd<jumpGroups.length;jd++) {
				jGroupsString += "<input type='radio' name='groupID' id='jumpGroupID" + jumpGroups[jd] 
				+ "' value='" + jumpGroups[jd] + "'>" 
				+ "<label for='jumpGroupID" + jumpGroups[jd] + "'> " + jumpGroups[jd] + " </label>";
			}
		} catch(e) {
			//alert('Exception encountered building jump running order groups: \n\n' + e.message);
			jGroupsString += "<input type='radio' name='groupID' id='jumpGroupID0" + "' value=''>";
		}
		jGroupsString += "</fieldset></div>";	
	} // END OF JUMP
		 
		
	eventString += "</fieldset></div>";
	submitString += "<div style='display:none' id='submitBtn'>" 
		+ "<button data-mini='true' data-inline='true' data-theme='b' name='submit' value='submit-value'>" 
		+ "Submit</button></div>";
	//submitString += "<div style='display:none' id='submitBtn'><button type='submit' data-mini='true' data-inline='true' data-theme='b' name='submit' value='submit-value'>Submit</button></div></form>";
	//var formEndString = "</form>";

	//$("#panel").append(formStartString).trigger('create');
	$("#panel").append(eventString).trigger('create');
	$("#panel").append(sGroupsString).trigger('create');
	$("#panel").append(tGroupsString).trigger('create');
	$("#panel").append(jGroupsString).trigger('create');
	$("#panel").append(submitString).trigger('create');
	//$("#panel").append(formEndString).trigger('create');
			
			
	$(document).ready(function(){
	  $("#slalom").click(function(){
		$("#tGroups,#jGroups").hide(350);
		$("#sGroups").show(100);
	  });
	});

	$(document).ready(function(){
	  $("#trick").click(function(){
		$("#sGroups,#jGroups").hide(350);
		$("#tGroups").show(200);
	  });
	});

	$(document).ready(function(){
	  $("#jump").click(function(){
		$("#sGroups,#tGroups").hide(350);
		$("#jGroups").show(200);
	  });
	});

	$(document).ready(function(){
		$("#sGroups :input,#tGroups :input,#jGroups :input").click(function(){
		$("#submitBtn").show(200);
	  });
	});

	$(document).ready(function(){
	$("#submitBtn").click(function() {
		$("#toggle").html("Loading tour running order...").trigger('create');
		$("#panel").hide(300);

		fields.push({name: 'sanctionID', value: TournamentInfo.sancID});  
		$.ajax({url:"getRunOrder_json.php",data:fields,type:'POST',success:function(runOrderHTML) {
			//$( "#runOrderID" ).empty();

			// POPULATE PAGE WITH RUN ORDERS
			$("#runOrderID").html(runOrderHTML).trigger('create'); 
			var selectedString = TournamentInfo.eventName + ': ';
			$.each(fields, function(i, val) {
				if (val.name=="skiEvent") selectedString += val.value + " ";
				if (val.name=="groupID") selectedString += val.value;
			});
			
			document.title = selectedString;
			$("#toggle").html(selectedString).trigger('create'); // PLACE SELECTED EVENT, GROUPS IN HEADER 
			$("#panel").die(); // NOT SURE WHY I HAD TO DO THIS BUT IT PREVENTS THE PANEL DIV FROM DUPLICATING AFTER THIS AJAX SUBMISSION
		}}) // END ajax function		*/

	});
	});


	$(document).ready(function(){
	$("#refreshBtn").click(function() {
		$("#toggle").html("Loading...").trigger('create');
		$("#panel").hide(300);

		fields.push({name: 'sanctionID', value: TournamentInfo.sancID});  
		$.ajax({url:"getRunOrder_json.php",data:fields,type:'POST',success:function(runOrderHTML) {
			//$( "#runOrderID" ).empty();

			// POPULATE PAGE WITH RUN ORDERS
			$("#runOrderID").html(runOrderHTML).trigger('create'); 
			var selectedString = TournamentInfo.eventName + ': ';
			$.each(fields, function(i, val) {
				if (val.name=="skiEvent") selectedString += val.value + " ";
				if (val.name=="groupID") selectedString += val.value;
			});
			
			document.title = selectedString;
			$("#toggle").html(selectedString).trigger('create'); // PLACE SELECTED EVENT, GROUP IN HEADER 
			$("#panel").die(); // NOT SURE WHY I HAD TO DO THIS BUT IT PREVENTS THE PANEL DIV FROM DUPLICATING AFTER THIS AJAX SUBMISSION
		}}) // END ajax function		*/

	});
	});


	// GLOBAL FIELDS OBJECT
	var fields="";
	function buildJsonObj() {
		fields = $( ":input" ).serializeArray();
	}

	$( ":radio" ).click( buildJsonObj );
	buildJsonObj();

	function showValues() {
		var fields = $( ":input" ).serialize();
		$( "#runOrderID" ).empty();
		jQuery.each( fields, function( i, field ) {
		  $( "#runOrderID" ).append( field.name + ":" + field.value + " " );
		});
	  }

	setDefaults(); // THIS IS FOR DEEP LINKING TO SCORES
} /* ********************************** END BUILDRUNORDERNAVHTML ********************************** */

/* ******************************************************************** */
// CREATES TOURNAMENT OBJECT AND STORES TOURNAMENT PROPERTIES.  DOING THIS ONE WITH JSON
function Tournament(inSancID, inEventName) { 
	this.eventName = inEventName;
	this.sancID = inSancID; // STORE SANCTION ID
	//this.allEvents = ""; // NOT SURE THIS IS NEEDED AS IMPLIED WITH SLALOM|TRICK|JUMP > 0
	//this.allRounds = ""; // NOT SURE THIS IS NEEDED AS IMPLIED WITH SLALOM|TRICK|JUMP > 0
	//this.allDivisions = ""; // NOT SURE THIS IS NEEDED...STORE DIVISION FOR WHOLE TOURNAMENT, NOT PER ROUND
	this.slalom = ""; // STORE ROUNDS OF SLALOM
	this.trick = ""; // STORE ROUNDS OF TRICK
	this.jump = ""; // STORE ROUNDS OF JUMP
	this.overall = ""; // STORE ROUNDS OF OVERALL
	this.slalomDivisions = ""; // STORE DIVISIONS OF SLALOM
	this.trickDivisions = ""; // STORE DIVISIONS OF TRICK
	this.jumpDivisions = ""; // STORE DIVISIONS OF JUMP
	this.overallDivisions = ""; // STORE DIVISIONS OF OVERALL
	this.slalomGroups = ""; // STORE GROUPS OF SLALOM
	this.trickGroups = ""; // STORE GROUPS OF TRICK
	this.jumpGroups = ""; // STORE GROUPS OF JUMP
	this.overallGroups = ""; // STORE GROUPS OF OVERALL

	this.getDivisionsPerEvent=getDivisionsPerEvent;
	this.getSancID=getSancID;

	function getSancID() {
		return this.sancID;	
	}
	
	//INIT FUNCTION
	this.init = function() {
		$.ajax({url:"getEvtsAndRds_json.php",data:{ sanctionID:this.sancID},type:'POST',dataType: 'json',success:function(eventRounds_json_obj) {
			buildEvtsAndRoundsHTML(eventRounds_json_obj);
		}}) // END ajax function
	} // END THIS.INIT FUNCTION
		
	// METHOD RETURNS DIVISIONS PER EVENT IN JSON
	function buildEvtsAndRoundsHTML(eventsAndRounds) { 
		for (var key in eventsAndRounds) {
		   if (eventsAndRounds.hasOwnProperty(key)) { // STORE ROUNDS IN PROPERTIES OF TOURNAMENT OBJECT for slalom, jump, trick
			  this.slalom = eventsAndRounds["Slalom"]; //
			  this.trick = eventsAndRounds["Trick"];
			  this.jump = eventsAndRounds["Jump"];
		   }
		}
		
		if (this.slalom>0) {
			getDivisionsPerEvent(inSancID, "Slalom");
			getGroupsPerEvent(inSancID, "Slalom");
		}
		if (this.trick>0) {
			getDivisionsPerEvent(inSancID, "Trick");
			getGroupsPerEvent(inSancID, "Trick");
		}
		if (this.jump>0) {
			getDivisionsPerEvent(inSancID, "Jump");
			getGroupsPerEvent(inSancID, "Jump");
		}
		this.overall = 0;
		if (this.slalom>0) {
			if (this.trick>0) {
				if (this.jump>0) {
					if ( this.slalom >= this.trick && this.slalom >= this.jump) {
						this.overall = this.slalom;
					} else if ( this.trick >= this.slalom && this.trick >= this.jump) {
						this.overall = this.trick;
					} else if ( this.jump >= this.slalom && this.jump >= this.trick) {
						this.overall = this.jump;
					} else {
						this.overall = this.slalom;
					}
					getDivisionsPerEvent(inSancID, "Overall");
					getGroupsPerEvent(inSancID, "Overall");
				}
			}
		}
			
	} // END getDivisionsPerEvent
		
	//METHOD DIVISIONS FOR EACH EVENT	
	function getDivisionsPerEvent(sanctionID, skiEvent) {
		$.ajax({url:"getDivisionsPerEvent_json.php",data:{ skiEvent:skiEvent, sanctionID:sanctionID },type:'POST',dataType: 'json',success:function(divPerEvent_json_obj) {
			if (skiEvent == "Slalom") slalomDivisions = divPerEvent_json_obj;
			if (skiEvent == "Trick") trickDivisions = divPerEvent_json_obj;
			if (skiEvent == "Jump") jumpDivisions = divPerEvent_json_obj;
			if (skiEvent == "Overall") overallDivisions = divPerEvent_json_obj;
		}}) // END ajax function
	}
	
	//METHOD DIVISIONS FOR EACH EVENT	
	function getGroupsPerEvent(sanctionID, skiEvent) {
		$.ajax({url:"getGroupsPerEvent_json.php",data:{ skiEvent:skiEvent, sanctionID:sanctionID },type:'POST',dataType: 'json',success:function(groupPerEvent_json_obj) {
			if (skiEvent == "Slalom") slalomGroups = groupPerEvent_json_obj;
			if (skiEvent == "Trick") trickGroups = groupPerEvent_json_obj;
			if (skiEvent == "Jump") jumpGroups = groupPerEvent_json_obj;
			if (skiEvent == "Overall") overallDivisions = overallGroups;
		}}) // END ajax function
	}
	
	try {
		this.init();
	} catch(e) {
		alert('Exception encountered with init method \n' + e);
	}

} /* ********************************** END TOURNAMENT OBJECT CONSTRUCTOR ********************************** */

/* ******************************************************************** */
// DATA REQUIRED TO BE SENT FROM RECENT PASSES PAGE TO GET SCORES FOR THE DIVISION OF SKIER SELECTED
function getScoresFromRecent (division,eventRound,eventType) { 
	document.forms['getRecentScoresForm'].divisionID.value=division;
	document.forms['getRecentScoresForm'].eventRoundID.value=eventRound;
	document.forms['getRecentScoresForm'].eventTypeID.value=eventType;
	document.forms['getRecentScoresForm'].submit();	
}


/* ******************************************************************** */
// USED FOR SENDING SANCTION INFO FROM TOURNY LIST PAGE TO GETSCORES
function getTourneyBySanctionID(sancID, EventName) { 
	document.forms['tourneyList'].sanctionID.value=sancID;
	document.forms['tourneyList'].EventName.value=EventName;
	document.forms['tourneyList'].submit();	
}

/* ******************************************************************** */
// AJAX SPECIFIC RELOAD FEATURE
function reloadPage() { 
  $.mobile.changePage(
    document.location.reload(true),
    {
      allowSamePageTransition : true,
      transition              : 'none',
      showLoadMsg             : false,
      reloadPage              : true
    }
  );
}

/* ******************************************************************** */
// SET COOKIES...CURRENTLY FOR RECENT PASSES REFRESH RATE
function setCookie(c_name,value,exdays) { 
	var exdate=new Date();
	exdate.setDate(exdate.getDate() + exdays);
	var c_value=escape(value) + ((exdays==null) ? "" : "; expires="+exdate.toUTCString());
	document.cookie=c_name + "=" + c_value;
}

/* ******************************************************************** */
// RETRIEVE AND RETURN COOKIE VALUES...CURRENTLY FOR RECENT PASSES REFRESH RATE
function getCookie(c_name) { 
	var c_value = document.cookie;
	var c_start = c_value.indexOf(" " + c_name + "=");
	if (c_start == -1)
	  {
	  c_start = c_value.indexOf(c_name + "=");
	  }
	if (c_start == -1)
	  {
	  c_value = null;
	  }
	else
	  {
	  c_start = c_value.indexOf("=", c_start) + 1;
	  var c_end = c_value.indexOf(";", c_start);
	  if (c_end == -1)
	  {
	c_end = c_value.length;
	}
	c_value = unescape(c_value.substring(c_start,c_end));
	}
	return c_value;
}
<!--#include virtual="/rankings/SettingsHQ.asp"-->
<%
Response.Buffer = True
Server.ScriptTimeout = 2400 

Dim tempLast, tempFirst, tempPlace, tempDiv, tempSL, tempTR, tempJU, tempScore, tempAlt
Dim tempEvent, tempPQ1, tempPQ2, tempSex, tempYOB, TempSlmMiss, tempSpecial, tempIWSF, tempExport
Dim sTourID, sTSanction, sTName, sTDateE, sTDPretty, sTSiteID, sTSite, sIWWFSubj
Dim eMailTo, eMailCC, SeedRep, Owner
Dim RecsSaved, RecBypassedNoScore, RecBypassedRampHght, RecBypassedForDiv
Dim ErrMsg, RecsNoLic, LastNoLicMem, TraceMsg
Dim InsertCmd, sSQL, emailBody

' Initialize a few things
ErrMsg = ""
TraceMsg = ""
RecsSaved = 0
RecBypassedNoScore = 0
RecBypassedRampHght = 0
RecBypassedForDiv = 0
RecsNoLic = 0
LastNoLicMem = "000000000"

' Validate TourID value for scores to be Exported.
sTourID = SQLClean(Session("TourID"))
IF len(sTourID) <=0 THEN
    sTourID = Request.QueryString("TourID")
END IF	

IF len(sTourID) <=0 THEN
    ErrMsg = ErrMsg & "<br />Tournament ID not provided.  Setting default"
    sTourID = "17C999A"
END IF	

TraceMsg = TraceMsg & "<br />TourId=" & sTourID

' ***************************************
' Set up Database and Record Set Connection
' ***************************************
set rs = Server.CreateObject("ADODB.recordset")

' ***************************************
' Get Sanction Table Information for this event and store locally, if present
' ***************************************
sSQL = "Select top 1 ST.TSanction, ST.TName, ST.TSiteID, ST.TSite,"
sSQL = sSQL & " Convert(char(8),ST.TDateE,112) as TDateE,"
sSQL = sSQL & " Convert(char(12),ST.TDateE,107) as TDPretty,"
sSQL = sSQL & " ST.TDirName, ST.TDirEMail, CJ.CJudgName," 
sSQL = sSQL & " CJ.CJudgEMail, CC.CScorName, CC.CScorEMail"
sSQL = sSQL & " FROM " & SanctionTableName & " ST LEFT JOIN " & TRegSetupTableName
sSQL = sSQL & " RT on RT.TournAppID = ST.TournAppID LEFT JOIN " & PostTourTableName
sSQL = sSQL & " PT on PT.TournAppID = ST.TournAppID LEFT JOIN (Select '" & left(sTourID,6)
sSQL = sSQL & "' as TournAppID, FirstName + ' ' + LastName as CJudgName, Email as CJudgEMail"
sSQL = sSQL & " FROM " & MemberTablename & " Where patindex('%@%',Email) > 0 and"
sSQL = sSQL & " PersonID in (Select Cast(case when len(CJudgePID)<9 then CJudgePID"
sSQL = sSQL & " else right(CJudgePID,8) end as integer) FROM " & TRegSetupTableName 
sSQL = sSQL & " WHERE TournAppID = '" & left(sTourID,6) & "' and isnumeric(CJudgePID) = 1))"
sSQL = sSQL & " CJ on CJ.TournAppID = ST.TournAppID LEFT JOIN (Select '" & left(sTourID,6)
sSQL = sSQL & "' as TournAppID, FirstName + ' ' + LastName as CScorName, Email as CScorEMail"
sSQL = sSQL & " FROM " & MemberTablename & " Where patindex('%@%',Email) > 0 and"
sSQL = sSQL & " PersonID in (Select Cast(case when len(CScorePID)<9 then CScorePID"
sSQL = sSQL & " else right(CScorePID,8) end as integer) FROM " & TRegSetupTableName 
sSQL = sSQL & " WHERE TournAppID = '" & left(sTourID,6) & "' and isnumeric(CScorePID) = 1))"
sSQL = sSQL & " CC on CC.TournAppID = ST.TournAppID where upper(ST.TournAppID) = '"
sSQL = sSQL & left(sTourID,6) & "'"

' WriteDebugSQL (sSQL)

' ***************************************
' Open and run SQL statement to retrieve sanction and contact information
' ***************************************
rs.open sSQL, sConnectionToSanctionTable, 3, 3

IF rs.eof THEN
   ErrMsg = ErrMsg & "<br><br><center><h2>Requested Tournament ID&nbsp; <b>" & sTourID & "</b>&nbsp; not found.</h2></center><br>"
ELSE
   sTSanction = rs("TSanction")
   sTName = Replace(rs("TName"),","," ")
   sTDateE = rs("TDateE")
   sTDPretty = rs("TDPretty")
   sTSiteID = rs("TSiteID")
   sTSite = rs("TSite")

   eMailTo = ""
   IF len(rs("TDirEMail")) > 0 THEN
		  eMailTo = """" & rs("TDirName") & """ <" & rs("TDirEMail") & ">"
   END IF
   IF len(rs("CJudgEmail")) > 0 and instr(eMailTo,rs("CJudgName")) = 0 THEN
      IF len(eMailTo) > 0 THEN eMailTo = eMailTo & "; "
      eMailTo = eMailTo & """" & rs("CJudgName") & """ <" & rs("CJudgEmail") & ">"
   END IF
   IF len(rs("CScorEmail")) > 0 and instr(eMailTo,rs("CScorName")) = 0 THEN
      IF len(eMailTo) > 0 THEN eMailTo = eMailTo & "; "
      eMailTo = eMailTo & """" & rs("CScorName") & """ <" & rs("CScorEmail") & ">"
   END IF	

   rs.close
   sIWWFSubj = sTName & "," & sTSanction & "," & sTSiteID & "," & Left(sTDateE,4) & "-" & Mid(sTDateE,5,2) & "-" & Right(sTDateE,2)
    TraceMsg = TraceMsg & "<br />Title: " & sIWWFSubj
END IF


EmailCC = """Melanie Hanson"" <mhanson@usawaterski.org>; ""Dave Clark"" <awsatechdude@comcast.net>"
IF mid(sTourID,3,1) = "C" THEN
   EmailCC = EmailCC & "; ""Danny LeBourgeois"" <dleboo@gmail.com>"
ELSEIF mid(sTourID,3,1) = "E" THEN
   EmailCC = EmailCC & "; ""Jennifer Frederick-Kelly"" <jennifer@frederickmachine.com>"
ELSEIF mid(sTourID,3,1) = "S" THEN
   EmailCC = EmailCC & "; ""Kirby Whetsel"" <kwhetsel@charter.net>"
ELSEIF mid(sTourID,3,1) = "W" THEN
   EmailCC = EmailCC & "; ""Judy Stanford"" <judy-don@sbcglobal.net>"
END IF

IF Session("Firstname") & Session("LastName") = "DannyLeBourgeois" and mid(sTourID,3,1) <> "C" THEN
   EmailCC = EmailCC & "; ""Danny LeBourgeois"" <dleboo@gmail.com>"
ELSEIF Session("Firstname") & Session("LastName") = "JenniferFrederick-Kelley" and mid(sTourID,3,1) <> "E" THEN
   EmailCC = EmailCC & "; ""Jennifer Frederick-Kelly"" <jennifer@frederickmachine.com>"
ELSEIF Session("Firstname") & Session("LastName") = "KirbyWhetsel" and mid(sTourID,3,1) <> "S" THEN
   EmailCC = EmailCC & "; ""Kirby Whetsel"" <kwhetsel@charter.net>" 	
ELSEIF Session("Firstname") & Session("LastName") = "JudyStanford" and mid(sTourID,3,1) <> "W" THEN	
   EmailCC = EmailCC & "; ""Judy Stanford"" <judy-don@sbcglobal.net>"	
END IF

%>
<html><head><title>Please Wait...</title>
<SCRIPT LANGUAGE="JavaScript">
// First we detect the browser type
if(document.getElementById) { // IE 5 and up, NS 6 and up
	var upLevel = true;
	}
else if(document.layers) { // Netscape 4
	var ns4 = true;
	}
else if(document.all) { // IE 4
	var ie4 = true;
	}

function showObject(obj) {
if (ns4) {
	obj.visibility = "show";
	}
else if (ie4 || upLevel) {
	obj.style.visibility = "visible";
	}
}

function hideObject(obj) {
if (ns4) {
	obj.visibility = "hide";
	}
if (ie4 || upLevel) {
	obj.style.visibility = "hidden";
	}
}

</SCRIPT>
</head>
<BODY>
    <DIV ID="splashScreen" STYLE="position:absolute;z-index:5;top:30%;left:35%;">
        <TABLE BGCOLOR="#000000" BORDER=1 BORDERCOLOR="#000000"	CELLPADDING=0 CELLSPACING=0 HEIGHT=200 WIDTH=300>
            <TR>
                <TD WIDTH="100%" HEIGHT="100%" BGCOLOR="#CCCCCC" ALIGN="CENTER" VALIGN="MIDDLE">
                    <BR><BR>
                    <FONT FACE="Helvetica,Verdana,Arial" SIZE=3 COLOR="#000066">
                    <B>Processing Records for Export.<br><br>
                    Please wait a moment ...<br><br>  
                    </B></FONT>
                    
                    <IMG SRC="/rankings/images/buttons/wait.gif" BORDER=1 WIDTH=75 HEIGHT=15><BR><BR>
                </TD>
            </TR>
        </TABLE>
    </DIV>

<%
response.flush

' ***************************************
  ' Set up output text file.
' ***************************************
Set objFSO = Server.CreateObject("Scripting.FileSystemObject")
ExportFile = PathToIWWF & "\" & left(sTourID,6) & "RS-TEST.TXT"
Set objTextOut = objFSO.opentextfile(ExportFile,2,true)

' ***************************************
' Invoke "standard" Email Server Configuration -- defines objMessage object
' ***************************************
SetupEmailService

' ***************************************
'Open Raw Scores Table and Pull applicable Score Records, along with necessary Membership table derivatives
' ***************************************
sSQL = "SELECT RS.FName, RS.LName, RS.MemberID, MT.Email, MT.Password, MT.FederationCode as MemberFed"
sSQL = sSQL & ", Convert(char(8),RS.EndDate,112) as EndDate, RS.TourID"
sSQL = sSQL & ", RS.Event, RS.Div, RS.Class, RS.Round, RS.Place"
sSQL = sSQL & ", Case when RS.Event = 'S' AND RS.AltScore < 0 AND RS.Perf_Qual1 < 1825 then 1825 else RS.Perf_Qual1 end as Perf_Qual1"
sSQL = sSQL & ", RS.Perf_Qual2, ABS(RS.AltScore) as AltScore"
sSQL = sSQL & ", Case when RS.Event = 'S' then RS.Score else RS.Score end as Score"
sSQL = sSQL & ", MT.Sex, MT.BirthDate, MT.ForFedID, MT.FedIDLen"
sSQL = sSQL & ", case when MT.ForFedID = RS.MemberID then 'USAWS-#' when MT.FedIDLen = 0 then 'Missing' when FFP.ForFedPatt is null then 'Invalid' else 'Present' end as ForFedStat "
sSQL = sSQL & "FROM " & RawScoresTableName & " as RS "
sSQL = sSQL & "  LEFT JOIN " & MemberWFedIDTableName & " as MT ON cast(right(RS.MemberID,8) as integer) = MT.PersonID "
sSQL = sSQL & "  LEFT JOIN " & FedIDPatternTableName & " as FFP ON FFP.ForFedPatt = MT.ForFedPatt "
sSQL = sSQL & "  LEFT JOIN " & SkiYearTableName & " as SY ON RS.EndDate between SY.BeginDate and SY.EndDate and SY.SkiYearID <> 1 "
sSQL = sSQL & "  LEFT JOIN " & DivisionsTableName & " as DT ON RS.Div = DT.Div and SY.skiyearid = DT.skiyearid "
sSQL = sSQL & "WHERE RS.Class in ('R','L') AND RS.TourID = '" & sTourID & "' "
sSQL = sSQL & "ORDER BY RS.MemberID, RS.Round, RS.Event"

'WriteDebugSQL (sSQL)

rs.open sSQL, SConnectionToTRATable, 3, 3
do while not rs.eof
    tempFirst = left(ucase(rs("FName")),1)
    tempFirst = tempFirst + mid(lcase(rs("FName")),2)
    tempFirst = replace(tempFirst,"'","")
    tempLast = replace(ucase(rs("LName")),"'","")

    tempSex = ucase(left(rs("sex"),1))
    tempYOB = right(rs("birthdate"),4)
    tempAge = 2000 + left(sTourID,2) - TempYOB

    If rs("MemberFed") = "USA" THEN 
    	tempIWSF = "USA" & rs("MemberID")
    ELSE 
    	if rs("ForFedStat") = "Present" THEN
    		tempIWSF = rs("MemberFed") & rs("ForFedID")
    	ELSE
    		tempIWSF = rs("MemberFed") & "-Unknown"
    	END IF
    END IF

    tempPlace = ucase(trim(rs("Place")))
    IF right(tempPlace,1) = "T" THEN tempPlace = left(tempPlace,len(tempPlace)-1)
    IF tempPlace = "" THEN tempPlace = "999"
    
    tempDiv = ucase(rs("Div"))
    tempEvent = rs("Event")
    tempExport = "X"

    tempSlmMiss = ""
    tempScore = rs("Score")

    IF rs("Perf_Qual1") = 0.239 THEN tempSpecial = "S": ELSE tempSpecial = ""
    IF Instr(tempDiv,"B") > 0 OR Instr(tempDiv,"G") > 0 THEN tempSpecial = "J"

    SELECT CASE tempEvent

    CASE "S"
      ' ***************************************
      ' Slalom data entry analyzed and formatted
      ' ***************************************
      tempAlt = FormatNumber(rs("AltScore"),2)
      tempPQ1 = FormatNumber(rs("Perf_Qual1")/100,2)
      tempPQ2 = rs("Perf_Qual2")
      tempSL = FormatNumber(tempScore,2)
      tempTR = ""
      tempJU = ""
      IF tempScore < 6 THEN tempSlmMiss = "*": ELSE tempSlmMiss = ""

      IF tempScore > 0 THEN 
          tempExport = "Y": 
      ELSE 
          tempExport = "N"
          RecBypassedNoScore = RecBypassedNoScore + 1
   	  END IF

      IF tempDiv = "B1" or tempDiv = "G1" or tempDiv = "B2" or tempDiv = "G2" THEN 
          tempExport = "N"
          RecBypassedForDiv = RecBypassedForDiv + 1
   	  END IF

    CASE "J"
      ' ***************************************
      ' Jump data entry analyzed and formatted
      ' ***************************************
      tempAlt = tempScore
      If rs("Perf_Qual1") = 0.275 THEN tempPQ1 = "0.271": ELSE IF rs("Perf_Qual1") < 0.235 then tempPQ1 = "0.235": ELSE tempPQ1 = FormatNumber(rs("Perf_Qual1"),3)
      tempPQ2 = rs("Perf_Qual2")
      tempSL = ""
      tempTR = ""
      tempJU = FormatNumber(rs("AltScore"),1)
      
      IF tempScore > 0 THEN 
          tempExport = "Y": 
      ELSE 
          tempExport = "N"
          RecBypassedNoScore = RecBypassedNoScore + 1
   	  END IF
      
      IF tempPQ1 < "0.235" THEN
          tempExport = "N"
          RecBypassedRampHght = RecBypassedRampHght + 1
   	  END IF
      IF tempDiv = "B1" or tempDiv = "G1" or tempDiv = "B2" or tempDiv = "G2" THEN 
          tempExport = "N"
          RecBypassedForDiv = RecBypassedForDiv + 1
   	  END IF
      
    CASE "T"
      ' ***************************************
      ' Trick data entry analyzed and formatted
      ' ***************************************
      tempAlt = ""
      tempPQ1 = ""
      tempPQ2 = ""
      tempSL = ""
      tempTR = tempScore
      tempJU = ""
      IF tempScore > 0 THEN 
          tempExport = "Y": 
      ELSE 
          tempExport = "N"
          RecBypassedNoScore = RecBypassedNoScore + 1
   	  END IF

    END SELECT

    'TraceMsg = TraceMsg & "<br />Skier: " & tempFirst & " " & tempLast & ", Age=" & tempAge& ", Div=" & tempDiv & ", IWSF=" & tempIWSF & ", Special=" & tempSpecial 
    'TraceMsg = TraceMsg & ", Event=" & tempEvent & ", Score=" & tempScore & ", tempAlt=" & tempAlt & ", tempPQ1=" & tempPQ1 & ", tempPQ2=" & tempPQ2
    'TraceMsg = TraceMsg & ", Export=" & tempExport & ", tempSL=" & tempSL & ", tempTR=" & tempTR & ", tempJU=" & tempJU

    IF tempExport = "Y" THEN
    	
        objTextOut.write ( tempLast & ";" )
        objTextOut.write ( tempFirst & ";" )
        objTextOut.write ( rs("MemberID") & ";;" )
        objTextOut.write ( rs("MemberFed") & ";" )
        objTextOut.write ( tempSex & ";" )
        objTextOut.write ( rs("TourID") & ";" )
        objTextOut.write ( tempSL & ";" )
        objTextOut.write ( tempTR & ";" )
        objTextOut.write ( tempJU & ";" )
        objTextOut.write ( tempAlt & ";" )
        objTextOut.write ( tempYOB & ";" )
        objTextOut.write ( rs("Class") & ";" )
        objTextOut.write ( trim(rs("Round")) & ";" )
        objTextOut.write ( tempDiv & ";" )
        objTextOut.write ( tempPQ1 & ";" )
        objTextOut.write ( tempPQ2 & ";" )
        objTextOut.write ( rs("EndDate") & ";" )
        objTextOut.write ( tempSpecial & ";" )
        objTextOut.write ( "Y;" )
        objTextOut.write ( tempSlmMiss & ";" )
        objTextOut.write ( tempPlace & ";" )
        objTextOut.write ( tempIWSF & ";" )
        objTextOut.writeline ( sTSiteID )
      
        RecsSaved = RecsSaved + 1

        IF rs("MemberFed") <> "USA" and rs("ForFedStat") <> "Present" and LastNoLicMem <> rs("MemberID") and InStr(rs("Email"),"@") > 0 THEN

            LastNoLicMem = rs("MemberID")
			emailBody = "<html><head><title>ACTION REQUIRED !!  IWWF License ID needed to submit your scores</title></head>"
			emailBody = emailBody & "<body><basefont face=""arial,sans-serif,helvetica,verdana,tahoma"" color=""#000000"" size=""2"">"

			emailBody = emailBody & "<div style=""border: double 20px #ff0505;"
			emailBody = emailBody & " padding: 25px;"
			emailBody = emailBody & " margin: 10;"
'			emailBody = emailBody & " text-align: justify;"
			emailBody = emailBody & " line-height: 23px;"
			emailBody = emailBody & " color: #070707;"
			emailBody = emailBody & " font-size: 18px"">"

			emailBody = emailBody & "<p>To:&nbsp;&nbsp;&nbsp;&nbsp; " & rs("FName") & " " & rs("LName")
			emailBody = emailBody & "<br>Re:&nbsp;&nbsp;&nbsp;&nbsp; Scores from " & sTName & " on " & sTDPretty
			emailBody = emailBody & "<br>Date:&nbsp; " & FormatDateTime(date(),1) & "</p>"

			'	Line below is header for when running in debug mode going to developers -- this documents who email would go to.
			'	emailBody = emailBody & "<p>HTML eMail to:  """ & rs("FName") & " " & rs("LName") & """ &lt;" & rs("Email") & "&gt;</p>"

			emailBody = emailBody & "<p>Dear " & rs("FName") & ",</p>"
			'	emailBody = emailBody & "<p>Dear " & rs("FName") & " " & rs("LName") & ",</p>"

			emailBody = emailBody & "<p>It has come to our attention that you recently recorded class L or R scores in a competition sanctioned"
			emailBody = emailBody & " by USA Water Ski –- " & sTName & " (" & sTSanction & "), held at " & STSite & " on " & sTDPretty & ".</p>"

			emailBody = emailBody & "<p>As you may already be aware, an IWWF License ID number must now be submitted with all Class L or R"
			emailBody = emailBody & " scores, in order for those scores to be properly included in the IWWF World Ranking Lists.&nbsp;"
				
			IF rs("ForFedStat") = "Missing" THEN
				emailBody = emailBody & " Unfortunately, we find that you have not yet entered your " & rs("MemberFed")
				emailBody = emailBody & " License ID number into your USA Water Ski Membership Profile.&nbsp;"
		
			ELSE		
				emailBody = emailBody & " Unfortunately, we now find that the License ID number value which you entered"
				emailBody = emailBody & " into your USA Water Ski Membership Profile -- """ & rs("ForFedID") 				
				emailBody = emailBody & """ -- is not valid for Federation code " & rs("MemberFed") & ".&nbsp;"

			END IF
				
			emailBody = emailBody & " As a result, we are unable to properly forward your scores from the above-referenced event to IWWF" 
			emailBody = emailBody & " for inclusion in the World Ranking Lists.</p>"

			emailBody = emailBody & "<p>Therefore, we ask you to do the following, <b><i>immediately:</i></b></p>"

			emailBody = emailBody & "<ul>"
				
			emailBody = emailBody & "<li>Obtain your IWWF License ID number.&nbsp; If you do not already know your License ID Number, "
			emailBody = emailBody & "<a href=""http://www.iwsftournament.com/homologation/showskiers.php"">Click Here</a> to access the IWWF skier"
			emailBody = emailBody & " listings.&nbsp; If your name and ID is <i><b>not</i></b> listed there, then you will need to contact your home"
			emailBody = emailBody & " country federation (" & rs("MemberFed") & ") to obtain your License ID number from them.<br>&nbsp;</li>"
				
			emailBody = emailBody & "<li>Then, <a href=""http://www.usawaterski.org/members/login/index.asp?m=" & rs("MemberID") & "&p=" & rs("Password") 
			emailBody = emailBody & """>Click Here</a> to go to your USA Water Ski ""Members-Only"" login page.&nbsp; Your membership number and password will" 
			emailBody = emailBody & " be filled in automatically; all you have to do is click the ""<b>Sign In</b>"" button to complete your login.<br>&nbsp;</li>"
				
			emailBody = emailBody & "<li>Click on the ""<b>Update my Membership Info</b>"" link, which appears on the left-hand side of the page.<br>&nbsp;</li>"
				
			emailBody = emailBody & "<li>When the profile page opens, scroll to the bottom of the page.<br>&nbsp;</li>"
				
			emailBody = emailBody & "<li>If necessary, correct your Home Federation Country from the drop-down selection list, based" 
			emailBody = emailBody & " on the 3-letter country code appearing at the front of your IWWF License ID.<br>&nbsp;</li>"
				
			emailBody = emailBody & "<li>Then add or correct your Home Federation ID Number, by using the remaining numbers from your IWWF License ID."
			emailBody = emailBody & "&nbsp; <b><i>Do Not</i></b> enter the 3 letters of your country code at the front.<br>&nbsp;</li>"
				
			emailBody = emailBody & "<li>Then click the ""<b>Update</b>"" button, to save your changes.<br>&nbsp;</li>"
				
			emailBody = emailBody & "<li>Then finally reply to this email, confirming that you have recorded your License ID in your USA Water Ski"
			emailBody = emailBody & " membership profile –- and we will then re-send your scores to IWWF, associated with that License ID.<br>&nbsp;</li>"
 
			emailBody = emailBody & "</ul>"
				
			emailBody = emailBody & "<p>Thank you in advance for your cooperation.</p>"

			emailBody = emailBody & "<p>Melanie Hanson<br>Director of Membership Programs"
			emailBody = emailBody & "<br>Office: 863-324-4341 Ext 115"
			emailBody = emailBody & "<br>Direct Line: 863-508-2096</p>"
			emailBody = emailBody & "</div></body></html>"

			'WriteDebugSQL (emailBody)
        	'WriteDebugSQL ("End License Email Body for " & """" & rs("FName") & " " & rs("LName") & """ <" & rs("Email") & ">")

            ' ***************************************
            ' DLA: Send email regarding something, not quite sure what 
            ' ***************************************
			'objMessage.HTMLBody = sSQL
			'objMessage.Send

    		RecsNoLic = RecsNoLic + 1
        
        END IF
    
    END IF

    ' ***************************************
    ' Read next available record
    ' ***************************************
    rs.movenext

loop

rs.close
set rs = nothing
objTextOut.close
set objTextOut = nothing

 %>
   <SCRIPT LANGUAGE="JavaScript">
   if(upLevel) {
      var splash = document.getElementById("splashScreen");
   }
   else if(ns4) {
     var splash = document.splashScreen;
   }
   else if(ie4) {
     var splash = document.all.splashScreen;
   }
   
   hideObject(splash);
   </SCRIPT>  

 <%
   WriteIndexPageHeader

    IF RecsSaved > 0 Then
        ' ***************************************
        ' Prepare and Send Notification eMail
        ' ***************************************
        objMessage.Subject = sIWWFSubj
        objMessage.From = """USA Water Ski"" <dclark@usawaterski.org>"

		' objMessage.To = """Dave Clark"" <AWSATechDude@comcast.net>"
		objMessage.To = """IWWF Ranking Data"" <rankingdata@iwsftournament.com>"
		objMessage.CC = """Dave Clark"" <AWSATechDude@comcast.net>; ""IWWF-EA"" <competitions@iwwfed-ea.org"

		objMessage.AddAttachment PathtoIWWF & "\" & left(sTourID,6) & "RS.TXT"

		objMessage.HTMLBody = ""
		objMessage.TextBody = sIWWFSubj

		objMessage.Send

        objFSO.CopyFile PathtoIWWF & "\" & left(sTourID,6) & "RS.TXT", PathtoIWWF & "\Archived\", TRUE
        objFSO.DeleteFile (PathtoIWWF & "\" & left(sTourID,6) & "RS.TXT")

        %>
        <br><br>
        <center><h2>File export complete.</h2><br>
        <%=RecsSaved%> Class R/L records were exported to file <%=left(sTourID,6)%>RS.TXT.<br><br>    

        <%
        IF RecsNoLic > 0 THEN 
            %>
    	    <%=RecsNoLic%> "Need IWWF License ID" Email notices generated and sent.<br><br>
            <% 
        END IF

    ELSE
        %>
        <br><br><center><h2>File export complete.</h2><br><br><br>
        <h4>No R/L Records were exported for&nbsp; <b><%=sTourID%><b>.</h4><br><br>
        <%
    END IF

    %>
    <form method=post action="DefaultHQ.asp?process=uploadany" method="post">
        <input type="submit" style="width:13em" value="Finished"  title="Return to the Upload Control Page">
    </form>
<%

    set objMessage=nothing
    set objFSO = nothing
    WriteIndexPageFooter    

%>
    <DIV>
        <br />Message<br />
        <%=TraceMsg %>

        <br /><br />Error Messages<br />
        <%=ErrMsg %>
        <br /><br />RecsSaved = <%=RecsSaved %>
        <br />RecBypassedNoScore = <%=RecBypassedNoScore %>
        <br />RecBypassedRampHght = <%=RecBypassedRampHght %>
        <br />RecBypassedForDiv = <%=RecBypassedForDiv %>
        <br />RecsNoLic = <%=RecsNoLic %>

    </DIV>

    <DIV>
    <br /><br />
    <center><h2>File export complete.</h2><br>
    <%=RecsSaved%> Class R/L records were exported to file <%=left(sTourID,6)%>RS.TXT.<br><br>    
    </DIV>

</BODY>


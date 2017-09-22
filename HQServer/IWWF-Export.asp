<!--#include virtual="/rankings/SettingsHQ.asp"-->
<%
Response.Buffer = True
Server.ScriptTimeout = 2400 


Dim tempLast, tempFirst, tempPlace, tempDiv, tempSL, tempTR, tempJU, tempScore, tempAlt
Dim tempPQ1, tempPQ2, tempSex, tempYOB, TempSlmMiss, tempSpecial, tempIWSF, tempExport
Dim sTourID, sTSanction, sTName, sTDateE, sTDPretty, sTSiteID, sTSite, sIWWFSubj
Dim eMailTo, eMailCC, SeedRep, Owner
Dim RecsSaved, ErrMsg, RecsNoLic, LastNoLicMem
Dim InsertCmd
Dim PreZBSScore

' Validate TourID value for scores to be Exported.
sTourID = SQLClean(Session("TourID"))

' Initialize a few things
ErrMsg = ""
RecsSaved = 0
RecsNoLic = 0
LastNoLicMem = "000000000"

Set objFSO = Server.CreateObject("Scripting.FileSystemObject")

' Set up Database and Record Set Connection
OpenCon
set rs = Server.CreateObject("ADODB.recordset")

' Invoke "standard" Email Server Configuration -- defines objMessage object
SetupEmailService

' Get Sanction Table Information for this event and store locally, if present
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
END IF


' WriteDebugSQL (sIWWFSubj)
WriteDebugSQL ("EmailTo = " & eMailTo)

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

WriteDebugSQL ("EmailCC = " & eMailCC)

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
<body>
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

  ' Set up output text file.

  Set objFSO = Server.CreateObject("Scripting.FileSystemObject")
  ExportFile = PathToIWWF & "\" & left(sTourID,6) & "RS.TXT"
  Set objTextOut = objFSO.opentextfile(ExportFile,2,true)

  'Open Raw Scores Table and Pull applicable Score Records, along with necessary Membership table derivatives
  sSQL = "Select RS.FName, RS.LName, RS.MemberID, MT.Email, MT.Password, MT.FederationCode as MemberFed, Convert(char(8),RS.EndDate,112)"
  sSQL = sSQL & " as EndDate, RS.TourID, RS.Event, RS.Div, RS.Class, RS.Round, RS.Place, RS.Perf_Qual1, RS.Perf_Qual2, RS.AltScore,"
  sSQL = sSQL & " Case when RS.Event = 'S' then RS.Score-DT.ZBSConversion else RS.Score end as Score, MT.Sex, MT.BirthDate,"
  sSQL = sSQL & " MT.ForFedID, MT.FedIDLen, case when MT.ForFedID = RS.MemberID then 'USAWS-#' when MT.FedIDLen = 0 then 'Missing'"
  sSQL = sSQL & " when FFP.ForFedPatt is null then 'Invalid' else 'Present' end as ForFedStat"
  sSQL = sSQL & " from " & RawScoresTableName & " as RS left join " & MemberWFedIDTableName & " as MT on cast(right(RS.MemberID,8)"
  sSQL = sSQL & " as integer) = MT.PersonID left join " & FedIDPatternTableName & " as FFP on FFP.ForFedPatt = MT.ForFedPatt"
  sSQL = sSQL & " left join " & SkiYearTableName & " as SY on RS.EndDate between SY.BeginDate and SY.EndDate and SY.SkiYearID <> 1"
  sSQL = sSQL & " left join " & DivisionsTableName & " as DT on RS.Div = DT.Div and SY.skiyearid = DT.skiyearid"
  sSQL = sSQL & " where RS.Class in ('R','L') and RS.TourID = '" & sTourID & "' order by RS.MemberID, RS.Round, RS.Event"

  WriteDebugSQL (sSQL)

  rs.open sSQL, SConnectionToTRATable, 3, 3

  do while not rs.eof

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

    '
    ' Formatting of Name fields.  Some names have a special character (apostrophe).  Replace it with a blank.
    '

    tempFirst = left(ucase(rs("FName")),1)
    tempFirst = tempFirst + mid(lcase(rs("FName")),2)
    tempFirst = replace(tempFirst,"'","")
    tempLast = replace(ucase(rs("LName")),"'","")

    tempPlace = ucase(trim(rs("Place")))
    IF right(tempPlace,1) = "T" THEN tempPlace = left(tempPlace,len(tempPlace)-1)
    IF tempPlace = "" THEN tempPlace = "999"

    tempDiv = ucase(rs("Div"))

    If rs("Perf_Qual1") = 0.239 THEN tempSpecial = "S": ELSE tempSpecial = ""
    If Instr(tempDiv,"B") > 0 or Instr(tempDiv,"G") > 0 then tempSpecial = "J"

    tempSlmMiss = ""
    tempScore = rs("Score")

	  ' WriteDebugSQL (tempFirst & " " & TempLast & "," & rs("Event") & trim(rs("Round")))

    SELECT CASE rs("Event")

    CASE "S"

      tempAlt = FormatNumber(rs("AltScore"),2)
      tempPQ1 = FormatNumber(rs("Perf_Qual1")/100,2)
      tempPQ2 = rs("Perf_Qual2")
      If left(tempDiv,1) = "W" or tempDiv = "OW" or left(tempDiv,1) = "G" then tempScore = tempScore - 6
      tempSL = FormatNumber(tempScore,2)
      tempTR = ""
      tempJU = ""
      IF tempScore < 6 THEN tempSlmMiss = "Y": ELSE tempSlmMiss = "N"

'     Move following line to outside Case -- now applies to all 3 events. 
'     IF tempScore > 0 THEN tempExport = "Y": ELSE tempExport = "N"

      IF tempDiv = "B1" or tempDiv = "G1" or tempDiv = "B2" or tempDiv = "G2" THEN tempExport = "N"
      IF tempDiv = "M7" or tempDiv = "M8" or tempDiv = "M9" or tempDiv = "MA" or tempDiv = "MB" THEN tempExport = "N"
      IF tempDiv = "W7" or tempDiv = "W8" or tempDiv = "W9" or tempDiv = "WA" or tempDiv = "WB" THEN tempExport = "N"

    CASE "J"

      tempAlt = tempScore
      If rs("Perf_Qual1") = 0.275 THEN tempPQ1 = "0.271": ELSE IF rs("Perf_Qual1") < 0.235 then tempPQ1 = "0.235": ELSE tempPQ1 = FormatNumber(rs("Perf_Qual1"),3)
      tempPQ2 = rs("Perf_Qual2")
      tempSL = ""
      tempTR = ""
      tempJU = FormatNumber(rs("AltScore"),1)
      
'			Remove following condition which imposed minimums.
'     IF (tempSex = "M" and tempScore >= 60) or (tempSex = "F" and tempScore >= 45)  THEN tempExport = "Y": ELSE tempExport = "N"

      IF tempPQ1 < "0.235" THEN tempExport = "N"
      
    CASE "T"

      tempAlt = ""
      tempPQ1 = ""
      tempPQ2 = ""
      tempSL = ""
      tempTR = tempScore
      tempJU = ""

'			Remove following condition which imposed minimums.
'     IF (tempSex = "M" and tempScore >= 800) or (tempSex = "F" and tempScore >= 600)  THEN tempExport = "Y": ELSE tempExport = "N"

    END SELECT

    IF tempAge < 35 and rs("Class") = "E" THEN tempExport = "N"

'   Only export scores greater than zero -- this condition formerly applied only in slalom.
    IF tempScore > 0 THEN tempExport = "Y": ELSE tempExport = "N"

    IF tempExport = "Y" THEN
    	
    	' WriteDebugSQL ("Begin Export for " & rs("MemberID") & " " & rs("Event") & " " & rs("Round"))

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

      ' Now Generate "License Email", if Non-USA and License ID missing, and we have an email ID, and we haven't already emailed this Member
      
      IF rs("MemberFed") <> "USA" and rs("ForFedStat") <> "Present" and LastNoLicMem <> rs("MemberID") and InStr(rs("Email"),"@") > 0 THEN

        LastNoLicMem = rs("MemberID")

    	  ' WriteDebugSQL ("Begin License Email Setup for " & """" & rs("FName") & " " & rs("LName") & """ <" & rs("Email") & ">")

				' Prepare and Send Notification eMail
				objMessage.Subject = "ACTION REQUIRED !!  IWWF License ID needed to submit your scores"
				objMessage.From = """USA Water Ski Membership (Melanie Hanson)"" <mhanson@usawaterski.org>"

				objMessage.To = """" & rs("FName") & " " & rs("LName") & """ <" & rs("Email") & ">"
			'	objMessage.To = """Dave Clark"" <awsatechdude@comcast.net>"
			'	objMessage.CC = """Dave Clark"" <AWSATechDude@comcast.net>"
				objMessage.CC = eMailCC & "; " & eMailTo


    	  ' WriteDebugSQL ("Begin License Email Body for " & """" & rs("FName") & " " & rs("LName") & """ <" & rs("Email") & ">")

				sSQL = "<html><head><title>ACTION REQUIRED !!  IWWF License ID needed to submit your scores</title></head>"
				sSQL = sSQL & "<body><basefont face=""arial,sans-serif,helvetica,verdana,tahoma"" color=""#000000"" size=""2"">"

				sSQL = sSQL & "<div style=""border: double 20px #ff0505;"
				sSQL = sSQL & " padding: 25px;"
				sSQL = sSQL & " margin: 10;"
	'			sSQL = sSQL & " text-align: justify;"
				sSQL = sSQL & " line-height: 23px;"
				sSQL = sSQL & " color: #070707;"
				sSQL = sSQL & " font-size: 18px"">"

				sSQL = sSQL & "<p>To:&nbsp;&nbsp;&nbsp;&nbsp; " & rs("FName") & " " & rs("LName")

				sSQL = sSQL & "<br>Re:&nbsp;&nbsp;&nbsp;&nbsp; Scores from " & sTName & " on " & sTDPretty

				sSQL = sSQL & "<br>Date:&nbsp; " & FormatDateTime(date(),1) & "</p>"

			'	Line below is header for when running in debug mode going to developers -- this documents who email would go to.
			'	sSQL = sSQL & "<p>HTML eMail to:  """ & rs("FName") & " " & rs("LName") & """ &lt;" & rs("Email") & "&gt;</p>"

				sSQL = sSQL & "<p>Dear " & rs("FName") & ",</p>"
			'	sSQL = sSQL & "<p>Dear " & rs("FName") & " " & rs("LName") & ",</p>"

				sSQL = sSQL & "<p>It has come to our attention that you recently recorded class L or R scores in a competition sanctioned"
				sSQL = sSQL & " by USA Water Ski –- " & sTName & " (" & sTSanction & "), held at " & STSite & " on " & sTDPretty & ".</p>"

				sSQL = sSQL & "<p>As you may already be aware, an IWWF License ID number must now be submitted with all Class L or R"
				sSQL = sSQL & " scores, in order for those scores to be properly included in the IWWF World Ranking Lists.&nbsp;"
				
				IF rs("ForFedStat") = "Missing" THEN

					sSQL = sSQL & " Unfortunately, we find that you have not yet entered your " & rs("MemberFed")
					sSQL = sSQL & " License ID number into your USA Water Ski Membership Profile.&nbsp;"
		
				ELSE		

					sSQL = sSQL & " Unfortunately, we now find that the License ID number value which you entered"
					sSQL = sSQL & " into your USA Water Ski Membership Profile -- """ & rs("ForFedID") 				
					sSQL = sSQL & """ -- is not valid for Federation code " & rs("MemberFed") & ".&nbsp;"

				END IF
				
				sSQL = sSQL & " As a result, we are unable to properly forward your scores from the above-referenced event to IWWF" 
				sSQL = sSQL & " for inclusion in the World Ranking Lists.</p>"

				sSQL = sSQL & "<p>Therefore, we ask you to do the following, <b><i>immediately:</i></b></p>"

				sSQL = sSQL & "<ul>"
				
				sSQL = sSQL & "<li>Obtain your IWWF License ID number.&nbsp; If you do not already know your License ID Number, "
				sSQL = sSQL & "<a href=""http://www.iwsftournament.com/homologation/showskiers.php"">Click Here</a> to access the IWWF skier"
				sSQL = sSQL & " listings.&nbsp; If your name and ID is <i><b>not</i></b> listed there, then you will need to contact your home"
				sSQL = sSQL & " country federation (" & rs("MemberFed") & ") to obtain your License ID number from them.<br>&nbsp;</li>"
				
				sSQL = sSQL & "<li>Then, <a href=""http://www.usawaterski.org/members/login/index.asp?m=" & rs("MemberID") & "&p=" & rs("Password") 
				sSQL = sSQL & """>Click Here</a> to go to your USA Water Ski ""Members-Only"" login page.&nbsp; Your membership number and password will" 
				sSQL = sSQL & " be filled in automatically; all you have to do is click the ""<b>Sign In</b>"" button to complete your login.<br>&nbsp;</li>"
				
				sSQL = sSQL & "<li>Click on the ""<b>Update my Membership Info</b>"" link, which appears on the left-hand side of the page.<br>&nbsp;</li>"
				
				sSQL = sSQL & "<li>When the profile page opens, scroll to the bottom of the page.<br>&nbsp;</li>"
				
				sSQL = sSQL & "<li>If necessary, correct your Home Federation Country from the drop-down selection list, based" 
				sSQL = sSQL & " on the 3-letter country code appearing at the front of your IWWF License ID.<br>&nbsp;</li>"
				
				sSQL = sSQL & "<li>Then add or correct your Home Federation ID Number, by using the remaining numbers from your IWWF License ID."
				sSQL = sSQL & "&nbsp; <b><i>Do Not</i></b> enter the 3 letters of your country code at the front.<br>&nbsp;</li>"
				
				sSQL = sSQL & "<li>Then click the ""<b>Update</b>"" button, to save your changes.<br>&nbsp;</li>"
				
				sSQL = sSQL & "<li>Then finally reply to this email, confirming that you have recorded your License ID in your USA Water Ski"
				sSQL = sSQL & " membership profile –- and we will then re-send your scores to IWWF, associated with that License ID.<br>&nbsp;</li>"
 
				sSQL = sSQL & "</ul>"
				
				sSQL = sSQL & "<p>Thank you in advance for your cooperation.</p>"

				sSQL = sSQL & "<p>Melanie Hanson<br>Director of Membership Programs"
				sSQL = sSQL & "<br>Office: 863-324-4341 Ext 115"
				sSQL = sSQL & "<br>Direct Line: 863-508-2096</p>"
				sSQL = sSQL & "</div></body></html>"

				WriteDebugSQL (sSQL)

    	  WriteDebugSQL ("End License Email Body for " & """" & rs("FName") & " " & rs("LName") & """ <" & rs("Email") & ">")

				objMessage.HTMLBody = sSQL

				objMessage.Send
				RecsNoLic = RecsNoLic + 1

      END IF      

    END IF

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

   <%WriteIndexPageHeader

   ' WriteDebugSQL (RecsSaved & " Records Saved")

  
   IF RecsSaved > 0 Then

			' Prepare and Send Notification eMail
			objMessage.Subject = sIWWFSubj
			objMessage.From = """USA Water Ski"" <dclark@usawaterski.org>"

			' objMessage.To = """Dave Clark"" <AWSATechDude@comcast.net>"
			objMessage.To = """IWWF Ranking Data"" <rankingdata@iwsftournament.com>"
			objMessage.CC = """Dave Clark"" <AWSATechDude@comcast.net>; ""IWWF-EA"" <competitions@iwwfed-ea.org"

			objMessage.AddAttachment PathtoIWWF & "\" & left(sTourID,6) & "RS.TXT"

			objMessage.HTMLBody = ""
			objMessage.TextBody = sIWWFSubj

			objMessage.Send

      ' IF objFSO.FileExists (PathtoIWWF & "\Archived\" & left(sTourID,6) & "RS.TXT") THEN
      '    objFSO.DeleteFile (PathtoIWWF & "\Archived\" & left(sTourID,6) & "RS.TXT")
      ' END IF
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
              <input type="submit" style="width:13em" value="Finished"
               title="Return to the Upload Control Page">
        </form>
<%

set objMessage=nothing
set objFSO = nothing
WriteIndexPageFooter    

%>

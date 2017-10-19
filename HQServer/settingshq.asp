<!--#include virtual="/rankings/tools_include.asp"-->

<%

' response.redirect "CompetitionMaintSplashPage.htm": ' uncomment to transfer to maintenance announcement page.

' Turn On Error Handling
' On Error Resume Next


Dim rid
randomize()
rid=round(rnd() * 1000000000000)

Dim RawScoresTableName, RawScoresOtherTableName, RawGRScoresTableName, EquivScoresTableName, EquivDivsTableName, OverAllScoresTableName 
Dim LoginTableName, DivisionsTableName, DivisionsOtherTableName, RankTableName, RankNumsTableName, SkiYearTableName 
Dim CutOffTableName, RegionTableName, MemberTableName, MemberLiveTableName, MemberShortTableName
Dim MemberWFedIDTableName, FedIDPatternTableName, MemberStatusTableName
Dim ConsMemTableName, MemberTypeTableName, MemberTypeOLRTableName, SanctionTableName, GuideBookTableName
Dim TRADbName, MemberDBName, SanctionDBName, RegGenTableName, TourGenTableName, TRegSetupTableName, SptsGrpTableName
Dim EmailTemplateTableName, EmailSendSummaryTableName, EmailSendDetailTableName
Dim SitesTableName, RegnSetupTableName, RegTransTableName, RegPaymentTableName, RegSurveyQuestionsTableName, RegSurveyAnswersTableName 
Dim TrafficTableName
Dim RegTempTableName, CCLogTableName, RegPWTableName, BioTableName, RegTemporary
Dim RegDetailTableName, RegDetailTempTableName,  ControlDisplayTableName
Dim RegQualifyTableName, LeagueQfyTableName, LeagueTableName, LeagueToursTableName, RegQfyHistoryTable
Dim Users999TableName, TmEvtScoTableName, TeamRankTableName, EliteDateTableName, PostTourTableName
Dim TeamTableName, TeamRosterTableName, TeamRotationsTableName
Dim IAC_ControlTableName, MobileAppUserTable

Dim V_TeamTableName, V_TeamTypeTableName, V_TeamMembersTableName, V_LeaguesTableName, V_LeagueTeamsTableName, V_LeagueTeamBenchmarkTableName


Dim TRAUser, HQUser
Dim TRAPass, HQPass
Dim Con, ConMember, ConSanction
Dim rs
Dim SConnectionToTRATable, SConnectionToMemberTable, SConnectionToSanctionTable
Dim sConnectionToOLRegFunction, sConnectionToSanctionUpdate
Dim strDocRoot, strImageRoot, strServerName, strImageWebRoot, strDocWebRoot
Dim validImageExtensions, imageArray
Dim validDocExtensions, documentArray
Dim maxImageFileSize, maxDocumentFileSize, maxSUBFolderInColumn
Dim PathtoExceptions, PathtoReasons, PathtoTRA, PathtoUploads, PathtoScratch
Dim PathtoZips, PathtoRawWSPs, PathtoScorebks, PathtoTiming, PathtoHQInBox
Dim PathtoTRA2, PathtoNews, PathtoWaivers, PathtoCommune, PathtoIWWF
Dim tempFSO, tempFSO2, tempObjStream, tempObjStream2, objMessage
Dim logObject
Dim News, NewsPageNum, RankPath, PathtoImages_Old

Dim SkiYearName, SkiYearBegin, SkiYearEnd
Dim USStatesList, USStatesList2, USStatesList3, AlphaList, MonthList

Dim MasterPallette, HeadColor1, TableColor1, TableColor2
Dim TextColor1, TextColor2, TextColor3, TextColor4, TextColor5 
Dim tcolor01, tcolor02, tcolor03, tcolor04, tcolor05, tcolor06, tcolor07, tcolor08, tcolor09, tcolor10
Dim scolor01, scolor02, scolor03, scolor04, scolor05, scolor06, scolor07, scolor08, scolor09, scolor10
Dim font1, font2, fontsize1, fontsize2, fontsize3, fontsize4
Dim HQSiteColor1, HQSiteColor2, HQSiteColor3
Dim TourTableWidth



' These relate to the images and news throughout the site
Dim MainHead_01, BannerImage
Dim NewsHead_01, NewsImage_01, NewsTitle_01, NewsImageCaption_01
Dim NewsHead_02, NewsImage_02, NewsTitle_02, NewsImageCaption_02

' These are definitions of the news boxes on the main page
Dim NewsBoxImage01, NewsBoxHead_01, NewsBoxText_01
Dim NewsBoxImage02, NewsBoxHead_02, NewsBoxText_02
Dim NewsBoxImage03, NewsBoxHead_03, NewsBoxText_03

' These define the data that gets displayed in the View Tours, Scores and Rankings News Pages
Dim tourhead_01, tourhead_02, tourimage, tourimagecaption, tourtitle_01
Dim rankhead_01, rankhead_02
Dim rankimage_SL, rankimagecaption_SL, ranktitle_SL
Dim rankimage_TR, rankimagecaption_TR, ranktitle_TR
Dim rankimage_JU, rankimagecaption_JU, ranktitle_JU
Dim scorehead_01, scorehead_02, scoreimage, scoreimagecaption, scoretitle_01

Dim TourMenuImage01, TourMenuImage02, ScoreMenuImage01, ScoreMenuImage02, RankMenuImage01, RankMenuImage02, Admin_Logo
Dim NewsBoxBalloon01, NewsBoxBalloon02, NewsBoxBalloon03
Dim RegisterMenuImage01, RegisterMenuImage02, DefaultMenuImage01, DefaultMenuImage02, SchedMenuImage01
Dim DefaultMenuLink

Dim TVImage01, TVHead01, TVImage02, TVHead02, TVImage03, TVHead03, TVImage04, TVHead04, TVBalloon01, TVBalloon02, TVBalloon03, TVBalloon04

Dim adult_waiver, minor_waiver
Dim marksemailaddress, USAWaterski_AccountingEmail, USAWaterski_CompetitionEmail

' -------------------------------------------------------
' --- IMPORTANT - This is the MASTER path definition ----
' --- Will be changing to newrankings.usawaterski.org ---
' -------------------------------------------------------


' --- Used in redirect back from CC_process.asp
RankPath = "http://usawaterski.org\rankings"

' ***** NOV 02 Add Start
Dim PathtoRenewalForm
PathtoRenewalForm = "https://www.usawaterski.org/renew/"
'PathtoRenewalForm = "https://sandbox.usawaterski.org/renew/"
' ***** NOV 02 Add End


'PathtoImages_Old = "http://rankings.usawaterski.org"

PathtoNews = Server.mappath("/")&"\rankings\news"

PathtoExceptions = Server.mappath("/rankings/exceptions/")
PathtoReasons = Server.mappath("/rankings/reasons/")
PathtoUploads = Server.mappath("/rankings/uploads/")

PathtoScratch = Server.mappath("/rankings/Scratch/")
PathtoZips = Server.mappath("/rankings/PostTourZips/")
PathtoRawWSPs = Server.mappath("/rankings/RawWSPs/")
PathtoScorebks = Server.mappath("/rankings/Scorebks/")
PathtoTiming = Server.mappath("/rankings/TimingRpts/")
PathtoHQInBox = Server.mappath("/rankings/HQInBox/")
PathtoIWWF = Server.mappath("/rankings/InBoxIWWF/")

PathtoWaivers = Server.mappath("/")&"\rankings\release"
PathtoCommune = Server.mappath("/")&"\rankings\communications"
PathtoTRA = Server.mappath("/")&"\rankings\" 


'PathtoExceptions = "d:\webs\usawaterski.org\rankings\exceptions"

'PathtoExceptions = "d:\webs\rankings.usawaterski.org\exceptions"
'PathtoReasons = "d:\webs\rankings.usawaterski.org\reasons"
'PathtoUploads = "d:\webs\rankings.usawaterski.org\uploads"
'PathtoWaivers = "d:\webs\rankings.usawaterski.org\release"
'PathtoTRA = "d:\webs\rankings.usawaterski.org\" 


' HQUser = "trastand22"
' HQPass = "ski33ret"
TRADBName = "00025"
MemberDBName = "USAWaterski"
SanctionDBName = "Sanctions"
MemberTableName = MemberDBName & ".dbo.members"
MemberLiveTableName = MemberDBName & ".dbo.memberslive"
MemberShortTableName = MemberDBName & ".dbo.membershort"
MemberWFedIDTableName = MemberDBName & ".dbo.memberwfedid"
FedIDPatternTableName = MemberDBName & ".dbo.foreignfedidpatterns"
MemberStatusTableName = MemberDBName & ".dbo.memberstatus"
ConsMemTableName = MemberDBName & ".dbo.consolidatedmembers"
MemberTypeTableName = MemberDBName & ".dbo.membershiptypes"
MemberTypeOLRTableName = MemberDBName & ".dbo.membershiptypesOLR"
'TblMemberTypeTableName = MemberDBName & ".dbo.membertype"
SanctionTableName = SanctionDBName & ".dbo.TSchedul"
PostTourTableName = SanctionDBName & ".Sanctions_Admin.S_PostTourn"
GuideBookTableName = SanctionDBName & ".dbo.GuideBk"
LoginTableName = SanctionDBName & ".dbo.Users"
RawScoresTableName = "usawsrank.Scores"
RawScoresOtherTableName = "usawsrank.ScoresOther"
RawGRScoresTableName = "usawsrank.ScoresGR"
EquivScoresTableName = "usawsrank.EquivScores"
EquivDivsTableName = "usawsrank.EquivDivs"
OverAllScoresTableName = "dbo.OverAllScores"
RegionTableName = "usawsrank.Region"
DivisionsTableName = "usawsrank.Division"
DivisionsOtherTableName = "usawsrank.DivisionOther"
RankTableName = "usawsrank.Rankings"
TmEvtScoTableName = "usawsrank.TmEvtScores"
TeamRankTableName = "usawsrank.TeamRankings"
RankNumsTableName = "usawsrank.RankNums"
CutOffTableName = "usawsrank.CutOffScores"
SkiYearTableName = "usawsrank.SkiYear"
SptsGrpTableName = "usawsrank.SportsDiscipline"
EliteDateTableName = "usawsrank.EliteDates"

TeamTableName = "usawsrank.TeamsList"
TeamRosterTableName = "usawsrank.TeamRoster"
TeamRotationsTableName = "usawsrank.TeamRotations"

'RegGenTableName = "usawsrank.RegisterGenNew"
RegGenTableName = "usawsrank.RegisterGen_05042014"
RegTempTableName = "usawsrank.RegisterGenTemp"
RegDetailTableName = "usawsrank.RegisterEvents"
RegDetailTempTableName = "usawsrank.RegisterEventsTemp"
RegTransTableName = "usawsrank.regtransactions"
RegPaymentTableName = "usawsrank.RegPaymentLog"
RegQualifyTableName = "usawsrank.RegisterQualify_TEST"
RegSurveyQuestionsTableName = "usawsrank.RegSurveyQuestions"
RegSurveyAnswersTableName = "usawsrank.RegSurveyAnswers"
RegQfyHistoryTable = "usawsrank.RegQfyHistory"
EmailTemplateTableName = "usawsrank.Register_Email_Template"
EmailSendSummaryTableName = "usawsrank.Register_Email_SendSummary"
EmailSendDetailTableName = "usawsrank.Register_Email_SendDetail"


TRegSetupTableName = SanctionDBName & ".dbo.registration"
SitesTableName = "dbo.WSSites"
RegnSetupTableName = SanctionDBName & ".dbo.RegnSetup"
TrafficTableName = "usawsrank.TrafficActivity"
CCLogTableName = "usawsrank.CCLogCardHolder"
RegPWTableName = "usawsrank.regpasswords"
BioTableName = "usawsrank.skierbios"
RegTemporary = "usawsrank.RegTemporary"

LeagueTableName = "usawsrank.Leagues"
LeagueToursTableName = "usawsrank.LeagueTours"
LeagueQfyTableName = "usawsrank.LeagueQualify"

Users999TableName = "usawaterski.dbo.Users999"

ControlDisplayTableName = "usawsrank.ControlDisplay"  ' --- Sets control of various functions - emails, etc. --
IAC_ControlTableName = "usawsrank.IAC_Control"		' --- Controls settings in IAC Elite Team Selection process ---

V_TeamTableName = "usawsrank.V_Team"
V_TeamTypeTableName = "usawsrank.V_Team_Type"
V_TeamMembersTableName = "usawsrank.V_Team_Members"
V_LeaguesTableName = "usawsrank.V_Leagues"
V_LeagueTeamsTableName = "usawsrank.V_League_Teams"
V_LeagueTeamBenchmarkTableName = "usawsrank.V_League_Team_Bench"

MobileAppUserTable = "usawsrank.Mobile_AppUsers"



TRAUser = "usawsrank"
TRAPass = "f20tRIAy"
USStatesList = "('AL','AK','AR','AZ','CA','CO','CT','DE','FL','GA','HI','ID','IA','IL','IN','KS','KY','LA','MA','MD','ME','MI','MN','MO','MS','MT','NC','ND','NE','NH','NJ','NM','NV','NY','OH','OK','OR','PA','RI','SD','SC','TN','TX','UT','VT','VA','WA','WI','WV','WY')"
USStatesList2 = ",AL,AK,AR,AZ,CA,CO,CT,DE,FL,GA,HI,ID,IA,IL,IN,KS,KY,LA,MA,MD,ME,MI,MN,MO,MS,MT,NC,ND,NE,NH,NJ,NM,NV,NY,OH,OK,OR,PA,RI,SC,SD,TN,TX,UT,VT,VA,WA,WI,WV,WY"
USStatesList3 = "All,AL,AK,AR,AZ,CA,CO,CT,DE,FL,GA,HI,ID,IA,IL,IN,KS,KY,LA,MA,MD,ME,MI,MN,MO,MS,MT,NC,ND,NE,NH,NJ,NM,NV,NY,OH,OK,OR,PA,RI,SC,SD,TN,TX,UT,VT,VA,WA,WI,WV,WY"
AlphaList = ",A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z"
MonthList = ",Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sep,Oct,Nov,Dec"


' --- Defines liability waiver - Changed to 2010 version on 3-6-2011 ---
adult_waiver = "adlt2010"
minor_waiver = "min_2010"
'adult_waiver = "adlt2007"
'minor_waiver = "min_2007"


' --- Defines the width of the table in the HQ formatted site ---
TourTableWidth=725


' ------- CHANGE FONT SPECIFICATION HERE  
font1="Verdana, Arial, Helvetica, sans-serif"
font2="arial"
fontsize1="0"
fontsize2="1"
fontsize3="2"
fontsize4="3"


TextColor1="#000000"
TextColor2="#0000CD"
TextColor3="Red"
'TextColor4="#0000CD"
TextColor5="FFFFFF"

HQSiteColor1="#203f5e"
HQSiteColor2="#0f77da"
HQSiteColor3="#2F4F4F"


' These act as the defaults
' Center Banner
MainHead_01 = "COMPETITION RESOURCES" 
BannerImage = "/images/buttons/Vertical_Shade_564x152_New.jpg"

marksemailaddress="cronemarka@gmail.com"
USAWaterski_AccountingEmail="onlinepayments@usawaterski.org"




' -----------  Color Definitions

MasterPallette = 3

SELECT CASE MasterPallette
	CASE 1 
		HeadColor1 = "#8FBC8F"  ' DarkSeaGreen
		TableColor1 = "#CCFFCC"  ' Mist Green (orig)
		TableColor2 = "#F5DEB3"  ' Wheat    
	CASE 2
		HeadColor1 = "#FA8072"  ' Salmon
		TableColor1 = "#CCCCFF"  ' Blue    
		TableColor2 = "#F5DEB3"  ' Wheat    
	CASE 3
		HeadColor1 = "#6495ED"  ' Cornflower Blue
'		TableColor1 = "#B0C4DE"  ' Light Steel Blue    
'		TableColor1 = "#E6E6FA"  ' Lavender 
'		TableColor1 = "#FFF5EE"	  ' Sea Shell
		TableColor1 = "#F5F5F5"  ' White Smoke    

		TableColor2 = "#87CEFA"  ' Light Sky Blue 

		tcolor01 = "#DDA0DD"  ' Plum
		tcolor02 = "#FFCCCC"  '  
'		tcolor03 = "#ADFF2F"  ' Bright Green
		tcolor03 = "#98FB98"  ' Pale Green
		' Level 8 Corresponds to color 3

		tcolor04 = "#FFFF66"  ' Yellow
		tcolor05 = "#CCFFCC"  ' Lt Green
		tcolor06 = "#CCCCFF"  ' Lt Steel Blue
		tcolor07 = "#CC99FF"  ' Plum
		' level 4 corresponds to color 7

		tcolor08 = "#F5DEB3"  ' Pea Green    
		tcolor09 = "#FFEBCD"  ' Bright Green 
		tcolor10 = "#FFFFFF"  ' Orignal  White (default)


		' --- Used for HQFormat Rankings ---
		scolor01 = "#FFFFFF"  ' Orignal  White (default)		
		scolor02 = "#FFEBCD"  ' Bright Green  
'   		scolor03 = "#E6E6FA"  ' Lavender
   		scolor03 = "#FFFACD"  ' Lemon Chiffon 

   		scolor04 = "#F5DEB3"  ' Wheat
		scolor05 = "#CC99FF"  ' Plum
		scolor06 = "#CCCCFF"  ' Lt Steel Blue 

		scolor07 = "#FFFF66"  ' Yellow 
		'scolor08 = "#98FB98"  ' Pale Green
		scolor08 = "#CCFFCC"  ' Lt Green 

		scolor09 = "#FFCCCC"  '   
		scolor10 = "#DDA0DD"  ' Plum

END SELECT







Response.Expires=0 'Prevent browsers from caching the page

' ----------- YOU SHOULD NOT HAVE TO EDIT ANYTHING BELOW HERE -------------------




IF trim(Session("SkiYear")) = "" THEN
  Session("SkiYear") = 1
  ' Note: 1 is the rolling 12-month default methodology.
END IF


' ---------------------------- SECURITY ----------------------------------------


Dim Scorers, Seeding, Seeding1, Seeding2, Administration
Scorers = 10
Seeding2 = 20
Seeding = 30
Administration = 50


' *****************************************************************************************
' WHERE do the Session-userlevel and Request-adminmenulevel values initially come from ??
' *****************************************************************************************

' Initially it appears that adminmenulevel = ""
' IF the current value of adminmenulevel is not null THEN make membermenulevel null
'    then if the userlevel is greater than the adminmenulevel value make the SESSION-adminmenulevel same as Request-adminmenulevel 
' IF userlevel is not greater than adminmenulevel THEN announce a menu error message



IF TRIM(Session("AdminMenuLevel"))<>"" AND trim(Request("adminmenulevel")) <> "" AND trim(Request("adminmenulevel")) <> "," THEN
	' --- Comma portion added to eliminate error appearing from an unknown area

	Session("membermenulevel") = ""
	IF session("userlevel") >= trim(request("adminmenulevel"))+0 THEN
		' --- FORMERLY - Session("adminmenulevel") = trim(Request("adminmenulevel"))
	ELSE
		session("message") = "Menu Error -- Please Log In Again"
    		response.redirect("/rankings/defaultHQ.asp?process=logout&rid=" & rid)
	END IF
END IF



IF trim(Request("membermenulevel")) <> "" THEN
  Session("membermenulevel") = trim(Request("membermenulevel"))
END IF



' ---------------------------- WRITE TO LOG ----------------------------------------



SUB writelog(lstr1)
  Set tempFSO=Server.CreateObject("Scripting.FileSystemObject")

  IF Not (tempFSO.FileExists(PathToTRA & "tra-log.txt")) = true THEN
    Set logobject=tempFSO.CreateTextFile(PathToTRA & "tra-log.txt",true)
  ELSE
    Set logobject=tempFSO.OpenTextFile(PathToTRA & "tra-log.txt",8,true)
  END IF

  logobject.WriteLine(lstr1 & " -+- " & session("UserName"))
  logobject.Close

  Set logobject=nothing
  Set tempFSO=nothing
END SUB


' ------------------------- WRITE TO SQL DEBUG LOG ----------------------------

Sub WriteDebugSQL(lstr1)

Set tempFSO=Server.CreateObject("Scripting.FileSystemObject")

IF Not (tempFSO.FileExists(PathToTRA & "sql-debug-log.txt")) = true THEN
   Set logobject=tempFSO.CreateTextFile(PathToTRA & "sql-debug-log.txt",true)
ELSE
   Set logobject=tempFSO.OpenTextFile(PathToTRA & "sql-debug-log.txt",8,true)
END IF

logobject.WriteLine("SQL = " & lstr1 & " -+- " & date() & " " & time() & " " & session("UserName"))
logobject.Close

Set logobject=nothing
Set tempFSO=nothing

END SUB



' ------------------------- WRITE TO Mark-DEBUG LOG ----------------------------

' -------------------------
  SUB MarkDebug(lstr1)
' -------------------------

Set tempFSO=Server.CreateObject("Scripting.FileSystemObject")

IF Not (tempFSO.FileExists(PathToTRA & "Mark-debug-log.txt")) = true THEN
   Set logobject=tempFSO.CreateTextFile(PathToTRA & "Mark-debug-log.txt",true)
ELSE
   Set logobject=tempFSO.OpenTextFile(PathToTRA & "Mark-debug-log.txt",8,true)
END IF

logobject.WriteLine("TextSent = " & lstr1 & " -+- " & date() & " " & time() & " " & session("UserName"))
logobject.Close

Set logobject=nothing
Set tempFSO=nothing

END SUB


' ------------------------------------
SUB KickTrafficCounter(CounterName)
' ------------------------------------

' This SUB is invoked with the name of a specific Counter field.
'   Hence --    KickTrafficCounter("RankPages")
' will kick the [RankPages] Counter, in the specific row in the
' Traffic Activity Table which matches the Current System Date.
' If this is the first activity on a date for which a row does
' NOT already exist, that row is inserted first, with zeroes.

' This table contains one Primary Key field, ActivityDate (Char(10)),
' followed by as many named Counter Fields, as we wish to create.
' The key to flexible success, is to have each such Counter field
' defined as "[CounterName] Int Default 0", so when we do an insert
' for a new activity date, the Database automatically initializes 
' all these counters to zero, for that new activity date row.

' An additional Counter can be added and then initialized, 
' using the following two SQL statements --
'    ALTER TABLE [TrafficTableName] ADD [NewCounterColumnName] Int Default 0;
'    UPDATE [TrafficTableName] Set [NewCounterColumnName] = 0;

' First step is to format the current system date, as YYYY-MM-DD (char(10))

Dim DateRaw, DateFmt, I1, I2
DateRaw = Date(): I1 = instr(DateRaw,"/"): I2 = instr(I1+1,DateRaw,"/")
DateFmt = Mid(DateRaw,I2+1): ' Start with Year value
IF I1=2 THEN DateFmt = DateFmt + "-0" + Left(DateRaw,1): ELSE DateFmt = DateFmt + "-" + Left(DateRaw,2)
IF I2-I1=2 THEN DateFmt = DateFmt + "-0" + Mid(DateRaw,I1+1,1): ELSE DateFmt = DateFmt + "-" + Mid(DateRaw,I1+1,2)

' Second step is to query the Traffic Counter Table, for this current
' Activity Date.  If a row for that date is NOT already present, 
' then a new row (with zero default counters) will be inserted.

Opencon

SET rsKick=Server.CreateObject("ADODB.recordset")
sSQL = "Select ActivityDate from "&TrafficTableName&" WHERE ActivityDate = '"&DateFmt&"'"
rsKick.open sSql, sConnectionToTRATable, 3, 3
IF rsKick.eof THEN
   sSQL = "INSERT INTO "&TrafficTableName&" (ActivityDate) VALUES ('"&DateFmt&"')"
   Con.Execute(sSQL)
END IF      
rsKick.close

' Final Step is to actually Kick the Specified Counter, for this Date row.

sSQL = "UPDATE "&TrafficTableName&" SET "&CounterName&" = "&CounterName&" + 1"
sSQL = sSQL + " WHERE ActivityDate = '"&DateFmt&"'"

Con.Execute(sSQL)

END SUB





' ---------------------------- MENU BAR ----------------------------------------



' ---------------------------- SQL CONNECTIONS ----------------------------------------

Dim OLRegUID, OLRegPW, ConOLReg
' OLRegUID = "Sanctions_P"
' OLRegPW = "43qe9ho6"

Dim SanUpdUID, SanUpdPW, ConSanUpd
' SanUpdUID = "Sanctions_Admin"
' SanUpdPW = "qej8h7w34w"


' ***** NOV 02 Add Start

' --- Provided by Arial on 5-17-2012 ---		
    sConnectionToTRATable = Application("sConnectionToTRATable")
    sConnectionToMemberTable = Application("sConnectionToMemberTable")
    sConnectionToSanctionTable = Application("sConnectionToSanctionTable")
    sConnectionToSanctionUpdate = Application("sConnectionToSanctionTable")
    
    ' --- Test 5-26-2012 ---
    sConnectionToOLRegFunction = Application("sConnectionToSanctionTable")

	' --- Delete after testing 
	'Application("WaterSkiConn") = "Provider=SQLOLEDB;SERVER=kingham-sql.epolk.net;Database=U54W5r4NK;uid=waterski;pwd=4QU4r4nkSAA"
	'Application("sConnectionToTRATable") = "Provider=SQLOLEDB;SERVER=kingham-sql.epolk.net;uid=U54W5r4NK;Password=4QU4r4nkSAA;Initial Catalog=cobra00025"
	'Application("sConnectionToMemberTable") = "Provider=SQLOLEDB;SERVER=kingham-sql.epolk.net;uid=U54W5r4NK;Password=4QU4r4nkSAA;Initial Catalog=USAWaterski"
	'Application("sConnectionToSanctionTable") = "Provider=SQLOLEDB;SERVER=kingham-sql.epolk.net;uid=U54W5r4NK;Password=4QU4r4nkSAA;Initial Catalog=Sanctions"

' ***** NOV 02 Add End


'For local SQL Server Connections
'    sConnectionToTRATable = "Provider=SQLOLEDB;Data Source=jaguar.epolk.net;User ID=" & HQuser & ";Password=" & HQpass & ";Initial Catalog=cobra00025"
'    sConnectionToMemberTable = "Provider=SQLOLEDB;Data Source=jaguar.epolk.net;User ID=" & HQuser & ";Password=" & HQpass & ";Initial Catalog=USAWaterski"
'    sConnectionToSanctionTable = "Provider=SQLOLEDB;Data Source=jaguar.epolk.net;User ID=" & HQuser & ";Password=" & HQpass & ";Initial Catalog=Sanctions"
'    sConnectionToOLRegFunction = "Provider=SQLOLEDB;SERVER=jaguar.epolk.net;Database=Sanctions;uid="&OLRegUID&";pwd="&OLRegPW
'    sConnectionToSanctionUpdate = "Provider=SQLOLEDB;SERVER=jaguar.epolk.net;Database=Sanctions;uid="&SanUpdUID&";pwd="&SanUpdPW

' "Provider=SQLOLEDB;SERVER=66.255.45.10;Database=waterski;uid=usawaterski;pwd=H2osl@l0m" -- Old HQ Server






' Open DSN SQL Server Connections
SUB OpenCon
  Set Con = Server.CreateObject("ADODB.Connection")
  Con.ConnectionTimeout = 3000
  Con.Open Application("sConnectionToTRATable")
  Con.CommandTimeout = 3000
END SUB

SUB OpenConMember
  Set ConMember = Server.CreateObject("ADODB.Connection")
  ConMember.ConnectionTimeout = 3000
  ConMember.CommandTimeout = 3000
  'Con.open(sConnectionToMemberTable)
  ConMember.open Application("sConnectionToMemberTable")
END SUB

SUB OpenConSanction
  Set ConSanction = Server.CreateObject("ADODB.Connection")
  'ConSanction.open(sConnectionToSanctionTable)
	ConSanction.open Application("sConnectionToSanctionTable")
END SUB

SUB OpenConOLReg
  Set ConOLReg = Server.CreateObject("ADODB.Connection")
  ConOLReg.open(sConnectionToOLRegFunction)
END SUB	

SUB OpenConSanUpd
  Set ConSanUpd = Server.CreateObject("ADODB.Connection")
  ConSanUpd.open(sConnectionToSanctionUpdate)
END SUB


' --- From D Clark 7-31-2013 ---
'	Dim objConn
'	Set objConn = Server.CreateObject("ADODB.Connection")
'	objConn.Open Application("WaterSkiConn")
'	objConn.CommandTimeout = 300 



' Close DSN SQL Server Connections
SUB CloseCon
  Con.close
  Set Con = Nothing
END SUB

SUB CloseConMember
  ConMember.close
  Set ConMember = Nothing
END SUB

SUB CloseConSanction
  ConSanction.close
  Set ConSanction = Nothing
END SUB

SUB CloseConOLReg
  ConOLReg.close
  Set ConOLReg = Nothing
END SUB

SUB CloseConSanUpd
  ConSanUpd.close
  Set ConSanUpd = Nothing
END SUB




' ---------------------------- EMAIL SERVICE CONFIGURATION ----------------------------------------


' -------------------------
   SUB SetupEmailService
' -------------------------


	Set objMessage = CreateObject("CDO.Message")

	'=====  This section provides the configuration information for the remote SMTP server.
	objMessage.Configuration.Fields.Item _
	("http://schemas.microsoft.com/cdo/configuration/sendusing") = 2
	'Name or IP of Remote SMTP Server
	objMessage.Configuration.Fields.Item _
	("http://schemas.microsoft.com/cdo/configuration/smtpserver") = "mail.epolk.net"
	'Type of authentication, NONE, Basic (Base64 encoded), NTLM
	objMessage.Configuration.Fields.Item _
	("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate") = 0
	'Your UserID on the SMTP server
	objMessage.Configuration.Fields.Item _
	("http://schemas.microsoft.com/cdo/configuration/sendusername") = "viper@usawaterski.org"
	'Your password on the SMTP server
	objMessage.Configuration.Fields.Item _
	("http://schemas.microsoft.com/cdo/configuration/sendpassword") = "V1p3rMAIL0090"
	'Server port (typically 25)
	objMessage.Configuration.Fields.Item _
	("http://schemas.microsoft.com/cdo/configuration/smtpserverport") = 25
	'Use SSL for the connection (False or True)
	objMessage.Configuration.Fields.Item _
	("http://schemas.microsoft.com/cdo/configuration/smtpusessl") = False
	'Connection Timeout in seconds (the maximum time CDO will try to establish a connection to the SMTP server)
	objMessage.Configuration.Fields.Item _
	("http://schemas.microsoft.com/cdo/configuration/smtpconnectiontimeout") = 60
	objMessage.Configuration.Fields.Update
	'=====  End remote SMTP server configuration section

END SUB





' ---------------------------- PAGE BORDERS AND TABLES ----------------------------------------


' -------------------------
   SUB WriteIndexPageHeader
' -------------------------

	' --- SUB in tools_include.asp ---
	HQHead1

END SUB


' -------------------------
   SUB WriteIndexPageHeader_NoMenu
' -------------------------

	' --- SUB in tools_include.asp ---
	HQHeadNoMenu

END SUB




' ---------------------------
   SUB WriteIndexPageFooter
' ---------------------------

	' --- SUB in tools_include.asp ---
	HQFooter1

END SUB






' ------------------------------
   SUB DisplayTimeOutNotice
' ------------------------------

'markdebug("INSIDE DisplayTimeoutNotice")

sSendingPage = RankPath&"/defaultHQ.asp" 

%>
<br><br>

<TABLE BORDER="4" ALIGN="CENTER" CELLPADDING="3" CELLSPACING="0" BGCOLOR="<% =TableColor1 %>" width=65%>
  <TR>
      <TD BGCOLOR="red"><center><font face=<% =font1 %> color="#FFFFFF" size="4"><b>Important Notice !!</b></font></TD>
  </TR>  

  <TR>
     <TD VALIGN="top">
	<TABLE BORDER="0" VALIGN="top" ALIGN="CENTER" CELLPADDING="3" CELLSPACING="0" BGCOLOR="<%=TableColor1%>" width=100%>
	   <tr>
	      <td VALIGN="top" ALIGN="center">
		<br>
		<font color="<% =TextColor1 %>" face="<% =font1 %>" size="4"><b><i>Your Session Timed Out</i></b></font>
		<br><br>
		<font face="<% =font1 %>" size="2">We are sorry for the inconvenience, but you may not continue without logging in again.</font>
		<br><br>	
		<font face="<% =font1 %>" size="2">The inactivity caused our server to reach the maximum time limit for maintaining your member and/or tournament selections.  Consequently, the record you were working on is no longer active. Please try again.  
		<br><br>
		If you have any questions, please contact:
		<br>
		USA Water Ski - Competition Dept at 800-533-5972</b></font>
	    </td>
	  </tr>
	<tr>
	   <td align="center">
		<br>
		<form action="<% =sSendingPage %>" method="post">
		  <center><input type="submit" value=" Continue "></center>
		</form>
		</TABLE>
		   </td>	
	</tr>
    </TD>
  </TR>
</TABLE>   <% 


END SUB





'--------------------------------------
  SUB WriteNewHeader (sTitle, sSubTitle)
'--------------------------------------

' ------------------  WHY DEFINE THIS WAY?  COULD THE NEW TEXT/IMAGE DEFINITION BENEFIT BY THIS APPROACH?
' Write Headers for DB Page
%>

<TABLE BORDER="0" CELLPADDING="6" CELLSPACING="0" WIDTH="100%" BGCOLOR="#C0C0C0" BORDERCOLOR="#C0C0C0" BORDERCOLORDARK="#C0C0C0" BORDERCOLORLIGHT="#C0C0C0" >
  <TR>
	<TD align="center" vAlign=bottom noWrap background="/images/buttons/Vertical_Shade_564x152_New.jpg">
		    <FONT face="Verdana, Arial, Helvetica, sans-serif" color=#ffffff size=5><B><%= sTitle %></B></FONT><br>
		    <FONT size=<% =fontsize3 %> face=<% =font2 %> color=#ffffff size=3><B><%= sSubTitle %></B></FONT>
		<br>
	</TD>	
  </TR>
</TABLE><%

END SUB




' ---------------------------- Various Functions ----------------------------------------

' -------------------------
   Function SQLClean(str)
' -------------------------

' This function cleans variables to remove any SQL protected symbols which might 
' be used to hack our SQL or crash the program.

Dim tempString

tempString = str
IF tempString <> "" THEN
	' --- A single apostrophe is replaced by double - WORKING ?	
	tempString = replace(tempString,"'","''")

	' --- double pluses 
	tempString = replace(tempString,"++","'")

	' --- semicolon
	tempString = replace(tempString,";","")

	' --- comma
	tempString = replace(tempString,","," ")

	' --- Less than sign
	tempString = replace(tempString,"<","(")

	' --- Greater than sign
	tempString = replace(tempString,">",")")
END IF
SQLClean = tempString
End Function



' -------------------------
   Function MarkTester
' -------------------------

' --- This function checks if the current user has Development Level authority and returns true

MarkTester=false
IF Session("AdminMenuLevel")>=50 THEN
	MarkTester = true
END IF

End Function




' -------------------------
   Function RemoveSpace(str)
' -------------------------

' This function cleans variables to remove any SQL protected symbols which might 
' be used to hack our SQL or crash the program.

Dim tempString

tempString = str
IF tempString <> "" THEN
  tempString = replace(tempString," ","")
END IF
RemoveSpace = tempString

End Function



' -------------------------
   Function isAlpha(str)
' -------------------------

Dim iPos, bolValid
iPos = 1
isalphaValid = True

DO WHILE iPos <= Len(str) and isalphaValid
  IF Asc(UCASE(Mid(str,iPos,1))) < Asc("A") or Asc(UCASE(Mid(str,iPos,1))) > Asc("Z") THEN 
    isalphavalid = False
  END IF
  iPos = iPos + 1
loop

isAlpha = isalphaValid
End Function


' ---------------------------------------------------
   FUNCTION AgeAtDate(sTourDate, sMemberID)
' ---------------------------------------------------

OpenCon



' --- NEW LOGIC as of 7-21-2008 ---

Dim EndofSkiYearDate, tempBirthDate, tempGender, sFullName, sSkiYear2Digit

set rsAge1=Server.CreateObject("ADODB.recordset")
sSQL = "SELECT TOP 1 * FROM "&MemberTableName&" WHERE PersonIDwithCheckDigit = "&sqlclean(sMemberID)
rsAge1.open sSQL, sConnectionToTRATable, 3, 1
tempBirthDate=rsAge1("BirthDate")
tempGender = UCASE(LEFT(rsAge1("Sex"),1)) 
sFullName = rsAge1("FirstName")&" "&rsAge1("LastName")
rsAge1.close



' Check the Division Table for the Current Division
set rsSY=Server.CreateObject("ADODB.recordset")
sSQL = "SELECT TOP 1 SkiYear FROM " & SkiYearTableName & " WHERE '"& CDate(sTourDate) &"' BETWEEN BeginDate AND EndDate AND SkiYearID <> 1"     
rsSY.open sSQL, SConnectionToTRATable, 3, 1



sSkiYear2Digit=RIGHT(rsSY("SkiYear"),2)
rsSY.close


IF LEFT(sSkiYear2Digit,1)="0" THEN 
	EndofSkiYearDate="12/31/200"&(sSkiYear2Digit-1)
ELSEIF sSkiYear2Digit="10" THEN
	EndofSkiYearDate="12/31/200"&(sSkiYear2Digit-1)
ELSE 
	EndofSkiYearDate="12/31/20"&(sSkiYear2Digit-1)
END IF

po=1
IF po=2 AND LEFT(sTourID,6)="10W099" THEN 
	response.write("<br>"&sSQL)
	response.write("<br>sSkiYear2Digit="&sSkiYear2Digit)
	response.write("<br>sTourDate = "&sTourDate)
	response.write("<br>tempBirthDate = "&tempBirthDate)
	response.write("<br>EndofSkiYearDate = "&EndofSkiYearDate)

	response.end
END IF

AgeAtDate = datediff("YYYY", tempBirthDate, CDate(EndofSkiYearDate))


END FUNCTION




' ---------------------------------------------------
   FUNCTION AgeAtDate_New(sTourDate, sMemberID)
' ---------------------------------------------------

OpenCon



' --- NEW LOGIC as of 7-21-2008 ---

Dim EndofSkiYearDate, tempBirthDate, tempGender, sFullName, sSkiYear2Digit

SET rsAge1=Server.CreateObject("ADODB.recordset")
sSQL = "SELECT TOP 1 PersonID, COALESCE(BirthDate,'1900-01-01') AS BirthDate, FirstName, LastName, Sex"
sSQL = sSQL + " FROM "&MemberLiveTableName
sSQL = sSQL + " WHERE PersonID = '"&RIGHT(sMemberID,8)&"'"
rsAge1.open sSQL, sConnectionToTRATable, 3, 1

		

tempBirthDate=rsAge1("BirthDate")
tempGender = UCASE(LEFT(rsAge1("Sex"),1)) 
sFullName = rsAge1("FirstName")&" "&rsAge1("LastName")
rsAge1.close

p=1
IF p=2 AND sMemberID="000001151" THEN
		response.write("<br><br>Line 944 SettingsHQ - "&sSQL)
		response.write("<br>tempBirthDate="&tempBirthDate)
		response.write("<br>sFullName="&sFullName)
		response.end
END IF


' --- Check the Division Table for the Current Division
set rsSY=Server.CreateObject("ADODB.recordset")
sSQL = "SELECT TOP 1 SkiYear FROM " & SkiYearTableName & " WHERE '"& CDate(sTourDate) &"' BETWEEN BeginDate AND EndDate AND SkiYearID <> 1"     
rsSY.open sSQL, SConnectionToTRATable, 3, 1



sSkiYear2Digit=RIGHT(rsSY("SkiYear"),2)
rsSY.close


IF LEFT(sSkiYear2Digit,1)="0" THEN 
		EndofSkiYearDate="12/31/200"&(sSkiYear2Digit-1)
ELSEIF sSkiYear2Digit="10" THEN
		EndofSkiYearDate="12/31/200"&(sSkiYear2Digit-1)
ELSE 
		EndofSkiYearDate="12/31/20"&(sSkiYear2Digit-1)
END IF

po=1
IF po=2 AND LEFT(sTourID,6)="10W099" THEN 
	response.write("<br>"&sSQL)
	response.write("<br>sSkiYear2Digit="&sSkiYear2Digit)
	response.write("<br>sTourDate = "&sTourDate)
	response.write("<br>tempBirthDate = "&tempBirthDate)
	response.write("<br>EndofSkiYearDate = "&EndofSkiYearDate)

	response.end
END IF

'IF tempBirthDate="1/1/1900" OR RIGHT(sMemberID,8)="00001151" THEN
IF tempBirthDate="1/1/1900" THEN
		AgeAtDate_New="Unk"
ELSE		
		AgeAtDate_New = datediff("YYYY", tempBirthDate, CDate(EndofSkiYearDate))
END IF

END FUNCTION



' ---------------------------------------------------
   FUNCTION PersonIDwChkDgt (PersonID)
' ---------------------------------------------------

' This function is given an integer "PersonID" value, and returns the
' 9-Character "PersonIDWithCheckDigit" value for that particular member.

Dim PIDSum, PIDChar, PIDLen, PIDPtr
 
PIDSum = 0: PIDChar = trim(PersonID): PIDLen = Len(PIDChar)

FOR PIDPtr = 1 TO PIDLen STEP 2
	PIDSum = PIDSum + (3*MID(PIDChar,PIDPtr,1))
	IF PIDPtr+1 <= PIDLen THEN PIDSum = PIDSum + MID(PIDChar,PIDPtr+1,1)
	NEXT

PersonIDwChkDgt = right(100-PIDSum,1) & Right(100000000+PersonID,8)

END FUNCTION















validImageExtensions = "jpg,gif,bmp"
imageArray = array(".jpg", ".gif", ".bmp")
validDocExtensions = "pdf,doc,txt,wpd,xls"
documentArray = array(".pdf", ".doc", ".txt", ".wpd", ".xls")
maxImageFileSize = 150132
maxDocumentFileSize = 1000000
maxSUBFolderInColumn = 6
strServerName = "http://" & request.servervariables("SERVER_NAME") & "/"
strImageWebRoot  =  strServerName
strDocWebRoot  =  strServerName






%>




 
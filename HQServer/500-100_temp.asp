<%

  'Version 1.02
  'Added On Resume Next around the part that display form data so this page
  'wint puke if the form data is binary (on upload pages)

  'Version 1.03
  'Added Session.Contents to error email.

  Const lngMaxFormBytes = 200

  Dim objASPError, blnErrorWritten, strServername, strServerIP, strRemoteIP
  Dim strMethod, lngPos, datNow, strQueryString, strURL

  If Response.Buffer Then
    Response.Clear
    Response.Status = "500 Internal Server Error"
    Response.ContentType = "text/html"
    Response.Expires = 0
  End If

  Set objASPError = Server.GetLastError

	noImages = 9
	Randomize
	randomNumber = Int(Rnd * noImages) + 1
	'randomNumber = 1

	Select Case randomNumber
		Case 1, 5
			fontColor = "#fff"
			linkColor = "#00f"
		Case 2
			fontColor = "#fff"
			linkColor = "#f0f"
		Case 3, 4
			fontColor = "#000"
			linkColor = "#00f"
	End Select
%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>USA Water Ski</title>
<style>
	body { background-color: #0269ab; font-family: Verdana, Arial, Helvetica, sans-serif; font-size: 13px; color: <%= fontColor %>; }
	a { color: <%= linkColor %>; }
	.image { width: 425px; height: 500px; margin: 1em auto; border: 8px solid #0793ea; background-color: #fff; background-image: url(/images/error/<%= randomNumber %>.jpg); }
	.content { margin: 0 1em; }
</style>
</head>
<body>
	<div class="image">
	<div class="content">
    <h1>Ouch! That's gotta hurt!</h1>
    <!--<p>Our site is currently experiencing technical difficulties. Support personnel are aware of the issue and are working to correct the situation. Please try again later.</p>
    <p>We apologize for the inconvenience.</p>-->
    <p>Our site had a little <em>crash</em> if its own.  Please <a href="javascript:history.go(-1);">go back</a> and retry the page.  If the problem persists, please try again later.  A notice has been sent to our crackerjack programmers...they are on it!</p>
    <p>All fun aside, we apologize for the inconvenience.</p>
  </div>
  </div>





<%
  Dim bakCodepage
  Dim ErrCategory
  Dim ErrASPCode
  Dim ErrNumber
  Dim ErrASPDescription
  Dim ErrDescription
  Dim ErrFile
  Dim ErrSource
  Dim ErrLine
  Dim ErrColumn
  on error resume next
  	  ErrCategory = objASPError.Category
	  ErrASPCode = objASPError.ASPCode
	  ErrNumber = objASPError.Number
	  ErrASPDescription = objASPError.ASPDescription
	  ErrDescription = objASPError.Description
	  ErrFile = objASPError.File
	  ErrSource = objASPError.Source
	  ErrLine = objASPError.Line
	  ErrColumn = objASPError.Column
	  bakCodepage = Session.Codepage
	  Session.Codepage = 1252



'If ErrNumber <> 0 then
Dim strBody
strBody = "An error was raised on " & Request.ServerVariables("SERVER_NAME") & " while doing a " & Request.ServerVariables("REQUEST_METHOD")
strBody = strBody & " to " & Request.ServerVariables("SCRIPT_NAME") & " at " & Now() & "." & VbCrLf
strBody = strBody & "Here is some information about the error:" & VbCrLf & VbCrLf
strBody = strBody & "Error Category        - " & ErrCategory & VbCrLf
strBody = strBody & "Error ASP Code        - " & ErrASPCode & VbCrLf
strBody = strBody & "Error Number          - " & ErrNumber & VbCrLf
strBody = strBody & "Error ASP Description - " & ErrASPDescription & VbCrLf
strBody = strBody & "Error Description     - " & ErrDescription & VbCrLf
strBody = strBody & "Error File            - " & ErrFile & VbCrLf
strBody = strBody & "Error Source          - " & ErrSource & VbCrLf
strBody = strBody & "Error Line            - " & ErrLine & VbCrLf
strBody = strBody & "Error Column          - " & ErrColumn & VbCrLf & VbCrLf
strBody = strBody & "----------------------------------------------------------" & VbCrLf & VbCrLf
strBody = strBody & "Here is the Form/Querystring/Session data:" & VbCrLf
strBody = strBody & "REQUEST.QUERYSTRING   -" & VbCrLf
Dim QS
For each QS in Request.QueryString
	strBody = strBody & "  " & Request.QueryString.Key(QS) & "=" & Request.QueryString.Item(Request.QueryString.Key(QS)) & VbCrLf
Next
strBody = strBody & "REQUEST.FORM          -" & VbCrLf
Dim Frm

'mok 2-8-2003
on error resume next

For each Frm in Request.Form
	strBody = strBody & "  " & Request.Form.Key(Frm) & "=" & Request.Form.Item(Request.Form.Key(Frm)) & VbCrLf
Next

strBody = strBody & "SESSION.CONTENTS      -" & VbCrLf

Dim item
For each item in Session.Contents
	strBody = strBody & "  " & item & "=" & Session.Contents(item) & VbCrLf
Next

'mok 2-8-2003
'on error goto 0

strBody = strBody & "----------------------------------------------------------" & VbCrLf & VbCrLf
strBody = strBody & "Here are all the ServerVariables:" & VbCrLf
Dim SV
For each SV in Request.ServerVariables
	strBody = strBody & "  " & Request.ServerVariables.Key(SV) & "=" & Request.ServerVariables.Item(Request.ServerVariables.Key(SV)) & VbCrLf
Next

%>

<%= strBody %>
</body>
</html>




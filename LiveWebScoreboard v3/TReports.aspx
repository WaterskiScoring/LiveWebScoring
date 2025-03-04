<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TReports.aspx.vb" Inherits="LiveWebScoreBoard.TReports" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
 <!-- Required meta tags -->
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Official Reports</title>
    <link rel="stylesheet" href="Content/bootstrap.min.css" />
    <!--   <script src="js/bootstrap.bundle.min.js"></script>  -->
    <style>
        .box{
           
            border: 1px solid blue;
            min-height: 50px;
            font-size: .6em;
            
        }
.legend .row:nth-of-type(odd) div {
background-color:antiquewhite;
}
.legend .row:nth-of-type(even) div {
background: #FFFFFF;
}
    </style>
</head>
<body>
    <form id="form1" runat="server">
       

        <!-- Display title bar and error label -->
        <div><asp:Label ID="lbl_Errors" runat="server" ForeColor="Red"/><br />
  <center><asp:Button ID="Btn_Back" runat="server" Text="Tourn Home" OnClientClick="JavaScript:window.history.back(1); return false;" /></center>
        </div>
      
      <div id="ReportList" runat="server">
        
          
   </div>
    </form>
</body>
</html>
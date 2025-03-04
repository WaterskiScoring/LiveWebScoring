<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Tournament.aspx.vb" Inherits="LiveWebScoreBoard.Tournament" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
     <!-- Required meta tags -->
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Tournament Detail</title>
    <link rel="stylesheet" href="Content/bootstrap.min.css" />
   <!--   <script src="js/bootstrap.bundle.min.js"></script>  -->
    
</head>
<body>
    <form id="form1" runat="server">
        <!-- Store user selections -->
        <asp:Panel ID="Panel1" runat="server" Visible="false">
            <asp:HiddenField ID="HF_SanctionID" runat="server" />
            <asp:HiddenField ID="HF_SkierID" runat="server" />
            <asp:HiddenField ID="HF_YearPkd" runat="server" /> 
            <asp:HiddenField ID="HF_TournName" runat="server" />
            <asp:HiddenField ID="HF_FormatCode" runat="server" />
            <asp:HiddenField ID="HF_SlalomRnds" runat="server" />
            <asp:HiddenField ID="HF_TrickRnds" runat="server" />
            <asp:HiddenField ID="HF_JumpRnds" runat="server" />
            <asp:HiddenField ID="HF_DisplayMetric" runat="server" />
       <asp:HiddenField ID="HF_ROCount" runat="server" />
       </asp:Panel>
        <!-- display Tournament Name and error messages if any -->
       <div id="TName" class="text-bg-dark text-center" runat="Server"><br/>
        <asp:Label ID="lbl_Errors" runat="server" ForeColor="white"/>
       </div>
        <div class="container-fluid">
        <!-- Display navivigation buttons -->
       
            
        <!-- Display Filter Droplists -->
        <div class="text-bg-info text-center">
           <asp:DropDownList ID="DDL_Format" runat="server" AutoPostBack="true"></asp:DropDownList>
         <asp:Button ID="Btn_Back2TList" runat="server" Text="Tournament List" />
            <asp:Button ID="Btn_Home" runat="server" Text="Home Page" />
         <asp:Button ID="Btn_Reports" runat="server" Text="Reports" />
            <br /><asp:Label ID="Lbl_ActiveEvents" runat="server" text="Active Events" ForeColor="red" font-bold="true"/>
           </div>
            <div class="row">
                <div class ="col-sm-4">
                        <div id="DisplayText" runat="Server"></div>
                </div>
                
                  <div class =" col-sm-4"> 
                      <div id="Officials"  runat="Server"></div>
                 </div>
                <asp:Panel ID="Panel_Teams" runat="server" Visible="false">
                    <div class="col-sm-4">
                        <h3>Participating Teams</h3>
                        <asp:Label ID="lbl_Teams" runat="server" Text="Label"></asp:Label>
                    </div>
                </asp:Panel>
                
            </div> <!-- End Div class row --> 
        </div> <!-- End of container -->    
 
    </form>
</body>
</html>
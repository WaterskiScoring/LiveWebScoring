<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Tournament.aspx.vb" Inherits="LiveWebScorebook22vb.Tournament" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
     <!-- Required meta tags -->
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Tournament</title>
    <link rel="stylesheet" href="Content/bootstrap.min.css" />
    <script src="js/bootstrap.bundle.min.js"></script>
    
</head>
<body>
    <form id="form1" runat="server">
        <!-- Store user selections -->
        <asp:HiddenField ID="HF_SanctionID" runat="server" />
        <asp:HiddenField ID="HF_SkierID" runat="server" />
        <asp:HiddenField ID="HF_Event" runat="server" />
        <asp:HiddenField ID="HF_AgeGroup" runat="server" />
        <asp:HiddenField ID="HF_YearPkd" runat="server" /> <!-- Stores ddl_PkYear.selected value on home page.  Recent = 0 -->
        <asp:HiddenField ID="HF_TournName" runat="server" />
        <asp:HiddenField ID="HF_FormatCode" runat="server" />
        <!-- display Tournament Name and error messages if any -->
       <div id="TName" class="text-bg-dark text-center" runat="Server">
           <asp:Label ID="lbl_Errors" runat="server" Text="Here I am" ForeColor="Red"/>
        </div>

        <!-- Display navivigation buttons -->
        <div class="text-center">
            <asp:Button ID="Btn_ScoreXSkier" runat="server" Text="Scores by Skier" />
           <asp:Button ID="Btn_PlacementXDv" runat="server" Text="Placement by Division" />
            <asp:Button ID="Btn_Back2TList" runat="server" Text="Different Tournament" />
            <asp:Button ID="Btn_Home" runat="server" Text="Home Page" />
        </div>

        <div class="container-fluid">
            <div class="row">
                <div class =" col-xs-1"> </div>
                    <div class =" col-xs-10">
                        <div id="DisplayText" runat="Server"></div>
                            <div><asp:CheckBox ID="cb_UseNOPS" runat="server" 
                                Text="Check to use NOPS scoring"/><br />
                                <asp:DropDownList ID="ddl_SelectFeatures" runat="server">
                                <asp:ListItem Selected="True" Value="0">Optional Features</asp:ListItem>
                                <asp:ListItem Value="BestRnd">Format Best Round</asp:ListItem>
                                <asp:ListItem Value="ByRnd">Format By Round</asp:ListItem>
                                <asp:ListItem Value="RndsCombo">Scores From Multiple Rounds Combined</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        
                            <div id="Officials"  runat="Server">
                        </div>
                    </div> <!-- End of col-xs-10 -->
                <div class =" col-xs-1"></div>
            </div> <!-- End Div class row --> 
        </div> <!-- End of container -->    
 
    </form>
</body>
</html>
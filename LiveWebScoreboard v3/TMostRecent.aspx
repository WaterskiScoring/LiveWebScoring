<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TMostRecent.aspx.vb" Inherits="LiveWebScoreBoard.TMostRecent" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
 <!-- Required meta tags -->
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Most Recent Performances</title>
    <link rel="stylesheet" href="Content/bootstrap.min.css" />
   <!--   <script src="js/bootstrap.bundle.min.js"></script>  -->
    <script src="../../Scripts/MicrosoftAjax.js" type="text/javascript"></script>
    <script src="../../Scripts/MicrosoftMvcAjax.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-1.2.6.min.js" type="text/javascript"></script>
    <style>
        .box2{
    border: 1px solid blue;
}

        .box{
            min-height: 50px;
            font-size: .9em;
        }
 

.ms-0{
	margin-left: 0;
	margin-right: 0;
    padding-left: 0;
    padding-right: 0;
    margin-top: 80px;
}
            .dropdown{
          margin-left: 0;
          margin-right: 0;
          min-height: 75px;
      }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager runat="server"></asp:ScriptManager>

   <!-- Store user selections -->
        <asp:Panel ID="Panel_HF" runat="server" Visible="false">
            <asp:HiddenField ID="HF_SanctionID" runat="server" />
            <asp:HiddenField ID="HF_SkierID" runat="server" />
            <asp:HiddenField ID="HF_Event" runat="server" />
            <asp:HiddenField ID="HF_AgeGroup" runat="server" />
            <asp:HiddenField ID="HF_RndPkd" runat="server" /> <!-- selectedvalue of ddl_PkRnd -->
            <asp:HiddenField ID="HF_YearPkd" runat="server" /> <!-- Stores ddl_PkYear.selected value on home page.  Recent = 0 -->
            <asp:HiddenField ID="HF_TournName" runat="server" />
            <asp:HiddenField ID="HF_FormatCode" runat="server" />
            <asp:HiddenField ID="HF_SlalomRnds" runat="server" />
            <asp:HiddenField ID="HF_TrickRnds" runat="server" />
            <asp:HiddenField ID="HF_JumpRnds" runat="server" />
            <asp:HiddenField ID="HF_UseNOPS" runat="server" />
         <asp:HiddenField ID="HF_UseTeams" runat="server" />
         <asp:HiddenField ID="HF_DisplayMetric" runat="server" />
         </asp:Panel>
<div class="fixed-top">
        <!-- Display title bar and error label -->
        <div id="TName" class="text-bg-dark text-center" runat="Server">
          
       </div>
        <!-- Display Filter Droplists

        <div class="col-12 text-bg-info text-center ">
                <asp:Label ID="lbl_Errors" runat="server"  ForeColor="Red" Font-Bold="true" visible="false" /><br />
        <b>Events </b> <asp:DropDownList ID="DDL_EventsPkd" runat="server" AutoPostBack="true"></asp:DropDownList>&nbsp;
                    <b>Divisions </b><asp:DropDownList ID="DDL_DvPkd" runat="server"  AutoPostBack="true"></asp:DropDownList>&nbsp;
                    <asp:DropDownList ID="DDL_PkRnd" runat="server" Enabled="false" Visible="false" ></asp:DropDownList>    
                   </div>    -->  

        <!-- Display navivigation buttons -->
        <div class="col-12 text-bg-info text-center">
                   <asp:Button ID="Btn_2Tournament" runat="server" Text="Tourn Home" />
                       <asp:Button ID="Btn_2TList" runat="server" Text="Tournament List" />
                        <asp:Button ID="Btn_Home" runat="server" Text="Scoreboard Home" />
                        <asp:Button ID="Btn_UpdateDisplay" runat="server" Text="Update Display"  />
</div>
 </div> <!-- end of fixed top -->
         <div class="dropdown"> under title</div>
<div class="container ms-0">

    <asp:Panel ID="Panel_OnWater" runat="server" Visible="True">
        <div class="row">
            <div class="col-xxl box" id="OnWater" runat="server" >
           </div>
        </div>
</asp:Panel>



</div> <!-- End of class Container -->

 
    </form>
</body>
    </html>
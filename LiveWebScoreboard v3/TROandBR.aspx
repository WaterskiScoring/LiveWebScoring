<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TROandBR.aspx.vb" Inherits="LiveWebScoreBoard.TROandBR" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
 <!-- Required meta tags -->
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Running Order and Best Round</title>
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
        .RunOrder{
            position: fixed;
  right: 0;
  top: 50%;
  width: 8em;
  margin-top: -2.5em;
  background-color:aqua;
        }
.ms-0{
	margin-left: 0;
	margin-right: 0;
    padding-left: 0;
    padding-right: 0;
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

        <!-- Display title bar and error label -->
        <div id="TName" class="text-bg-dark text-center" runat="Server">
           <asp:Label ID="lbl_Errors" runat="server"  ForeColor="white" Font-Bold="true"/>
        
       </div>
        <!-- Display Filter Droplists -->
<asp:UpdatePanel ID="UpdatePanel_Filters" runat="server">
    <ContentTemplate>
<asp:Panel ID="Panel_Filters" runat="server" Visible="true">
        <div class="col-12 text-bg-info text-center ">
                <b>Events </b> <asp:DropDownList ID="DDL_EventsPkd" runat="server" AutoPostBack="true"></asp:DropDownList>&nbsp;
                    <b>Divisions </b><asp:DropDownList ID="DDL_DvPkd" runat="server"></asp:DropDownList>&nbsp;
                    <b>Rounds </b><asp:DropDownList ID="DDL_PkRnd" runat="server" ></asp:DropDownList>         
        </div>
  </asp:Panel>      </ContentTemplate>
    </asp:UpdatePanel>    
        <!-- Display navivigation buttons -->
        
        <div class="col-12 text-bg-info text-center">
                   <asp:Button ID="Btn_2Tournament" runat="server" Text="Tourn Home" />
                       <asp:Button ID="Btn_2TList" runat="server" Text="Tournament List" />
                        <asp:Button ID="Btn_Home" runat="server" Text="Scoreboard Home" />
        </div>
 
  
        <asp:UpdatePanel ID="UpdatePanelResults"  runat="server">
            <ContentTemplate>
                
        <!-- Display created in code and put into div id=InsertHere.  AJAX reload that div when reload button is clicked. -->
       
           <div class="col-12 text-bg-info text-center">
               
                    <asp:Button ID="Btn_UpdateDisplay" runat="server" Text="Update Display" CssClass="text-center" />
             
           </div>
  

<div class="container-fluid ms-0">


    <asp:Panel ID="Panel_OnWater" runat="server" Visible="True">
        <div class="row">
            <div class="col-xxl box" id="OnWater" runat="server" >
           </div>
        </div>
</asp:Panel>
     <asp:Panel ID="Panel_Row2" runat="server" Visible="True">
         <div class="row">
                <div class="col-xxl box" id="ColumnScores" runat="server" >
                </div> 
       </div
</asp:Panel>


</div> <!-- End of class Container -->
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
    </html>
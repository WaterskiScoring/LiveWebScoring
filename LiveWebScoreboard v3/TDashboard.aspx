<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TDashboard.aspx.vb" Inherits="LiveWebScoreBoard.TDashboard" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
 <!-- Required meta tags -->
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Scores Dashboard</title>
    <link rel="stylesheet" href="Content/bootstrap.min.css" />
  
    <script src="../../Scripts/MicrosoftAjax.js" type="text/javascript"></script>
    <script src="../../Scripts/MicrosoftMvcAjax.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-1.2.6.min.js" type="text/javascript"></script>
      <style>
      .box2{
 border: 1px solid blue;
      }
          .box{

          min-height: 50px;
          font-size: .8em;
      }
      .ms-0{

    margin-top:auto;
    padding-left: 2px;
    padding-right: 0;
}
      .dropdown{
          margin-left: 0;
          margin-right: 0;
          min-height: 125px;
      }
      .red-text {

 color: #F00;

}
  </style>
</head>
<body>
    <form id="form2" runat="server">
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
               <asp:HiddenField ID="HF_MultiS" runat="server" />
            <asp:HiddenField ID="HF_MultiT" runat="server" />
            <asp:HiddenField ID="HF_MultiJ" runat="server" />

         </asp:Panel>
        <!-- Display title bar and error label -->
        
<div class="fixed-top">
        <div id="TName" class="text-bg-dark text-center" runat="Server">
           <asp:Label ID="lbl_Errors" runat="server"  ForeColor="white" Font-Bold="true"/>
       </div>
        <!-- Display Filter Droplists -->

    <div class="text-bg-info text-center">
            <asp:DropDownList ID="DDL_Event_Div" runat="server" AutoPostBack="true"></asp:DropDownList>
            <asp:DropDownList ID="DDL_PkRnd" runat="server"  AutoPostBack="true" ></asp:DropDownList>
        <asp:Button ID="Btn_UpdateDisplay" runat="server" Text="Update Display" />
    </div>

        <!-- Display navivigation buttons -->
          <div class="text-bg-info text-center">
             <asp:Button ID="Btn_2Tournament" runat="server" Text="Tourn Home" />
            <asp:Button ID="Btn_2TList" runat="server" Text="Tournament List" />
            <asp:Button ID="Btn_Home" runat="server" Text="Home Page" />
        </div>     
</div>
        <div class="dropdown"></div>
        <!-- Display created in code and put into div id=InsertHere.   -->
        <div class="container ms-0">
            <div class="row">
                <div class="col-xxl box2 ms-0" id="OnWater" runat="server" ></div>
           </div>
        </div>

        <div class="container ms-0">
            <div class="row">
                <div id="RunOrd" runat="server" class="col-5 box "></div>
                <div id="Results" runat="server" class="col-7   box "></div>
           </div>

    </div>  <!-- End of class Container -->
           
    </form>
</body>
    </html>
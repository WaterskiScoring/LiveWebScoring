<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TXDivisionPro.aspx.vb" Inherits="LiveWebScoreBoard.TXDivisionPro" %>
<%@ Register Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
 <!-- Required meta tags -->
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Scores By Division</title>
    <link rel="stylesheet" href="Content/bootstrap.min.css" />
  
    <script src="../../Scripts/MicrosoftAjax.js" type="text/javascript"></script>
    <script src="../../Scripts/MicrosoftMvcAjax.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-1.2.6.min.js" type="text/javascript"></script>
      <style>
      .box{
         
          border: 1px solid blue;
          min-height: 50px;
          font-size: .6em;
          
      }
      .RunOrder{
          position: fixed;
right: 0;
top: 50%;
width: 8em;
margin-top: -2.5em;
background-color:aqua;
      }
      .Details{
margin-top: 100px;
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
         </asp:Panel>
        <!-- Display title bar and error label -->

       <div class="fixed-top">
        <div id="TName" class="text-bg-dark text-center" runat="Server">
           <asp:Label ID="lbl_Errors" runat="server"  ForeColor="white" Font-Bold="true"/>
        
       </div>
        <!-- Display navivigation buttons -->
        
        
        <!-- Display Filter Droplists -->
<asp:UpdatePanel ID="UpdatePanel_Filters" runat="server">
    <ContentTemplate>
<div class="text-bg-info text-center">
    <b>Events: </b> <asp:DropDownList ID="DDL_EventsPkd" runat="server" AutoPostBack="true"></asp:DropDownList>
            <b>Divisions: </b><asp:DropDownList ID="DDL_DvPkd" runat="server"></asp:DropDownList>
            <b>Rounds: </b><asp:DropDownList ID="DDL_PkRnd" runat="server"></asp:DropDownList>
</div>
        </ContentTemplate>
    </asp:UpdatePanel>     


           <div class="text-bg-info text-center">
                   <asp:Button ID="Btn_2Tournament" runat="server" Text="Tourn Home" />
           <asp:Button ID="Btn_2TList" runat="server" Text="Tournament List" />
            <asp:Button ID="Btn_Home" runat="server" Text="Home Page" />
    
        </div>   
            <div class="text-bg-info text-center">
                    <asp:Button ID="Btn_UpdateDisplay" runat="server" Text="Update Display" />
 </div> <!-- End of Fixed-Top  -->

       
<!--
<div id="RunOrd" class="RunOrder" >
    Up Next<br />
    <ul><li>Me</li>
        <li>Myself</li>
        <li>and I</li>
    </ul>
</div> -->



           </div> 
       <div class="container"><!-- Display created in code and put into div id=InsertHere.  -->
           <div class="row">
    <asp:Panel ID="Panel_R1C1" runat="server" Visible="True">
       
            <div class="col-xxl">
                <div id="ColumnOne" runat="server" class="box">
               </div>
            </div>
    </asp:Panel>
     <asp:Panel ID="Panel_R1C2" runat="server" Visible="True">
        <div class="col-xxl">
            <div id="ColumnTwo" runat="server" class="box">

           </div>
        </div>
         </asp:Panel>
     <asp:Panel ID="Panel_R1C3" runat="server" Visible="True">
        <div class="col-xxl">
            <div id="ColumnThree" runat="server" class="box">

           </div>
        </div> 
    </asp:Panel>
<asp:Panel ID="Panel_R1C4" runat="server" Visible="True">
        <div class="col-xxl">
            <div id="ColumnFour" runat="server" class="box">
            </div>
        </div> <!-- End of fourth column -->
         </asp:Panel>

    </div> <!-- End of row 1 holding columns column -->

</div> <!-- End of class Container -->

    </form>
</body>
    </html>
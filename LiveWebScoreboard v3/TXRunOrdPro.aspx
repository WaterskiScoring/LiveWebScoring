<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TXRunOrdPro.aspx.vb" Inherits="LiveWebScoreBoard.TXRunOrdPro" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
 <!-- Required meta tags -->
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Running Order</title>
    <link rel="stylesheet" href="Content/bootstrap.min.css" />
   <!--   <script src="js/bootstrap.bundle.min.js"></script>  -->
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
        <div id="TName" class="text-bg-dark text-center" runat="Server">
           <asp:Label ID="lbl_Errors" runat="server"  ForeColor="white" Font-Bold="true"/>
        
       </div>
        <!-- Display Filter Droplists -->
<asp:UpdatePanel ID="UpdatePanel_Filters" runat="server">
    <ContentTemplate>
<asp:Panel ID="Panel_Filters" runat="server" Visible="true">
        <div class="text-bg-info text-center">
            <table>
                <tr><td width="25%"><b>Events </b> <asp:DropDownList ID="DDL_EventsPkd" runat="server" AutoPostBack="true"></asp:DropDownList></td>
                    <td width="25%"><b>Divisions </b><asp:DropDownList ID="DDL_DvPkd" runat="server"></asp:DropDownList> </td>
                    <td width="25%"><b>Rounds </b><asp:DropDownList ID="DDL_PkRnd" runat="server"></asp:DropDownList> </td></tr>
            </table>
        </div>
  </asp:Panel>      </ContentTemplate>
    </asp:UpdatePanel>    
        <!-- Display navivigation buttons -->
        
        <div class="text-bg-info text-center">
                   <table>
<tr><td width="33%"><asp:Button ID="Btn_2Tournament" runat="server" Text="Tourn Home" /></td>
           <td width="33%"><asp:Button ID="Btn_2TList" runat="server" Text="Tournament List" /></td>
            <td width="33%"><asp:Button ID="Btn_Home" runat="server" Text="Scoreboard Home" /></td>
    </tr></table>
        </div>
 
        
        <asp:UpdatePanel ID="UpdatePanelResults"  runat="server">
            <ContentTemplate>
                
        <!-- Display created in code and put into div id=InsertHere.  AJAX reload that div when reload button is clicked. -->
       <div class="container">
           <div class="row">
               <div class="col-12 text-center text-bg-info">
                    <asp:Button ID="Btn_UpdateDisplay" runat="server" Text="Update Display" CssClass="text-center" />
               </div>
           </div>




    <asp:Panel ID="Panel_OnWater" runat="server" Visible="True">
        <div class="row">
            <div clase="col-12" id="OnWater" runat="server" class="box">
           </div>
        </div>
</asp:Panel>
     <asp:Panel ID="Panel_Row2" runat="server" Visible="True">
         <div class="row">
            <div class="col-3">
                <div id="ColumnLeft" runat="server" class="box">

                </div>
            </div>
            <div class="col-9">
                <div id="ColumnRight" runat="server" class="box">

                </div>
            </div> 
       </div
</asp:Panel>


</div> <!-- End of class Container -->
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
    </html>

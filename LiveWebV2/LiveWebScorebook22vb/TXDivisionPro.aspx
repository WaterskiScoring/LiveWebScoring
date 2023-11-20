<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TXDivisionPro.aspx.vb" Inherits="LiveWebScorebook22vb.TXDivisionPro" %>
<%@ Register Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
 <!-- Required meta tags -->
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Placement By Division</title>
    <link rel="stylesheet" href="Content/bootstrap.min.css" />
    <script src="js/bootstrap.bundle.min.js"></script>
    <script src="../../Scripts/MicrosoftAjax.js" type="text/javascript"></script>
    <script src="../../Scripts/MicrosoftMvcAjax.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-1.2.6.min.js" type="text/javascript"></script>
    <style>
        .box{
           
            border: 1px solid blue;
            min-height: 50px;
            font-size: .6em;
            
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
            <asp:HiddenField ID="HF_YearPkd" runat="server" /> <!-- Stores ddl_PkYear.selected value on home page.  Recent = 0 -->
            <asp:HiddenField ID="HF_TournName" runat="server" />
            <asp:HiddenField ID="HF_FormatCode" runat="server" />
            <asp:HiddenField ID="HF_UseNOPS" runat="server" />
         </asp:Panel>
        <!-- Display title bar and error label -->
        <div id="TName" class="text-bg-dark text-center" runat="Server">
           <asp:Label ID="lbl_Errors" runat="server" Text="Here I am" ForeColor="Red"/>
        
       </div>
        <div>
            TEST RECAP<asp:HyperLink ID="HyperLink1" runat="server" target = "_blank">James Bryans OM</asp:HyperLink>
            <br />TEST RECAP<asp:HyperLink ID="HyperLink2" runat="server" target = "_blank">Tristan Duplan-Fribourg JM</asp:HyperLink>
        </div>


        <!-- Display navivigation buttons -->
        <div class="text-center">
            <asp:Button ID="Btn_2Tournament" runat="server" Text="Tournament Detail" />
           <asp:Button ID="Btn_2TList" runat="server" Text="Tournament List" />
            <asp:Button ID="Btn_Home" runat="server" Text="Home Page" />
        </div>
        <!-- Display Filter Droplists -->
<div class="text-bg-info text-center">
    <table>
        <tr><td width="33%"><b>Event Selector:</b> 
            <asp:DropDownList ID="DDL_Events" runat="server" AutoPostBack="true"></asp:DropDownList></td>
            <td width="33%"><b>Round Selector: </b><asp:DropDownList ID="DDL_Round" runat="server"></asp:DropDownList> </td>
            <td width="33%"><b>Division Selector: </b><asp:DropDownList ID="DDL_Divisions" runat="server"></asp:DropDownList></td></tr>
    </table>
    
     
</div>
        <asp:UpdatePanel ID="UpdatePanelResults" 
             runat="server">
            <ContentTemplate>
               
        <!-- Display created in code and put into div id=InsertHere.  AJAX reload that div when reload is clicked. -->
       <div class="container">
           <div class="row">
               <div class="col-12 text-center">
                    <asp:Button ID="Btn_UpdateDisplay" runat="server" Text="Update Display" CssClass="text-center" />
               </div>
           </div>
        <div class="row">
        <div class="col-xxl">
            <div id="ColumnOne" runat="server" class="box">

           </div>
        </div>
        <div class="col-xxl">
            <div id="ColumnTwo" runat="server" class="box">

           </div>
        </div>
        <div class="col-xxl ">
            <div id="ColumnThree" runat="server" class="box">

           </div>
        </div> <!-- End of classRow -->
    </div>
    <hr /> 
     </div> <!-- End of class Container -->
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
    </html>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Library.aspx.cs" Inherits="CDWKS.BXC.Web.Library" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title></title>
    </head>
    <body>
        <form id="form1" runat="server">
            <asp:ScriptManager ID="ScriptManager1" EnablePageMethods="true" runat="server">
            </asp:ScriptManager>
            <script type="text/javascript">
        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }

        function returnToParent() {
            //create the argument that will be returned to the parent page
            var oArg = new Object();

            //get the city's name
            oArg.library = document.getElementById("ddLibraries").value;

            //Close the RadWindow and send the argument to the parent page
            PageMethods.UpdateLibrary(oArg.library, onSucceed, onError);


        }

        function onSucceed(results) {
            if (results) {
                //create the argument that will be returned to the parent page
                var oArg = new Object();

                //get the city's name
                oArg.library = document.getElementById("ddLibraries").value;
                //get a reference to the current RadWindow
                var oWnd = GetRadWindow();
                oWnd.close(oArg);
            } else {
                document.getElementById("divNotFound").style.visibility = 'visible';
            }

        }

        function onError(results) {
            alert("Error");
        }

//get a reference to the current RadWindow
        var oWnd = GetRadWindow();
    </script>
            <center>
                <div class="d">
                <h3>Enter Code to Add Libraries:</h3>
                <div class="acc-info-field-div">
                    <label><asp:Literal ID="litEnterPromo" Text="Code:" runat="server" /></label><br />
                    <asp:TextBox ID="txtEnterPromo" CssClass="text" runat="server"/><br />
                    <span id="codeerror" runat="server" style="color: Red;"><asp:Literal ID="litError" runat="server" Visible="false" Text="Code not Found!"></asp:Literal></span>
                </div><br />
                <asp:ImageButton ID="btnAddPromo" CssClass="button" AlternateText="Add Libraries"  ImageUrl="~/Images/btnEnterCode.png" Height="35px" OnClick="BtnAddPromoClick" runat="server" />
                <h3>Change Library</h3>
                <asp:DropDownList ID="ddLibraries" runat="server"></asp:DropDownList><br /><br />
                <asp:ImageButton ID="btnChangeLib" CssClass="button" AlternateText="Change Library" ImageUrl="~/Images/btnChangeLibrary2.png" Height="35px" OnClientClick="returnToParent(); return false;" runat="server" />
            </center>
            <asp:Literal ID="litLibrary" runat="server" Visible="false" ></asp:Literal>
        </div>
        </form>
    </body>
</html>
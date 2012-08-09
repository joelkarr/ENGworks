<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="CDWKS.BXC.Web.Login" %>

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
            oArg.userName = document.getElementById("txtEmail").value;
            oArg.password = document.getElementById("txtPass").value;

            //Close the RadWindow and send the argument to the parent page
            PageMethods.UserLogin(oArg.userName, oArg.password, onSucceed, onError);


        }

        function onSucceed(results) {
            if (results) {
                //create the argument that will be returned to the parent page
                var oArg = new Object();

                //get the city's name
                oArg.userName = document.getElementById("txtEmail").text;
                oArg.password = document.getElementById("txtPass").text;
                //get a reference to the current RadWindow
                var oWnd = GetRadWindow();
                oWnd.close(oArg);
            } else {
                document.getElementById("divNotFound").style.visibility = 'visible';
            }

        }

        function onError(err, url, line) {
            alert("Error:" + err + " Line:" + line);
        }

//get a reference to the current RadWindow
        var oWnd = GetRadWindow();
    </script>
            <center>
                <div style="width: 300px; height: 400px;">
                    <asp:UpdatePanel ID="upLogin" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div style="height: 150px; width: 100"></div>
                            <div>
                                <div id="divNotFound" style="visibility: hidden; color: Red;"> 
                                    <asp:Literal ID="litNotFound" Text="User/Password not found." Visible="true" runat="server" /> 
                                </div>
                                <table>
                                    <tr>
                                        <td>Email:</td>
                                        <td><asp:TextBox ID="txtEmail" runat="server"></asp:TextBox><asp:RequiredFieldValidator ID="reEmail" ControlToValidate="txtEmail" runat="server"></asp:RequiredFieldValidator></td>
                                    </tr>
                                    <tr>
                                        <td>Password:</td>
                                        <td><asp:TextBox ID="txtPass" TextMode="Password" runat="server"></asp:TextBox><asp:RequiredFieldValidator ID="rePassword" ControlToValidate="txtPass" runat="server"></asp:RequiredFieldValidator></td>
                                    </tr>
                                </table>
                                <asp:ImageButton ID="btnLogin" Height="40px" style="padding-top: 20px;" ImageUrl="~/images/loginbutton.png" OnClientClick="returnToParent(); return false;" CausesValidation="true" runat="server" /></td>
                                <br />
                                <div id="Reg" style="padding-top: 15px;">
                                    <asp:HyperLink ID="hlRegister" Text="Need to Register?" NavigateUrl="~/Registration.aspx" runat="server" />
                                </div>
                            </div>
                            <asp:ValidationSummary ID="validsum" DisplayMode="BulletList" ShowMessageBox="true" ShowSummary="false" runat="server"/>
                        </ContentTemplate></asp:UpdatePanel></div></center>
        </form>
    </body>
</html>
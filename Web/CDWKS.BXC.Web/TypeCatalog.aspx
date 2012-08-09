<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TypeCatalog.aspx.cs" Inherits="CDWKS.BXC.Web.TypeCatalog" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title></title>
        <link rel = "Stylesheet" type = "text/css" href="styles/contentstyle.css" />
        <link rel = "Stylesheet" type = "text/css" href="styles/jQuery.css" />
    </head>
    <body style="background-color: White;">
        <form id="form1" runat="server">
            <asp:ScriptManager ID="ScriptManager1" EnablePageMethods="true" runat="server">
            </asp:ScriptManager>
            <script type="text/javascript" language="javascript">
                var grid;

                function GetSelectedIds() {
                    var grid = $find("<%=grdTypes.ClientID%>");
                    var MasterTable = grid.get_masterTableView();
                    var selectedRows = MasterTable.get_selectedItems();
                    var ItemIds = new Object();
                    for (var i = 0; i < selectedRows.length; i++) {
                        var id = $find("<%=grdTypes.ClientID%>").get_masterTableView().get_selectedItems()[i].getDataKeyValue("ID");
                        ItemIds[i] = id;
                    }
                    PageMethods.CreateDownloadPackage(ItemIds, onSucceed, onError);
                }

                function GridCreated() {
                    grid = this;
                }

                function GetRadWindow() {
                    var oWindow = null;
                    if (window.radWindow) oWindow = window.radWindow;
                    else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                    return oWindow;
                }

                function onSucceed(results) {
                    //create the argument that will be returned to the parent page
                    var oArg = new Object();
                    oArg.Package = results;
                    //get a reference to the current RadWindow
                    var oWnd = GetRadWindow();
                    oWnd.close(oArg);
                }

                function onError(results) {
                    alert("Error");
                }

//get a reference to the current RadWindow
                var oWnd = GetRadWindow();
        </script>
            <div>
                <asp:UpdatePanel runat="server">
                    <ContentTemplate>
                        <telerik:RadGrid ID="grdTypes" runat="server"
                                         AllowSorting="True" GridLines="None" Skin="WebBlue" 
                                         AllowMultiRowSelection="True" Height="500px" Width="750px" 
                                         AutoGenerateColumns="True">
                            <MasterTableView DataKeyNames="ID" ClientDataKeyNames="ID">
                                <Columns>
                                    <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn" />
                                </Columns>
                            </MasterTableView>
                            <ClientSettings>
                                <Selecting AllowRowSelect="True" />
                                <Scrolling AllowScroll="True" ScrollHeight="420px" />
                                <ClientEvents OnGridCreated="GridCreated" ></ClientEvents>
                            </ClientSettings>
                        </telerik:RadGrid>
                        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
                            <AjaxSettings>
                                <telerik:AjaxSetting AjaxControlID="RadGrid1">
                                    <UpdatedControls>
                                        <telerik:AjaxUpdatedControl ControlID="RadGrid1" />
                                    </UpdatedControls>
                                </telerik:AjaxSetting>
                            </AjaxSettings>
                        </telerik:RadAjaxManager>
                        <center>
                            <asp:ImageButton ID="btnDownload" runat="server" 
                                             ImageUrl="Images/LoadFileButton.png" Height="40px" OnClientClick="GetSelectedIds(); return false;"/>
                        </center>
                    </ContentTemplate></asp:UpdatePanel>
            </div>
        </form>
    </body>
</html>
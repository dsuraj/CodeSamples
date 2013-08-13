<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebMethodExample.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Button ID="btnGetDate" runat="server" Text="Get date time" OnClientClick="GetDateTime();return false;" />
            <br />
            <asp:Label ID="lblDatetime" runat="server" ForeColor="Red" Font-Bold="true" Font-Size="X-Large"></asp:Label>
            <script src="http://code.jquery.com/jquery-1.10.1.min.js"></script>
            <script type="text/javascript">
                function GetDateTime() {
                    $.ajax({
                        type: "POST",
                        url: "Default.aspx/GetDateTime",
                        data: "{}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (msg) {
                            $("#<%=lblDatetime.ClientID%>").text(msg.d);
                        },
                        error: function (msg) {                           
                        }
                    });
                }
            </script>
        </div>
    </form>
</body>
</html>

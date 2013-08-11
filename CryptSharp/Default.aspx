<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:TextBox ID="txtPassword" runat="server"></asp:TextBox>
            <asp:Button ID="btnEncryptPassword" runat="server" OnClick="btnEncryptPassword_Click" Text="Encrypt my password" />
        </div>
    </form>
</body>
</html>

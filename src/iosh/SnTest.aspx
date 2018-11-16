<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SnTest.aspx.cs" Inherits="iosh.SnTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Go" />
        <br />
        <asp:ListBox ID="ListBox1" runat="server" Height="264px" Width="331px"></asp:ListBox>
    
    </div>
    </form>
</body>
</html>

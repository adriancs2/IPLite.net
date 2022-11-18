<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication1.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="btCheckCurrent" runat="server" Text="Check Current Visitor's IP" OnClick="btCheckCurrent_Click" />
        <br /><br />
        <asp:Button ID="btManualCheck" runat="server" Text="Check This IP Address" OnClick="btManualCheck_Click" />
    &nbsp;<asp:TextBox ID="TextBox1" runat="server" Width="175px"></asp:TextBox>
    </div>
    </form>
    The system uses IP database provided by IP2Location.<br />
    <br />
    You may download the Product: DB11<br />
    <br />
    <a href="https://lite.ip2location.com/database/db11-ip-country-region-city-latitude-longitude-zipcode-timezone">https://lite.ip2location.com/database/db11-ip-country-region-city-latitude-longitude-zipcode-timezone</a>
</body>
</html>

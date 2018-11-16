<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="ReportTest.aspx.cs" Inherits="iosh.ReportTest" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FormContentPlaceHolder" runat="server">
    PaperID : <asp:DropDownList ID="ddlReport" runat="server"></asp:DropDownList>
    <asp:Button ID="btnReport" runat="server" Text="工作適能診斷報告" OnClick="btnReport_Click" /><br />
    PaperID : <asp:DropDownList ID="ddlBody" runat="server"></asp:DropDownList>
    <asp:Button ID="btnBody" runat="server" Text="肌肉骨骼評估報告" OnClick="btnBody_Click" /><br />
</asp:Content>

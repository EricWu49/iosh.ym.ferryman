<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Pressure.aspx.cs" Inherits="iosh.Pressure" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FormContentPlaceHolder" runat="server">
    <asp:Panel ID="pnlGood" runat="server" >
        <div class="bg-info" style="width: 100%; vertical-align: middle; margin: 10px;">
            <p style="font-size: 12pt;">
                <asp:Literal ID="litPressure" runat="server"></asp:Literal>
                <asp:Literal ID="litSatisfy" runat="server"></asp:Literal>
            </p>
        </div>
    </asp:Panel>
    <div style="padding: 10px; text-align: center">
        <asp:Button ID="btnReturn" CssClass="btn btn-info noprint" Width="160" runat="server" Text="返回構面選項頁" OnClick="btnReturn_Click" />
    </div>
</asp:Content>

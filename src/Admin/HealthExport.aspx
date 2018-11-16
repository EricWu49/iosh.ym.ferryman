<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="HealthExport.aspx.cs" Inherits="Admin.HealthExport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div id="Panel_Setting" class="easyui-panel" title="健康狀況 資料匯出" style="width:100%;height:80px;padding:10px;">
        <span>資料統計區間：</span>
        從<asp:TextBox ID="txtStartDate" CssClass="easyui-datebox" data-options="formatter:myformatter,parser:myparser"  Width="150" runat="server"></asp:TextBox>
        至<asp:TextBox ID="txtEndDate" CssClass="easyui-datebox"  data-options="formatter:myformatter,parser:myparser" Width="150" runat="server"></asp:TextBox>
        <asp:Button ID="btnSearch" CssClass="easyui-linkbutton" Width="120" Height="25" data-options="iconCls:'icon-search'" runat="server" Text="查詢" OnClick="btnSearch_Click" />
    </div>
</asp:Content>

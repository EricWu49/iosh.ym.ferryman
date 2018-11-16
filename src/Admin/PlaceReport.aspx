<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="PlaceReport.aspx.cs" Inherits="Admin.PlaceReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script src="js/highcharts.js"></script>
	<script src="js/highcharts-more.js"></script>
	<script src="js/modules/exporting.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div style="width: 100%; text-align: left; margin-top: 20px; margin-bottom: 20px;">
        <span>資料統計區間：</span>
        從<asp:TextBox ID="txtStartDate" CssClass="easyui-datebox" Width="100" runat="server"></asp:TextBox>
        至<asp:TextBox ID="txtEndDate" CssClass="easyui-datebox" Width="100" runat="server"></asp:TextBox>
        <asp:Button ID="btnSearch" runat="server" Text="查詢" OnClick="btnSearch_Click" />
    </div>
    <asp:Panel ID="pnlTotalRatio" Visible="false" runat="server">
        <div style="text-align: left; font-size: 14pt; width: 100%; height: 30px; vertical-align: middle; color: #000000; background-color: #00ffff;">
            工作能力為普通或弱的同仁共有<asp:Label ID="lblTotalCount" runat="server" ForeColor="#ff0000" Text=""></asp:Label>人
        </div>
        <div style="text-align: left; font-size: 14pt; width: 100%; height: 30px; vertical-align: middle; color: #000000; background-color: #00ffff;">
            勾選工作環境危害因子構面的人數共有<asp:Label ID="lblThisCount" runat="server" ForeColor="#ff0000" Text=""></asp:Label>人
        </div>
        <div style="text-align: left; font-size: 14pt; width: 100%; height: 30px; vertical-align: middle; color: #000000; background-color: #00ffff;">
            選填的比率為<asp:Label ID="lblRatio" runat="server" ForeColor="#ff0000" Text=""></asp:Label>
        </div>
        <div id="myChart_Bar_1" style="width: 100%; height: 500px; margin: 10px; border: solid 1px;"></div>
        <div id="myChart_Bar_2" style="width: 100%; margin: 10px; border: solid 1px;"></div>
        <asp:Repeater ID="rptOther" runat="server">
            <HeaderTemplate>
                <ul>
            </HeaderTemplate>
            <ItemTemplate>
                    <li class="reddot-list"><%# Eval("SelectOption")%>(<%# Eval("PCount")%>)</li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
        </asp:Repeater>
        <asp:Panel ID="pnlEmpty" runat="server" Visible="false">
            <asp:Label ID="lblNoOther" runat="server" Text="工作環境沒有其他可能遭遇的危險"></asp:Label>
        </asp:Panel>
        <div id="myChart_Bar_3" style="width: 100%; margin: 10px; border: solid 1px;"></div>
        <div id="myChart_Bar_4" style="width: 100%; margin: 10px; border: solid 1px;"></div>
    </asp:Panel>
    <asp:PlaceHolder ID="phdChart" runat="server">
    </asp:PlaceHolder>
</asp:Content>

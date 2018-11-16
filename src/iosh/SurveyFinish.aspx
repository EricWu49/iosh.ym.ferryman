<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="SurveyFinish.aspx.cs" Inherits="iosh.SurveyFinish" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FormContentPlaceHolder" runat="server">
    <p class="bg-primary">
        <p class="font-size-h2">問卷已結束，感謝您</p>
        <p>本問卷旨在幫助填答者自我辨識工作環境狀況。</p>
        <p>若您是使用工作單位提供之單機版系統，您的資料將會去除可辨識身分的內容，並且將所有填答者(非個人單筆填答內容)的數據統整，提供　貴單位作為參考。</p>
    </p>
    <div style="padding: 10px; text-align: center">
        <asp:Button ID="btnReturn" CssClass="btn btn-info noprint" Width="160" runat="server" Text="返回構面選項頁" OnClick="btnReturn_Click" />
    </div>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Survey.aspx.cs" Inherits="iosh.Survey" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <style>
        input button{
            margin-right: 20px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FormContentPlaceHolder" runat="server">
    <div class="container">
        <div class="row">
            <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                <h2><asp:Literal ID="litSurveyName" runat="server"></asp:Literal></h2>
            </div>
        </div>
        <div class="row">
            <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                <p><asp:Literal ID="litInstruction" runat="server"></asp:Literal></p>
            </div>
        </div>
        <div style="float:left; margin: 0 auto;">
            <asp:Button ID="btnGo" CssClass="btn btn-primary" runat="server" Text="開始填寫" OnClick="btnGo_Click" />
            <asp:Button ID="btnQuery" CssClass="btn btn-info" runat="server" Text="查詢報告" OnClick="btnQuery_Click"  />
            <asp:Label ID="lblSurveyPage" runat="server" Text="" Visible="false"></asp:Label>
        </div>
    </div>
</asp:Content>

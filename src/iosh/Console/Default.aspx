﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Console/Console.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="iosh.Console.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:Panel ID="Panel_Login" runat="server">
    <div class="easyui-panel" title="廠護登入" style="width:100%;max-width:400px;padding:30px 60px; ">
        <div style="margin-bottom:20px; height:30px;">
            <div style="width: 25%; text-align: right; float: left;">
                <asp:Label ID="Label1" runat="server" Text="帳號："></asp:Label>
            </div>
            <div style="width: 75%; text-align: left;float: left;">
                <asp:TextBox ID="txtAccount" CssClass="easyui-textbox" runat="server" style="width:100%" data-options="required:true, missingMessage: '請輸入帳號!'"></asp:TextBox>
            </div>
        </div>
        <div style="margin-bottom:20px; height: 30px;">
            <div style="width: 25%; text-align: right;float: left;">
                <asp:Label ID="Label2" runat="server" Text="密碼："></asp:Label>
            </div>
            <div style="width: 75%; text-align: left;float: left;">
                <asp:TextBox ID="txtPassword" TextMode="Password" CssClass="easyui-textbox" runat="server" style="width:100%" data-options="required:true, missingMessage: '請輸入密碼!'"></asp:TextBox>
            </div>
        </div>
		<div style="text-align:center;padding:5px 0">
            <asp:Button ID="btnLogin" ClientIDMode="Static" CssClass="easyui-linkbutton" runat="server" Text="登入" style="width:120px; height: 30px;" OnClick="btnLogin_Click" onClientClick="javascript: return submitForm();" />
		</div>
		<div style="text-align:center;padding:5px 0">
            <a href="NewCompany.aspx" class="easyui-linkbutton" style="width:120px; height: 30px;">新公司註冊</a>
		</div>
    </div>
    </asp:Panel>
</asp:Content>
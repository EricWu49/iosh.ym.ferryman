<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="iosh.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="../jquery-easyui-1.5.5.6/themes/default/easyui.css" />
    <link rel="stylesheet" type="text/css" href="../jquery-easyui-1.5.5.6/themes/icon.css" />
    <script type="text/javascript" src="../jquery-easyui-1.5.5.6/jquery.easyui.min.js"></script>
    <script src="../js/easyui-common.js"></script>
    <style>
        .info-box {
	        position: relative;
	        padding: 0.75rem 1.25rem;
	        margin-bottom: 1rem;
	        border: 1px solid transparent;
	        border-radius: 0.25rem;
        }

        .info-text1{
            color: #004085;
            /*background-color: #cce5ff;
            border-color: #b8daff;*/
        }

        .info-text2{
            color: #721c24;
            /*background-color: #f8d7da;
            border-color: #f5c6cb;*/
        }

        .info-text3{
            color: #0c5460;
            /*background-color: #d1ecf1;
            border-color: #bee5eb;*/
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FormContentPlaceHolder" runat="server">
    <div style="width: 100%; text-align:right;">
        <a href="ReportTest.aspx"><span style="color: #ffffff; width: 50px;">診斷報告測試</span></a>
    </div>
    <asp:Panel ID="Panel_Login" runat="server">
    <div class="row">
        <div class="col-lg-8 col-lg-offset-2 col-md-10 col-md-offset-1 col-sm-12 col-xs-12">
            <p class="info-text1">請輸入您的帳號及密碼進行登入；註冊用戶將可以在本平台查詢到過去的檢測結果。</p>
            <p class="info-text3">如果您還未註冊，歡迎點擊「用戶註冊」按鈕申請一個帳號。</p>
            <p class="info-text2">如果您不想註冊，請直接點選功能選單開始使用。</p>
        </div>
        <div class="col-lg-6 col-lg-offset-3 col-md-8 col-md-offset-2 col-sm-12 col-xs-12">
            <div class="easyui-panel" title="註冊用戶登入" style="width:100%;padding:30px 60px; ">
                <div style="margin-bottom:20px; height:30px;">
                    <div style="width: 25%; text-align: right; float: left;">
                        <asp:Label ID="Label1" runat="server" Text="帳號："></asp:Label>
                    </div>
                    <div style="width: 75%; text-align: left;float: left;">
                        <asp:TextBox ID="txtAccount" CssClass="easyui-textbox" ClientIDMode="Static" runat="server" style="width:100%" data-options="required:true, missingMessage: '請輸入帳號!'"></asp:TextBox>
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
                    <a href="UserRegister.aspx" class="easyui-linkbutton" data-options="width:120, height:30" style="margin-left: 30px">用戶註冊</a>
		        </div>
            </div>
        </div>        
    </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="JScriptPlaceHolder" runat="server" >
    <asp:Literal ID="litScript" runat="server"></asp:Literal>
</asp:Content>

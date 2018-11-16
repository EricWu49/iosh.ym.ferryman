<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="UserVerify.aspx.cs" Inherits="iosh.UserVerify" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="../jquery-easyui-1.5.5.6/themes/default/easyui.css" />
    <link rel="stylesheet" type="text/css" href="../jquery-easyui-1.5.5.6/themes/icon.css" />
    <script type="text/javascript" src="../jquery-easyui-1.5.5.6/jquery.easyui.min.js"></script>
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
            background-color: #cce5ff;
            border-color: #b8daff;
        }

        .info-text2{
            color: #721c24;
            background-color: #f8d7da;
            border-color: #f5c6cb;
        }

        .info-text3{
            color: #0c5460;
            background-color: #d1ecf1;
            border-color: #bee5eb;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FormContentPlaceHolder" runat="server">
    <div id="info-ok" class="info-box info-text1" style="width:100%; text-align: center;">
        帳號已經完成啟用，您可以到首頁進行登入，以保存您的檢測報告。
    </div>
    <div id="info-ng" class="info-box info-text2" style="width:100%; text-align: center;">
        帳號啟用不成功，可能是已經啟用過，或者您還未註冊。
    </div>
    <div style="width:100%; text-align: center;">
        <a href="Default.aspx" class="easyui-linkbutton" data-options="width: 120">回首頁</a>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="JScriptPlaceHolder" runat="server">
    <asp:Literal ID="Literal1" runat="server"></asp:Literal>
</asp:Content>

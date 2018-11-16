<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="UserRegister.aspx.cs" Inherits="iosh.UserRegister" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="../jquery-easyui-1.5.5.6/themes/default/easyui.css" />
    <link rel="stylesheet" type="text/css" href="../jquery-easyui-1.5.5.6/themes/icon.css" />
    <script type="text/javascript" src="../jquery-easyui-1.5.5.6/jquery.easyui.min.js"></script>
    <script src="../js/easyui-common.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FormContentPlaceHolder" runat="server">
    <div class="row">
        <div class="col-lg-6 col-lg-offset-3 col-md-8 col-md-offset-2 col-sm-12 col-xs-12">
            <div class="easyui-panel" title="用戶註冊" style="width:100%;padding:30px 60px; ">
                <div style="margin-bottom:20px; height:30px;">
                    <div style="width: 25%; text-align: right; float: left;">
                        <asp:Label ID="Label1" runat="server" Text="Email："></asp:Label>
                    </div>
                    <div style="width: 75%; text-align: left;float: left;">
                        <asp:TextBox ID="txtAccount" CssClass="easyui-validatebox" ClientIDMode="Static" runat="server" style="width:100%" 
                            data-options="validType:'email', required:true, missingMessage: '請輸入Email!', invalidMessage: 'Email格式不正確!'"></asp:TextBox>
                        <span>Email將會是您的登入帳號</span>
                    </div>
                </div>
                 <div style="margin-bottom:20px; height:30px;">
                    <div style="width: 25%; text-align: right; float: left;">
                        <asp:Label ID="Label4" runat="server" Text="姓名："></asp:Label>
                    </div>
                    <div style="width: 75%; text-align: left;float: left;">
                        <asp:TextBox ID="txtName" CssClass="easyui-textbox" ClientIDMode="Static" runat="server" style="width:100%" data-options="required:true, missingMessage: '請輸入姓名!'"></asp:TextBox>
                    </div>
                </div>
               <div style="margin-bottom:20px; height: 30px;">
                    <div style="width: 25%; text-align: right;float: left;">
                        <asp:Label ID="Label2" runat="server" Text="密碼："></asp:Label>
                    </div>
                    <div style="width: 75%; text-align: left;float: left;">
                        <asp:TextBox ID="txtPassword" CssClass="easyui-passwordbox" ClientIDMode="Static" runat="server" style="width:100%" data-options="required:true, missingMessage: '請輸入密碼!'"></asp:TextBox>
                    </div>
                </div>
                <div style="margin-bottom:20px; height: 30px;">
                    <div style="width: 25%; text-align: right;float: left;">
                        <asp:Label ID="Label3" runat="server" Text="密碼確認："></asp:Label>
                    </div>
                    <div style="width: 75%; text-align: left;float: left;">
                        <asp:TextBox ID="txtConfirmPWD" CssClass="easyui-passwordbox" ClientIDMode="Static" runat="server" style="width:100%" data-options="required:true, missingMessage: '請再輸入一次密碼!'"></asp:TextBox>
                        <span>請再輸入一次密碼確認沒有輸入錯誤。</span>
                    </div>
                </div>
		        <div style="text-align:center;padding:5px 0">
                    <asp:Button ID="btnRegister" ClientIDMode="Static" CssClass="easyui-linkbutton" runat="server" Text="註冊" style="width:120px; height: 30px;" OnClick="btnLogin_Click" onClientClick="javascript: return register_check();" />
		        </div>
            </div>
        </div>        
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="JScriptPlaceHolder" runat="server">
    <script type="text/javascript">
        function register_check() {
            if ($("#txtPassword").passwordbox("getValue")==$("#txtConfirmPWD").passwordbox("getValue"))
            {
                return submitForm();
            }
            else
            {
                window.alert("密碼與密碼確認不一致!");
                return false;
            }
        }
    </script>
</asp:Content>

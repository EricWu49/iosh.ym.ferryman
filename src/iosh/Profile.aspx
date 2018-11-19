<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="iosh.Profile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="css/jquery-ui.min.css" />
    <script type="text/javascript" src="js/jquery-ui.min.js"></script>
    <script type="text/javascript" src="js/form2json.js"></script>
    <script type="text/javascript" src="js/jquery.validate.min.js"></script>
    <script type="text/javascript" src="js/additional-methods.min.js"></script>
    <script type="text/javascript" src="js/messages_zh_TW.min.js"></script>
    <script type="text/javascript" src="js/dotnet.js"></script>
    <link rel="stylesheet" type="text/css" href="css/profile.css"/>
    <style type="text/css">
        .option-item label
        {
            margin-right: 20px;
        }

        .error
        {
            color: #ff0000;
            font-weight: bold;
            font-style:italic;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FormContentPlaceHolder" runat="server">
   	<div class="title1 row">
            <div class=" col-xs-12 col-sm-12 col-md-12 col-lg-12">
                <h2>個人基本資料</h2>
            </div>
    </div>
    <img class="pofile_img" src="images/profile_resume_img.svg" />
    <div class="row">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="table-responsive">
                <asp:Table ID="fmPageTable" ClientIDMode="Static" runat="server" CssClass="profile_table col-md-11 col-md-offset-1">
                </asp:Table>
                <asp:HiddenField ID="hidResult" ClientIDMode="Static" runat="server" />
            </div>
        </div>
    </div>
<%--    <asp:HiddenField ID="hidProgress" ClientIDMode="Static" runat="server" />--%>
    <div class="row" style="margin-top: 10px;">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="text-align:center;">
            <div class="btn-next">
                <asp:Button ID="btnSubmit" CssClass="btn btn-primary col-md-3 col-sm-6 col-xs-12" ClientIDMode="Static" runat="server" Text="下一頁" OnClick="btnSubmit_Click" />
            </div>
        </div>
    </div>
            <asp:Label ID="lblSurveyPage" runat="server" Text="" Visible="false"></asp:Label>
    <script type="text/javascript" >
        <asp:Literal id="litValidator" runat="server"></asp:Literal>

        $("#form1").submit(function () {
            //window.aler($("form").valid());
            //window.alert($("#form1").valid());
            if ($("#form1").valid()) {
                <asp:Literal id="litScript" runat="server"></asp:Literal>
                $('#hidResult').val(JSON.stringify($('#form1').serializeObject()));
                //window.alert("驗證成功。");
            } else {
                window.alert("資料不完整，請檢查。");
            };
        });

        function relative_check(target, require)
        {
            if (require)
            {
                $("."+target).removeAttr("disabled");
            }
            else
            {
                $("."+target).attr("disabled", "disabled");
            }
        }
    </script>
</asp:Content>

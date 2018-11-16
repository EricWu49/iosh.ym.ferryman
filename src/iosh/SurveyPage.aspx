<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="SurveyPage.aspx.cs" Inherits="iosh.SurveyPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="css/jquery-ui.min.css" />
    <script type="text/javascript" src="js/jquery-ui.min.js"></script>
    <script type="text/javascript" src="js/form2json.js"></script>
    <script type="text/javascript" src="js/jquery.validate.min.js"></script>
    <script type="text/javascript" src="js/additional-methods.min.js"></script>
    <script type="text/javascript" src="js/messages_zh_TW.min.js"></script>
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
    <script type="text/javascript">
        $( function() {
            $("#progressbar").progressbar({
                value: $('#hidProgress').val()
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FormContentPlaceHolder" runat="server">
<%--    <div class="row" style="margin-top: 10px;">
        <div class="col-lg-4 col-lg-offset-8 col-md-6 col-md-offset-6 col-sm-12 col-xs-12">
            <div style="width: 100%; float:right;">
                填寫進度：<div id="progressbar" style="width: 50%; float:right;"></div>
            </div>
        </div>
    </div>--%>
    <div class="row" style="margin-top: 10px;">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="table-responsive">
                <asp:Table ID="fmPageTable" ClientIDMode="Static" runat="server">
                </asp:Table>
                <asp:HiddenField ID="hidResult" ClientIDMode="Static" runat="server" />
            </div>
        </div>
    </div>
    <asp:HiddenField ID="hidProgress" ClientIDMode="Static" runat="server" />
    <div class="row" style="margin-top: 10px;">
        <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6" style="text-align:center;">
            <button type="button" class="btn btn-success" style="width: 200px; height: 30px;" onclick="javascript: history.go(-1);">上一頁</button>
        </div>
        <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6" style="text-align:center;">
            <asp:Button ID="btnSubmit" CssClass="btn btn-primary" Width="200" Height="30" ClientIDMode="Static" runat="server" Text="下一頁" OnClick="btnSubmit_Click" />
        </div>
    </div>
    <script type="text/javascript" >
        //$("#btnSubmit").click(function () {
        //        window.aler($("form").valid());
        //        window.alert($("#form1").valid());
        //    if ($("#form1").valid()) {
        //        $('#hidResult').val(JSON.stringify($('form').serializeObject()));
        //    } else return false;
        //})
        //$("#form1").validate();

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

        function others_check(target, require)
        {
            if (require)
            {
                $("#"+target).removeAttr("disabled");
            }
            else
            {
                $("#"+target).attr("disabled", "disabled");
            }
        }

        function show_related(target, show)
        {
            if (show)
            {
                $("."+target).show();
            }
            else
            {
                $("."+target).hide();
            }
        }
    </script>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="SurveyResult.aspx.cs" Inherits="iosh.SurveyResult" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="themes/default/easyui.css" />
    <link rel="stylesheet" type="text/css" href="themes/icon.css" />
    <script src="js/easyui-common.js"></script>
    <script type="text/javascript" src="js/jquery.easyui.min.js"></script>
	<script src="js/highcharts.js"></script>
	<script src="js/highcharts-more.js"></script>
    <style type="text/css" media="print">
        .noprint
        {
            display: none;
        }

        #btnClose
        {
            margin-left: 30px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FormContentPlaceHolder" runat="server">
    <div class="row" style="margin-top: 10px;">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="page-header"><h3>親愛的朋友，感謝您填寫完工作適能量表，您的評定結果如下：</h3></div>
        </div>
    </div>
    <div class="row" style="margin-top: 10px;">
        <div class="col-lg-2 col-md-4 col-sm-8 col-xs-10">
            <asp:Label ID="Label1" runat="server" Text="工作適能指數："></asp:Label>
        </div>
        <div class="col-lg-10 col-md-8 col-sm-4 col-xs-2">
            <asp:Label ID="lblIndex" runat="server" Text=""></asp:Label>
        </div>
    </div>
    <div class="row" style="margin-top: 10px;">
        <div class="col-lg-2 col-md-4 col-sm-8 col-xs-10">
            <asp:Label ID="Label2" runat="server" Text="工作適能等級："></asp:Label>
        </div>
        <div class="col-lg-10 col-md-8 col-sm-4 col-xs-2">
            <asp:Label ID="lblLevel" runat="server" Text=""></asp:Label>
        </div>
    </div>
    <asp:Panel ID="pnlWarning" runat="server">
        <div class="row" style="margin-top: 10px;">
            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                <div class='alert alert-warning' role='alert'>您的工作適能等級評定結果偏低，建議您可以前往「<a href="#action">個人構面分析</a>」做進一步的診斷。</div>
            </div>
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlInfo" runat="server">
        <div class="row" style="margin-top: 10px;">
            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                <div class='alert alert-success' role='alert'>
                    恭喜您！目前您的工作適能指數相當良好，代表您對於目前的工作相當得心應手！<br />
                    此外，為了幫助您持續保持良好的工作適能以及預防未來可能出現會影響您工作適能的情況，您可以接續填寫後續的問卷，在更多構面上自我評估，本系統也將提供您相關的健康管理建議。<br />
                    前往「<a href="#action">個人構面分析</a>」
                </div>
            </div>
        </div>
    </asp:Panel>
    <div class="row" style="margin-top: 10px;">
        <div class="col-lg-6 col-md-12 col-sm-12 col-xs-12">
            <div id="myChart_Radar" style="min-width: 400px; max-width: 600px; height: 300px; margin: 0 auto"></div>
        </div>
        <div class="col-lg-6 col-md-12 col-sm-12 col-xs-12">
            <div id="myChart_Bar" style="min-width: 400px; max-width: 600px; height: 300px; margin: 0 auto"></div>
        </div>
    </div>

    <div style="padding: 10px; text-align: center">
        <a class="btn btn-info noprint" href="javascript: doprint();">列印工作適能指數報告</a>
    </div>

    <asp:Panel ID="pnlAction" runat="server">
        <div class="row noprint" style="margin-top: 10px;">
            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                <div class='alert alert-info' role='alert'><a name="action">個人構面分析</a></div>
            </div>
        </div>
        <div class="row noprint" style="margin-top: 10px;">
            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
               <p>根據研究指出，影響個人工作適能有下列許多因素；改善這些因素有助提升或維持個人工作適能。</p>
                <p>請您選擇可能影響您工作適能指數的因素，六項都可以選，可以複選，然後進入填答。系統將針對您各項因素填答的情況，提供您分析結果與相關建議。</p>
                <p>※ 若您是進入線上工作適能指數平台進行填答，內容僅供系統做大量資料統計。</p>
                <p>※ 若您是使用工作單位提供之單機版系統，亦為不記名填寫，填答的內容也會消除任何可辨識個人身份的資訊，因此無需擔心工作單位會得知您個人的填答狀況，貴單位可輸出之統計資料，僅包括所有填寫者的統整資料，而非個人單筆填答內容，請您安心作答。</p>
            </div>
        </div>
        <div class="row noprint" style="margin-top: 10px;">
            <div class="col-lg-2 col-md-3 col-sm-4 col-xs-6" style="margin: 5px;">
                <asp:Button ID="btnLife" CssClass="btn btn-default" Width="160" CommandArgument="3" runat="server" Text="個人生活狀況" OnClick="Action_Click" />
            </div>
            <div class="col-lg-2 col-md-3 col-sm-4 col-xs-6" style="margin: 5px;">
                <asp:Button ID="btnPlace" CssClass="btn btn-default" Width="160" CommandArgument="5" runat="server" Text="工作環境危害因子分析" OnClick="Action_Click" />
            </div>
            <div class="col-lg-2 col-md-3 col-sm-4 col-xs-6" style="margin: 5px;">
                <asp:Button ID="btnHuman" CssClass="btn btn-default" Width="160" CommandArgument="6" runat="server" Text="人因簡易檢核表" OnClick="Action_Click" />
            </div>
            <div class="col-lg-2 col-md-3 col-sm-4 col-xs-6" style="margin: 5px;">
                <asp:Button ID="btnHealth" CssClass="btn btn-default" Width="160" CommandArgument="4" runat="server" Text="個人健康狀況" OnClick="Action_Click" />
            </div>
            <div class="col-lg-2 col-md-3 col-sm-4 col-xs-6" style="margin: 5px;">
                <asp:Button ID="btnBody" CssClass="btn btn-default" Width="160" CommandArgument="10" runat="server" Text="自覺肌肉骨骼症狀評估" OnClick="Action_Click" />
            </div>
            <div class="col-lg-2 col-md-3 col-sm-4 col-xs-6" style="margin: 5px;">
                <asp:Button ID="btnPressure" CssClass="btn btn-default" Width="160" CommandArgument="8" runat="server" Text="工作壓力檢測" OnClick="Action_Click" />
            </div>
        </div>
    </asp:Panel>

    <script type="text/javascript">
        function doprint()
        {
            window.print();
            //return false;
        }
    </script>
</asp:Content>

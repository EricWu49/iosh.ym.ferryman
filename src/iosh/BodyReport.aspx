<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="BodyReport.aspx.cs" Inherits="iosh.BodyReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="css/jquery-ui.min.css" />
    <script type="text/javascript" src="js/jquery-ui.min.js"></script>
	<script src="js/highcharts.js"></script>
	<script src="js/highcharts-more.js"></script>
    <style type="text/css" media="print">
        .noprint
        {
            display: none;
        }
    </style>
    <style type="text/css">
        .level1
        {
            list-style-image:url("images/blue.png");
        }
        .level2
        {
            list-style-image:url("images/green.png");
        }
        .level3
        {
            list-style-image:url("images/yellow.png");
        }
        .level4
        {
            list-style-image:url("images/red.png");
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FormContentPlaceHolder" runat="server">
        <div class="title1 row">
            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                <div>
                  <h2>自評結果</h2>
                </div>
            </div>
        </div>
    <div class="row" style="margin-top: 10px;">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="page-header"><h3>親愛的朋友，感謝您填寫完肌肉骨骼健康評估，您的評估結果如下：</h3></div>
        </div>
    </div>
    <div class="row" style="margin-top: 10px;">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <asp:Label ID="Label1" runat="server" Text="肌肉骨骼健康風險值："></asp:Label>
            <asp:Label ID="lblIndex" runat="server" Text=""></asp:Label>
            <asp:HiddenField ID="hidRiskID" ClientIDMode="Static" runat="server" />
        </div>
    </div>
    <asp:Panel ID="pnlPaperSN" runat="server">
        <div class="row" style="margin-top: 10px;">
            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                <div>評估報告序號：<b><asp:Label ID="lblPaperSN" runat="server" Text=""></asp:Label></b></div>
                <div>請記住您的報告序號，將序號給予您的護理師或物理治療師更進一步協助。</div>
            </div>
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlInfo" runat="server">
        <div class="row" style="margin-top: 10px;">
            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                <div id="info1" class='alert alert-success' role='alert' style="display:none;">
                    恭喜您目前沒有肌肉骨骼症狀，但是還是要繼續維持良好的運動習慣。我們提供給您以下的辦公室運動影片，可以搭配進行，維持良好的肌肉骨骼健康。<br />
                    若肌肉骨骼症狀加重，建議尋求物理治療師/職業醫護人員進行協助。
                </div>
                <div id="info2" class='alert alert-info' role='alert' style="display:none;">
                    建議根據您肌肉骨骼不適之部位，觀看我們所提供之肌肉骨骼自我運動衛教影片。<br />
                    若肌肉骨骼症狀加重，建議尋求物理治療師/職業醫護人員進行協助
                </div>
                <div id="info3" class='alert alert-warning' role='alert' style="display:none;">
                    建議尋找物理治療師進行協助，由物理治療師進行進階動作控制篩檢。
                </div>
                <div id="info4" class='alert alert-danger' role='alert' style="display:none;">
                    建議尋找物理治療師進行協助，建議由物理治療師進行進階動作控制篩檢，並且遵循復工指引逐漸重回職場。
                </div>
                <div id="info5" class='alert alert-info' role='alert' style="display:none;">
                    恭喜您目前沒有肌肉骨骼症狀，但是還是要繼續維持良好的運動習慣。我們提供給您以下的辦公室運動影片，可以搭配進行，維持良好的肌肉骨骼健康。<br />
                    若肌肉骨骼症狀加重，建議尋求物理治療師/職業醫護人員進行協助。
                </div>
                <div id="info6" class='alert alert-warning' role='alert' style="display:none;">
                    建議您尋求物理治療師/職業醫護人員進行基礎動作篩檢測試。<br />
                    在此之前建議根據您肌肉骨骼不適之部位，觀看我們所提供之肌肉骨骼自我運動衛教影片。
                </div>
            </div>
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlChart" runat="server">
        <div class="row" style="margin-top: 10px;">
            <div class="col-lg-6 col-md-12 col-sm-12 col-xs-12">
                <div id="myChart_Radar" style="min-width: 400px; max-width: 600px; height: 300px; margin: 0 auto"></div>
            </div>
            <div class="col-lg-6 col-md-12 col-sm-12 col-xs-12">
                <asp:Literal ID="litResultList" runat="server"></asp:Literal>
            </div>
        </div>
    </asp:Panel>
    <asp:PlaceHolder ID="PlaceHolder1" runat="server">
        <div id="tabs">
            <asp:Repeater ID="Repeater1" runat="server">
                <HeaderTemplate>
                    <ul>
                </HeaderTemplate>
                <ItemTemplate>
                        <li><a href='<%#  Eval("ResourceCode", "#tab-{0}") %>'><%# Eval("QTitle") %></a></li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Panel ID="Panel1" runat="server"></asp:Panel>
        </div>
    </asp:PlaceHolder>
    <div style="padding: 10px; text-align: center" class="noprint">
        <div class="col-lg-3 col-lg-offset-3 col-md-4 col-md-offset-2 col-sm-6 col-xs-6" style="text-align: center;">
            <a class="btn btn-info noprint" style="width: 160px;" href="javascript: doprint();">列印評估報告</a>
        </div>
        <div class="col-lg-3 col-md-4 col-sm-6 col-xs-6" style="text-align: center;">
            <asp:Button ID="btnReturn" CssClass="btn btn-info noprint" Width="160" runat="server" Text="返回" OnClick="btnReturn_Click" />
        </div>
    </div>
    <!-- Youtube 影片播放器 -->
<div tabindex="1" class="modal fade" id="exampleModal" role="dialog" aria-labelledby="exampleModalLabel" style="display: none;">
    <div class="modal-dialog modal-lg" role="document">
        <div class="modal-content">
            <div class="modal-body" style="height: 350px;">
                <div id="player"></div>
            </div>
            <div class="modal-footer">
                <button class="btn btn-default" id="btnClose" type="button" data-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
<script id="www-widgetapi-script" src="https://s.ytimg.com/yts/jsbin/www-widgetapi-vfljsDGBQ/www-widgetapi.js" type="text/javascript" async=""></script>
<script src="https://www.youtube.com/iframe_api"></script>
    <!-- Youtube 影片播放器 -->
    <script type="text/javascript">
        $(function () {
            $("#info" + $("#hidRiskID").val()).css("display", "block");

            $("#tabs").tabs({
                beforeLoad: function (event, ui) {
                    ui.jqXHR.fail(function () {
                        ui.panel.html("無法載入結果。");
                    });
                }
            });

            // Youtube影片播放控制
            $('#btnClose').click(function () {
                //stopVideo();
                player.stopVideo();
                //$('#youtube1').each(function () { this.player.pause() })
            });

            //$('.panel').on('show.bs.collapse', function (e) {
            //    // alert('Event fired on #' + e.currentTarget.id);
            //})

            //$('.modal').on('show.bs.modal', function (e) {
            //    //alert('Event modal on #' + e.currentTarget.id);
            //})
        })

        function GetVedio(vName) {
            if (player.loadVideoById == undefined) {
                $('#exampleModal').modal('toggle')
                return false;
            }

            player.loadVideoById(vName, 0, "default");
        }

        // 2. This code loads the IFrame Player API code asynchronously.
        var tag = document.createElement('script');

        tag.src = "https://www.youtube.com/iframe_api";
        var firstScriptTag = document.getElementsByTagName('script')[0];
        firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);

        // 3. This function creates an <iframe> (and YouTube player)
        //    after the API code downloads.
        var player;

        function onYouTubeIframeAPIReady() {
            player = new YT.Player('player', {
                height: '100%',
                width: '100%',
                //videoId: '',//iJTYEYNZUXo
                playerVars: {
                    'autoplay': 0,
                    'controls': 1,
                    'rel': 0
                },
                events: {
                    'onReady': onPlayerReady,
                    'onStateChange': onPlayerStateChange
                }
            });
        }

        var playerReady = false;
        // 4. The API will call this function when the video player is ready.
        function onPlayerReady(event) {
            playerReady = true;
        }

        // 5. The API calls this function when the player's state changes.
        //    The function indicates that when playing a video (state=1),
        //    the player should play for six seconds and then stop.
        function onPlayerStateChange(event) {
            if (event.data == YT.PlayerState.ENDED) {
                //Console.log("done");
            }
        }

        function doprint() {
            window.print();
        }
    </script>
</asp:Content>

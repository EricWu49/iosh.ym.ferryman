<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="BodyFinish.aspx.cs" Inherits="iosh.BodyFinish" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="css/jquery-ui.min.css" />
    <script type="text/javascript" src="js/jquery-ui.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FormContentPlaceHolder" runat="server">
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
    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible="false">
        <div style='width: 100%; text-align: center; height: 40px; margin: 10px;' class='bg-primary '><h2 style='color: #ffffff;'>恭喜您，您的肌肉骨骼沒有痠痛問題。</h2></div>
    </asp:PlaceHolder>
    <div style="padding: 10px; text-align: center">
            <asp:Button ID="btnReturn" CssClass="btn btn-info noprint" Width="160" runat="server" Text="返回" OnClick="btnReturn_Click" />
    </div>
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

<script>
        $(function () {
            $("#tabs").tabs({
                beforeLoad: function (event, ui) {
                    ui.jqXHR.fail(function () {
                        ui.panel.html("無法載入所需要的衛教資源。");
                    });
                }
            });

            $('#btnClose').click(function () {
                //stopVideo();
                player.stopVideo();
                //$('#youtube1').each(function () { this.player.pause() })
            });

            $('.panel').on('show.bs.collapse', function (e) {
                // alert('Event fired on #' + e.currentTarget.id);
            })

            $('.modal').on('show.bs.modal', function (e) {
                //alert('Event modal on #' + e.currentTarget.id);
            })
        });

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

    </script>
</asp:Content>

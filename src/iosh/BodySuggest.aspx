<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="BodySuggest.aspx.cs" Inherits="iosh.BodySuggest" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="css/jquery-ui.min.css" />
    <script type="text/javascript" src="js/jquery-ui.min.js"></script>
    <style>
        /* 讓每一個網格的高度相同 */
        .row-flex {
            display: -webkit-box;
            display: -webkit-flex;
            display: -ms-flexbox;
            display: flex;
            flex-wrap: wrap;
        }

        .row-flex > [class*='col-'] {
            display: flex;
            flex-direction: column;
        }

        .row > .card-flex {
            display: flex;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FormContentPlaceHolder" runat="server">
    <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
        <HeaderTemplate>
            <div class="panel-group" id="accordion" role="tablist" aria-multiselectable="true">
        </HeaderTemplate>
        <ItemTemplate>
            <div class="panel panel-default">
                <div class="panel-heading" role="tab" id='<%#  Eval("EvaluateCode", "heading-{0}") %>'>
                    <h4 class="panel-title">
                        <a role="button" data-toggle="collapse" data-parent="#accordion" href='<%#  Eval("EvaluateCode", "#collapse-{0}") %>' aria-expanded="true" aria-controls='<%#  Eval("EvaluateCode", "#collapse-{0}") %>'>
                            <%# Eval("QTitle") %>
                        </a>
                    </h4>
                </div>
                <div id='<%#  Eval("EvaluateCode", "collapse-{0}") %>' class="panel-collapse collapse in" role="tabpanel" aria-labelledby='<%#  Eval("EvaluateCode", "#heading-{0}") %>'>
                    <div class="panel-body">
                        <asp:PlaceHolder ID="PlaceResult" runat="server"></asp:PlaceHolder>
                    </div>
                </div>
            </div>
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </asp:Repeater>

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
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="JScriptPlaceHolder" runat="server">
    <!-- Youtube播放所需Script -->
    <script id="www-widgetapi-script" src="https://s.ytimg.com/yts/jsbin/www-widgetapi-vfljsDGBQ/www-widgetapi.js" type="text/javascript" async=""></script>
    <script src="https://www.youtube.com/iframe_api"></script>
    
    <!-- Line加好友所需Script -->
    <%--<script src="https://d.line-scdn.net/r/web/social-plugin/js/thirdparty/loader.min.js" async="async" defer="defer"></script>--%>
    
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

        function GetVideo(vName) {
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

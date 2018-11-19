<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Resource.aspx.cs" Inherits="iosh.Resource" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
<%--    <link rel="stylesheet" type="text/css" href="themes/default/easyui.css" />
    <link rel="stylesheet" type="text/css" href="themes/icon.css" />--%>
    <link rel="stylesheet" type="text/css" href="css/resource.css" />
    <script type="text/javascript" src="js/jquery.easyui.min.js"></script>
<%--    <script src="js/easyui-common.js"></script>--%>
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
    <div class="title1 row">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div>
                <h2>肌肉骨骼保健運動影片查詢</h2>
            </div>
        </div>
    </div>
    <div style="font-size: 17px;">
        <div class="row">
            <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                        <div class="resource_searchbar easyui-panel panel-body" title="" >
                            <div class="row">
                                <div class="col-md-4 col-sm-4">
                                    <label>身體部位：</label>
                                    <asp:DropDownList ID="ddlBody" ClientIDMode="Static" CssClass="combobox-f combo-f textbox-f" runat="server"></asp:DropDownList>
                                </div>
                                <div class="col-md-5 col-sm-4">
                                    <label>關鍵字：</label>
                                    <span class="textbox" >
                                    <asp:TextBox ID="txtKeyword" ClientIDMode="Static" CssClass="textbox-text validatebox-text textbox-prompt" runat="server"></asp:TextBox>
                                    </span>
                                </div>
                                <div class="col-md-3 col-sm-4">
                                    <asp:Button ID="btnQuery" CssClass="search_btn easyui-linkbutton l-btn l-btn-small" runat="server" Text="查詢" OnClick="btnQuery_Click" />
                                </div>
                            </div>
                            
                    </div>
            </div>
        </div>
        <asp:HiddenField ID="hidSetID" ClientIDMode="Static" Value="" runat="server" />
        <div class="box1" style="margin-top: 5px;">
            <div class="resource_select row">
                <div class="col-md-1-5">
                    <button onclick="$('#hidSetID').val('1');">頭頸保健操</button>
                </div>
                <div class="col-md-1-5">
                    <button onclick="$('#hidSetID').val('2');">肩膀與上臂保健操</button>
                </div>
                <div class="col-md-1-5">
                    <button onclick="$('#hidSetID').val('3');">脊椎保健操</button>
                </div>
                <div class="col-md-1-5">
                    <button onclick="$('#hidSetID').val('4');">腿部保健操</button>
                </div>
                <div class="col-md-1-5">
                    <button onclick="$('#hidSetID').val('5');">全身保健操</button>
                </div>
            </div>
        </div>
        <br />
        <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
            <HeaderTemplate>
                <div class="row">
                    <div class="title3 col-xs-12">
                        <asp:Label ID="lblDescription" runat="server" Text=""></asp:Label>
                    </div>
                </div>
                <div class="row row-flex">
            </HeaderTemplate>
            <ItemTemplate>
                <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
                    <div class="card bg-info" style="height: 100%; margin-bottom: 5px;">
                        <div class="card-body">
                            <div class="card-title text-center">
                                <a class="btn" onclick='<%#  Eval("YoutubeID", "GetVideo(\"{0}\")") %>' href="#exampleModal" data-toggle="modal" role="button">
                                    <img src='https://i.ytimg.com/vi/<%#  Eval("YoutubeID") %>/default.jpg' alt='<%#  Eval("VideoName") %>' />
                                </a>
                            </div>
                            <p class="card-text card-flex">
                                <%#  Eval("Description") %>
                            </p>
                            <div class="card-fotter text-center">
                            </div>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
            <FooterTemplate>
                </div>
            </FooterTemplate>
        </asp:Repeater>
        <!-- Line Start -->
        <div class="row">
            <div class="col-sm-12 col-xs-12 col-md-12 col-lg-12">
                <p class="bg-success text-info">
                    <div class="qrcode"><img src="images/LineId.png" alt="加入好友"/></div>
                    <div class="add_line">
                        <a href="https://line.me/R/ti/p/%40ohr2012s">
                            <img height="40" border="0" alt="加入好友" src="https://scdn.line-apps.com/n/line_add_friends/btn/zh-Hant.png"/>
                        </a>
                        <div>將本網站的Line官方帳號加入您的好友後，就可以把查詢到的影片傳送到您的Line帳號中保存。</div>
                    </div>
                </p>
                <%--<div class="line-it-button" style="display: none;" data-lang="zh_Hant" data-type="friend" data-lineId="@ohr2012s"></div>--%>
            </div>
        </div>
        <!-- Line End -->
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

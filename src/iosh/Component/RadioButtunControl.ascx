<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RadioButtunControl.ascx.cs" Inherits="iosh.Component.RadioButtunControl" %>
<div class="row">
    <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
        <asp:Label ID="lblQuestion" runat="server" Text=""></asp:Label>
        <asp:Label ID="lblQID" runat="server" Text="" Visible="false"></asp:Label>
    </div>
    <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
        <ItemTemplate>
            <div class="col-lg-3 col-md-4 col-sm-6 col-xs-12">
                <asp:RadioButton ID="rdbAnswer" runat="server" GroupName="answer-group" Width="200px" Text='<%# Eval("SelectOption")%>' value='<%# Eval("OptionID")%>' OptionID='<%# Eval("OptionID")%>' />
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>

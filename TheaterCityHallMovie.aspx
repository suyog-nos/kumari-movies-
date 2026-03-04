<%@ Page Title="Theater City Hall Movie Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TheaterCityHallMovie.aspx.cs" Inherits="kumari.TheaterCityHallMovie" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .alert { padding: 8px 12px; border-radius: 4px; margin-bottom: 16px; }
        .alert-success { color: #155724; background-color: #d4edda; }
        .alert-danger { color: #721c24; background-color: #f8d7da; }
    </style>

    <div class="container">
        <h2 class="page-heading">Theater City Hall Movie</h2>
        <asp:Label ID="lblMessage" runat="server" CssClass="d-block mb-2" />

        <div class="row">

            <div class="col-md-6">

                <div class="form-group">

                    <label for="ddlCity">City:</label>

                    <asp:DropDownList ID="ddlCity" runat="server" CssClass="form-control"></asp:DropDownList>

                </div>

            </div>

            <div class="col-md-6">

                <div class="form-group">

                    <label>&nbsp;</label><br />

                    <asp:Button ID="btnLoadReport" runat="server" Text="Load Report" CssClass="btn btn-primary" OnClick="btnLoadReport_Click" />

                </div>

            </div>

        </div>

        <asp:GridView ID="gvReport" runat="server" AutoGenerateColumns="False" CssClass="table table-striped">

            <Columns>

                <asp:BoundField DataField="theater_name" HeaderText="Theater Name" />

                <asp:BoundField DataField="theater_city" HeaderText="Theater City" />

                <asp:BoundField DataField="hall_name" HeaderText="Hall Name" />

                <asp:BoundField DataField="movie_title" HeaderText="Movie Title" />

                <asp:BoundField DataField="show_start" HeaderText="Show Start" />

                <asp:BoundField DataField="total_shows" HeaderText="Total Shows Per Hall" />

            </Columns>

        </asp:GridView>

    </div>

</asp:Content>

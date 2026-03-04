<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MovieOccupancyReport.aspx.cs" Inherits="kumari.MovieOccupancyReport" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        /* Page spacing  */
        .movie-occupancy-page {
            max-width: 1200px;
            margin: 20px auto;
            padding: 20px 24px;
        }

        /* GridView spacing */
        .occupancy-grid.table th,
        .occupancy-grid.table td {
            padding: 12px 14px !important;
            text-align: center;
            vertical-align: middle;
        }

        .occupancy-grid thead th {
            background-color: #f7f7f7;
            border-bottom: 2px solid #dee2e6;
            white-space: nowrap;
        }

        .occupancy-grid tbody tr:hover {
            background-color: #f5f5f5;
        }

        .movie-occupancy-page h2 {
            margin-bottom: 16px;
            font-weight: 600;
        }
    </style>

    <div class="movie-occupancy-page">
        <h2 class="page-heading">Movie Occupancy Report</h2>

        <asp:GridView 
            ID="gvOccupancy" 
            runat="server" 
            AutoGenerateColumns="False" 
            CssClass="table table-striped occupancy-grid">

            <Columns>
                <asp:BoundField DataField="movie_title" HeaderText="Movie Title" />
                <asp:BoundField DataField="theater_name" HeaderText="Theater Name" />
                <asp:BoundField DataField="hall_name" HeaderText="Hall Name" />
                <asp:BoundField DataField="hall_capacity" HeaderText="Hall Capacity" />
                <asp:BoundField DataField="total_shows" HeaderText="Total Shows" />
                <asp:BoundField DataField="paid_tickets" HeaderText="Paid Tickets" />
                <asp:BoundField DataField="occupancy_percentage" HeaderText="Occupancy Percentage (%)" DataFormatString="{0:0.##}" />
            </Columns>
        </asp:GridView>
    </div>

</asp:Content>
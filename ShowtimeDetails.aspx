<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowtimeDetails.aspx.cs" Inherits="kumari.ShowtimeDetails" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        /* Page spacing */
        .showtime-page {
            max-width: 1200px;
            margin: 20px auto;
            padding: 20px 24px;
        }

        /*  GridView table padding */
        .showtime-grid.table th,
        .showtime-grid.table td {
            padding: 12px 14px !important;
            vertical-align: middle;
        }

        .showtime-grid thead th {
            background-color: #f7f7f7;
            border-bottom: 2px solid #dee2e6;
            white-space: nowrap;
        }

        .showtime-grid tbody tr:hover {
            background-color: #f5f5f5;
        }

        .showtime-grid td:first-child {
            text-align: center;
            width: 110px;
        }

        .showtime-grid td:last-child {
            text-align: center;
            font-weight: 500;
        }

        /* Form spacing */
        .showtime-form {
            margin-top: 24px;
        }

        .showtime-form .form-label {
            font-weight: 600;
            margin-bottom: 4px;
        }

        .showtime-actions {
            margin-top: 16px;
            display: flex;
            gap: 10px;
        }
    </style>

    <div class="showtime-page">
        <h2 class="page-heading">Showtime Management</h2>

        <asp:GridView 
            ID="gvShowtimes" 
            runat="server" 
            AutoGenerateColumns="False" 
            DataKeyNames="showtime_id" 
            CssClass="table table-striped showtime-grid"
            OnSelectedIndexChanged="gvShowtimes_SelectedIndexChanged">
            <Columns>
                <asp:BoundField DataField="showtime_id" HeaderText="Showtime ID" />
                <asp:BoundField DataField="movie_title" HeaderText="Movie Title" />
                <asp:BoundField DataField="hall_name" HeaderText="Hall Name" />
                <asp:BoundField DataField="show_start" HeaderText="Show Start" DataFormatString="{0:MM/dd/yyyy hh:mm tt}" />
                <asp:CommandField ShowSelectButton="True" SelectText="Select" />
            </Columns>
        </asp:GridView>

        <div class="row showtime-form mt-4">
            <div class="col-md-3">
                <label class="form-label" for="txtShowtimeId">Showtime ID</label>
                <asp:TextBox ID="txtShowtimeId" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-3">
                <label class="form-label" for="ddlMovie">Movie</label>
                <asp:DropDownList ID="ddlMovie" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-3">
                <label class="form-label" for="ddlHall">Hall</label>
                <asp:DropDownList ID="ddlHall" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-3">
                <label class="form-label" for="txtShowStart">Show Start</label>
                <asp:TextBox ID="txtShowStart" runat="server" CssClass="form-control" TextMode="DateTimeLocal" />
            </div>
        </div>

        <div class="showtime-actions">
            <asp:Button ID="btnInsert" Text="Insert" runat="server" CssClass="btn btn-primary" OnClick="btnInsert_Click" />
            <asp:Button ID="btnUpdate" Text="Update" runat="server" CssClass="btn btn-warning" OnClick="btnUpdate_Click" />
            <asp:Button ID="btnDelete" Text="Delete" runat="server" CssClass="btn btn-danger" OnClick="btnDelete_Click" />
            <asp:Button ID="btnClear" Text="Clear" runat="server" CssClass="btn btn-secondary" OnClick="btnClear_Click" />
        </div>
    </div>

</asp:Content>

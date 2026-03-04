<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserTicketReport.aspx.cs" Inherits="kumari.UserTicketReport" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .report-page {
            max-width: 1200px;
            margin: 20px auto;
            padding: 20px 24px;
        }

        .report-grid.table th,
        .report-grid.table td {
            padding: 12px 14px !important;
            vertical-align: middle;
        }

        .report-grid thead th {
            background-color: #f7f7f7;
            border-bottom: 2px solid #dee2e6;
            white-space: nowrap;
        }

        .report-grid tbody tr:hover {
            background-color: #f5f5f5;
        }

        .filter-card {
            border: 1px solid #e2e2e2;
            border-radius: 8px;
            padding: 16px 18px;
            margin-bottom: 20px;
            background-color: #fafafa;
        }

        .alert { padding: 8px 12px; border-radius: 4px; margin-bottom: 16px; }
        .alert-success { color: #155724; background-color: #d4edda; }
        .alert-danger { color: #721c24; background-color: #f8d7da; }
    </style>

    <div class="report-page">
        <h2 class="page-heading">User Ticket Report</h2>
        <asp:Label ID="lblMessage" runat="server" CssClass="d-block mb-2" />

        <div class="filter-card">
            <div class="row">
                <div class="col-md-3">
                    <label>User</label>
                    <asp:DropDownList ID="ddlUser" CssClass="form-control" runat="server" />
                </div>
                <div class="col-md-3">
                    <label>From Date</label>
                    <asp:TextBox ID="txtFromDate" CssClass="form-control" TextMode="Date" runat="server" />
                </div>
                <div class="col-md-3">
                    <label>To Date</label>
                    <asp:TextBox ID="txtToDate" CssClass="form-control" TextMode="Date" runat="server" />
                </div>
                <div class="col-md-3 d-flex align-items-end">
                    <asp:Button ID="btnLoadReport" Text="Load Report" CssClass="btn btn-primary w-100" OnClick="btnLoadReport_Click" runat="server" />
                </div>
            </div>
        </div>

        <asp:GridView ID="gvReport" AutoGenerateColumns="False" CssClass="table table-striped report-grid" runat="server">
            <Columns>
                <asp:BoundField DataField="booking_id" HeaderText="Booking ID" />
                <asp:BoundField DataField="movie_title" HeaderText="Movie Title" />
                <asp:BoundField DataField="hall_name" HeaderText="Hall Name" />
                <asp:BoundField DataField="show_start" HeaderText="Show Start" />
                <asp:BoundField DataField="seat_label" HeaderText="Seat Label" />
                <asp:BoundField DataField="final_price" HeaderText="Final Price" />
                <asp:BoundField DataField="payment_status" HeaderText="Payment Status" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
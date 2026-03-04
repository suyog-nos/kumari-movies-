<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TicketDetails.aspx.cs" Inherits="kumari.TicketDetails"
    MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .ticket-page {
            max-width: 1200px;
            margin: 20px auto;
            padding: 20px 24px;
        }

        .ticket-grid.table th,
        .ticket-grid.table td {
            padding: 12px 14px !important;
            vertical-align: middle;
        }

        .ticket-grid thead th {
            background-color: #f7f7f7;
            border-bottom: 2px solid #dee2e6;
            white-space: nowrap;
        }

        .ticket-grid tbody tr:hover {
            background-color: #f5f5f5;
        }

        .section-card {
            border: 1px solid #e2e2e2;
            border-radius: 8px;
            padding: 16px 18px;
            margin-bottom: 24px;
            background-color: #fafafa;
        }

        .section-title {
            font-size: 18px;
            font-weight: 600;
            margin-bottom: 12px;
        }

        .ticket-actions {
            margin-top: 12px;
        }
    </style>

    <div class="ticket-page">
        <h2 class="page-heading">Ticket Booking & Payment</h2>

        <!-- Pricing Management Section -->
        <div class="section-card">
            <div class="section-title">Manage Showtime Pricing</div>
            <div class="row">
                <div class="col-md-3">
                    <label>Showtime</label>
                    <asp:DropDownList ID="ddlShowtimePrice" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="ddlShowtimePrice_SelectedIndexChanged" />
                </div>
                <div class="col-md-3">
                    <label>Price Type</label>
                    <asp:DropDownList ID="ddlPriceType" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="ddlPriceType_SelectedIndexChanged">
                        <asp:ListItem Value="NORMAL">Normal</asp:ListItem>
                        <asp:ListItem Value="HOLIDAY">Holiday</asp:ListItem>
                        <asp:ListItem Value="NEW_RELEASE">New Release</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-md-2">
                    <label>Ticket Price</label>
                    <asp:TextBox ID="txtTicketPrice" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-2 d-flex align-items-end">
                    <asp:Button ID="btnAddPricing" Text="Add Pricing" runat="server" CssClass="btn btn-success w-100" OnClick="btnAddPricing_Click" />
                </div>
            </div>
        </div>

        <!-- Pricing Table -->
        <h4>Existing Pricing</h4>
        <asp:GridView ID="gvPricing" runat="server" AutoGenerateColumns="False" CssClass="table table-striped ticket-grid">
            <Columns>
                <asp:BoundField DataField="showtime_id" HeaderText="Showtime ID" />
                <asp:BoundField DataField="movie_title" HeaderText="Movie" />
                <asp:BoundField DataField="hall_name" HeaderText="Hall" />
                <asp:BoundField DataField="price_type" HeaderText="Price Type" />
                <asp:BoundField DataField="ticket_price" HeaderText="Price" DataFormatString="{0:N2}" />
            </Columns>
        </asp:GridView>

        <!-- Bookings -->
        <h4 class="mt-4">Bookings</h4>
        <asp:GridView ID="gvBookings" runat="server" AutoGenerateColumns="False" DataKeyNames="booking_id"
            CssClass="table table-striped ticket-grid" OnRowCommand="gvBookings_RowCommand">
            <Columns>
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:Button ID="btnPaid" runat="server" Text="Paid" CommandName="Paid" CommandArgument='<%# Eval("booking_id") %>' 
                            CssClass="btn btn-sm btn-success me-1" Visible='<%# Eval("booking_status").ToString() == "BOOKED" %>' />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CommandName="CancelBooking" CommandArgument='<%# Eval("booking_id") %>' 
                            CssClass="btn btn-sm btn-danger" OnClientClick="return confirm('Are you sure you want to cancel this booking?');" 
                            Visible='<%# Eval("booking_status").ToString() == "BOOKED" %>' />
                        <asp:Label ID="lblNoActions" runat="server" Text="No actions available" CssClass="text-muted" 
                            Visible='<%# Eval("booking_status").ToString() != "BOOKED" %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="booking_id" HeaderText="Booking ID" />
                <asp:BoundField DataField="user_name" HeaderText="User" />
                <asp:BoundField DataField="movie_title" HeaderText="Movie" />
                <asp:BoundField DataField="hall_name" HeaderText="Hall" />
                <asp:BoundField DataField="seat_number" HeaderText="Seat" />
                <asp:BoundField DataField="show_start" HeaderText="Showtime" DataFormatString="{0:dd-MMM-yyyy hh:mm tt}" />
                <asp:BoundField DataField="booking_status" HeaderText="Status" />
                <asp:BoundField DataField="final_price" HeaderText="Price" DataFormatString="{0:N2}" />
            </Columns>
        </asp:GridView>

        <!-- Booking Section -->
        <div class="section-card">
            <div class="section-title">Booking Section</div>
            <div class="row">
                <div class="col-md-4">
                    <label>User</label>
                    <asp:DropDownList ID="ddlUser" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-4">
                    <label>Showtime</label>
                    <asp:DropDownList ID="ddlShowtime" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="ddlShowtime_SelectedIndexChanged" />
                </div>
                <div class="col-md-4">
                    <label>Seat</label>
                    <asp:DropDownList ID="ddlSeat" runat="server" CssClass="form-control" />
                </div>
            </div>
            <div class="row mt-3">
                <div class="col-md-3">
                    <label>Price Type</label>
                    <asp:DropDownList ID="ddlBookingPriceType" runat="server" CssClass="form-control">
                        <asp:ListItem Value="NORMAL">NORMAL</asp:ListItem>
                        <asp:ListItem Value="HOLIDAY">HOLIDAY</asp:ListItem>
                        <asp:ListItem Value="NEW_RELEASE">NEW_RELEASE</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-md-3 d-flex align-items-end">
                    <asp:Button ID="btnSwitchPrice" Text="Switch Price" runat="server" CssClass="btn btn-info" OnClick="btnSwitchPrice_Click" />
                </div>
                <div class="col-md-3">
                    <asp:Label ID="lblPrice" runat="server" CssClass="fw-bold" />
                </div>
                <div class="col-md-3 text-end">
                    <asp:Button ID="btnBook" Text="Create Booking" runat="server" CssClass="btn btn-primary" OnClick="btnBook_Click" />
                </div>
            </div>
        </div>

        <div class="mt-3">
            <asp:Label ID="lblMessage" runat="server" />
        </div>

    </div>
</asp:Content>
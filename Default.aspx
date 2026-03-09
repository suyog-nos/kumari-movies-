<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
CodeBehind="Default.aspx.cs" Inherits="kumari._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .dashboard-page {
            max-width: 1200px;
            margin: 20px auto;
            padding: 20px 24px;
        }

        .dashboard-header {
            border-bottom: 2px solid #ddd;
            padding-bottom: 12px;
            margin-bottom: 20px;
            text-align: center;
        }

        .dashboard-header h1 {
            font-size: 28px;
            margin-bottom: 4px;
            font-weight: 600;
        }

        .dashboard-header p {
            font-size: 0.9rem;
            color: #666;
            margin: 0;
        }

        .stats-card {
            border: 1px solid #ddd;
            border-radius: 6px;
            background-color: #fafafa;
        }

        .stats-card .card-body {
            padding: 14px 16px;
        }

        .stats-card .card-title {
            font-size: 0.8rem;
            text-transform: uppercase;
            color: #666;
            margin-bottom: 6px;
            letter-spacing: 0.4px;
            font-weight: 600;
        }

        .stats-number {
            font-size: 2rem;
            font-weight: 700;
            margin: 0;
        }

        h2 {
            font-size: 20px;
            font-weight: 600;
            margin: 24px 0 12px;
            border-bottom: 1px solid #ddd;
            padding-bottom: 4px;
        }

        .nav-card {
            border: 1px solid #ddd;
            border-radius: 6px;
            background-color: #fff;
            height: 100%;
        }

        .nav-card .card-body {
            padding: 14px 16px;
            display: flex;
            flex-direction: column;
        }

        .nav-card .card-title {
            font-weight: 600;
            margin-bottom: 4px;
        }

        .nav-card .card-text {
            font-size: 0.85rem;
            color: #666;
            margin-bottom: 12px;
        }

        .btn-dark {
            margin-top: auto;
            padding: 8px;
            font-size: 0.85rem;
            font-weight: 500;
        }
    </style>

    <div class="dashboard-page">

        <div class="dashboard-header">
            <h1>Kumari Cinemas Dashboard</h1>
            <p>Cinema Management System</p>
        </div>

        <!-- Stats -->
        <div class="row g-3 mb-4">
            <div class="col-md-3">
                <div class="card stats-card">
                    <div class="card-body">
                        <div class="card-title">Total Movies</div>
                        <div class="stats-number"><asp:Label ID="lblTotalMovies" runat="server" /></div>
                    </div>
                </div>
            </div>

            <div class="col-md-3">
                <div class="card stats-card">
                    <div class="card-body">
                        <div class="card-title">Total Users</div>
                        <div class="stats-number"><asp:Label ID="lblTotalUsers" runat="server" /></div>
                    </div>
                </div>
            </div>

            <div class="col-md-3">
                <div class="card stats-card">
                    <div class="card-body">
                        <div class="card-title">Total Bookings</div>
                        <div class="stats-number"><asp:Label ID="lblTotalBookings" runat="server" /></div>
                    </div>
                </div>
            </div>

            <div class="col-md-3">
                <div class="card stats-card">
                    <div class="card-body">
                        <div class="card-title">Paid Tickets</div>
                        <div class="stats-number"><asp:Label ID="lblTotalPaidTickets" runat="server" /></div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Navigation -->
        <h2 class="page-heading">Navigation</h2>

        <div class="row g-3">
            <div class="col-md-4">
                <div class="card nav-card">
                    <div class="card-body">
                        <h5 class="card-title">Movie Management</h5>
                        <p class="card-text">Manage movies, genres, and showtimes</p>
                        <asp:LinkButton runat="server" PostBackUrl="MovieDetails.aspx" CssClass="btn btn-dark">Go to Movies</asp:LinkButton>
                    </div>
                </div>
            </div>

            <div class="col-md-4">
                <div class="card nav-card">
                    <div class="card-body">
                        <h5 class="card-title">User Management</h5>
                        <p class="card-text">Manage users and their information</p>
                        <asp:LinkButton runat="server" PostBackUrl="UserDetails.aspx" CssClass="btn btn-dark">Go to Users</asp:LinkButton>
                    </div>
                </div>
            </div>

            <div class="col-md-4">
                <div class="card nav-card">
                    <div class="card-body">
                        <h5 class="card-title">Theater & Hall</h5>
                        <p class="card-text">Manage theaters and halls</p>
                        <asp:LinkButton runat="server" PostBackUrl="TheaterCityHall.aspx" CssClass="btn btn-dark">Go to Theaters</asp:LinkButton>
                    </div>
                </div>
            </div>

            <div class="col-md-4">
                <div class="card nav-card">
                    <div class="card-body">
                        <h5 class="card-title">Showtime</h5>
                        <p class="card-text">Manage movie showtimes</p>
                        <asp:LinkButton runat="server" PostBackUrl="ShowtimeDetails.aspx" CssClass="btn btn-dark">Go to Showtimes</asp:LinkButton>
                    </div>
                </div>
            </div>

            <div class="col-md-4">
                <div class="card nav-card">
                    <div class="card-body">
                        <h5 class="card-title">Ticket Booking</h5>
                        <p class="card-text">Book tickets and manage payments</p>
                        <asp:LinkButton runat="server" PostBackUrl="TicketDetails.aspx" CssClass="btn btn-dark">Go to Booking</asp:LinkButton>
                    </div>
                </div>
            </div>

            <div class="col-md-4">
                <div class="card nav-card">
                    <div class="card-body">
                        <h5 class="card-title">Reports</h5>
                        <p class="card-text">View occupancy and ticket reports</p>
                        <asp:LinkButton runat="server" PostBackUrl="MovieOccupancyReport.aspx" CssClass="btn btn-dark">Go to Reports</asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>

        <!-- Centered row for About Us and Contact cards -->
        <div class="row g-3 justify-content-center">
            <div class="col-md-4">
                <div class="card nav-card">
                    <div class="card-body">
                        <h5 class="card-title">About Us</h5>
                        <p class="card-text">Learn more about Kumari Cinemas</p>
                        <asp:LinkButton runat="server" PostBackUrl="About.aspx" CssClass="btn btn-dark">Go to About</asp:LinkButton>
                    </div>
                </div>
            </div>

            <div class="col-md-4">
                <div class="card nav-card">
                    <div class="card-body">
                        <h5 class="card-title">Contact</h5>
                        <p class="card-text">Get in touch with Kumari Cinemas</p>
                        <asp:LinkButton runat="server" PostBackUrl="Contact.aspx" CssClass="btn btn-dark">Go to Contact</asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>

    </div>

</asp:Content>
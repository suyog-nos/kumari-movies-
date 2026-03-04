<%@ Page Title="Theater and Hall Management" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TheaterCityHall.aspx.cs" Inherits="kumari.TheaterCityHall" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        /* Full width content */
        .theater-page {
            max-width: 1600px;   /* thicker layout */
            margin: 16px auto;
            padding: 20px 40px;
        }

        .success-message { color: #155724; background-color: #d4edda; padding: 8px 12px; border-radius: 4px; }
        .error-message { color: #721c24; background-color: #f8d7da; padding: 8px 12px; border-radius: 4px; }

        h2 {
            font-weight: 600;
            margin-bottom: 16px;
        }

        .section-card {
            background: #fff;
            border: 1px solid #ddd;
            border-radius: 6px;
            padding: 24px 28px;
            margin-bottom: 36px;
        }

        .form-grid {
            display: grid;
            grid-template-columns: 180px 1fr 180px 1fr; /* wider form */
            gap: 12px 20px;
            margin-top: 16px;
            margin-bottom: 20px;
        }

        .button-group {
            display: flex;
            gap: 12px;
        }

        .table th,
        .table td {
            padding: 12px 16px;
            vertical-align: middle;
        }

        .table thead th {
            background-color: #f7f7f7;
            border-bottom: 2px solid #ddd;
            white-space: nowrap;
        }

        .table tbody tr:hover {
            background-color: #f5f5f5;
        }
    </style>

    <div class="theater-page">
        <asp:Label ID="lblTheaterMessage" runat="server" CssClass="d-block mb-2" />
        <asp:Label ID="lblHallMessage" runat="server" CssClass="d-block mb-3" />

        <!-- THEATER MANAGEMENT -->
        <div class="section-card">
            <h2 class="page-heading">Theater Management</h2>

            <asp:GridView ID="gvTheaters" runat="server" AutoGenerateColumns="False"
                DataKeyNames="theater_id" OnSelectedIndexChanged="gvTheaters_SelectedIndexChanged"
                CssClass="table table-striped">
                <Columns>
                    <asp:BoundField DataField="theater_id" HeaderText="Theater ID" />
                    <asp:BoundField DataField="theater_name" HeaderText="Theater Name" />
                    <asp:BoundField DataField="theater_city" HeaderText="Theater City" />
                    <asp:CommandField ShowSelectButton="True" SelectText="Select" />
                </Columns>
            </asp:GridView>

            <div class="form-grid">
                <label>Theater ID:</label>
                <asp:TextBox ID="txtTheaterId" runat="server" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvTheaterId" runat="server" ControlToValidate="txtTheaterId" ErrorMessage="Theater ID is required" CssClass="text-danger" Display="Dynamic" />

                <label>Theater Name:</label>
                <asp:TextBox ID="txtTheaterName" runat="server" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvTheaterName" runat="server" ControlToValidate="txtTheaterName" ErrorMessage="Theater Name is required" CssClass="text-danger" Display="Dynamic" />

                <label>Theater City:</label>
                <asp:TextBox ID="txtTheaterCity" runat="server" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvTheaterCity" runat="server" ControlToValidate="txtTheaterCity" ErrorMessage="Theater City is required" CssClass="text-danger" Display="Dynamic" />
            </div>

            <div class="button-group">
                <asp:Button ID="btnTheaterInsert" runat="server" Text="Insert" CssClass="btn btn-primary" OnClick="btnTheaterInsert_Click" />
                <asp:Button ID="btnTheaterUpdate" runat="server" Text="Update" CssClass="btn btn-warning" OnClick="btnTheaterUpdate_Click" />
                <asp:Button ID="btnTheaterDelete" runat="server" Text="Delete" CssClass="btn btn-danger" OnClick="btnTheaterDelete_Click" />
                <asp:Button ID="btnTheaterClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnTheaterClear_Click" />
            </div>
        </div>

        <!-- HALL MANAGEMENT -->
        <div class="section-card">
            <h2 class="page-heading">Hall Management</h2>

            <asp:GridView ID="gvHalls" runat="server" AutoGenerateColumns="False"
                DataKeyNames="hall_id" OnSelectedIndexChanged="gvHalls_SelectedIndexChanged"
                CssClass="table table-striped">
                <Columns>
                    <asp:BoundField DataField="hall_id" HeaderText="Hall ID" />
                    <asp:BoundField DataField="theater_id" HeaderText="Theater ID" />
                    <asp:BoundField DataField="hall_name" HeaderText="Hall Name" />
                    <asp:BoundField DataField="hall_capacity" HeaderText="Capacity" />
                    <asp:CommandField ShowSelectButton="True" SelectText="Select" />
                </Columns>
            </asp:GridView>

            <div class="form-grid">
                <label>Hall ID:</label>
                <asp:TextBox ID="txtHallId" runat="server" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvHallId" runat="server" ControlToValidate="txtHallId" ErrorMessage="Hall ID is required" CssClass="text-danger" Display="Dynamic" />

                <label>Hall Name:</label>
                <asp:TextBox ID="txtHallName" runat="server" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvHallName" runat="server" ControlToValidate="txtHallName" ErrorMessage="Hall Name is required" CssClass="text-danger" Display="Dynamic" />

                <label>Hall Capacity:</label>
                <asp:TextBox ID="txtHallCapacity" runat="server" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvHallCapacity" runat="server" ControlToValidate="txtHallCapacity" ErrorMessage="Hall Capacity is required" CssClass="text-danger" Display="Dynamic" />

                <label>Theater:</label>
                <asp:DropDownList ID="ddlTheater" runat="server" CssClass="form-control"
                    DataValueField="theater_id" DataTextField="theater_name"></asp:DropDownList>
            </div>

            <div class="button-group">
                <asp:Button ID="btnHallInsert" runat="server" Text="Insert" CssClass="btn btn-primary" OnClick="btnHallInsert_Click" />
                <asp:Button ID="btnHallUpdate" runat="server" Text="Update" CssClass="btn btn-warning" OnClick="btnHallUpdate_Click" />
                <asp:Button ID="btnHallDelete" runat="server" Text="Delete" CssClass="btn btn-danger" OnClick="btnHallDelete_Click" />
                <asp:Button ID="btnHallClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnHallClear_Click" />
            </div>
        </div>

    </div>

</asp:Content>

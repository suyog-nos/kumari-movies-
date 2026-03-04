<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserDetails.aspx.cs" Inherits="kumari.UserDetails" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        /* Page spacing */
        .user-page {
            max-width: 1200px;
            margin: 20px auto;
            padding: 20px 24px;
        }

        /* GridView table padding */
        .user-grid.table th,
        .user-grid.table td {
            padding: 12px 14px !important;
            vertical-align: middle;
        }

        .user-grid thead th {
            background-color: #f7f7f7;
            border-bottom: 2px solid #dee2e6;
            white-space: nowrap;
        }

        .user-grid tbody tr:hover {
            background-color: #f5f5f5;
        }

        /* Action column */
        .user-grid td:last-child {
            text-align: center;
            font-weight: 500;
        }

        /* Message styles */
        .message-label { display: block; margin-bottom: 8px; font-weight: 600; }
        .message-success { color: #155724; background-color: #d4edda; padding: 8px 12px; border-radius: 4px; }
        .message-error { color: #721c24; background-color: #f8d7da; padding: 8px 12px; border-radius: 4px; }

        /* Validation summary: remove double/outer box and match inline error styling */
        .validation-summary {
            background: transparent; /* remove alert background */
            border: none;            /* remove alert border */
            padding: 0;              /* remove outer padding */
            margin: 0;               /* keep spacing controlled by layout */
            color: #721c24;         /* error text color */
        }

        .validation-summary ul {
            margin: 0.25rem 0 0 0;   /* small top margin for list */
            padding-left: 1.25rem;  /* keep bullet indented */
        }

        /* When you want a boxed alert for global messages, use .message-error instead */
    </style>

    <div class="user-page">
        <h2 class="page-heading">User Management</h2>
        <asp:Label ID="lblMessage" runat="server" CssClass="message-label" />
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="validation-summary mt-2" HeaderText="Please fix the following:" DisplayMode="BulletList" />

        <asp:GridView 
            ID="gvUsers" 
            runat="server" 
            AutoGenerateColumns="False" 
            DataKeyNames="user_id" 
            CssClass="table table-striped user-grid" 
            OnSelectedIndexChanged="gvUsers_SelectedIndexChanged">
            <Columns>
                <asp:BoundField DataField="user_id" HeaderText="User ID" />
                <asp:BoundField DataField="user_name" HeaderText="User Name" />
                <asp:BoundField DataField="user_address" HeaderText="User Address" />
                <asp:CommandField ShowSelectButton="True" SelectText="Select" />
            </Columns>
        </asp:GridView>

        <div class="row mt-4">
            <div class="col-md-3">
                <label for="txtUserId">User ID:</label>
                <asp:TextBox ID="txtUserId" runat="server" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvUserId" runat="server" ControlToValidate="txtUserId" ErrorMessage="User ID is required" CssClass="text-danger" Display="Dynamic" />
                <asp:CompareValidator ID="cmpUserId" runat="server" ControlToValidate="txtUserId" ErrorMessage="User ID must be an integer" Operator="DataTypeCheck" Type="Integer" CssClass="text-danger" Display="Dynamic" />
            </div>
            <div class="col-md-3">
                <label for="txtUserName">User Name:</label>
                <asp:TextBox ID="txtUserName" runat="server" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvUserName" runat="server" ControlToValidate="txtUserName" ErrorMessage="User Name is required" CssClass="text-danger" Display="Dynamic" />
            </div>
            <div class="col-md-3">
                <label for="txtUserAddress">User Address:</label>
                <asp:TextBox ID="txtUserAddress" runat="server" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvUserAddress" runat="server" ControlToValidate="txtUserAddress" ErrorMessage="User Address is required" CssClass="text-danger" Display="Dynamic" />
            </div>
        </div>

        <div class="row mt-3">
            <div class="col-md-12 d-flex gap-2">
                <asp:Button ID="btnInsert" Text="Insert" runat="server" CssClass="btn btn-primary me-2" OnClick="btnInsert_Click" />
                <asp:Button ID="btnUpdate" Text="Update" runat="server" CssClass="btn btn-warning me-2" OnClick="btnUpdate_Click" />
                <asp:Button ID="btnDelete" Text="Delete" runat="server" CssClass="btn btn-danger me-2" OnClick="btnDelete_Click" />
                <asp:Button ID="btnClear" Text="Clear" runat="server" CssClass="btn btn-secondary" OnClick="btnClear_Click" />
            </div>
        </div>
    </div>

</asp:Content>
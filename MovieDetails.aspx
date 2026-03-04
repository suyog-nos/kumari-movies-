<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MovieDetails.aspx.cs" Inherits="kumari.MovieDetails" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        /* Page spacing  */
        .movie-page {
            max-width: 1200px;
            margin: 20px auto;
            padding: 20px 24px;
        }

        /* GridView spacing  */
        .movie-grid.table th,
        .movie-grid.table td {
            padding: 12px 14px !important;
            vertical-align: middle;
        }

        .movie-grid thead th {
            background-color: #f7f7f7;
            border-bottom: 2px solid #dee2e6;
            white-space: nowrap;
        }

        .movie-grid tbody tr:hover {
            background-color: #f5f5f5;
        }

        /* Action column */
        .movie-grid td:last-child {
            text-align: center;
            font-weight: 500;
        }

        /* Form section */
        .form-section {
            border: 1px solid #ddd;
            padding: 16px 20px;
            border-radius: 6px;
            background-color: #f9f9f9;
            margin-top: 20px;
        }

        .form-grid {
            display: grid;
            grid-template-columns: 140px 1fr;
            gap: 12px 16px;
            align-items: center;
        }

        .form-label {
            font-weight: 600;
            color: #444;
        }
    </style>

    <div class="movie-page">
        <h2 class="page-heading">Movie Management</h2>

        <asp:GridView 
            ID="gvMovies" 
            runat="server" 
            AutoGenerateColumns="False" 
            DataKeyNames="movie_id"
            CssClass="table table-striped movie-grid"
            OnSelectedIndexChanged="gvMovies_SelectedIndexChanged">

            <Columns>
                <asp:BoundField DataField="movie_id" HeaderText="Movie ID" ReadOnly="True" />
                <asp:BoundField DataField="movie_title" HeaderText="Title" />
                <asp:BoundField DataField="duration_minutes" HeaderText="Duration (Minutes)" />
                <asp:BoundField DataField="language" HeaderText="Language" />
                <asp:BoundField DataField="genre" HeaderText="Genre" />
                <asp:BoundField DataField="release_date" HeaderText="Release Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:CommandField ShowSelectButton="True" SelectText="Select" />
            </Columns>
        </asp:GridView>

        <div class="form-section">
            <div class="form-grid">
                <div class="form-label">Movie ID:</div>
                <asp:TextBox ID="txtMovieId" runat="server" CssClass="form-control"></asp:TextBox>

                <div class="form-label">Title:</div>
                <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control"></asp:TextBox>

                <div class="form-label">Duration (Minutes):</div>
                <asp:TextBox ID="txtDuration" runat="server" CssClass="form-control"></asp:TextBox>

                <div class="form-label">Language:</div>
                <asp:TextBox ID="txtLanguage" runat="server" CssClass="form-control"></asp:TextBox>

                <div class="form-label">Genre:</div>
                <asp:TextBox ID="txtGenre" runat="server" CssClass="form-control"></asp:TextBox>

                <div class="form-label">Release Date:</div>
                <asp:TextBox ID="txtReleaseDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
            </div>

            <div class="mt-3 d-flex gap-2">
                <asp:Button ID="btnInsert" runat="server" Text="Insert" CssClass="btn btn-success me-2" OnClick="btnInsert_Click" />
                <asp:Button ID="btnUpdate" runat="server" Text="Update" CssClass="btn btn-primary me-2" OnClick="btnUpdate_Click" />
                <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-danger me-2" OnClick="btnDelete_Click" />
                <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnClear_Click" />
            </div>
        </div>
    </div>

</asp:Content>
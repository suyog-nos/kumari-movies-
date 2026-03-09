<%@ Page Title="About Us" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs"
    Inherits="kumari.About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container mt-4">

        <h2 class="text-center">About Kumari Cinemas</h2>
        <hr />

        <div class="row">

            <div class="col-md-6">
                <img src="/images/cinema_hall.jpg" class="img-fluid rounded" alt="Theatre Image"/>
            </div>

            <div class="col-md-6">

                <h3>Welcome to Kumari Theatre</h3>

                <p>
                    Kumari Theatre is one of the most popular movie halls providing
                    the latest movies with high quality sound and comfortable seating.
                </p>

                <p>
                    Our mission is to deliver the best cinematic experience to our
                    customers with modern facilities and affordable ticket prices.
                </p>

                <ul>
                    <li> Latest Hollywood & Nepali Movies</li>
                    <li> Comfortable Seating</li>
                    <li> Online Ticket Booking</li>
                    <li> High Quality Sound System</li>
                </ul>

            </div>

        </div>

        <hr />

        <div class="row text-center">

            <div class="col-md-4">
                <h4> Movies</h4>
                <p>Watch the latest blockbusters every week.</p>
            </div>

            <div class="col-md-4">
                <h4> Snacks</h4>
                <p>Enjoy delicious popcorn and drinks.</p>
            </div>

            <div class="col-md-4">
                <h4> Experience</h4>
                <p>Enjoy cinema in a comfortable environment.</p>
            </div>

        </div>

    </div>

</asp:Content>
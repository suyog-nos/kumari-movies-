<%@ Page Title="Contact" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="kumari.Contact" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

<div class="container mt-5">

    <div class="text-center mb-4">
        <h2>Contact Kumari Cinemas</h2>
        <p class="text-muted">For inquiries, support, and booking assistance</p>
        <hr />
    </div>

    <div class="row">

        <!-- Location -->
        <div class="col-md-4">
            <div class="card shadow-sm p-3">
                <h4>Our Location</h4>
                <address>
                    <strong>Kumari Cinemas</strong><br />
                    Hospital Chowk, Pokhara<br />
                    Nepal<br /><br />

                    <strong>Phone:</strong><br />
                    +977-1-1234567
                </address>
            </div>
        </div>

        <!-- Contact Info -->
        <div class="col-md-4">
            <div class="card shadow-sm p-3">
                <h4>Contact Information</h4>

                <p>
                    <strong>Box Office</strong><br />
                    <a href="mailto:info@kumaricinemas.com">info@kumaricinemas.com</a>
                </p>

                <p>
                    <strong>Customer Support</strong><br />
                    <a href="mailto:support@kumaricinemas.com">support@kumaricinemas.com</a>
                </p>

                <p>
                    <strong>Group Bookings</strong><br />
                    groups@kumaricinemas.com
                </p>
            </div>
        </div>

        <!-- Opening Hours -->
        <div class="col-md-4">
            <div class="card shadow-sm p-3">
                <h4>Opening Hours</h4>

                <p>
                    <strong>Monday – Friday</strong><br />
                    10:00 AM – 10:00 PM
                </p>

                <p>
                    <strong>Saturday – Sunday</strong><br />
                    9:00 AM – 11:00 PM
                </p>

                <p>
                    <strong>Public Holidays</strong><br />
                    9:00 AM – 11:00 PM
                </p>
            </div>
        </div>

    </div>

    <div class="row mt-4">
        <div class="col-12 text-center">

            <h4>Additional Information</h4>

            <p class="text-muted">
                For group bookings, corporate events, or private screenings,
                please contact Kumari Cinemas at least 48 hours in advance.
            </p>

            <p>
                Customers may contact the cinema for assistance regarding
                ticket bookings, showtime schedules, and payment inquiries.
            </p>

        </div>
    </div>

</div>

</asp:Content>
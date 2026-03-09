<%@ Page Title="Contact" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="kumari.Contact" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2 class="text-center">Contact Kumari Cinemas</h2>
        <hr />
        
        <div class="row">
            <div class="col-md-6">
                <h4>Our Location</h4>
                <address>
                    <strong>Kumari Cinemas</strong><br />
                    Hospital Chowk , Pokhara<br />
                    Nepal<br />
                    <abbr title="Phone">Phone Number</abbr>
                    +977-1-1234567
                </address>
                
                <h4>Contact Information</h4>
                <address>
                    <strong>Box Office:</strong>   <a href="mailto:info@kumaricinemas.com">info@kumaricinemas.com</a><br />
                    <strong>Support:</strong> <a href="mailto:support@kumaricinemas.com">support@kumaricinemas.com</a>
                </address>
            </div>
            
            <div class="col-md-6">
                <h4>Opening Hours</h4>
                <p>
                    <strong>Monday - Friday:</strong> 10:00 AM - 10:00 PM<br />
                    <strong>Saturday - Sunday:</strong> 9:00 AM - 11:00 PM<br />
                    <strong>Holidays:</strong> 9:00 AM - 11:00 PM
                </p>
            </div>
        </div>
        
        <hr />
        
        <div class="row">
            <div class="col-12">
                <h4>Additional Information</h4>
                <p>
                    For group bookings, special events, or corporate inquiries, please contact us at least 48 hours in advance.
                    We accept online booking through our website and mobile app.
                </p>
                <p>
                    Follow us on social media for the latest movie updates and special offers!
                </p>
            </div>
        </div>
    </div>
</asp:Content>

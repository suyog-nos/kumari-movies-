# Kumari Cinemas - Movie Ticket Booking System

A web-based movie ticket booking system developed using ASP.NET Web Forms and Oracle Database for academic purposes.

## Project Overview

Kumari Cinemas is a cinema management system that allows users to browse movies, select showtimes, book tickets, and manage bookings. The system includes administrative features for theater management and reporting.

## Technology Stack

- **Frontend**: ASP.NET Web Forms (.NET Framework 4.7.2), Bootstrap 5, jQuery 3.7.0
- **Backend**: C#, Oracle Database (Oracle XE)
- **Development**: Visual Studio 2019/2022, NuGet Package Manager

## Database Schema

### Core Tables
- **app_user** - User information and profiles
- **movie** - Movie catalog with details
- **theater** - Cinema locations
- **hall** - Individual theater halls
- **seat** - Seat management per hall
- **showtime** - Movie scheduling
- **showtime_pricing** - Dynamic pricing (Normal/Holiday/New Release)
- **booking** - Booking transactions
- **ticket** - Individual ticket records
- **payment** - Payment processing

### Key Features
- Dynamic pricing support
- Seat management with unique allocation
- Booking expiry system
- Payment status tracking

## Setup Instructions

### Prerequisites
- Visual Studio 2019/2022 with ASP.NET Web Forms
- Oracle Database XE or Oracle Database 19c+
- Oracle Managed Data Access Client

### Step 1: Database Setup
1. Install Oracle Database XE
2. Create a new user/schema: KUMARI_MOVIES with password KUMARI123
3. Execute the kumartisql.sql script to create tables and sample data:
   ```sql
   CONNECT KUMARI_MOVIES/KUMARI123@localhost:1521/xe;
   @kumarisql.sql;
   ```

### Step 2: Application Configuration
1. Clone/download the project
2. Open kumari.sln in Visual Studio
3. Update the connection string in Web.config if needed:
   ```xml
   <add name="OracleDb"
        connectionString="User Id=KUMARI_MOVIES;Password=KUMARI123;Data Source=localhost:1521/xe;"
        providerName="Oracle.ManagedDataAccess.Client" />
   ```

### Step 3: Restore Dependencies
1. Right-click the project in Solution Explorer
2. Select "Manage NuGet Packages"
3. Click "Restore" to install all required packages

### Step 4: Run the Application
1. Set the project as startup project
2. Press F5 or click "Start Debugging"
3. The application will open in your default browser at https://localhost:44343/

## Key Features

### User Features
- Movie browsing with details
- Showtime selection
- Interactive seat selection
- Online booking process
- Payment processing
- Booking history

### Administrative Features
- Theater and hall management
- Movie scheduling
- Pricing control
- Occupancy reports
- User management

### System Features
- Responsive design for desktop and mobile
- Real-time seat availability
- Input validation and error handling
- Secure authentication

## Sample Data

The system includes:
- 5 sample users from various cities
- 5 movies (English, Hindi, Nepali)
- 3 theaters (Pokhara, Lalitpur, Kathmandu)
- 4 halls with capacities 150-220 seats
- 750+ seats across all halls
- Multiple showtimes and pricing scenarios

## Configuration

### Database Connection
Modify the connection string in Web.config:
```xml
<connectionStrings>
    <add name="OracleDb"
         connectionString="User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;Data Source=YOUR_HOST:1521/xe;"
         providerName="Oracle.ManagedDataAccess.Client" />
</connectionStrings>
```


### Common Issues

1. Database Connection Error
   - Verify Oracle Database is running
   - Check connection string parameters
   - Ensure Oracle client is installed

2. Build Errors
   - Restore NuGet packages
   - Clean and rebuild solution
   - Check .NET Framework compatibility

3. Runtime Errors
   - Enable detailed error messages in Web.config
   - Check Event Viewer for logs
   - Verify database permissions

4. Roslyn Compiler Error
   - If you get "Could not find a part of the path" error for Roslyn compiler
   - Run the following command in the project directory:
   ```
   xcopy /E /I /Y "packages\Microsoft.CodeDom.Providers.DotNetCompilerPlatform.2.0.1\tools\RoslynLatest\*" "bin\roslyn\"
   ```
   - Clean and rebuild the solution

## Development Notes

### Code Organization
- Code-behind files separate presentation and logic
- Master pages provide consistent layout
- Bootstrap-based responsive design
- JavaScript for client-side validation

### Database Practices
- Sequences for auto-incrementing IDs
- Foreign keys and constraints for integrity
- Optimized indexes for common queries
- Proper error handling and transactions

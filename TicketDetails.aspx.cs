using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace kumari
{
    public partial class TicketDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUsers();
                LoadShowtimes();
                LoadShowtimesForPricing();
                LoadSeats();
                LoadBookings();
            }
            
            // Always load pricing to reflect updates
            LoadPricing();

            // Check for expired bookings and cancel them automatically
            ExpireOldBookings();
        }

        private void LoadUsers()
        {
            string connString = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connString))
            {
                conn.Open();
                string query = "SELECT user_id, user_name FROM app_user ORDER BY user_name";
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        ddlUser.Items.Clear();
                        ddlUser.Items.Add(new ListItem("--Select User--", "0"));
                        while (reader.Read())
                        {
                            string displayText = $"{reader["user_id"]} - {reader["user_name"]}";
                            ddlUser.Items.Add(new ListItem(displayText, reader["user_id"].ToString()));
                        }
                    }
                }
            }
        }

        private void LoadShowtimes()
        {
            string connString = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connString))
            {
                conn.Open();
                string query = @"SELECT st.showtime_id, m.movie_title, h.hall_name, st.show_start 
                                FROM showtime st 
                                JOIN movie m ON st.movie_id = m.movie_id 
                                JOIN hall h ON st.hall_id = h.hall_id 
                                ORDER BY st.show_start";
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        ddlShowtime.Items.Clear();
                        ddlShowtime.Items.Add(new ListItem("--Select Showtime--", "0"));
                        while (reader.Read())
                        {
                            string displayText = $"{reader["showtime_id"]} - {reader["movie_title"]} ({reader["hall_name"]})";
                            ddlShowtime.Items.Add(new ListItem(displayText, reader["showtime_id"].ToString()));
                        }
                    }
                }
            }
        }

        private void LoadSeats()
        {
            string connString = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connString))
            {
                conn.Open();
                string query = @"SELECT s.seat_id, h.hall_name, s.seat_label 
                                FROM seat s 
                                JOIN hall h ON s.hall_id = h.hall_id 
                                ORDER BY h.hall_name, s.seat_label";
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        ddlSeat.Items.Clear();
                        ddlSeat.Items.Add(new ListItem("-- Select Seat (Select Showtime First) --", "0"));
                        while (reader.Read())
                        {
                            string displayText = $"{reader["seat_id"]} - {reader["hall_name"]} ({reader["seat_label"]})";
                            ddlSeat.Items.Add(new ListItem(displayText, reader["seat_id"].ToString()));
                        }
                    }
                }
            }
        }

        private void LoadAvailableSeats(int showtimeId)
        {
            string connString = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connString))
            {
                conn.Open();

                // Get hall capacity and hall info
                string capacityQuery = @"SELECT h.hall_capacity, h.hall_name
                                        FROM showtime st
                                        JOIN hall h ON st.hall_id = h.hall_id
                                        WHERE st.showtime_id = :showtime_id";
                int hallCapacity = 0;
                string hallName = "";
                using (OracleCommand cmd = new OracleCommand(capacityQuery, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("showtime_id", OracleDbType.Int32)).Value = showtimeId;
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            hallCapacity = Convert.ToInt32(reader["hall_capacity"]);
                            hallName = reader["hall_name"].ToString();
                        }
                    }
                }

                // Get already booked seats for this showtime (only from active bookings)
                string bookedSeatsQuery = @"SELECT t.seat_id 
                                          FROM ticket t 
                                          JOIN booking b ON t.booking_id = b.booking_id 
                                          WHERE t.showtime_id = :showtime_id 
                                          AND b.booking_status IN ('BOOKED', 'PAID')";
                HashSet<int> bookedSeatIds = new HashSet<int>();
                using (OracleCommand cmd = new OracleCommand(bookedSeatsQuery, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("showtime_id", OracleDbType.Int32)).Value = showtimeId;
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bookedSeatIds.Add(Convert.ToInt32(reader["seat_id"]));
                        }
                    }
                }

                // Check if capacity is full
                if (bookedSeatIds.Count >= hallCapacity)
                {
                    ddlSeat.Items.Clear();
                    ddlSeat.Items.Add(new ListItem($"-- HALL FULL ({hallName}: {hallCapacity}/{hallCapacity}) --", "0"));
                    lblMessage.Text = "Hall is at full capacity. No more seats available.";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                // Generate all possible seats for this hall (since seat table has limited data)
                ddlSeat.Items.Clear();
                int availableCount = 0;

                // Generate numeric seat labels only (1, 2, 3...)
                for (int i = 1; i <= hallCapacity; i++)
                {
                    // Create seat label as number only
                    string seatLabel = i.ToString();

                    // Get actual seat_id from database for this seat label
                    string getSeatIdQuery = @"SELECT seat_id FROM seat 
                                            WHERE hall_id = (SELECT hall_id FROM showtime WHERE showtime_id = :showtime_id) 
                                            AND seat_label = :seat_label";
                    int actualSeatId = 0;
                    using (OracleCommand cmd = new OracleCommand(getSeatIdQuery, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("showtime_id", OracleDbType.Int32)).Value = showtimeId;
                        cmd.Parameters.Add(new OracleParameter("seat_label", OracleDbType.Varchar2)).Value = seatLabel;
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            actualSeatId = Convert.ToInt32(result);
                        }
                    }

                    // Check if this seat is already booked (using actual seat_id)
                    bool isAvailable = !bookedSeatIds.Contains(actualSeatId);
                    
                    if (isAvailable)
                    {
                        string displayText = $"{hallName} - Seat {seatLabel}";
                        ddlSeat.Items.Add(new ListItem(displayText, actualSeatId.ToString())); // Use actual seat_id
                        availableCount++;
                    }
                }

                // Add header item
                ddlSeat.Items.Insert(0, new ListItem($"-- Select Seat ({availableCount}/{hallCapacity} available in {hallName}) --", "0"));

                // Update message
                lblMessage.Text = $"{hallName} capacity: {hallCapacity} | Booked: {bookedSeatIds.Count} | Available: {availableCount}";
                lblMessage.ForeColor = System.Drawing.Color.Green;
            }
        }

        private void LoadBookings()
        {
            string connString = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connString))
            {
                conn.Open();
                string query = @"SELECT b.booking_id, u.user_name, m.movie_title, h.hall_name, 
                                  t.seat_id as seat_number, 
                                  TO_CHAR(st.show_start, 'DD-MON HH24:MI') as show_start, 
                                  b.booking_status, t.final_price
                                  FROM booking b
                                  JOIN app_user u ON b.user_id = u.user_id
                                  JOIN showtime st ON b.showtime_id = st.showtime_id
                                  JOIN movie m ON st.movie_id = m.movie_id
                                  JOIN hall h ON st.hall_id = h.hall_id
                                  JOIN ticket t ON b.booking_id = t.booking_id
                                  ORDER BY b.booking_id DESC";
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        System.Data.DataTable dt = new System.Data.DataTable();
                        da.Fill(dt);
                        gvBookings.DataSource = dt;
                        gvBookings.DataBind();
                    }
                }
            }
        }

        private void ExpireOldBookings()
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;
                using (OracleConnection conn = new OracleConnection(connString))
                {
                    conn.Open();

                    // Find bookings that are past their expiration time and still in 'BOOKED' status
                    string expiredQuery = @"SELECT booking_id FROM booking 
                                          WHERE booking_status = 'BOOKED' 
                                          AND booking_expires_at < SYSTIMESTAMP";
                    List<int> expiredBookingIds = new List<int>();
                    
                    using (OracleCommand cmd = new OracleCommand(expiredQuery, conn))
                    {
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                expiredBookingIds.Add(Convert.ToInt32(reader["booking_id"]));
                            }
                        }
                    }

                    // Update expired bookings to 'EXPIRED' status
                    if (expiredBookingIds.Count > 0)
                    {
                        string updateQuery = @"UPDATE booking SET booking_status = 'EXPIRED' 
                                             WHERE booking_id IN (" + string.Join(",", expiredBookingIds) + ")";
                        
                        using (OracleCommand cmd = new OracleCommand(updateQuery, conn))
                        {
                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                lblMessage.Text = $"✓ Auto-cancelled {rowsAffected} expired booking(s). Booking status changed to EXPIRED";
                                lblMessage.ForeColor = System.Drawing.Color.Orange;
                                
                                // Refresh the bookings grid and seat availability
                                LoadBookings();
                                
                                // Refresh seat availability for currently selected showtime
                                if (ddlShowtime.SelectedValue != "0")
                                {
                                    int currentShowtimeId = Convert.ToInt32(ddlShowtime.SelectedValue);
                                    LoadAvailableSeats(currentShowtimeId);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't show to user since this is background operation
                // In production, you'd log this to a file or monitoring system
                System.Diagnostics.Debug.WriteLine($"Error in ExpireOldBookings: {ex.Message}");
            }
        }

        protected void btnBook_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if dropdowns have valid selections (not just default values)
                if (ddlUser.SelectedIndex <= 0 || ddlShowtime.SelectedIndex <= 0 || ddlSeat.SelectedIndex <= 0)
                {
                    lblMessage.Text = "Please select User, Showtime, and Seat.";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                int showtimeId = Convert.ToInt32(ddlShowtime.SelectedValue);
                int seatId = Convert.ToInt32(ddlSeat.SelectedValue);
                
                string connString = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;
                
                // First, validate capacity before booking
                using (OracleConnection conn = new OracleConnection(connString))
                {
                    conn.Open();

                    // Get hall capacity
                    string capacityQuery = @"SELECT h.hall_capacity 
                                            FROM showtime st 
                                            JOIN hall h ON st.hall_id = h.hall_id 
                                            WHERE st.showtime_id = :showtime_id";
                    int hallCapacity;
                    using (OracleCommand cmd = new OracleCommand(capacityQuery, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("showtime_id", OracleDbType.Int32)).Value = showtimeId;
                        hallCapacity = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Count already booked tickets
                    string countQuery = "SELECT COUNT(*) FROM ticket WHERE showtime_id = :showtime_id";
                    int bookedCount;
                    using (OracleCommand cmd = new OracleCommand(countQuery, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("showtime_id", OracleDbType.Int32)).Value = showtimeId;
                        bookedCount = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Check if capacity is exceeded
                    if (bookedCount >= hallCapacity)
                    {
                        lblMessage.Text = $"ERROR: Hall capacity ({hallCapacity}) has been reached. No more seats available.";
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                        return;
                    }

                    // All validations passed, proceed with booking
                    using (OracleTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            // Get show_start and price
                            string showStartQuery = "SELECT show_start FROM showtime WHERE showtime_id = :showtime_id";
                            DateTime showStart;
                            using (OracleCommand cmd = new OracleCommand(showStartQuery, conn))
                            {
                                cmd.Transaction = trans;
                                cmd.Parameters.Add(new OracleParameter("showtime_id", OracleDbType.Int32)).Value = showtimeId;
                                showStart = (DateTime)cmd.ExecuteScalar();
                            }

                            string priceType = ddlBookingPriceType.SelectedValue;
                            string priceQuery = @"SELECT ticket_price FROM showtime_pricing 
                                WHERE showtime_id = :showtime_id AND price_type = :price_type";
                            decimal price;
                            using (OracleCommand cmd = new OracleCommand(priceQuery, conn))
                            {
                                cmd.Transaction = trans;
                                cmd.Parameters.Add(new OracleParameter("showtime_id", OracleDbType.Int32)).Value = showtimeId;
                                cmd.Parameters.Add(new OracleParameter("price_type", OracleDbType.Varchar2)).Value = priceType;
                                object result = cmd.ExecuteScalar();
                                if (result == null)
                                {
                                    throw new Exception($"Price not found for selected showtime with {priceType} pricing. Please check showtime pricing setup.");
                                }
                                price = Convert.ToDecimal(result);
                            }

                            // Insert booking
                            string insertBooking = "INSERT INTO booking (booking_id, user_id, showtime_id, booking_status, booking_expires_at) VALUES (booking_seq.NEXTVAL, :user_id, :showtime_id, 'BOOKED', :expires)";
                            int bookingId;
                            using (OracleCommand cmd = new OracleCommand(insertBooking, conn))
                            {
                                cmd.Transaction = trans;
                                cmd.Parameters.Add(new OracleParameter("user_id", OracleDbType.Int32)).Value = Convert.ToInt32(ddlUser.SelectedValue);
                                cmd.Parameters.Add(new OracleParameter("showtime_id", OracleDbType.Int32)).Value = showtimeId;
                                cmd.Parameters.Add(new OracleParameter("expires", OracleDbType.TimeStamp)).Value = showStart.AddHours(-1);
                                cmd.ExecuteNonQuery();
                            }

                            // Get the generated booking_id
                            using (OracleCommand cmd = new OracleCommand("SELECT booking_seq.CURRVAL FROM dual", conn))
                            {
                                cmd.Transaction = trans;
                                bookingId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            // Insert ticket with dynamic seat creation for any hall
                            string insertTicket = "INSERT INTO ticket (ticket_id, booking_id, showtime_id, seat_id, final_price) VALUES (ticket_seq.NEXTVAL, :booking_id, :showtime_id, :seat_id, :price)";
                            using (OracleCommand ticketCmd = new OracleCommand(insertTicket, conn))
                            {
                                ticketCmd.Transaction = trans;
                                ticketCmd.Parameters.Add(new OracleParameter("booking_id", OracleDbType.Int32)).Value = bookingId;
                                ticketCmd.Parameters.Add(new OracleParameter("showtime_id", OracleDbType.Int32)).Value = showtimeId;
                                
                                // Check if seat exists for this hall, create if not
                                string checkSeatQuery = @"SELECT seat_id FROM seat 
                                                      WHERE hall_id = (SELECT hall_id FROM showtime WHERE showtime_id = :showtime_id) 
                                                      AND seat_label = :seat_label";
                                using (OracleCommand checkCmd = new OracleCommand(checkSeatQuery, conn))
                                {
                                    checkCmd.Transaction = trans;
                                    checkCmd.Parameters.Add(new OracleParameter("showtime_id", OracleDbType.Int32)).Value = showtimeId;
                                    
                                    // Extract seat number from dropdown (e.g., "Hall A - Seat 11" -> "11")
                                    string selectedSeatText = ddlSeat.SelectedItem.Text;
                                    string seatNumber = selectedSeatText.Split('-')[1].Trim().Split(' ')[1]; // Extract "11" from "Hall A - Seat 11"
                                    
                                    checkCmd.Parameters.Add(new OracleParameter("seat_label", OracleDbType.Varchar2)).Value = seatNumber;
                                    object seatIdResult = checkCmd.ExecuteScalar();
                                    
                                    if (seatIdResult != null)
                                    {
                                        // Seat exists, use it
                                        ticketCmd.Parameters.Add(new OracleParameter("seat_id", OracleDbType.Int32)).Value = Convert.ToInt32(seatIdResult);
                                    }
                                    else
                                    {
                                        // Create new seat for any hall
                                        string createSeatQuery = @"INSERT INTO seat (seat_id, hall_id, seat_label) 
                                                              VALUES (seat_seq.NEXTVAL, 
                                                                     (SELECT hall_id FROM showtime WHERE showtime_id = :showtime_id), 
                                                                     :seat_label)";
                                        using (OracleCommand createCmd = new OracleCommand(createSeatQuery, conn))
                                        {
                                            createCmd.Transaction = trans;
                                            createCmd.Parameters.Add(new OracleParameter("showtime_id", OracleDbType.Int32)).Value = showtimeId;
                                            createCmd.Parameters.Add(new OracleParameter("seat_label", OracleDbType.Varchar2)).Value = seatNumber;
                                            createCmd.ExecuteNonQuery();
                                            
                                            // Get the new seat_id
                                            using (OracleCommand getNewIdCmd = new OracleCommand("SELECT seat_seq.CURRVAL FROM dual", conn))
                                            {
                                                getNewIdCmd.Transaction = trans;
                                                int newSeatId = Convert.ToInt32(getNewIdCmd.ExecuteScalar());
                                                ticketCmd.Parameters.Add(new OracleParameter("seat_id", OracleDbType.Int32)).Value = newSeatId;
                                            }
                                        }
                                    }
                                }
                                
                                ticketCmd.Parameters.Add(new OracleParameter("price", OracleDbType.Decimal)).Value = price;
                                ticketCmd.ExecuteNonQuery();
                            }

                            trans.Commit();
                            lblMessage.Text = "✓ Booking status changed to BOOKED";
                            lblMessage.ForeColor = System.Drawing.Color.Green;
                            LoadBookings();
                            LoadAvailableSeats(showtimeId); // Refresh seat availability
                        }
                        catch (OracleException ex)
                        {
                            trans.Rollback();
                            if (ex.Number == 1) // unique constraint violation
                            {
                                lblMessage.Text = "ERROR: Seat already booked. Please select another seat.";
                            }
                            else
                            {
                                lblMessage.Text = "Database Error: " + ex.Message;
                            }
                            lblMessage.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void ddlShowtime_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlShowtime.SelectedValue != "0")
            {
                int showtimeId = Convert.ToInt32(ddlShowtime.SelectedValue);
                LoadAvailableSeats(showtimeId);
                ShowPrice(showtimeId);
            }
            else
            {
                LoadSeats(); // Load all seats if no showtime selected
                lblPrice.Text = "Price: ";
                lblPrice.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void ShowPrice(int showtimeId)
        {
            string connString = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connString))
            {
                conn.Open();
                string priceQuery = @"SELECT ticket_price, price_type FROM showtime_pricing 
                                    WHERE showtime_id = :showtime_id AND price_type = 'NORMAL'
                                    UNION ALL
                                    SELECT ticket_price, price_type FROM showtime_pricing 
                                    WHERE showtime_id = :showtime_id AND ROWNUM = 1";
                using (OracleCommand cmd = new OracleCommand(priceQuery, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("showtime_id", OracleDbType.Int32)).Value = showtimeId;
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            decimal price = Convert.ToDecimal(reader["ticket_price"]);
                            string priceType = reader["price_type"].ToString();
                            lblPrice.Text = "Price: " + price.ToString("N2") + " (" + priceType + ")";
                        }
                        else
                        {
                            lblPrice.Text = "Price: Not Available - Please set up pricing for this showtime";
                            lblPrice.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                }
            }
        }

        private void LoadShowtimesForPricing()
        {
            string connString = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connString))
            {
                conn.Open();
                string query = @"SELECT st.showtime_id, m.movie_title, h.hall_name, st.show_start 
                                FROM showtime st 
                                JOIN movie m ON st.movie_id = m.movie_id 
                                JOIN hall h ON st.hall_id = h.hall_id 
                                ORDER BY st.show_start";
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        ddlShowtimePrice.Items.Clear();
                        ddlShowtimePrice.Items.Add(new ListItem("--Select Showtime--", "0"));
                        while (reader.Read())
                        {
                            string displayText = $"{reader["showtime_id"]} - {reader["movie_title"]} ({reader["hall_name"]})";
                            ddlShowtimePrice.Items.Add(new ListItem(displayText, reader["showtime_id"].ToString()));
                        }
                    }
                }
            }
        }

        private void LoadPricing()
        {
            string connString = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connString))
            {
                conn.Open();
                string query = @"SELECT sp.showtime_id, m.movie_title, h.hall_name, sp.price_type, sp.ticket_price 
                                FROM showtime_pricing sp 
                                JOIN showtime st ON sp.showtime_id = st.showtime_id 
                                JOIN movie m ON st.movie_id = m.movie_id 
                                JOIN hall h ON st.hall_id = h.hall_id 
                                ORDER BY sp.showtime_id, sp.price_type";
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        System.Data.DataTable dt = new System.Data.DataTable();
                        da.Fill(dt);
                        gvPricing.DataSource = dt;
                        gvPricing.DataBind();
                    }
                }
            }
        }

        private void LoadExistingPricing(int showtimeId)
        {
            string connString = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connString))
            {
                conn.Open();
                string query = "SELECT price_type, ticket_price FROM showtime_pricing WHERE showtime_id = :showtime_id AND price_type = :price_type";
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("showtime_id", OracleDbType.Int32)).Value = showtimeId;
                    cmd.Parameters.Add(new OracleParameter("price_type", OracleDbType.Varchar2)).Value = ddlPriceType.SelectedValue;
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Don't change the dropdown selection, just update the price textbox
                            txtTicketPrice.Text = Convert.ToDecimal(reader["ticket_price"]).ToString("N2");
                        }
                        else
                        {
                            txtTicketPrice.Text = "";
                        }
                    }
                }
            }
        }

        protected void ddlShowtimePrice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlShowtimePrice.SelectedValue != "0")
            {
                int showtimeId = Convert.ToInt32(ddlShowtimePrice.SelectedValue);
                ddlPriceType.SelectedValue = "NORMAL";
                LoadExistingPricing(showtimeId);
            }
            else
            {
                txtTicketPrice.Text = "";
                ddlPriceType.SelectedValue = "NORMAL";
            }
        }

        protected void ddlPriceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlShowtimePrice.SelectedValue != "0")
            {
                int showtimeId = Convert.ToInt32(ddlShowtimePrice.SelectedValue);
                LoadExistingPricing(showtimeId);
            }
            else
            {
                txtTicketPrice.Text = "";
            }
        }

        protected void btnAddPricing_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlShowtimePrice.SelectedValue == "0")
                {
                    lblMessage.Text = "Please select a showtime.";
                    return;
                }

                if (string.IsNullOrEmpty(txtTicketPrice.Text))
                {
                    lblMessage.Text = "Please enter a ticket price.";
                    return;
                }

                decimal price;
                if (!decimal.TryParse(txtTicketPrice.Text.Trim(), out price) || price <= 0)
                {
                    lblMessage.Text = "Please enter a valid positive number for price.";
                    return;
                }

                int showtimeId = Convert.ToInt32(ddlShowtimePrice.SelectedValue);
                string priceType = ddlPriceType.SelectedValue;

                string connString = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;
                using (OracleConnection conn = new OracleConnection(connString))
                {
                    conn.Open();

                    string insertQuery = "INSERT INTO showtime_pricing (pricing_id, showtime_id, price_type, ticket_price) VALUES (pricing_seq.NEXTVAL, :showtime_id, :price_type, :ticket_price)";
                    using (OracleCommand cmd = new OracleCommand(insertQuery, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("showtime_id", OracleDbType.Int32)).Value = showtimeId;
                        cmd.Parameters.Add(new OracleParameter("price_type", OracleDbType.Varchar2)).Value = priceType;
                        cmd.Parameters.Add(new OracleParameter("ticket_price", OracleDbType.Decimal)).Value = price;
                        cmd.ExecuteNonQuery();
                    }

                    lblMessage.Text = "Pricing added successfully.";
                }

                LoadPricing();
            }
            catch (OracleException ex)
            {
                if (ex.Number == 1)
                {
                    lblMessage.Text = "This pricing already exists.";
                }
                else
                {
                    lblMessage.Text = "Database Error: " + ex.Message;
                }
            }
            catch (FormatException)
            {
                lblMessage.Text = "Invalid price format. Please enter a valid number.";
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
            }
        }

        protected void gvBookings_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int bookingId = Convert.ToInt32(e.CommandArgument);
                string connString = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;

                using (OracleConnection conn = new OracleConnection(connString))
                {
                    conn.Open();

                    if (e.CommandName == "Paid")
                    {
                        // Check current booking status
                        string statusQuery = "SELECT booking_status FROM booking WHERE booking_id = :booking_id";
                        string status;
                        using (OracleCommand cmd = new OracleCommand(statusQuery, conn))
                        {
                            cmd.Parameters.Add(new OracleParameter("booking_id", bookingId));
                            status = (string)cmd.ExecuteScalar();
                        }

                        if (status == "CANCELLED" || status == "EXPIRED")
                        {
                            lblMessage.Text = $"Cannot mark booking {bookingId} as paid. Status is {status}.";
                            lblMessage.ForeColor = System.Drawing.Color.Red;
                            return;
                        }

                        if (status == "PAID")
                        {
                            lblMessage.Text = $"Booking {bookingId} is already paid.";
                            lblMessage.ForeColor = System.Drawing.Color.Orange;
                            return;
                        }

                        // Get ticket_id for this booking
                        int ticketId;
                        string getTicketQuery = "SELECT ticket_id FROM ticket WHERE booking_id = :booking_id";
                        using (OracleCommand cmd = new OracleCommand(getTicketQuery, conn))
                        {
                            cmd.Parameters.Add(new OracleParameter("booking_id", bookingId));
                            ticketId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        using (OracleTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                // Check if payment already exists
                                string checkPaymentQuery = "SELECT COUNT(*) FROM payment WHERE ticket_id = :ticket_id";
                                bool paymentExists = false;
                                using (OracleCommand checkCmd = new OracleCommand(checkPaymentQuery, conn))
                                {
                                    checkCmd.Transaction = trans;
                                    checkCmd.Parameters.Add(new OracleParameter("ticket_id", ticketId));
                                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                                    if (count > 0)
                                    {
                                        paymentExists = true;
                                    }
                                }

                                if (paymentExists)
                                {
                                    lblMessage.Text = $"Payment already exists for booking {bookingId}.";
                                    lblMessage.ForeColor = System.Drawing.Color.Orange;
                                    return;
                                }

                                // Insert payment
                                string insertPayment = "INSERT INTO payment (payment_id, ticket_id, payment_status, paid_at) VALUES (payment_seq.NEXTVAL, :ticket_id, 'PAID', SYSTIMESTAMP)";
                                using (OracleCommand cmd = new OracleCommand(insertPayment, conn))
                                {
                                    cmd.Transaction = trans;
                                    cmd.Parameters.Add(new OracleParameter("ticket_id", ticketId));
                                    cmd.ExecuteNonQuery();
                                }

                                // Update booking status to PAID
                                string updateBooking = "UPDATE booking SET booking_status = 'PAID' WHERE booking_id = :booking_id";
                                using (OracleCommand cmd = new OracleCommand(updateBooking, conn))
                                {
                                    cmd.Transaction = trans;
                                    cmd.Parameters.Add(new OracleParameter("booking_id", bookingId));
                                    cmd.ExecuteNonQuery();
                                }

                                trans.Commit();
                                lblMessage.Text = "✓ Booking status changed to PAID";
                                lblMessage.ForeColor = System.Drawing.Color.Green;
                                LoadBookings(); // Refresh the grid
                            }
                            catch (Exception ex)
                            {
                                trans.Rollback();
                                lblMessage.Text = "Error processing payment: " + ex.Message;
                                lblMessage.ForeColor = System.Drawing.Color.Red;
                            }
                        }
                    }
                    else if (e.CommandName == "CancelBooking")
                    {
                        // Check current booking status
                        string statusQuery = "SELECT booking_status FROM booking WHERE booking_id = :booking_id";
                        string status;
                        using (OracleCommand cmd = new OracleCommand(statusQuery, conn))
                        {
                            cmd.Parameters.Add(new OracleParameter("booking_id", bookingId));
                            status = (string)cmd.ExecuteScalar();
                        }

                        if (status == "CANCELLED")
                        {
                            lblMessage.Text = $"Booking {bookingId} is already cancelled.";
                            lblMessage.ForeColor = System.Drawing.Color.Orange;
                            return;
                        }

                        if (status == "PAID")
                        {
                            lblMessage.Text = $"Cannot cancel booking {bookingId}. Status is PAID.";
                            lblMessage.ForeColor = System.Drawing.Color.Red;
                            return;
                        }

                        // Update booking status to CANCELLED
                        using (OracleTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                string updateBooking = "UPDATE booking SET booking_status = 'CANCELLED' WHERE booking_id = :booking_id";
                                using (OracleCommand cmd = new OracleCommand(updateBooking, conn))
                                {
                                    cmd.Transaction = trans;
                                    cmd.Parameters.Add(new OracleParameter("booking_id", bookingId));
                                    cmd.ExecuteNonQuery();
                                }

                                trans.Commit();
                                lblMessage.Text = "✓ Booking status changed to CANCELLED";
                                lblMessage.ForeColor = System.Drawing.Color.Green;
                                LoadBookings(); // Refresh the grid

                                // Refresh seat availability if showtime is selected
                                if (ddlShowtime.SelectedValue != "0")
                                {
                                    int currentShowtimeId = Convert.ToInt32(ddlShowtime.SelectedValue);
                                    LoadAvailableSeats(currentShowtimeId);
                                }
                            }
                            catch (Exception ex)
                            {
                                trans.Rollback();
                                lblMessage.Text = "Error cancelling booking: " + ex.Message;
                                lblMessage.ForeColor = System.Drawing.Color.Red;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnSwitchPrice_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlShowtime.SelectedValue == "0")
                {
                    lblPrice.Text = "Please select a showtime first.";
                    lblPrice.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                int showtimeId = Convert.ToInt32(ddlShowtime.SelectedValue);
                string priceType = ddlBookingPriceType.SelectedValue;

                string connString = ConfigurationManager.ConnectionStrings["OracleDb"].ConnectionString;
                using (OracleConnection conn = new OracleConnection(connString))
                {
                    conn.Open();
                    string query = "SELECT ticket_price FROM showtime_pricing WHERE showtime_id = :showtime_id AND price_type = :price_type";
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("showtime_id", OracleDbType.Int32)).Value = showtimeId;
                        cmd.Parameters.Add(new OracleParameter("price_type", OracleDbType.Varchar2)).Value = priceType;
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            decimal price = Convert.ToDecimal(result);
                            lblPrice.Text = $"Price: {price.ToString("N2")} ({priceType})";
                            lblPrice.ForeColor = System.Drawing.Color.Green;
                        }
                        else
                        {
                            lblPrice.Text = $"Price: Not Available - No {priceType} pricing set for this showtime";
                            lblPrice.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblPrice.Text = "Error loading price: " + ex.Message;
                lblPrice.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}
/* =========================================================
   KUMARI CINEMAS  DATABASE SCHEMA
   ========================================================= */

/* ================================
   USERS
   ================================ */
CREATE TABLE app_user (
    user_id NUMBER(10) PRIMARY KEY,
    user_name VARCHAR2(100) NOT NULL,
    user_address VARCHAR2(255) NOT NULL
);


/* ================================
   MOVIE
   ================================ */
CREATE TABLE movie (
    movie_id NUMBER(10) PRIMARY KEY,
    movie_title VARCHAR2(150) NOT NULL,
    duration_minutes NUMBER(5) NOT NULL,
    language VARCHAR2(50) NOT NULL,
    genre VARCHAR2(50) NOT NULL,
    release_date DATE NOT NULL
);


/* ================================
   THEATER
   ================================ */
CREATE TABLE theater (
    theater_id NUMBER(10) PRIMARY KEY,
    theater_name VARCHAR2(150) NOT NULL,
    theater_city VARCHAR2(100) NOT NULL
);

/* ================================
   HALL
   ================================ */
CREATE TABLE hall (
    hall_id NUMBER(10) PRIMARY KEY,
    theater_id NUMBER(10) NOT NULL,
    hall_name VARCHAR2(50) NOT NULL,
    hall_capacity NUMBER(5) NOT NULL,
    CONSTRAINT fk_hall_theater
        FOREIGN KEY (theater_id)
        REFERENCES theater(theater_id)
);
/* ================================
   SEAT
   ================================ */
CREATE TABLE seat (
    seat_id NUMBER(10) PRIMARY KEY,
    hall_id NUMBER(10) NOT NULL,
    seat_label VARCHAR2(10) NOT NULL,
    CONSTRAINT fk_seat_hall
        FOREIGN KEY (hall_id)
        REFERENCES hall(hall_id),
    CONSTRAINT uq_hall_seat
        UNIQUE (hall_id, seat_label)
);


/* ================================
   SHOWTIME
   ================================ */
CREATE TABLE showtime (
    showtime_id NUMBER(10) PRIMARY KEY,
    movie_id NUMBER(10) NOT NULL,
    hall_id NUMBER(10) NOT NULL,
    show_start TIMESTAMP NOT NULL,
    CONSTRAINT fk_showtime_movie
        FOREIGN KEY (movie_id)
        REFERENCES movie(movie_id),
    CONSTRAINT fk_showtime_hall
        FOREIGN KEY (hall_id)
        REFERENCES hall(hall_id),
    CONSTRAINT uq_hall_show
        UNIQUE (hall_id, show_start)
);

/* ================================
   SHOWTIME PRICING
   ================================ */
CREATE TABLE showtime_pricing (
    pricing_id NUMBER(10) PRIMARY KEY,
    showtime_id NUMBER(10) NOT NULL,
    price_type VARCHAR2(20) NOT NULL,
    ticket_price NUMBER(8,2) NOT NULL,
    CONSTRAINT fk_pricing_showtime
        FOREIGN KEY (showtime_id)
        REFERENCES showtime(showtime_id),
    CONSTRAINT uq_show_price
        UNIQUE (showtime_id, price_type),
    CONSTRAINT chk_price_type
        CHECK (price_type IN ('NORMAL','HOLIDAY','NEW_RELEASE'))
);


/* ================================
   BOOKING
   ================================ */
CREATE TABLE booking (
    booking_id NUMBER(10) PRIMARY KEY,
    user_id NUMBER(10) NOT NULL,
    showtime_id NUMBER(10) NOT NULL,
    booking_created_at TIMESTAMP DEFAULT SYSTIMESTAMP,
    booking_expires_at TIMESTAMP NOT NULL,
    booking_status VARCHAR2(20) NOT NULL,
    CONSTRAINT fk_booking_user
        FOREIGN KEY (user_id)
        REFERENCES app_user(user_id),
    CONSTRAINT fk_booking_showtime
        FOREIGN KEY (showtime_id)
        REFERENCES showtime(showtime_id),
    CONSTRAINT chk_booking_status
        CHECK (booking_status IN ('BOOKED','PAID','CANCELLED','EXPIRED'))
);


/* ================================
   TICKET
   ================================ */
CREATE TABLE ticket (
    ticket_id NUMBER(10) PRIMARY KEY,
    booking_id NUMBER(10) NOT NULL,
    showtime_id NUMBER(10) NOT NULL,
    seat_id NUMBER(10) NOT NULL,
    final_price NUMBER(8,2) NOT NULL,
    CONSTRAINT fk_ticket_booking
        FOREIGN KEY (booking_id)
        REFERENCES booking(booking_id),
    CONSTRAINT fk_ticket_showtime
        FOREIGN KEY (showtime_id)
        REFERENCES showtime(showtime_id),
    CONSTRAINT fk_ticket_seat
        FOREIGN KEY (seat_id)
        REFERENCES seat(seat_id),
    CONSTRAINT uq_showtime_seat
        UNIQUE (showtime_id, seat_id)
);





/* ================================
   PAYMENT
   ================================ */
CREATE TABLE payment (
    payment_id NUMBER(10) PRIMARY KEY,
    ticket_id NUMBER(10) NOT NULL UNIQUE,
    paid_at TIMESTAMP,
    payment_status VARCHAR2(20) NOT NULL,
    CONSTRAINT fk_payment_ticket
        FOREIGN KEY (ticket_id)
        REFERENCES ticket(ticket_id),
    CONSTRAINT chk_payment_status
        CHECK (payment_status IN ('PAID','REFUNDED'))
);
CREATE SEQUENCE user_seq
START WITH 6
INCREMENT BY 1
NOCACHE
NOCYCLE;

CREATE SEQUENCE booking_seq
START WITH 1000
INCREMENT BY 1
NOCACHE
NOCYCLE;

CREATE SEQUENCE ticket_seq
START WITH 1000
INCREMENT BY 1
NOCACHE
NOCYCLE;

CREATE SEQUENCE payment_seq
START WITH 1000
INCREMENT BY 1
NOCACHE
NOCYCLE;

CREATE SEQUENCE seat_seq
START WITH 751
INCREMENT BY 1
NOCACHE
NOCYCLE;

CREATE SEQUENCE pricing_seq
START WITH 601
INCREMENT BY 1
NOCACHE
NOCYCLE;


/* =========================================
    DEMO DATA INSERT 
   ========================================= */

/* -------------------------------
   1. USERS
---------------------------------*/
INSERT INTO app_user VALUES (1,'Ram Shrestha','Kathmandu');
INSERT INTO app_user VALUES (2,'Sita Karki','Pokhara');
INSERT INTO app_user VALUES (3,'Hari Thapa','Lalitpur');
INSERT INTO app_user VALUES (4,'Gita Rai','Biratnagar');
INSERT INTO app_user VALUES (5,'Suman Lama','Butwal');

/* -------------------------------
   2. MOVIES
---------------------------------*/
INSERT INTO movie VALUES (101,'Avatar Fire and Ash',192,'English','Fiction',DATE '2025-12-19');
INSERT INTO movie VALUES (102,'Jatra 3',140,'Nepali','Comedy',DATE '2025-10-10');
INSERT INTO movie VALUES (103,'KGF 2',168,'Hindi','Action',DATE '2024-04-14');
INSERT INTO movie VALUES (104,'Animal',180,'Hindi','Drama',DATE '2025-01-01');
INSERT INTO movie VALUES (105,'Chhakka Panja 5',150,'Nepali','Comedy',DATE '2025-09-01');

/* -------------------------------
   3. THEATERS
---------------------------------*/
INSERT INTO theater VALUES (201,'Kumari Cinemas','Pokhara');
INSERT INTO theater VALUES (202,'Labim Mall Cinema','Lalitpur');
INSERT INTO theater VALUES (203,'QFX Cinema','Kathmandu');

/* -------------------------------
   4. HALLS
---------------------------------*/
INSERT INTO hall VALUES (301,201,'Hall A',200);
INSERT INTO hall VALUES (302,201,'Hall B',180);
INSERT INTO hall VALUES (303,202,'Hall C',150);
INSERT INTO hall VALUES (304,203,'Hall D',220);

/* -------------------------------
   5. SEATS (Numeric Labels Only)
---------------------------------*/
-- Hall A (capacity 200): 1-200
INSERT INTO seat SELECT seat_seq.NEXTVAL, 301, LEVEL FROM dual CONNECT BY LEVEL <= 200;

-- Hall B (capacity 180): 1-180  
INSERT INTO seat SELECT seat_seq.NEXTVAL, 302, LEVEL FROM dual CONNECT BY LEVEL <= 180;

-- Hall C (capacity 150): 1-150
INSERT INTO seat SELECT seat_seq.NEXTVAL, 303, LEVEL FROM dual CONNECT BY LEVEL <= 150;

-- Hall D (capacity 220): 1-220
INSERT INTO seat SELECT seat_seq.NEXTVAL, 304, LEVEL FROM dual CONNECT BY LEVEL <= 220;

/* -------------------------------
   6. SHOWTIMES
---------------------------------*/
INSERT INTO showtime VALUES (501,101,301,TIMESTAMP '2026-12-21 10:00:00');
INSERT INTO showtime VALUES (502,101,302,TIMESTAMP '2026-12-21 13:00:00');
INSERT INTO showtime VALUES (503,102,303,TIMESTAMP '2026-12-22 18:00:00');
INSERT INTO showtime VALUES (504,103,304,TIMESTAMP '2026-12-23 15:00:00');

/* -------------------------------
   7. SHOWTIME PRICING
---------------------------------*/
INSERT INTO showtime_pricing VALUES (601,501,'NORMAL',390);
INSERT INTO showtime_pricing VALUES (602,501,'HOLIDAY',450);
INSERT INTO showtime_pricing VALUES (603,502,'NEW_RELEASE',420);
INSERT INTO showtime_pricing VALUES (604,502,'NORMAL',390);
INSERT INTO showtime_pricing VALUES (605,503,'NORMAL',350);
INSERT INTO showtime_pricing VALUES (606,504,'NORMAL',300);

/* -------------------------------
   8. BOOKINGS
---------------------------------*/
INSERT INTO booking VALUES (701,1,501,SYSTIMESTAMP,SYSTIMESTAMP + INTERVAL '1' HOUR,'PAID');
INSERT INTO booking VALUES (702,2,501,SYSTIMESTAMP,SYSTIMESTAMP + INTERVAL '1' HOUR,'PAID');
INSERT INTO booking VALUES (703,3,502,SYSTIMESTAMP,SYSTIMESTAMP + INTERVAL '1' HOUR,'BOOKED');
INSERT INTO booking VALUES (704,4,503,SYSTIMESTAMP,SYSTIMESTAMP + INTERVAL '1' HOUR,'CANCELLED');
INSERT INTO booking VALUES (705,5,504,SYSTIMESTAMP,SYSTIMESTAMP + INTERVAL '1' HOUR,'PAID');

/* -------------------------------
   9. TICKETS
---------------------------------*/
INSERT INTO ticket VALUES (801,701,501,751,390);  -- Hall A - Seat 1
INSERT INTO ticket VALUES (802,702,501,752,390);  -- Hall A - Seat 2
INSERT INTO ticket VALUES (803,703,502,951,420);  -- Hall B - Seat 1
INSERT INTO ticket VALUES (804,705,504,1281,300); -- Hall D - Seat 1

/* -------------------------------
   10. PAYMENTS
---------------------------------*/
INSERT INTO payment VALUES (901,801,SYSTIMESTAMP,'PAID');
INSERT INTO payment VALUES (902,802,SYSTIMESTAMP,'PAID');
INSERT INTO payment VALUES (903,804,SYSTIMESTAMP,'PAID');

COMMIT;



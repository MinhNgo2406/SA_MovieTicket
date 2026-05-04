USE master;
GO

-- Xóa DB cũ nếu đã tồn tại để làm mới hoàn toàn
IF DB_ID('MovieBooking') IS NOT NULL
BEGIN
    ALTER DATABASE MovieBooking SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE MovieBooking;
END
GO

-- Tạo DB mới
CREATE DATABASE MovieBooking;
GO

USE MovieBooking;
GO

-- ==========================================
-- 1. TẠO BẢNG
-- ==========================================

CREATE TABLE [User] (
    Id int IDENTITY(1,1) PRIMARY KEY ,
    Name NVARCHAR(100),
    Email NVARCHAR(100),
    Phone NVARCHAR(20),
    Role NVARCHAR(50),
    Password varchar(255)
);

CREATE TABLE Movies (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200),
    Description NVARCHAR(MAX),
    Image NVARCHAR(255),
    ReleaseDate DATETIME,
    Duration INT,  
    Rating FLOAT
);

CREATE TABLE Cinema (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100),
    Address NVARCHAR(255),
    City NVARCHAR(100),
    Hotline NVARCHAR(20),
    Status NVARCHAR(50)
);

CREATE TABLE Room (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100),
    Capacity INT,
    CinemaId INT,
    FOREIGN KEY (CinemaId) REFERENCES Cinema(Id)
);

CREATE TABLE Seat (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RoomId INT,
    RowName NVARCHAR(10),
    SeatNumber INT,
    FOREIGN KEY (RoomId) REFERENCES Room(Id)
);

CREATE TABLE Showtime (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    MovieId INT,
    RoomId INT,
    StartTime DATETIME,
    EndTime DATETIME,
    FOREIGN KEY (MovieId) REFERENCES Movies(Id),
    FOREIGN KEY (RoomId) REFERENCES Room(Id)
);

CREATE TABLE ShowtimeSeat (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ShowtimeId INT,
    SeatId INT,
    Status NVARCHAR(50),
    FOREIGN KEY (ShowtimeId) REFERENCES Showtime(Id),
    FOREIGN KEY (SeatId) REFERENCES Seat(Id)
);

CREATE TABLE [Order] (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId int,
    TotalAmount DECIMAL(10,2),
    Status NVARCHAR(50),
    CreatedAt DATETIME,
    FOREIGN KEY (UserId) REFERENCES [User](Id)
);

CREATE TABLE Ticket (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    ShowTimeId INT NOT NULL,
    SeatId INT NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    Status NVARCHAR(50) DEFAULT 'Booked',
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (OrderId) REFERENCES [Order](Id),
    FOREIGN KEY (ShowTimeId) REFERENCES Showtime(Id),
    FOREIGN KEY (SeatId) REFERENCES Seat(Id),
    CONSTRAINT UQ_Ticket UNIQUE (ShowTimeId, SeatId)
);

CREATE TABLE Payment (
    Id int IDENTITY(1,1) PRIMARY KEY,
    OrderId int,
    Amount DECIMAL(10,2),
    PaymentMethod NVARCHAR(50),
    TransactionId NVARCHAR(100),
    Status NVARCHAR(50),
    PaymentDate DATETIME,
    FOREIGN KEY (OrderId) REFERENCES [Order](Id)
);

CREATE TABLE Review (
    Id int IDENTITY(1,1) PRIMARY KEY ,
    UserId int,
    MovieId INT,
    Rating INT,
    Comment NVARCHAR(MAX),
    CreatedAt DATETIME,
    FOREIGN KEY (UserId) REFERENCES [User](Id),
    FOREIGN KEY (MovieId) REFERENCES Movies(Id)
);
GO

-- ==========================================
-- 2. RÀNG BUỘC VÀ TRIGGER
-- ==========================================

ALTER TABLE Seat
ADD CONSTRAINT UQ_Seat_Room_Row_Number
UNIQUE (RoomId, RowName, SeatNumber);
GO

ALTER TABLE Movies
ADD CONSTRAINT UQ_Movies UNIQUE (Title, ReleaseDate);
GO

CREATE TRIGGER TRG_Prevent_Duplicate_Showtime
ON Showtime
AFTER INSERT, UPDATE
AS
BEGIN
    IF EXISTS (
        SELECT 1
        FROM Showtime s
        JOIN inserted i 
            ON s.MovieId = i.MovieId
            AND s.StartTime = i.StartTime
            AND s.Id <> i.Id
    )
    BEGIN
        RAISERROR (N'Đã tồn tại suất chiếu cùng phim và cùng giờ!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO

-- ==========================================
-- 3. CHÈN DỮ LIỆU
-- ==========================================

INSERT INTO [User] (Name, Email, Phone, Role, Password) VALUES
(N'Nguyễn Văn A', 'a@gmail.com', '0900000001', 'User', '123456'),
(N'Trần Thị B', 'b@gmail.com', '0900000002', 'User', '123456'),
(N'Lê Văn C', 'c@gmail.com', '0900000003', 'User', '123456'),
(N'Phạm Thị D', 'd@gmail.com', '0900000004', 'User', '123456'),
(N'Hoàng Văn E', 'e@gmail.com', '0900000005', 'User', '123456'),
(N'Võ Thị F', 'f@gmail.com', '0900000006', 'User', '123456'),
(N'Đỗ Văn G', 'g@gmail.com', '0900000007', 'User', '123456'),
(N'Bùi Thị H', 'h@gmail.com', '0900000008', 'User', '123456'),
(N'Ngô Văn I', 'i@gmail.com', '0900000009', 'User', '123456'),
(N'Admin', 'admin@gmail.com', '0999999999', 'Admin', 'admin123');

INSERT INTO Cinema (Name, Address, City, Hotline, Status) VALUES
(N'CGV Vincom', N'Q1', N'HCM', '19006017', 'Active'),
(N'Lotte Cinema', N'Q7', N'HCM', '19001225', 'Active'),
(N'Galaxy Cinema', N'Q10', N'HCM', '19002224', 'Active');

INSERT INTO Movies (Title, Description, Image, ReleaseDate, Duration, Rating) VALUES
(N'Avengers: Endgame', N'Marvel', 'endgame.jpg', '2019-04-26', 181, 8.4),
(N'Spider-Man: No Way Home', N'Siêu anh hùng', 'spiderman.jpg', '2021-12-17', 148, 8.2),
(N'Fast & Furious 9', N'Đua xe', 'ff9.jpg', '2021-06-25', 145, 5.2),
(N'The Batman', N'Người dơi', 'batman.jpg', '2022-03-04', 176, 7.8),
(N'John Wick 4', N'Sát thủ', 'johnwick4.jpg', '2023-03-24', 169, 8.0),
(N'Avatar 2', N'Viễn tưởng', 'avatar2.jpg', '2022-12-16', 192, 7.6),
(N'Inception', N'Giấc mơ', 'inception.jpg', '2010-07-16', 148, 8.8),
(N'Interstellar', N'Không gian', 'interstellar.jpg', '2014-11-07', 169, 8.6),
(N'Oppenheimer', N'Phim về cha đẻ bom nguyên tử', 'oppenheimer.jpg', '2023-07-21', 180, 8.7),
(N'Barbie', N'Phim hài giả tưởng', 'barbie.jpg', '2023-07-21', 114, 7.0),
(N'The Nun II', N'Kinh dị', 'nun2.jpg', '2023-09-08', 110, 5.6),
(N'Transformers: Rise of the Beasts', N'Robot đại chiến', 'transformers.jpg', '2023-06-09', 127, 6.1),
(N'Mission Impossible: Dead Reckoning', N'Hành động', 'mi7.jpg', '2023-07-12', 163, 7.9),
(N'Guardians of the Galaxy Vol.3', N'Marvel', 'gotg3.jpg', '2023-05-05', 150, 8.0),
(N'Black Panther: Wakanda Forever', N'Marvel', 'blackpanther2.jpg', '2022-11-11', 161, 6.7),
(N'Dune: Part Two', N'Khoa học viễn tưởng', 'dune2.jpg', '2024-03-01', 166, 8.5),
(N'Joker', N'Tâm lý tội phạm', 'joker.jpg', '2019-10-04', 122, 8.4),
(N'Titanic', N'Tình cảm', 'titanic.jpg', '1997-12-19', 195, 7.9),
(N'The Conjuring', N'Kinh dị', 'conjuring.jpg', '2013-07-19', 112, 7.5),
(N'Annabelle', N'Kinh dị', 'annabelle.jpg', '2014-10-03', 99, 5.4),
(N'Doctor Strange', N'Marvel phép thuật', 'drstrange.jpg', '2016-11-04', 115, 7.5),
(N'Captain America: Civil War', N'Marvel', 'civilwar.jpg', '2016-05-06', 147, 7.8),
(N'Iron Man', N'Marvel', 'ironman.jpg', '2008-05-02', 126, 7.9),
(N'Thor: Ragnarok', N'Marvel', 'thor3.jpg', '2017-11-03', 130, 7.9),
(N'Shutter Island', N'Tâm lý', 'shutter.jpg', '2010-02-19', 138, 8.2),
(N'The Matrix', N'Khoa học viễn tưởng', 'matrix.jpg', '1999-03-31', 136, 8.7),
(N'Parasite', N'Phim Hàn', 'parasite.jpg', '2019-05-30', 132, 8.6),
(N'Your Name', N'Anime tình cảm', 'yourname.jpg', '2016-08-26', 106, 8.4);

INSERT INTO Room (Name, Capacity, CinemaId) VALUES
(N'Room 1', 50, 1),
(N'Room 2', 40, 1),
(N'Room 3', 60, 2),
(N'Room 4', 45, 2),
(N'Room 5', 70, 3),
(N'Room 6', 30, 3);

INSERT INTO Showtime (MovieId, RoomId, StartTime, EndTime) VALUES
(1,1,'2026-05-01 10:00','2026-05-01 12:30'),
(1,1,'2026-05-01 18:00','2026-05-01 20:30'),
(2,2,'2026-05-01 19:00','2026-05-01 21:30'),
(3,3,'2026-05-02 20:00','2026-05-02 22:00'),
(4,4,'2026-05-02 21:00','2026-05-02 23:00'),
(5,5,'2026-05-03 18:00','2026-05-03 20:00'),
(7,1,'2026-05-01 14:00','2026-05-01 16:30'),
(8,2,'2026-05-01 15:00','2026-05-01 17:30'),
(9,3,'2026-05-01 16:00','2026-05-01 18:00'),
(10,4,'2026-05-01 17:00','2026-05-01 19:00'),
(11,5,'2026-05-01 18:00','2026-05-01 20:00'),
(12,6,'2026-05-01 19:00','2026-05-01 21:30'),
(13,1,'2026-05-01 20:00','2026-05-01 22:30'),
(14,2,'2026-05-01 21:00','2026-05-01 23:30'),
(15,1,'2026-05-02 08:00','2026-05-02 10:30'),
(16,2,'2026-05-02 09:00','2026-05-02 11:30'),
(17,3,'2026-05-02 10:00','2026-05-02 12:30'),
(18,4,'2026-05-02 11:00','2026-05-02 13:30'),
(19,5,'2026-05-02 12:00','2026-05-02 14:00'),
(20,6,'2026-05-02 13:00','2026-05-02 15:00'),
(21,1,'2026-05-02 14:00','2026-05-02 16:00'),
(22,2,'2026-05-02 15:00','2026-05-02 17:30'),
(23,3,'2026-05-02 16:00','2026-05-02 18:30'),
(24,4,'2026-05-02 17:00','2026-05-02 19:00'),
(25,5,'2026-05-02 18:00','2026-05-02 20:30'),
(26,6,'2026-05-02 19:00','2026-05-02 21:30'),
(27,1,'2026-05-02 20:00','2026-05-02 22:30'),
(28,2,'2026-05-02 21:00','2026-05-02 23:30'),
(1,1,'2026-05-03 08:00','2026-05-03 10:30'),
(2,1,'2026-05-03 10:45','2026-05-03 13:00'),
(3,1,'2026-05-03 13:15','2026-05-03 15:15'),
(4,1,'2026-05-03 15:30','2026-05-03 17:30'),
(5,1,'2026-05-03 17:45','2026-05-03 19:45'),
(6,1,'2026-05-03 20:00','2026-05-03 22:30'),
(7,2,'2026-05-03 08:00','2026-05-03 10:30'),
(8,2,'2026-05-03 10:45','2026-05-03 13:15'),
(9,2,'2026-05-03 13:30','2026-05-03 15:30'),
(10,2,'2026-05-03 15:45','2026-05-03 17:45'),
(11,2,'2026-05-03 18:00','2026-05-03 20:00'),
(12,2,'2026-05-03 20:15','2026-05-03 22:30'),
(13,3,'2026-05-03 08:00','2026-05-03 10:30'),
(14,3,'2026-05-03 10:45','2026-05-03 13:15'),
(15,3,'2026-05-03 13:30','2026-05-03 16:00'),
(16,3,'2026-05-03 16:15','2026-05-03 18:45'),
(17,3,'2026-05-03 19:00','2026-05-03 21:30'),
(18,1,'2026-05-04 08:00','2026-05-04 10:30'),
(19,1,'2026-05-04 10:45','2026-05-04 12:45'),
(20,1,'2026-05-04 13:00','2026-05-04 15:00'),
(21,1,'2026-05-04 15:15','2026-05-04 17:15'),
(22,1,'2026-05-04 17:30','2026-05-04 20:00'),
(23,1,'2026-05-04 20:15','2026-05-04 22:45'),
(24,2,'2026-05-04 08:00','2026-05-04 10:30'),
(25,2,'2026-05-04 10:45','2026-05-04 13:15'),
(26,2,'2026-05-04 13:30','2026-05-04 16:00'),
(27,2,'2026-05-04 16:15','2026-05-04 18:45'),
(28,2,'2026-05-04 19:00','2026-05-04 21:30'),
(1,3,'2026-05-04 08:00','2026-05-04 10:30'),
(2,3,'2026-05-04 10:45','2026-05-04 13:15'),
(3,3,'2026-05-04 13:30','2026-05-04 15:30'),
(4,3,'2026-05-04 15:45','2026-05-04 17:45'),
(5,3,'2026-05-04 18:00','2026-05-04 20:00'),
(6,3,'2026-05-04 20:15','2026-05-04 22:30'),
(7,4,'2026-05-05 08:00','2026-05-05 10:30'),
(8,4,'2026-05-05 10:45','2026-05-05 13:15'),
(9,4,'2026-05-05 13:30','2026-05-05 15:30'),
(10,4,'2026-05-05 15:45','2026-05-05 17:45'),
(11,4,'2026-05-05 18:00','2026-05-05 20:00'),
(12,4,'2026-05-05 20:15','2026-05-05 22:30'),
(13,5,'2026-05-05 08:00','2026-05-05 10:30'),
(14,5,'2026-05-05 10:45','2026-05-05 13:15'),
(15,5,'2026-05-05 13:30','2026-05-05 16:00'),
(16,5,'2026-05-05 16:15','2026-05-05 18:45'),
(17,5,'2026-05-05 19:00','2026-05-05 21:30'),
(18,6,'2026-05-05 08:00','2026-05-05 10:30'),
(19,6,'2026-05-05 10:45','2026-05-05 12:45'),
(20,6,'2026-05-05 13:00','2026-05-05 15:00'),
(21,6,'2026-05-05 15:15','2026-05-05 17:15'),
(22,6,'2026-05-05 17:30','2026-05-05 20:00'),
(23,6,'2026-05-05 20:15','2026-05-05 22:45');

INSERT INTO Seat (RoomId, RowName, SeatNumber) VALUES
(1,'A',1),(1,'A',2),(1,'A',3),(1,'A',4),(1,'A',5),(1,'A',6),(1,'A',7),(1,'A',8),(1,'A',9),(1,'A',10),
(1,'B',1),(1,'B',2),(1,'B',3),(1,'B',4),(1,'B',5),(1,'B',6),(1,'B',7),(1,'B',8),(1,'B',9),(1,'B',10),
(1,'C',1),(1,'C',2),(1,'C',3),(1,'C',4),(1,'C',5),(1,'C',6),(1,'C',7),(1,'C',8),(1,'C',9),(1,'C',10),
(1,'D',1),(1,'D',2),(1,'D',3),(1,'D',4),(1,'D',5),(1,'D',6),(1,'D',7),(1,'D',8),(1,'D',9),(1,'D',10),
(1,'E',1),(1,'E',2),(1,'E',3),(1,'E',4),(1,'E',5),(1,'E',6),(1,'E',7),(1,'E',8),(1,'E',9),(1,'E',10),
(2,'A',1),(2,'A',2),(2,'A',3),(2,'A',4),(2,'A',5),(2,'A',6),(2,'A',7),(2,'A',8),(2,'A',9),(2,'A',10),
(2,'B',1),(2,'B',2),(2,'B',3),(2,'B',4),(2,'B',5),(2,'B',6),(2,'B',7),(2,'B',8),(2,'B',9),(2,'B',10),
(2,'C',1),(2,'C',2),(2,'C',3),(2,'C',4),(2,'C',5),(2,'C',6),(2,'C',7),(2,'C',8),(2,'C',9),(2,'C',10),
(2,'D',1),(2,'D',2),(2,'D',3),(2,'D',4),(2,'D',5),(2,'D',6),(2,'D',7),(2,'D',8),(2,'D',9),(2,'D',10),
(3,'A',1),(3,'A',2),(3,'A',3),(3,'A',4),(3,'A',5),(3,'A',6),(3,'A',7),(3,'A',8),(3,'A',9),(3,'A',10),
(3,'B',1),(3,'B',2),(3,'B',3),(3,'B',4),(3,'B',5),(3,'B',6),(3,'B',7),(3,'B',8),(3,'B',9),(3,'B',10),
(3,'C',1),(3,'C',2),(3,'C',3),(3,'C',4),(3,'C',5),(3,'C',6),(3,'C',7),(3,'C',8),(3,'C',9),(3,'C',10),
(3,'D',1),(3,'D',2),(3,'D',3),(3,'D',4),(3,'D',5),(3,'D',6),(3,'D',7),(3,'D',8),(3,'D',9),(3,'D',10),
(3,'E',1),(3,'E',2),(3,'E',3),(3,'E',4),(3,'E',5),(3,'E',6),(3,'E',7),(3,'E',8),(3,'E',9),(3,'E',10),
(3,'F',1),(3,'F',2),(3,'F',3),(3,'F',4),(3,'F',5),(3,'F',6),(3,'F',7),(3,'F',8),(3,'F',9),(3,'F',10),
(4,'A',1),(4,'A',2),(4,'A',3),(4,'A',4),(4,'A',5),(4,'A',6),(4,'A',7),(4,'A',8),(4,'A',9),(4,'A',10),
(4,'B',1),(4,'B',2),(4,'B',3),(4,'B',4),(4,'B',5),(4,'B',6),(4,'B',7),(4,'B',8),(4,'B',9),(4,'B',10),
(4,'C',1),(4,'C',2),(4,'C',3),(4,'C',4),(4,'C',5),(4,'C',6),(4,'C',7),(4,'C',8),(4,'C',9),(4,'C',10),
(4,'D',1),(4,'D',2),(4,'D',3),(4,'D',4),(4,'D',5),(4,'D',6),(4,'D',7),(4,'D',8),(4,'D',9),(4,'D',10),
(4,'E',1),(4,'E',2),(4,'E',3),(4,'E',4),(4,'E',5),
(5,'A',1),(5,'A',2),(5,'A',3),(5,'A',4),(5,'A',5),(5,'A',6),(5,'A',7),(5,'A',8),(5,'A',9),(5,'A',10),
(5,'B',1),(5,'B',2),(5,'B',3),(5,'B',4),(5,'B',5),(5,'B',6),(5,'B',7),(5,'B',8),(5,'B',9),(5,'B',10),
(5,'C',1),(5,'C',2),(5,'C',3),(5,'C',4),(5,'C',5),(5,'C',6),(5,'C',7),(5,'C',8),(5,'C',9),(5,'C',10),
(5,'D',1),(5,'D',2),(5,'D',3),(5,'D',4),(5,'D',5),(5,'D',6),(5,'D',7),(5,'D',8),(5,'D',9),(5,'D',10),
(5,'E',1),(5,'E',2),(5,'E',3),(5,'E',4),(5,'E',5),(5,'E',6),(5,'E',7),(5,'E',8),(5,'E',9),(5,'E',10),
(5,'F',1),(5,'F',2),(5,'F',3),(5,'F',4),(5,'F',5),(5,'F',6),(5,'F',7),(5,'F',8),(5,'F',9),(5,'F',10),
(5,'G',1),(5,'G',2),(5,'G',3),(5,'G',4),(5,'G',5),(5,'G',6),(5,'G',7),(5,'G',8),(5,'G',9),(5,'G',10),
(6,'A',1),(6,'A',2),(6,'A',3),(6,'A',4),(6,'A',5),(6,'A',6),(6,'A',7),(6,'A',8),(6,'A',9),(6,'A',10),
(6,'B',1),(6,'B',2),(6,'B',3),(6,'B',4),(6,'B',5),(6,'B',6),(6,'B',7),(6,'B',8),(6,'B',9),(6,'B',10),
(6,'C',1),(6,'C',2),(6,'C',3),(6,'C',4),(6,'C',5),(6,'C',6),(6,'C',7),(6,'C',8),(6,'C',9),(6,'C',10);

INSERT INTO ShowtimeSeat (ShowtimeId, SeatId, Status) VALUES
(1,1,'Available'),
(1,2,'Booked'),
(1,3,'Available'),
(2,1,'Booked'),
(2,2,'Booked'),
(3,1,'Available'),
(3,2,'Available'),
(4,1,'Booked');

INSERT INTO [Order] (UserId, TotalAmount, Status, CreatedAt) VALUES
(1, 200000, 'Completed', GETDATE()),
(2, 300000, 'Completed', GETDATE()),
(3, 150000, 'Pending', GETDATE()),
(4, 400000, 'Completed', GETDATE()),
(5, 250000, 'Cancelled', GETDATE()),
(6, 500000, 'Completed', GETDATE()),
(7, 180000, 'Pending', GETDATE()),
(8, 220000, 'Completed', GETDATE());

INSERT INTO Ticket (OrderId, ShowTimeId, SeatId, Price, Status) VALUES
(1, 1, 1, 100000, 'Booked'),
(1, 1, 2, 100000, 'Booked'),
(2, 2, 3, 100000, 'Booked'),
(2, 2, 4, 100000, 'Booked'),
(2, 2, 5, 100000, 'Booked'),
(3, 3, 6, 150000, 'Pending'),
(4, 4, 7, 100000, 'Booked'),
(4, 4, 8, 100000, 'Booked'),
(4, 4, 9, 100000, 'Booked'),
(4, 4, 10, 100000, 'Booked'),
(5, 5, 11, 125000, 'Cancelled'),
(5, 5, 12, 125000, 'Cancelled'),
(6, 6, 13, 100000, 'Booked'),
(6, 6, 14, 100000, 'Booked'),
(6, 6, 15, 100000, 'Booked'),
(6, 6, 16, 100000, 'Booked'),
(6, 6, 17, 100000, 'Booked'),
(7, 1, 18, 180000, 'Pending'),
(8, 2, 19, 110000, 'Booked'),
(8, 2, 20, 110000, 'Booked');

INSERT INTO Payment (OrderId, Amount, PaymentMethod, TransactionId, Status, PaymentDate) VALUES
(1,200000,'Momo','TXN001','Success',GETDATE()),
(2,150000,'ZaloPay','TXN002','Pending',GETDATE()),
(3,300000,'Cash','TXN003','Success',GETDATE()),
(4,100000,'Momo','TXN004','Failed',GETDATE());

INSERT INTO Review (UserId, MovieId, Rating, Comment, CreatedAt) VALUES
(1,1,5,N'Rất hay',GETDATE()),
(2,2,4,N'Ổn áp',GETDATE()),
(3,3,3,N'Bình thường',GETDATE()),
(4,4,5,N'Đỉnh cao',GETDATE()),
(5,5,4,N'Hay',GETDATE());
GO
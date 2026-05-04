- TỔNG QUAN
  - Dự án: Hệ thống đặt vé xem phim (ASP.NET Core MVC).
  - Phân hệ Khách hàng: Xem phim, chọn ghế, thanh toán và nhận vé.
  - Phân hệ Quản trị (Admin): Quản lý danh sách phim và lịch chiếu.

- CẤU TRÚC SOURCE CODE
  - Controllers/HomeController.cs: Quản lý toàn bộ luồng của người dùng.
  - Controllers/AdminController.cs: Quản lý Dashboard và dữ liệu quản trị.
  - Views/Shared/_Layout.cshtml: Layout chung cho khách hàng (Navbar).
  - Views/Shared/_AdminLayout.cshtml: Layout riêng cho Admin (Sidebar).
  - Views/Home/: Chứa giao diện Index, Details, Login, Register, Profile, SeatSelection, Processing, BookingSuccess.
  - Views/Admin/: Chứa giao diện Movies, Showtimes.
  - wwwroot/css/site.css: CSS chung cho toàn hệ thống.
  - wwwroot/css/auth.css: CSS dùng riêng cho các trang có giao diện thẻ Card (Login, Register, Profile, Processing, Success).

- CÁC LUỒNG XỬ LÝ QUAN TRỌNG
  - Luồng Đặt Vé (Booking Flow): Index -> Details -> SeatSelection -> Processing -> BookingSuccess.
  - Tại SeatSelection: Sử dụng JavaScript để xử lý chọn ghế và tính tiền động ($15/vé).
  - Tại Processing: Giả lập thời gian chờ 3 giây (Timeout) để mô phỏng hệ thống Messaging Queue xử lý bất đồng bộ.
  - Tại BookingSuccess: Mã QR được tạo động từ Backend (Base64) chứa mã Order ID.
  - Luồng Xác Thực (Auth Flow): Trang Login và Register được thiết lập Layout = null để ẩn Navbar chung, dùng chung file auth.css.
  - Phân hệ Admin: Sử dụng Sidebar cố định bên trái, nội dung nhúng qua RenderBody().

- CÔNG NGHỆ & THƯ VIỆN SỬ DỤNG
  - Framework: .NET (ASP.NET Core MVC).
  - UI: Bootstrap 5, Bootstrap Icons.
  - QR Generation: Thư viện QRCoder (Cần chạy 'dotnet add package QRCoder' trong terminal nếu báo lỗi).
  - CSS: Custom Flexbox & CSS Animations (cho phần Spinner loading).

- HƯỚNG DẪN CHẠY DỰ ÁN
  - Mở terminal tại thư mục dự án.
  - Chạy lệnh: dotnet run để mở trang
  - Hoặc Chạy lệnh: dotnet watch (để tự động cập nhật khi sửa code).
  - Truy cập Trang chủ tại: http://localhost:[Port]
  - Truy cập Trang Admin tại: http://localhost:[Port]/Admin/Movies

- CÔNG VIỆC CẦN TIẾP TỤC (DÀNH CHO NHÓM)
  - Dữ liệu hiện tại đang được "Hard-code" trực tiếp trong View và Controller.
  - Cần tạo Model và kết nối SQL Server/Entity Framework để lưu trữ phim, người dùng và đơn hàng.
  - Tích hợp Identity để xử lý đăng nhập thực tế.
  - Kết nối logic xử lý hàng đợi (RabbitMQ) cho màn hình Processing.

- CHẠY BÊN Admin   
http://localhost:5xxx/Admin/Movies

http://localhost:5xxx/Admin/Showtimes
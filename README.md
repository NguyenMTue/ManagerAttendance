# ManagerAttendance - Hệ Thống Quản Lý Nhân Sự & Điểm Danh Nhân Viên

    [![Framework](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
    [![C#](https://img.shields.io/badge/C%23-13.0-239120?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
    [![Database](https://img.shields.io/badge/SQL_Server-EF_Core_10.0-CC292B?logo=microsoftsqlserver)](https://docs.
  microsoft.com/en-us/ef/core/)
    [![Tests](https://img.shields.io/badge/Unit_Tests-15_Passed-brightgreen?logo=nunit)](https://nunit.org/)

    **ManagerAttendance** là một hệ thống backend Web API RESTful kết hợp ứng dụng Console Client tương tác trực quan được
  xây dựng trên nền tảng **.NET 10 (C#)**. Dự án áp dụng kiến trúc **Repository & Unit of Work Pattern**, **Entity Framework
  Core (Kế thừa TPH - Table Per Hierarchy)**, **ASP.NET Core Identity**, **JWT Bearer Authentication** và hỗ trợ **Nhập nhân
  viên hàng loạt từ Excel với chế độ tự động chạy thử (Auto Dry-Run)**.

    ---

    ## 🌟 Tính Năng Nổi Bật

    ### 1. 🔑 Xác Thực & Phân Quyền (Authentication & Authorization)
    - **Đăng nhập bằng JWT Token**: Phân quyền chi tiết theo vai trò (`Admin`, `Manager`, `Employee`).
    - **Khóa tài khoản bị sa thải/vô hiệu hóa**: Tài khoản có `IsActive = false` (sa thải / thôi việc) bị ngăn chặn tuyệt
  đối khi đăng nhập.
    - **Menu tự động theo Role**: Ứng dụng Console Client tự động ẩn/hiện các chức năng phù hợp với quyền hạn của tài khoản
  đang đăng nhập.

    ### 2. 👥 Quản Lý Nhân Sự (Employee Management)
    - **Mô hình Kế thừa TPH (Table Per Hierarchy)** trong EF Core cho 3 nhóm nhân viên:
      - **Developer**: Quản lý `TechnicalDirection` (Backend/Frontend/Fullstack), `CodingSkillsFlag`.
      - **QA**: Quản lý `TestingMethodology` (Automation/Manual), `AutomationSkills`.
      - **Manager**: Quản lý `ManagerType` (Technical/Project/Operations/General), `ManagedDepartment`.
    - **Thăng chức & Đổi vị trí (`Promote`)**: Cập nhật cấp bậc (`BandType`: Junior, Mid, Senior, Lead, Principal) và Phòng
  ban.
    - **Điều chỉnh Trạng thái**: Sa thải, tạm khóa hoặc kích hoạt lại tài khoản.

    ### 3. 📁 Nhập Nhân Viên Hàng Loạt Từ Excel (.xlsx) với Dry-Run Auto-Validation
    - **Tự động Chạy thử (Dry-Run Auto-Validation)**: Khi tải file `.xlsx` lên, hệ thống tự động kiểm tra tính hợp lệ của
  từng dòng dữ liệu (Email trùng lặp, sai định dạng, sai Enum,...) **mà KHÔNG lưu vào Database**.
    - **Hiển thị Bảng trực quan (Console Table Format)**: Xuất kết quả các dòng hợp lệ và các dòng bị lỗi dưới dạng Bảng
  Console chia cột căn chỉnh rõ ràng.
    - **Xác nhận người dùng trước khi lưu**: Cho phép người dùng quyết định có tiến hành lưu các dòng hợp lệ vào Database
  hay Hủy thao tác.
    - **Xử lý Bất đồng bộ (Async Streaming)**: Sử dụng `ExcelDataReader` xử lý mượt mà cho các file Excel dữ liệu lớn mà
  không gây quá tải bộ nhớ.
    - **Hỗ trợ đa ngôn ngữ Header**: Đọc tốt cả Header Tiếng Việt (`Danh_sach_nhan_vien_VI.xlsx`) lẫn Tiếng Anh
  (`EmployeeList_EN.xlsx`).

    ### 4. ⏱️ Quản Lý Điểm Danh (Attendance Management)
    - **Chấm công tự động cho bản thân**: Check-in và Check-out trực tiếp cho tài khoản đang đăng nhập mà không cần nhập ID
  thủ công.
    - **Tự động tính trạng thái điểm danh**: Phân loại `Present` (Đúng giờ), `Late` (Đi muộn), `Leave` (Vắng mặt).
    - **Xem lịch sử điểm danh cá nhân**: Xem lại toàn bộ ca làm việc của bản thân (`my-history`).
    - **Quản lý lịch sử toàn hệ thống**: Role `Manager` & `Admin` có quyền tra cứu lịch sử điểm danh của tất cả nhân viên.

    ---

    ## 🛠️ Công Nghệ Sử Dụng

    - **Framework**: .NET 10.0 (C# 13)
    - **Database**: SQL Server (Entity Framework Core 10.0)
    - **Authentication**: ASP.NET Core Identity + JWT Bearer
    - **Object Mapper**: AutoMapper 16.1.1
    - **Excel Reader**: ExcelDataReader 3.7.0
    - **API Documentation**: Swagger / Swashbuckle (Annotations Enabled)
    - **Testing**: NUnit, Moq, Shouldly (15 Unit Tests Passed 100%)

    ---

    ## 📁 Cấu Trúc Thư Mục Dự Án

    ```text
    ManagerAttendance/
    ├── FileExcel/                            # Các file Excel mẫu dùng để nhập dữ liệu
    │   ├── Danh_sach_nhan_vien_VI.xlsx       # File mẫu tiếng Việt chuẩn 100%
    │   └── EmployeeList_EN.xlsx              # File mẫu tiếng Anh
    ├── src/
    │   └── ManagerAttendance/                # Source code chính của hệ thống
    │       ├── Common/                       # Filter & API Response chuẩn
    │       ├── Configuration/                # DbContext config, Mapping profile, JWT settings
    │       ├── ConsoleClient/                # Ứng dụng Console Client tương tác với Web API
    │       │   └── Program.cs
    │       ├── Controllers/                  # RESTful API Controllers (Auth, Employee, Attendance)
    │       ├── DTOs/                         # Data Transfer Objects
    │       ├── Enums/                        # Gender, Department, Band, ManagerType, AttendanceStatus
    │       ├── Middlewares/                  # Exception Middleware & Logging Middleware
    │       ├── Models/                       # Entity Models (Employee, Developer, QA, Manager, AttendanceRecord)
    │       ├── Repositories/                 # Generic Repository & Unit of Work Pattern
    │       ├── Services/                     # Business Logic Services
    │       ├── DatabaseSeeder.cs             # Khởi tạo dữ liệu mẫu mặc định
    │       └── Program.cs                    # Entry point của Web API Server
    └── tests/
        └── ManagerAttendance.Tests/          # Unit Tests cho Repository & Service layers
  ──────
  ## 🚀 Hướng Dẫn Cài Đặt & Chạy Dự Án

  ### 1. Yêu Cầu Tiền Đề

  • Cài đặt .NET 10 SDK https://dotnet.microsoft.com/download
  • Cài đặt SQL Server (LocalDB hoặc SQL Server Management Studio)

  ### 2. Cấu Hình Chuỗi Kết Nối Database

  Mở file src/ManagerAttendance/appsettings.json và cập nhật chuỗi kết nối DefaultConnection:

    "ConnectionStrings": {
      "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=ManagementAttendanceSystem;Trusted_Connection=True;
  TrustServerCertificate=True;MultipleActiveResultSets=true"
    }

  ### 3. Khởi Chạy Web API Server

  Chạy câu lệnh sau tại thư mục gốc:

    dotnet run --project src/ManagerAttendance/ManagerAttendance.csproj

  • Khi server khởi chạy thành công, Web API chạy tại: http://localhost:5000
  • Giao diện tài liệu Swagger UI hiển thị tại: http://localhost:5000/index.html

  ### 4. Khởi Chạy Console Client App

  Mở một cửa sổ Terminal mới và chạy:

    dotnet run --project src/ManagerAttendance/ConsoleClient/ConsoleClient.csproj

  • Nhập Base API URL (mặc định bấm Enter để chọn http://localhost:5000).
  ──────
  ## 🔑 Tài Khoản Đăng Nhập Mẫu (Seeded Users)

  Hệ thống tự động khởi tạo các tài khoản mặc định khi khởi chạy lần đầu:

   Vai trò (Role)  │ Email              │ Mật khẩu       │ Quyền hạn
  ─────────────────┼────────────────────┼────────────────┼──────────────────────────────────────────────────────────────────
   Admin           │ admin@system.com   │ Admin123!      │ Toàn quyền quản trị hệ thống
   Manager         │ manager@system.com │ Manager123!    │ Tạo/Sửa NV, Thăng chức, Sa thải, Nhập Excel, Xem lịch sử toàn bộ
   Developer       │ dev@system.com     │ Dev123!        │ Chấm công cá nhân, Xem lịch sử cá nhân
   QA              │ qa@system.com      │ Qa123!         │ Chấm công cá nhân, Xem lịch sử cá nhân
  ──────
  ## 📊 Cấu Trúc File Excel Chuẩn Để Import

  File Excel (.xlsx) tải lên hệ thống chứa các cột theo cấu trúc chuẩn sau:

   Header Tiếng Việt                   │ Header Tiếng Anh                    │ Ví dụ dữ liệu hợp lệ
  ─────────────────────────────────────┼─────────────────────────────────────┼──────────────────────────────────────────────
   Họ và Tên đệm                       │ FirstName                           │ Trần Thị
   Tên                                 │ LastName                            │ Mai
   Email                               │ Email                               │ mai.tran@example.com
   Mật khẩu                            │ Password                            │ DevPass123!
   Giới tính                           │ Gender                              │ Female, Male, Other
   Phòng ban                           │ Department                          │ Development, QA, Management, HR, IT
   Cấp bậc                             │ Band                                │ Junior, Mid, Senior, Lead, Principal
   Loại nhân viên                      │ EmployeeType                        │ Developer, QA, Manager
   Hướng chuyên môn                    │ TechnicalDirection                  │ Backend, Frontend, Fullstack (cho Developer)
   Kỹ năng lập trình                   │ CodingSkillsFlag                    │ C#, ASP.NET Core (cho Developer)
   Phương pháp kiểm thử                │ TestingMethodology                  │ Automation, Manual (cho QA)
   Kỹ năng Tự động hóa                 │ AutomationSkills                    │ TRUE / FALSE (cho QA)
   Loại quản lý                        │ ManagerType                         │ Technical, Project, Operations (cho Manager)
   Phòng ban quản lý                   │ ManagedDepartment                   │ Software Engineering (cho Manager)
  ──────
  ## 🧪 Kiểm Thử Tự Động (Unit Tests)

  Để thực thi toàn bộ bộ kiểm thử đơn vị:

    dotnet test tests/ManagerAttendance.Tests/ManagerAttendance.Tests.csproj
  ──────
  ## 📝 Giấy Phép & Tác Giả

  Dự án được phát triển phục vụ cho mục đích Học tập, Nghiên cứu & Quản lý nhân sự nội bộ.

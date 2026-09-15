using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using ManagerAttendance.DTOs;
using ManagerAttendance.Enums;

namespace ConsoleClient;

class Program
{
    private static readonly HttpClient _httpClient = CreateHttpClient();
    private static string? _jwtToken = null;
    private static string _userEmail = "Chưa đăng nhập";
    private static string _userRole = "Guest";
    private static int? _employeeId = null;
    private static string _baseUrl = "http://localhost:5000";

    static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        Console.Clear();
        Console.WriteLine("==========================================================================");
        Console.WriteLine("    HỆ THỐNG QUẢN LÝ ĐIỂM DANH NHÂN VIÊN - MANAGER ATTENDANCE CONSOLE    ");
        Console.WriteLine("==========================================================================");

        Console.Write("Nhập Base API URL [Mặc định: http://localhost:5000: ");
        var inputUrl = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(inputUrl))
        {
            var trimmed = inputUrl.Trim().TrimEnd('/');
            if (!trimmed.StartsWith("http://") && !trimmed.StartsWith("https://"))
            {
                trimmed = "http://" + trimmed;
            }
            _baseUrl = trimmed;
        }

        bool exit = false;
        while (!exit)
        {
            RenderMenu();
            Console.Write("\nChọn chức năng [0-7]: ");
            var choice = Console.ReadLine()?.Trim();

            Console.WriteLine();
            switch (choice)
            {
                case "1":
                    await LoginAsync();
                    break;
                case "2":
                    await CheckInAsync();
                    break;
                case "3":
                    await CheckOutAsync();
                    break;
                case "4":
                    await GetAllEmployeesAsync();
                    break;
                case "5":
                    await GetAttendanceHistoryAsync();
                    break;
                case "6":
                    await CreateEmployeeAsync();
                    break;
                case "7":
                    Logout();
                    break;
                case "0":
                    exit = true;
                    Console.WriteLine("Cảm ơn bạn đã sử dụng hệ thống ManagerAttendance. Tạm biệt!");
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Lựa chọn không hợp lệ! Vui lòng chọn từ 0 đến 7.");
                    Console.ResetColor();
                    break;
            }

            if (!exit)
            {
                Console.WriteLine("\nBấm phím bất kỳ để tiếp tục...");
                Console.ReadKey();
            }
        }
    }

    private static void RenderMenu()
    {
        Console.WriteLine("\n--------------------------------------------------------------------------");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($" Trạng thái: {_userEmail} | Role: {_userRole} | EmployeeId: {(_employeeId.HasValue ? _employeeId.Value.ToString() : "N/A")}");
        Console.ResetColor();
        Console.WriteLine("--------------------------------------------------------------------------");
        Console.WriteLine(" 1. 🔑 Đăng nhập (Login)");
        Console.WriteLine(" 2. 🟢 Chấm công vào ca (Check-In)");
        Console.WriteLine(" 3. 🔴 Kết thúc ca làm việc (Check-Out)");
        Console.WriteLine(" 4. 👥 Xem danh sách nhân viên (Get All Employees)");
        Console.WriteLine(" 5. 📅 Xem lịch sử điểm danh (Attendance History)");
        Console.WriteLine(" 6. ➕ Tạo nhân viên mới (Create Employee - Admin Only)");
        Console.WriteLine(" 7. 🚪 Đăng xuất (Logout)");
        Console.WriteLine(" 0. ❌ Thoát ứng dụng");
        Console.WriteLine("--------------------------------------------------------------------------");
    }

    private static async Task LoginAsync()
    {
        Console.WriteLine("=== ĐĂNG NHẬP HỆ THỐNG ===");
        Console.Write("Email: ");
        var email = Console.ReadLine()?.Trim();
        Console.Write("Mật khẩu: ");
        var password = ReadPassword();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowError("Email và mật khẩu không được để trống!");
            return;
        }

        try
        {
            var loginDto = new LoginDto { Email = email, Password = password };
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/auth/login", loginDto);

            if (response.IsSuccessStatusCode)
            {
                var authResult = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                if (authResult != null)
                {
                    _jwtToken = authResult.Token;
                    _userEmail = authResult.Email;
                    _userRole = authResult.Role;
                    _employeeId = authResult.EmployeeId;

                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);

                    ShowSuccess($"Đăng nhập thành công! Quyền hạn: {_userRole}");
                }
            }
            else
            {
                ShowError("Đăng nhập thất bại! Sai Email hoặc Mật khẩu.");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Lỗi kết nối tới máy chủ API: {ex.Message}");
        }
    }

    private static async Task CheckInAsync()
    {
        Console.WriteLine("=== CHẤM CÔNG ĐẦU NGÀY (CHECK-IN) ===");
        int targetEmployeeId;

        if (_employeeId.HasValue)
        {
            Console.Write($"Nhập Employee ID [Mặc định từ tài khoản: {_employeeId.Value}]: ");
            var inputId = Console.ReadLine();
            targetEmployeeId = string.IsNullOrWhiteSpace(inputId) ? _employeeId.Value : int.Parse(inputId);
        }
        else
        {
            Console.Write("Nhập Employee ID: ");
            if (!int.TryParse(Console.ReadLine(), out targetEmployeeId))
            {
                ShowError("Employee ID không hợp lệ!");
                return;
            }
        }

        Console.Write("Ghi chú [Không bắt buộc]: ");
        var notes = Console.ReadLine();

        try
        {
            var checkInDto = new CheckInDto { EmployeeId = targetEmployeeId, Notes = notes };
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/attendance/check-in", checkInDto);

            if (response.IsSuccessStatusCode)
            {
                var record = await response.Content.ReadFromJsonAsync<AttendanceRecordDto>();
                ShowSuccess($"Check-in thành công! ID: {record?.Id} | Thời gian: {record?.ArrivalTime.ToLocalTime()} | Trạng thái: {record?.Status}");
            }
            else
            {
                var errorObj = await response.Content.ReadAsStringAsync();
                ShowError($"Check-in thất bại! Phản hồi từ Server: {errorObj}");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Lỗi kết nối: {ex.Message}");
        }
    }

    private static async Task CheckOutAsync()
    {
        Console.WriteLine("=== KẾT THÚC CA LÀM VIỆC (CHECK-OUT) ===");
        int targetEmployeeId;

        if (_employeeId.HasValue)
        {
            Console.Write($"Nhập Employee ID [Mặc định từ tài khoản: {_employeeId.Value}]: ");
            var inputId = Console.ReadLine();
            targetEmployeeId = string.IsNullOrWhiteSpace(inputId) ? _employeeId.Value : int.Parse(inputId);
        }
        else
        {
            Console.Write("Nhập Employee ID: ");
            if (!int.TryParse(Console.ReadLine(), out targetEmployeeId))
            {
                ShowError("Employee ID không hợp lệ!");
                return;
            }
        }

        Console.Write("Ghi chú ra về [Không bắt buộc]: ");
        var notes = Console.ReadLine();

        try
        {
            var checkOutDto = new CheckOutDto { EmployeeId = targetEmployeeId, Notes = notes };
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/attendance/check-out", checkOutDto);

            if (response.IsSuccessStatusCode)
            {
                var record = await response.Content.ReadFromJsonAsync<AttendanceRecordDto>();
                ShowSuccess($"Check-out thành công! Thời gian ra: {record?.DepartureTime?.ToLocalTime()} | Ghi chú: {record?.Notes}");
            }
            else
            {
                ShowError("Check-out thất bại! Không tìm thấy bản ghi Check-in ngày hôm nay.");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Lỗi kết nối: {ex.Message}");
        }
    }

    private static async Task GetAllEmployeesAsync()
    {
        Console.WriteLine("=== DANH SÁCH TẤT CẢ NHÂN VIÊN ===");
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/employee");
            if (response.IsSuccessStatusCode)
            {
                var employees = await response.Content.ReadFromJsonAsync<List<EmployeeDto>>();
                if (employees != null && employees.Any())
                {
                    Console.WriteLine($"\n{"ID",-5} | {"Họ và Tên",-20} | {"Email",-28} | {"Loại NV",-12} | {"Phòng ban",-12} | {"Cấp bậc",-10}");
                    Console.WriteLine(new string('-', 95));
                    foreach (var emp in employees)
                    {
                        Console.WriteLine($"{emp.Id,-5} | {emp.FirstName + " " + emp.LastName,-20} | {emp.Email,-28} | {emp.EmployeeType,-12} | {emp.Department,-12} | {emp.Band,-10}");
                    }
                }
                else
                {
                    Console.WriteLine("Chưa có dữ liệu nhân viên.");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                ShowError("Bạn không có quyền truy cập chức năng này! (Yêu cầu quyền Admin hoặc Manager).");
            }
            else
            {
                ShowError($"Lỗi server: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Lỗi kết nối: {ex.Message}");
        }
    }

    private static async Task GetAttendanceHistoryAsync()
    {
        Console.WriteLine("=== LỊCH SỬ ĐIỂM DANH NHÂN VIÊN ===");
        Console.Write("Nhập Employee ID để xem lịch sử [Hoặc bấm Enter để xem tất cả nếu là Manager/Admin]: ");
        var inputId = Console.ReadLine();

        try
        {
            string requestUrl = string.IsNullOrWhiteSpace(inputId) 
                ? $"{_baseUrl}/api/attendance" 
                : $"{_baseUrl}/api/attendance/employee/{inputId}";

            var response = await _httpClient.GetAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                var records = await response.Content.ReadFromJsonAsync<List<AttendanceRecordDto>>();
                if (records != null && records.Any())
                {
                    Console.WriteLine($"\n{"ID",-5} | {"Tên nhân viên",-20} | {"Thời gian vào (CheckIn)",-22} | {"Thời gian ra (CheckOut)",-22} | {"Trạng thái",-10}");
                    Console.WriteLine(new string('-', 85));
                    foreach (var rec in records)
                    {
                        var arrStr = rec.ArrivalTime.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
                        var depStr = rec.DepartureTime.HasValue ? rec.DepartureTime.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm") : "Chưa Check-out";
                        Console.WriteLine($"{rec.Id,-5} | {rec.EmployeeName,-20} | {arrStr,-22} | {depStr,-22} | {rec.Status,-10}");
                    }
                }
                else
                {
                    Console.WriteLine("Không tìm thấy dữ liệu điểm danh.");
                }
            }
            else
            {
                ShowError($"Lấy dữ liệu điểm danh thất bại (HTTP {response.StatusCode})");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Lỗi kết nối: {ex.Message}");
        }
    }

    private static async Task CreateEmployeeAsync()
    {
        Console.WriteLine("=== TẠO MỚI NHÂN VIÊN (ADMIN ONLY) ===");
        if (_userRole != "Admin")
        {
            ShowError("Chức năng này chỉ dành cho Admin!");
            return;
        }

        Console.WriteLine("Chọn loại nhân viên muốn tạo:");
        Console.WriteLine("1. Developer");
        Console.WriteLine("2. QA");
        Console.WriteLine("3. Manager");
        Console.Write("Lựa chọn: ");
        var typeChoice = Console.ReadLine();

        Console.Write("User ID (Identity User Id): ");
        var userId = Console.ReadLine() ?? string.Empty;
        Console.Write("Họ: ");
        var firstName = Console.ReadLine() ?? string.Empty;
        Console.Write("Tên: ");
        var lastName = Console.ReadLine() ?? string.Empty;
        Console.Write("Email: ");
        var email = Console.ReadLine() ?? string.Empty;

        try
        {
            HttpResponseMessage response;
            if (typeChoice == "1")
            {
                Console.Write("Hướng kỹ thuật (Backend/Frontend/Fullstack): ");
                var tech = Console.ReadLine() ?? "Backend";
                var dto = new CreateDeveloperDto
                {
                    UserId = userId,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    TechnicalDirection = tech,
                    CodingSkillsFlag = "C#, SQL"
                };
                response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/employee/developer", dto);
            }
            else if (typeChoice == "2")
            {
                Console.Write("Phương pháp testing (Automation/Manual): ");
                var method = Console.ReadLine() ?? "Automation";
                var dto = new CreateQADto
                {
                    UserId = userId,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    TestingMethodology = method,
                    AutomationSkills = method.ToLower().Contains("auto")
                };
                response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/employee/qa", dto);
            }
            else
            {
                var dto = new CreateManagerDto
                {
                    UserId = userId,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    ManagerType = ManagerType.Technical,
                    ManagedDepartment = "Software Engineering"
                };
                response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/employee/manager", dto);
            }

            if (response.IsSuccessStatusCode)
            {
                var created = await response.Content.ReadFromJsonAsync<EmployeeDto>();
                ShowSuccess($"Tạo nhân viên thành công! ID: {created?.Id} | Họ tên: {created?.FirstName} {created?.LastName}");
            }
            else
            {
                ShowError($"Tạo nhân viên thất bại! HTTP Status: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Lỗi kết nối: {ex.Message}");
        }
    }

    private static void Logout()
    {
        _jwtToken = null;
        _userEmail = "Chưa đăng nhập";
        _userRole = "Guest";
        _employeeId = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
        ShowSuccess("Đã đăng xuất khỏi hệ thống.");
    }

    private static HttpClient CreateHttpClient()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };
        return new HttpClient(handler);
    }

    private static string ReadPassword()
    {
        var password = new StringBuilder();
        while (true)
        {
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter) break;
            if (key.Key == ConsoleKey.Backspace)
            {
                if (password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b");
                }
            }
            else
            {
                password.Append(key.KeyChar);
                Console.Write("*");
            }
        }
        Console.WriteLine();
        return password.ToString();
    }

    private static void ShowSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n✔ {message}");
        Console.ResetColor();
    }

    private static void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n✖ {message}");
        Console.ResetColor();
    }
}

/****************/
/* 20227234
   Bùi Phạm Quang Huy */
/****************/

using System;
using System.Collections.Generic;

namespace PayrollSystem
{
    // ==========================================
    // B.1. LỚP CƠ SỞ EMPLOYEE
    // ==========================================
    public abstract class Employee
    {
        protected string employeeId;
        protected string fullName;
        protected string department;
        protected double monthlyBonus;

        public string EmployeeId => employeeId;
        public string FullName => fullName;
        public string Department => department;
        public double MonthlyBonus => monthlyBonus;

        // Constructor nạp chồng 1: Chỉ có mã và họ tên (phòng ban mặc định, thưởng 0)
        public Employee(string employeeId, string fullName) 
            : this(employeeId, fullName, "Unassigned", 0) {}

        // Constructor nạp chồng 2: Có mã, họ tên và phòng ban (thưởng 0)
        public Employee(string employeeId, string fullName, string department) 
            : this(employeeId, fullName, department, 0) {}

        // Constructor đầy đủ bảo vệ các lớp dẫn xuất
        protected Employee(string employeeId, string fullName, string department, double monthlyBonus)
        {
            if (string.IsNullOrWhiteSpace(employeeId)) throw new ArgumentException("Mã nhân sự không được rỗng.");
            if (string.IsNullOrWhiteSpace(fullName)) throw new ArgumentException("Họ tên không được rỗng.");
            if (string.IsNullOrWhiteSpace(department)) throw new ArgumentException("Phòng ban không được rỗng.");
            if (monthlyBonus < 0) throw new ArgumentException("Thưởng không được âm.");

            this.employeeId = employeeId;
            this.fullName = fullName;
            this.department = department;
            this.monthlyBonus = monthlyBonus;
        }

        // --- Ba phiên bản nạp chồng phương thức addBonus() ---
        // 1. Thêm một khoản thưởng cố định
        public void AddBonus(double amount)
        {
            if (amount <= 0) throw new ArgumentException("Khoản thưởng phải lớn hơn 0.");
            monthlyBonus += amount;
        }

        // 2. Thêm khoản thưởng cố định kèm lý do
        public void AddBonus(double amount, string reason)
        {
            if (amount <= 0) throw new ArgumentException("Khoản thưởng phải lớn hơn 0.");
            if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Lý do thưởng không được rỗng.");
            monthlyBonus += amount;
        }

        // 3. Tính thưởng theo tỷ lệ của một giá trị tham chiếu, kèm lý do
        public void AddBonus(double rate, double referenceAmount, string reason)
        {
            if (rate <= 0 || rate > 0.5) throw new ArgumentException("Tỷ lệ thưởng phải lớn hơn 0 và không quá 0.5.");
            if (referenceAmount <= 0) throw new ArgumentException("Giá trị tham chiếu phải lớn hơn 0.");
            if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Lý do thưởng không được rỗng.");
            monthlyBonus += rate * referenceAmount;
        }

        // Phương thức đặt lại thưởng khi bắt đầu kỳ lương mới
        public void ResetBonus()
        {
            monthlyBonus = 0;
        }

        // Khai báo phương thức trừu tượng cần ghi đè
        public abstract double CalculateGrossPay();
        public abstract string GetEmployeeType();

        public virtual void DisplayPayrollInfo()
        {
            Console.WriteLine($"Mã: {employeeId} | Họ tên: {fullName} | Phòng: {department} | Loại: {GetEmployeeType()} | Thưởng: {monthlyBonus:N0} | Thu nhập: {CalculateGrossPay():N0}");
        }
    }

    // ==========================================
    // B.2. LỚP SALARIED EMPLOYEE
    // ==========================================
    public class SalariedEmployee : Employee
    {
        private double monthlySalary;
        private double responsibilityAllowance;

        public double MonthlySalary => monthlySalary;
        public double ResponsibilityAllowance => responsibilityAllowance;

        // Constructor rút gọn
        public SalariedEmployee(string employeeId, string fullName, double monthlySalary) 
            : this(employeeId, fullName, "Unassigned", monthlySalary, 0, 0) {}

        // Constructor với lương và phụ cấp nhưng không gán phòng ban/thưởng
        public SalariedEmployee(string employeeId, string fullName, double monthlySalary, double responsibilityAllowance)
            : this(employeeId, fullName, "Unassigned", monthlySalary, responsibilityAllowance, 0) {}

        // Constructor đầy đủ
        public SalariedEmployee(string employeeId, string fullName, string department, double monthlySalary, double responsibilityAllowance, double monthlyBonus = 0) 
            : base(employeeId, fullName, department, monthlyBonus)
        {
            if (monthlySalary < 0) throw new ArgumentException("Lương tháng không được âm.");
            if (responsibilityAllowance < 0) throw new ArgumentException("Phụ cấp trách nhiệm không được âm.");
            this.monthlySalary = monthlySalary;
            this.responsibilityAllowance = responsibilityAllowance;
        }

        public override double CalculateGrossPay()
        {
            return monthlySalary + responsibilityAllowance + monthlyBonus;
        }

        public override string GetEmployeeType()
        {
            return "SalariedEmployee";
        }

        public override void DisplayPayrollInfo()
        {
            base.DisplayPayrollInfo();
            Console.WriteLine($"   -> Lương tháng: {monthlySalary:N0} | Phụ cấp: {responsibilityAllowance:N0}");
        }
    }

    // ==========================================
    // B.3. LỚP HOURLY EMPLOYEE
    // ==========================================
    public class HourlyEmployee : Employee
    {
        private double hourlyRate;
        private double workedHours;

        public double HourlyRate => hourlyRate;
        public double WorkedHours => workedHours;

        // Constructor rút gọn
        public HourlyEmployee(string employeeId, string fullName, double hourlyRate, double workedHours) 
            : this(employeeId, fullName, "Unassigned", hourlyRate, workedHours, 0) {}

        // Constructor đầy đủ
        public HourlyEmployee(string employeeId, string fullName, string department, double hourlyRate, double workedHours, double monthlyBonus = 0) 
            : base(employeeId, fullName, department, monthlyBonus)
        {
            if (hourlyRate < 0) throw new ArgumentException("Đơn giá giờ không được âm.");
            if (workedHours < 0 || workedHours > 250) throw new ArgumentException("Số giờ làm hợp lệ từ 0 đến 250.");
            this.hourlyRate = hourlyRate;
            this.workedHours = workedHours;
        }

        public override double CalculateGrossPay()
        {
            double basePay;
            if (workedHours <= 160)
            {
                basePay = workedHours * hourlyRate;
            }
            else
            {
                basePay = 160 * hourlyRate + (workedHours - 160) * hourlyRate * 1.5;
            }
            return basePay + monthlyBonus;
        }

        public override string GetEmployeeType()
        {
            return "HourlyEmployee";
        }

        public override void DisplayPayrollInfo()
        {
            base.DisplayPayrollInfo();
            Console.WriteLine($"   -> Đơn giá giờ: {hourlyRate:N0} | Số giờ làm: {workedHours}");
        }
    }

    // ==========================================
    // B.4. LỚP SALES EMPLOYEE
    // ==========================================
    public class SalesEmployee : Employee
    {
        private double baseSalary;
        private double salesRevenue;
        private double commissionRate;

        public double BaseSalary => baseSalary;
        public double SalesRevenue => salesRevenue;
        public double CommissionRate => commissionRate;

        // Constructor rút gọn
        public SalesEmployee(string employeeId, string fullName, double baseSalary) 
            : this(employeeId, fullName, "Unassigned", baseSalary, 0, 0, 0) {}

        // Constructor với lương cơ bản, doanh số và tỷ lệ hoa hồng nhưng không gán phòng ban/thưởng
        public SalesEmployee(string employeeId, string fullName, double baseSalary, double salesRevenue, double commissionRate)
            : this(employeeId, fullName, "Unassigned", baseSalary, salesRevenue, commissionRate, 0) {}

        // Constructor đầy đủ
        public SalesEmployee(string employeeId, string fullName, string department, double baseSalary, double salesRevenue, double commissionRate, double monthlyBonus = 0) 
            : base(employeeId, fullName, department, monthlyBonus)
        {
            if (baseSalary < 0) throw new ArgumentException("Lương cơ bản không được âm.");
            if (salesRevenue < 0) throw new ArgumentException("Doanh số không được âm.");
            if (commissionRate < 0 || commissionRate > 0.3) throw new ArgumentException("Tỷ lệ hoa hồng nằm trong khoảng từ 0 đến 0.3.");

            this.baseSalary = baseSalary;
            this.salesRevenue = salesRevenue;
            this.commissionRate = commissionRate;
        }

        // Phương thức cập nhật doanh số có kiểm soát
        public void UpdateSales(double newSales)
        {
            if (newSales < 0) throw new ArgumentException("Doanh số cập nhật không được âm.");
            this.salesRevenue = newSales;
        }

        public override double CalculateGrossPay()
        {
            return baseSalary + (salesRevenue * commissionRate) + monthlyBonus;
        }

        public override string GetEmployeeType()
        {
            return "SalesEmployee";
        }

        public override void DisplayPayrollInfo()
        {
            base.DisplayPayrollInfo();
            Console.WriteLine($"   -> Lương cơ bản: {baseSalary:N0} | Doanh số: {salesRevenue:N0} | Hoa hồng: {commissionRate * 100}%");
        }
    }

    // ==========================================
    // B.5. LỚP PAYROLL (QUẢN LÝ BẢNG LƯƠNG)
    // ==========================================
    public class Payroll
    {
        private string period;
        private List<Employee> employees;

        public string Period => period;
        public int Count => employees.Count;

        public Payroll(string period)
        {
            this.period = period;
            this.employees = new List<Employee>();
        }

        // Không thêm nhân sự trùng mã
        public bool AddEmployee(Employee employee)
        {
            if (employee == null) return false;
            if (FindEmployee(employee.EmployeeId) != null)
            {
                return false;
            }
            employees.Add(employee);
            return true;
        }

        public Employee? FindEmployee(string employeeId)
        {
            foreach (var emp in employees)
            {
                if (emp.EmployeeId == employeeId) return emp;
            }
            return null;
        }

        // Tính tổng bảng lương bằng lời gọi đa hình (CalculateGrossPay qua kiểu chung Employee)
        public double CalculateTotalPayroll()
        {
            if (employees.Count == 0) return 0;
            double total = 0;
            foreach (var emp in employees)
            {
                total += emp.CalculateGrossPay();
            }
            return total;
        }

        // Tính tổng theo phòng ban
        public double CalculatePayrollByDepartment(string department)
        {
            if (employees.Count == 0) return 0;
            double total = 0;
            foreach (var emp in employees)
            {
                if (emp.Department.Equals(department, StringComparison.OrdinalIgnoreCase))
                {
                    total += emp.CalculateGrossPay();
                }
            }
            return total;
        }

        // Tìm người có thu nhập cao nhất
        public Employee? FindHighestPaidEmployee()
        {
            if (employees.Count == 0) return null;
            Employee highest = employees[0];
            double maxPay = highest.CalculateGrossPay();
            for (int i = 1; i < employees.Count; i++)
            {
                double pay = employees[i].CalculateGrossPay();
                if (pay > maxPay)
                {
                    maxPay = pay;
                    highest = employees[i];
                }
            }
            return highest;
        }

        // Hiển thị bảng lương bằng lời gọi đa hình
        public void DisplayPayroll()
        {
            Console.WriteLine($"\n=== BẢNG LƯƠNG KỲ: {period} ===");
            if (employees.Count == 0)
            {
                Console.WriteLine("Danh sách nhân sự rỗng.");
                return;
            }
            foreach (var emp in employees)
            {
                emp.DisplayPayrollInfo();
                Console.WriteLine("--------------------------------------------------");
            }
        }
    }

    // ==========================================
    // CHƯƠNG TRÌNH CHÍNH & BỘ KIỂM THỬ (10 TÌNH HUỐNG)
    // ==========================================
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== HỆ THỐNG TÍNH LƯƠNG VÀ THƯỞNG NHÂN SỰ ===");

            // Khởi tạo bảng lương với dữ liệu từ đề bài
            Payroll payroll = new Payroll("2026-09");

            // E001: Nhân viên lương cố định
            var e1 = new SalariedEmployee("E001", "Nguyễn Minh An", "Đào tạo", 15000000, 2000000);
            e1.AddBonus(1000000, "Thưởng cố định");
            payroll.AddEmployee(e1);

            // E002: Nhân viên theo giờ, không có giờ vượt ngưỡng
            var e2 = new HourlyEmployee("E002", "Trần Thu Bình", "Hỗ trợ", 100000, 150);
            e2.AddBonus(500000);
            payroll.AddEmployee(e2);

            // E003: Nhân viên theo giờ, có giờ vượt ngưỡng (170 giờ)
            var e3 = new HourlyEmployee("E003", "Lê Hoàng Chi", "Hỗ trợ", 100000, 170);
            payroll.AddEmployee(e3);

            // E004: Nhân viên kinh doanh
            var e4 = new SalesEmployee("E004", "Phạm Quốc Dũng", "Kinh doanh", 8000000, 200000000, 0.05);
            e4.AddBonus(0.02, 50000000, "Thưởng theo tỷ lệ doanh số");
            payroll.AddEmployee(e4);

            // Hiển thị bảng lương chuẩn
            payroll.DisplayPayroll();

            Console.WriteLine($"\nTổng bảng lương: {payroll.CalculateTotalPayroll():N0} VNĐ (Mong đợi: 70.000.000)");
            Console.WriteLine($"Tổng lương phòng Hỗ trợ: {payroll.CalculatePayrollByDepartment("Hỗ trợ"):N0} VNĐ (Mong đợi: 33.000.000)");

            var highest = payroll.FindHighestPaidEmployee();
            if (highest != null)
            {
                Console.WriteLine($"Nhân viên thu nhập cao nhất: {highest.FullName} ({highest.CalculateGrossPay():N0} VNĐ)");
            }

            // Chạy bộ kiểm thử biên và kiểm thử lỗi (10 tình huống)
            RunUnitTests();
        }

        public static void RunUnitTests()
        {
            Console.WriteLine("\n========================================");
            Console.WriteLine("BỘ 10 TÌNH HUỐNG KIỂM THỬ BIÊN VÀ LỖI");
            Console.WriteLine("========================================");
            int passed = 0;
            int total = 10;

            PrintTestCase(1, "Lương tháng âm", "Tạo SalariedEmployee với lương -1.000; chương trình phải ném ArgumentException.");
            // TC1: Kiểm thử lương tháng âm của SalariedEmployee
            try
            {
                var t = new SalariedEmployee("T01", "Test", -1000, 0);
                Console.WriteLine("Kết quả: KHÔNG ĐẠT - Đối tượng vẫn được tạo với lương âm.");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Kết quả: ĐẠT - ArgumentException được ném ra, lương âm bị từ chối.");
                passed++;
            }

            PrintTestCase(2, "Phụ cấp trách nhiệm âm", "Tạo SalariedEmployee với phụ cấp -500; chương trình phải ném ArgumentException.");
            // TC2: Kiểm thử phụ cấp âm của SalariedEmployee
            try
            {
                var t = new SalariedEmployee("T02", "Test", 10000, -500);
                Console.WriteLine("Kết quả: KHÔNG ĐẠT - Đối tượng vẫn được tạo với phụ cấp âm.");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Kết quả: ĐẠT - ArgumentException được ném ra, phụ cấp âm bị từ chối.");
                passed++;
            }

            PrintTestCase(3, "Số giờ làm vượt giới hạn", "Tạo HourlyEmployee với 260 giờ; chương trình phải ném ArgumentException vì giới hạn là 250 giờ.");
            // TC3: Kiểm thử số giờ làm vượt ngưỡng tối đa (> 250 giờ)
            try
            {
                var t = new HourlyEmployee("T03", "Test", 50000, 260);
                Console.WriteLine("Kết quả: KHÔNG ĐẠT - Đối tượng vẫn được tạo với 260 giờ làm.");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Kết quả: ĐẠT - ArgumentException được ném ra, số giờ vượt giới hạn bị từ chối.");
                passed++;
            }

            PrintTestCase(4, "Giờ làm đúng ngưỡng tính tăng ca", "Tạo HourlyEmployee làm đúng 160 giờ với đơn giá 100.000; thu nhập mong đợi là 16.000.000.");
            // TC4: Kiểm thử biên giờ làm đúng bằng 160 giờ (không vượt ngưỡng)
            try
            {
                var t = new HourlyEmployee("T04", "Test", 100000, 160);
                if (t.CalculateGrossPay() == 16000000)
                {
                    Console.WriteLine($"Kết quả: ĐẠT - Thu nhập thực tế {t.CalculateGrossPay():N0}, đúng 16.000.000.");
                    passed++;
                }
                else
                {
                    Console.WriteLine($"Kết quả: KHÔNG ĐẠT - Thu nhập thực tế {t.CalculateGrossPay():N0}, không đúng 16.000.000.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kết quả: KHÔNG ĐẠT - Phát sinh ngoại lệ ngoài dự kiến: {ex.Message}");
            }

            PrintTestCase(5, "Tỷ lệ hoa hồng vượt giới hạn", "Tạo SalesEmployee với hoa hồng 0,35; chương trình phải ném ArgumentException vì mức tối đa là 0,3.");
            // TC5: Kiểm thử tỷ lệ hoa hồng vượt ngưỡng quy định (> 0.3)
            try
            {
                var t = new SalesEmployee("T05", "Test", 5000000, 100000000, 0.35);
                Console.WriteLine("Kết quả: KHÔNG ĐẠT - Đối tượng vẫn được tạo với hoa hồng 0,35.");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Kết quả: ĐẠT - ArgumentException được ném ra, hoa hồng vượt giới hạn bị từ chối.");
                passed++;
            }

            PrintTestCase(6, "Tỷ lệ thưởng vượt giới hạn", "Gọi AddBonus với tỷ lệ 0,6; chương trình phải ném ArgumentException vì mức tối đa là 0,5.");
            // TC6: Kiểm thử tham số tỷ lệ addBonus vượt quá giới hạn (> 0.5)
            try
            {
                var t = new SalariedEmployee("T06", "Test", 5000000, 0);
                t.AddBonus(0.6, 1000000, "Lý do");
                Console.WriteLine("Kết quả: KHÔNG ĐẠT - Phương thức vẫn chấp nhận tỷ lệ thưởng 0,6.");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Kết quả: ĐẠT - ArgumentException được ném ra, tỷ lệ thưởng vượt giới hạn bị từ chối.");
                passed++;
            }

            PrintTestCase(7, "Thêm nhân viên trùng mã", "Thêm hai nhân viên cùng mã E999; nhân viên thứ hai phải bị từ chối và danh sách vẫn có 1 người.");
            // TC7: Kiểm thử quy tắc không cho phép thêm nhân sự trùng mã trong Payroll
            try
            {
                var pTest = new Payroll("TEST");
                pTest.AddEmployee(new SalariedEmployee("E999", "A", 1000000));
                bool addedSecond = pTest.AddEmployee(new SalariedEmployee("E999", "B", 2000000));
                if (!addedSecond && pTest.Count == 1)
                {
                    Console.WriteLine($"Kết quả: ĐẠT - Nhân viên thứ hai bị từ chối; số nhân viên hiện tại là {pTest.Count}.");
                    passed++;
                }
                else
                {
                    Console.WriteLine($"Kết quả: KHÔNG ĐẠT - Trạng thái thêm nhân viên thứ hai: {addedSecond}; số nhân viên: {pTest.Count}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kết quả: KHÔNG ĐẠT - Phát sinh ngoại lệ ngoài dự kiến: {ex.Message}");
            }

            PrintTestCase(8, "Bảng lương không có nhân viên", "Tạo Payroll rỗng; tổng lương phải bằng 0, nhân viên lương cao nhất phải là null và không phát sinh lỗi.");
            // TC8: Kiểm thử danh sách rỗng (Payroll rỗng trả về tổng lương 0 và không sập)
            try
            {
                var pEmpty = new Payroll("EMPTY");
                if (pEmpty.CalculateTotalPayroll() == 0 && pEmpty.FindHighestPaidEmployee() == null)
                {
                    Console.WriteLine("Kết quả: ĐẠT - Tổng lương bằng 0, nhân viên lương cao nhất là null.");
                    passed++;
                }
                else
                {
                    Console.WriteLine($"Kết quả: KHÔNG ĐẠT - Tổng lương thực tế là {pEmpty.CalculateTotalPayroll()} hoặc kết quả nhân viên cao nhất không đúng.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kết quả: KHÔNG ĐẠT - Phát sinh ngoại lệ ngoài dự kiến: {ex.Message}");
            }

            PrintTestCase(9, "Cập nhật doanh số hợp lệ", "Cập nhật doanh số từ 100.000.000 lên 250.000.000; giá trị sau cập nhật phải bằng 250.000.000.");
            // TC9: Kiểm thử phương thức cập nhật doanh số có kiểm soát của SalesEmployee
            try
            {
                var sEmp = new SalesEmployee("T09", "Test", 5000000, 100000000, 0.1);
                sEmp.UpdateSales(250000000);
                if (sEmp.SalesRevenue == 250000000)
                {
                    Console.WriteLine($"Kết quả: ĐẠT - Doanh số sau cập nhật là {sEmp.SalesRevenue:N0}.");
                    passed++;
                }
                else
                {
                    Console.WriteLine($"Kết quả: KHÔNG ĐẠT - Doanh số thực tế là {sEmp.SalesRevenue:N0}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kết quả: KHÔNG ĐẠT - Phát sinh ngoại lệ ngoài dự kiến: {ex.Message}");
            }

            PrintTestCase(10, "Đặt lại thưởng cho kỳ lương mới", "Cộng thưởng 2.000.000 rồi gọi ResetBonus; tiền thưởng sau khi đặt lại phải bằng 0.");
            // TC10: Kiểm thử tính năng đặt lại thưởng (ResetBonus) cho kỳ lương mới
            try
            {
                var rEmp = new SalariedEmployee("T10", "Test", 10000000, 0);
                rEmp.AddBonus(2000000);
                rEmp.ResetBonus();
                if (rEmp.MonthlyBonus == 0)
                {
                    Console.WriteLine($"Kết quả: ĐẠT - Tiền thưởng sau khi đặt lại là {rEmp.MonthlyBonus:N0}.");
                    passed++;
                }
                else
                {
                    Console.WriteLine($"Kết quả: KHÔNG ĐẠT - Tiền thưởng sau khi đặt lại là {rEmp.MonthlyBonus:N0}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kết quả: KHÔNG ĐẠT - Phát sinh ngoại lệ ngoài dự kiến: {ex.Message}");
            }

            Console.WriteLine($"\nKẾT QUẢ TỔNG KẾT: Đạt {passed}/{total} tình huống kiểm thử.");
        }

        private static void PrintTestCase(int number, string situation, string handling)
        {
            Console.WriteLine($"\nTC{number} - Tình huống: {situation}");
            Console.WriteLine($"Cách xử lý: {handling}");
        }
    }
}
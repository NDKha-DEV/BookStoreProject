// Vị trí: Bookstore.Web/Modules/NV5_payment/Program.cs
using System;

namespace BookStorePaymentSystem
{
    // =================================================================
    // 1. MODEL ĐƠN HÀNG
    // =================================================================
    public class Order
    {
        public string OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string DeliveryAddress { get; set; }
        public string Status { get; set; }
    }

    // =================================================================
    // 2. STRATEGY INTERFACE (Mẫu Chiến Lược)
    // =================================================================
    public interface IPaymentStrategy
    {
        bool ProcessPayment(Order order);
    }

    // =================================================================
    // 3. TEMPLATE METHOD (Lớp cha trừu tượng cho nhóm Thanh toán Online)
    // =================================================================
    public abstract class OnlinePaymentTemplate : IPaymentStrategy
    {
        // Đây chính là Template Method (Phương thức khuôn mẫu)
        // Nó định nghĩa bộ khung thuật toán cố định cho mọi cổng thanh toán online.
        public bool ProcessPayment(Order order)
        {
            InitTransaction(order);
            
            if (!ValidateAndSign())
            {
                Console.WriteLine("[Lỗi] Xác thực hoặc chữ ký bảo mật không hợp lệ.");
                return false;
            }

            bool apiResult = SendApiRequest(order.TotalAmount);
            
            WritePaymentLog(order.OrderId, apiResult);

            return apiResult;
        }

        // Các bước chung được cài đặt sẵn (Tái sử dụng mã nguồn công nghệ)
        private void InitTransaction(Order order)
        {
            Console.WriteLine($"\n[Hệ thống] Khởi tạo giao dịch trực tuyến cho đơn hàng: {order.OrderId}");
        }

        private void WritePaymentLog(string orderId, bool isSuccess)
        {
            string status = isSuccess ? "Thành công" : "Thất bại";
            Console.WriteLine($"[Log DB] Ghi nhận lịch sử giao dịch đơn hàng {orderId}: Trạng thái {status}");
        }

        // Các bước trừu tượng để các lớp con tự cài đặt chi tiết (Các bước biến đổi)
        protected abstract bool ValidateAndSign();
        protected abstract bool SendApiRequest(decimal amount);
    }

    // =================================================================
    // 4. CÁC CHIẾN LƯỢC CỤ THỂ (CONCRETE STRATEGIES)
    // =================================================================

    // --- Phương thức 1: Ví điện tử MoMo (Kế thừa Template Online) ---
    public class MoMoPayment : OnlinePaymentTemplate
    {
        protected override bool ValidateAndSign()
        {
            Console.WriteLine("[MoMo] Đang tạo chữ ký bảo mật SHA256 với AccessKey và SecretKey...");
            return true; // Giả lập xác thực thành công
        }

        protected override bool SendApiRequest(decimal amount)
        {
            Console.WriteLine($"[MoMo API] Đang kết nối app-to-app. Yêu cầu trừ tiền: {amount:N0} VNĐ");
            Console.Write("Kích hoạt MoMo OTP và bấm xác nhận trên App thành công? (y/n): ");
            return Console.ReadLine().ToLower() == "y";
        }
    }

    // --- Phương thức 2: Thẻ Ngân Hàng - Visa/Mastercard (Kế thừa Template Online) ---
    public class CardPayment : OnlinePaymentTemplate
    {
        protected override bool ValidateAndSign()
        {
            Console.WriteLine("[Card] Đang kiểm tra định dạng số thẻ và mã CVV bảo mật...");
            return true; 
        }

        protected override bool SendApiRequest(decimal amount)
        {
            Console.WriteLine($"[Cổng Thẻ] Gửi yêu cầu cổng thanh toán quốc tế kiểm tra số dư: {amount:N0} VNĐ");
            Console.Write("Nhập mã OTP OTP gửi về SMS chính xác? (y/n): ");
            return Console.ReadLine().ToLower() == "y";
        }
    }

    // --- Phương thức 3: Tài khoản ngân hàng - VietQR Chuyển khoản (Kế thừa Template Online) ---
    public class BankAccountPayment : OnlinePaymentTemplate
    {
        protected override bool ValidateAndSign()
        {
            Console.WriteLine("[VietQR] Đang sinh mã QR động chứa số tiền và nội dung đơn hàng...");
            return true;
        }

        protected override bool SendApiRequest(decimal amount)
        {
            Console.WriteLine("========================================");
            Console.WriteLine($"STK: 1903XXXXXX - Techcombank");
            Console.WriteLine($"Số tiền: {amount:N0} VNĐ");
            Console.WriteLine("[QR CODE ĐỘNG ĐÃ HIỂN THỊ TRÊN MÀN HÌNH]");
            Console.WriteLine("========================================");
            Console.Write("Hệ thống Banking nhận được tiền IPN tự động chưa? (y/n): ");
            return Console.ReadLine().ToLower() == "y";
        }
    }

    // --- Phương thức 4: COD - Thanh toán khi nhận hàng (Chiến lược độc lập, không dùng Template Online) ---
    public class CODPayment : IPaymentStrategy
    {
        public bool ProcessPayment(Order order)
        {
            Console.WriteLine("\n=== THANH TOÁN KHI NHẬN HÀNG (COD) ===");
            Console.WriteLine($"Giao hàng tới địa chỉ: {order.DeliveryAddress}");
            Console.WriteLine($"Số tiền Shipper sẽ thu: {order.TotalAmount:N0} VNĐ");
            Console.Write("Khách hàng bấm xác nhận đặt đơn hàng COD? (y/n): ");
            
            return Console.ReadLine().ToLower() == "y";
        }
    }

    // =================================================================
    // 5. CONTEXT - DỊCH VỤ THANH TOÁN (PaymentService)
    // =================================================================
    public class PaymentService
    {
        private IPaymentStrategy _paymentStrategy;

        // Cho phép thay đổi chiến lược linh hoạt lúc runtime
        public void SetPaymentStrategy(IPaymentStrategy paymentStrategy)
        {
            _paymentStrategy = paymentStrategy;
        }

        public void ExecuteCheckout(Order order)
        {
            if (_paymentStrategy == null)
            {
                Console.WriteLine("Vui lòng chọn phương thức thanh toán trước!");
                return;
            }

            Console.WriteLine("\n--- BẮT ĐẦU XỬ LÝ THANH TOÁN ---");
            bool success = _paymentStrategy.ProcessPayment(order);

            if (success)
            {
                // Tính đa hình giúp tự động cập nhật trạng thái chuẩn mà không cần 'is' hay 'if-else' kiểm tra loại lớp
                order.Status = (_paymentStrategy is CODPayment) ? "Chờ xác nhận (COD)" : "Đã thanh toán";
                Console.WriteLine($"\n[Thành công] Đơn hàng {order.OrderId} xử lý hoàn tất.");
                Console.WriteLine($"Trạng thái đơn hàng hiện tại: {order.Status}");
            }
            else
            {
                order.Status = "Thanh toán thất bại";
                Console.WriteLine($"\n[Thất bại] Xử lý đơn hàng {order.OrderId} không thành công.");
            }
        }
    }

    // =================================================================
    // 6. CHƯƠNG TRÌNH CHẠY THỬ (Main)
    // =================================================================
    class Program
    {
        static void Main(string[] args)
        {
            // Khởi tạo một đơn hàng mẫu
            Order myOrder = new Order
            {
                OrderId = "ORD-2026-999",
                TotalAmount = 750000,
                DeliveryAddress = "Số 1 Đại Cồ Việt, Hai Bà Trưng, Hà Nội",
                Status = "Chưa thanh toán"
            };

            PaymentService paymentService = new PaymentService();

            Console.WriteLine("=== CHỌN PHƯƠNG THỨC THANH TOÁN ===");
            Console.WriteLine("1. Ví điện tử MoMo");
            Console.WriteLine("2. Thẻ ngân hàng (Visa/Mastercard)");
            Console.WriteLine("3. Tài khoản ngân hàng (Quét mã VietQR)");
            Console.WriteLine("4. Thanh toán khi nhận hàng (COD)");
            Console.Write("Lựa chọn của bạn (1-4): ");
            
            string choice = Console.ReadLine();

            // Áp dụng Strategy Pattern để gán lớp xử lý tương ứng
            switch (choice)
            {
                case "1":
                    paymentService.SetPaymentStrategy(new MoMoPayment());
                    break;
                case "2":
                    paymentService.SetPaymentStrategy(new CardPayment());
                    break;
                case "3":
                    paymentService.SetPaymentStrategy(new BankAccountPayment());
                    break;
                case "4":
                    paymentService.SetPaymentStrategy(new CODPayment());
                    break;
                default:
                    Console.WriteLine("Lựa chọn không hợp lệ. Hủy giao dịch.");
                    return;
            }

            // Thực thi thanh toán đơn hàng
            paymentService.ExecuteCheckout(myOrder);

            Console.WriteLine("\n=== KẾT THÚC USE CASE THANH TOÁN ===");
        }
    }
}
// Vị trí: Bookstore.Web/Modules/NV5_Payment/PaymentAdapters.cs
using Bookstore.Core.Interfaces;

namespace Bookstore.Web.Modules.NV5_Payment
{
    // 1. Giả lập SDK của MoMo từ bên thứ ba (Hệ thống cũ/Khác biệt cấu trúc)
    public class ThirdPartyMomoSDK
    {
        public void MakeMomoTransaction(double amount, string description)
        {
            Console.WriteLine($"[ ADAPTER] SDK MoMo nhận lệnh: Xử lý số tiền {amount:N0}đ. Nội dung: {description}");
        }
    }

    // 2. Giả lập SDK của VNPAY từ bên thứ ba
    public class ThirdPartyVnpayAPI
    {
        public void ExecuteVnpayTransfer(long totalVnd)
        {
            Console.WriteLine($"[ ADAPTER] API VNPAY nhận lệnh: Đang khởi tạo cổng truyền mạch giá trị {totalVnd:N0} VND.");
        }
    }

    // =========================================================================
    // CÁC LỚP ADAPTER ĐỒNG BỘ VỀ CHUẨN IPaymentProcessor CỦA ĐỒ ÁN
    // =========================================================================

    public class MomoAdapter : IPaymentProcessor
    {
        private readonly ThirdPartyMomoSDK _momoSDK = new ThirdPartyMomoSDK();

        public bool ProcessPayment(int orderId, decimal amount)
        {
            // Chuyển đổi kiểu dữ liệu từ decimal sang double để vừa với SDK cấu trúc cũ của MoMo
            double convertedAmount = (double)amount;
            string description = $"Thanh toan don hang mã #{orderId}";
            
            _momoSDK.MakeMomoTransaction(convertedAmount, description);
            return true;
        }
    }

    public class VnpayAdapter : IPaymentProcessor
    {
        private readonly ThirdPartyVnpayAPI _vnpayAPI = new ThirdPartyVnpayAPI();

        public bool ProcessPayment(int orderId, decimal amount)
        {
            // Chuyển đổi kiểu dữ liệu từ decimal sang long theo yêu cầu kỹ thuật của VNPAY
            long totalVnd = (long)amount;
            
            _vnpayAPI.ExecuteVnpayTransfer(totalVnd);
            return true;
        }
    }
}
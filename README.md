# Đồ Án Môn Design Pattern - Website Quản Lý Bán Sách Mini

## 🚀 Quy trình phối hợp Git (Bắt buộc):

1. Không ai được phép commit/push trực tiếp lên nhánh `main`.
2. Trước khi code, chuyển về main và kéo code mới nhất của trưởng nhóm về:
   `git checkout main` -> `git pull origin main`
3. Tự tạo nhánh riêng theo nhiệm vụ của mình để làm việc:
   - NV1: `git checkout -b feature/nv1-account`
   - NV2: `git checkout -b feature/nv2-book`
   - NV3: `git checkout -b feature/nv3-cart`
   - NV4: `git checkout -b feature/nv4-order`
   - NV5: `git checkout -b feature/nv5-payment`
4. Code XONG HOÀN TOÀN thì push nhánh của mình lên GitHub và báo Trưởng nhóm tạo Pull Request để duyệt code vào nhánh `main`.

## 📂 Vị trí viết code:

- Toàn bộ Class con, tầng Logic xử lý: Viết trong project `Bookstore.Web/Modules/NVx_...` của mình.
- Tuyệt đối không tự ý sửa đổi file trong project `Bookstore.Core` khi chưa thông qua trưởng nhóm.

# Project 02 - My Paint

Thành viên nhóm:

MSSV | Họ và tên | Github
---- | --------- | ------
19120575 | Nguyễn Đức Mạnh | [manhnguyen1515](https://github.com/manhnguyen1515)
19120671 | Lê Nguyễn Nhất Thọ | [croissain](https://github.com/thole20042001)

## ⛳ Mục tiêu
Cài đặt ứng dụng vẽ như MSPaint.
## 📝 Các lưu ý cần thực hiện để chạy chương trình
- Nạp động các file .dll của từng shape trong thư mục Shape vào MyPaint (đặt trong folder ShapesDLL).
- Chạy file MyPaint.exe để thực thi ứng dụng.
## Yêu cầu cốt lõi (7 điểm)
- Nạp động các shape từ file .dll bên ngoài.
- Người dùng có thể chọn shape cụ thể để vẽ.
- Người dùng có thể thấy trước shape đang vẽ.
- Người dùng có thể hoàn thành xem trước bản vẽ và thay đổi của họ trở thành vĩnh viễn với các đối tượng đã vẽ trước đó.
- [x] Danh sách các hình vẽ được lưu và tải lại cho lần tiếp theo, lưu dưới dạng binary.
- Lưu bản vẽ và tải các bản vẽ dưới dạng hình ảnh bmp/png/jpg/jpeg/cvs
### Yêu cầu kỹ thuật
- Áp dụng mẫu thiết kế: Abstract factory, prototype.
- Kiến trúc plugin
- Delegate & event

### Sơ đồ lớp
## 🎯 Các chức năng đã hoàn thiện
- Ứng dụng hỗ trợ vẽ các shape cơ bản như: Line, Rectangle, Ellipse.
## 📈 Các chức năng phát triển thêm (3 điểm)
- Giao diện Fluent thân thiện với người dùng.
- Thêm tính năng pencil và eraser.
- Thêm một số hình nâng cao: Tam giác, Ngũ giác, Lục giác, Mũi tên,...
- Cho phép người dùng chọn màu vẽ, kích cỡ, loại viền (dash, dot, dash dot dot,...)
- Thêm tính năng text, cho phép lựa chọn kiểu chữ (in đậm, in nghiêng, gạch chân, gạch ngang), font chữ, kích cỡ chữ.
- Thêm ảnh vào trong canvas.
- Hạn chế nhấp nháy khi xem trước bản vẽ bằng cách sử dụng bộ đệm để vẽ lại tất cả hình có trong canvas.
- Thêm tính năng vùng chọn.
- Thêm tính năng di chuyển, xoay các shape, vùng chọn.
- Kéo thả hình ảnh từ ngoài vào canvas.
- Thu phóng (Ctrl + MouseWheel) và di chuyển khi thu phóng canvas (giữ Space + kéo thả LeftMouse)
- Cut (Ctrl + X) / Copy (Ctrl + C) / Paste (Ctrl + P)
- Undo (Ctrl + Z) / Redo (Ctrl + Y)
- Crop hình theo vùng chọn.
- Cho phép resize canvas.
- Tô màu Solid/Gradient.
- Thêm tính năng trích màu từ canvas.
- Giữ shift để vẽ các hình Vuông, Tròn, Tam giác cân,...
## 🎥 Video demo
> **Điểm đề xuất: 10**

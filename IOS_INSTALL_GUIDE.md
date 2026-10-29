# Cài Word Galaxy lên iPhone

Hướng dẫn này dùng Unity `6000.6.7f1` và quy trình Xcode Build Profiles. Bản cài phát triển cần Mac, Xcode, Unity iOS Build Support và một Apple Account. Hướng dẫn không yêu cầu tài khoản Apple Developer trả phí.

## 1. Chuẩn bị iPhone và Xcode

1. Cập nhật Xcode và iOS SDK để tương thích với phiên bản iOS trên iPhone.
2. Bật Wi-Fi trên iPhone và kết nối iPhone cùng Mac vào một mạng nội bộ có cho phép các thiết bị tìm thấy nhau.
3. Trong Xcode, mở **Xcode > Open Developer Tool > Device Hub**.
4. Chọn **+ > Pair Nearby Device**, chọn iPhone và làm theo hướng dẫn. Bật **Developer Mode** trên iPhone nếu Xcode yêu cầu; sau khi iPhone khởi động lại, xác nhận bật chế độ này.
5. Chờ iPhone xuất hiện trong Device Hub và danh sách thiết bị chạy của Xcode.

Mạng Wi-Fi quán cà phê, khách sạn và mạng khách có thể chặn Bonjour hoặc liên lạc trực tiếp giữa các thiết bị. Cùng tên Wi-Fi không đảm bảo hai máy có thể tìm thấy nhau. Nếu ghép nối không được, thử mạng riêng ở nhà hoặc ghép qua cáp USB trước.

## 2. Build project trong Unity

1. Mở thư mục project `WordShooter` trong Unity Hub bằng Unity `6000.6.7f1` có module **iOS Build Support**.
2. Mở `Assets/Scenes/Prototype.unity`.
3. Chọn **File > Build Profiles**, chọn **iOS**, rồi nhấn **Switch Platform** nếu cần.
4. Trong **Player Settings**, đặt **Bundle Identifier** thành ID duy nhất, ví dụ `com.viet.wordgalaxy.dev`. Giữ hướng màn hình Portrait.
5. Chọn **Build** và lưu Xcode project vào thư mục riêng, chẳng hạn `Desktop/WordShooter-iOS-Build`, ngoài `Assets`.

Scene `Prototype` đã được đăng ký trong Build Settings. UI và game controller được tạo khi ứng dụng chạy.

## 3. Ký và cài bằng Xcode

1. Mở `Unity-iPhone.xcodeproj` vừa tạo.
2. Chọn project **Unity-iPhone**, target **Unity-iPhone**, rồi mở **Signing & Capabilities**.
3. Bật **Automatically manage signing** và chọn **Personal Team** của Apple Account. Nếu chưa có Team, đăng nhập trong **Xcode > Settings > Accounts**.
4. Nếu Xcode báo Bundle Identifier đã được dùng, đổi sang ID khác.
5. Chọn iPhone làm run destination và nhấn **Run** hoặc **Command+R**.
6. Chấp nhận Trust hoặc Developer Mode trên iPhone nếu được yêu cầu.

Personal Team thường đủ để cài bản phát triển lên thiết bị cá nhân. Hồ sơ ký miễn phí có thời hạn ngắn và có thể cần build, cài lại sau khi hết hạn. TestFlight và phân phối App Store cần chương trình Apple Developer phù hợp.

## Xử lý sự cố

- **iPhone không xuất hiện trong Device Hub:** kiểm tra cả hai máy đang ở cùng mạng nội bộ, tắt VPN tạm thời, rồi thử khởi động lại Wi-Fi và Xcode.
- **Cả hai ở cùng Wi-Fi nhưng không tìm thấy nhau:** hỏi quản trị mạng xem có bật client/AP isolation, mạng guest, chặn Bonjour hoặc lọc multicast không. Ping thành công chỉ xác nhận một phần kết nối IP, không xác nhận Xcode discovery.
- **Không ghép được qua Wi-Fi:** ghép iPhone bằng cáp USB, xác nhận **Trust This Computer**, sau đó bật kết nối mạng cho thiết bị trong Xcode.
- **Unity không có iOS trong Build Profiles:** trong Unity Hub, cài **iOS Build Support** cho Unity `6000.6.7f1`.
- **Lỗi ký ứng dụng:** kiểm tra Personal Team, Bundle Identifier duy nhất và trạng thái Developer Mode trên iPhone.

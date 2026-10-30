# Việc cần làm trước khi phát hành Word Galaxy

Cập nhật: 30/10/2026.

Danh sách công việc còn lại để đưa game lên App Store và Google Play. Đánh dấu `[x]` khi đã hoàn thành và kiểm tra thực tế. Chế độ admin tiếp tục được giữ trong production.

## 1. Chốt các quyết định và bản quyền

- [ ] Chốt tên ứng dụng chính thức: cấu hình hiện là `WordShooter`, giao diện là `WORD GALAXY`.
- [ ] Chốt Bundle ID iOS và application ID Android trước khi tạo ứng dụng trên store. ID hiện tại: `com.viet.WordShooter`.
- [ ] Chọn đối tượng người dùng: phổ thông hay trẻ em; nếu chọn trẻ em, rà soát Apple Kids và Google Play Families.
- [ ] Chọn hỗ trợ cả iPhone/iPad hay chỉ iPhone; cấu hình hiện hỗ trợ cả hai.
- [ ] Chốt phiên bản iOS tối thiểu; cấu hình hiện là iOS 15.0 và cần thử trên thiết bị chạy phiên bản thấp nhất được hỗ trợ.
- [ ] Xác nhận quyền phân phối thương mại audio giọng Samantha; nếu không xác nhận được, thay bằng audio có giấy phép phù hợp.
- [ ] Xác nhận nguồn gốc và giấy phép nhạc nền `WordGalaxyArcadeTheme.wav`.
- [ ] Xác nhận quyền sử dụng nội dung từ vựng và câu ví dụ.
- [ ] Duyệt phần giải thích admin trong [ghi chú tuân thủ và review](compliance-answers.md).

**Lưu ý:** Không phát hành công khai trước khi giải quyết quyền phân phối audio. Âm thanh hiệu ứng Kenney và hình khẩu pháo/tháp đã có ghi nhận giấy phép CC0.

## 2. Tài khoản và khóa ký

- [ ] Chuẩn bị tài khoản Apple Developer Program và quyền App Store Connect.
- [ ] Chuẩn bị tài khoản Google Play Console, hoàn tất xác minh theo yêu cầu của tài khoản.
- [ ] Tạo hồ sơ ứng dụng trên hai store bằng ID đã chốt.
- [ ] Thiết lập Play App Signing.
- [ ] Tạo khóa upload Android, sao lưu ở ít nhất hai nơi an toàn ngoài repository và lưu thông tin khôi phục.
- [ ] Thiết lập Apple Team, certificate và provisioning profile để ký bản archive iOS.
- [ ] Chốt quy tắc tăng version/build number; tăng số build trước mỗi lần upload mới.

Không commit khóa ký hoặc mật khẩu, và không gửi chúng qua chat.

## 3. Icon, hình ảnh và trang giới thiệu

- [ ] Làm và gắn icon iOS, gồm icon App Store 1024 × 1024.
- [ ] Làm và gắn launcher/adaptive icon Android.
- [ ] Chốt splash/launch screen; xác nhận chấp nhận màn hình mặc định hoặc thay bằng nhận diện riêng.
- [ ] Chụp ảnh gameplay, bản đồ chủ đề, chọn cấp độ và màn hình tổng kết trên iPhone/Android.
- [ ] Nếu giữ hỗ trợ iPad: kiểm tra bố cục và chuẩn bị ảnh chụp iPad theo yêu cầu hiện hành của App Store Connect.
- [ ] Làm feature graphic 1024 × 500 cho Google Play.
- [ ] Duyệt mô tả, từ khóa và danh mục trong [bản nháp store listing](store-listing.md).
- [ ] Điền email hỗ trợ và URL hỗ trợ công khai.
- [ ] Không quảng bá A1–B2 là nội dung được phân loại chuẩn CEFR nếu chưa có rà soát chuyên môn.

## 4. Quyền riêng tư và khai báo store

- [ ] Điền tên nhà phát triển, email và ngày hiệu lực trong [chính sách quyền riêng tư](privacy-policy.md).
- [ ] Rà lại chính sách theo bản build cuối; lưu ý dữ liệu lưu cục bộ có thể chịu cơ chế sao lưu/khôi phục của hệ điều hành.
- [ ] Đăng chính sách lên URL công khai ổn định và gắn URL vào cả hai store.
- [ ] Hoàn thành Apple App Privacy và Google Play Data safety theo SDK, quyền và hành vi của bản build cuối.
- [ ] Kiểm tra privacy manifests và required-reason APIs trong Xcode archive cuối.
- [ ] Hoàn thành bảng xếp hạng độ tuổi trung thực, gồm cách mô tả hiệu ứng bắn/nổ vào từ.
- [ ] Xác nhận khai báo mã hóa của Apple; build hiện đặt `ITSAppUsesNonExemptEncryption = NO`.
- [ ] Hoàn thành EU trader status nếu áp dụng.
- [ ] Đưa hướng dẫn bật/tắt admin vào review notes/app access; admin không được dùng để che giấu tính năng với người duyệt.

Tham khảo [bản nháp câu trả lời tuân thủ](compliance-answers.md); đây không phải chứng nhận pháp lý hoặc bảo đảm store sẽ duyệt.

## 5. Build phát hành

### Android

- [ ] Build AAB bằng khóa upload thật qua các biến môi trường `WORDSHOOTER_*`; xem [hướng dẫn build](../../README.md#mobile-validation-and-release-builds).
- [ ] Xác nhận bản AAB cuối có đúng ID, version, target SDK, ARM64, quyền và native symbols.
- [ ] Kiểm tra lại căn chỉnh native/đóng gói 16 KB nếu đổi Unity, SDK hoặc plugin.
- [ ] Upload lên Google Play internal testing và xử lý các cảnh báo của Console.

### iOS

- [ ] Export project, archive, ký và Validate App trong Xcode.
- [ ] Kiểm tra Bundle ID, version/build number, icon, signing và privacy report.
- [ ] Đánh giá cảnh báo libc++ của toolchain trên bản build cuối; build thành công chưa chứng minh tương thích với thiết bị cũ.
- [ ] Upload TestFlight và xử lý cảnh báo App Store Connect.

## 6. Kiểm tra trên thiết bị thật

- [ ] Kiểm tra cài mới và cập nhật từ dữ liệu lưu cũ.
- [ ] Kiểm tra chế độ máy bay/offline, force-close, mở lại và chơi nhiều lượt liên tiếp.
- [ ] Kiểm tra khóa máy, chuyển app, mất focus, cuộc gọi và Control Center; game phải tạm dừng và không tự chơi tiếp.
- [ ] Android: kiểm tra Back, predictive Back, điều hướng cử chỉ và điều hướng bằng nút.
- [ ] iPhone/iPad: kiểm tra tai thỏ, home indicator, vùng an toàn và các tỷ lệ màn hình hỗ trợ.
- [ ] Kiểm tra dấu tiếng Việt, chữ dài, kích thước nút và thao tác bằng một tay.
- [ ] Đo CPU, bộ nhớ, thời gian mỗi frame và cấp phát bộ nhớ trên máy yếu.
- [ ] Đo thời gian tải chủ đề và bộ nhớ sau nhiều lần đổi chủ đề.
- [ ] Nghe thử hàng đợi phát âm khi bắn nhanh; xác nhận không gây khó hiểu.
- [ ] Đo tác động của `PlayerPrefs.Save()`; chỉ tối ưu tần suất nếu cần và vẫn bảo toàn tiến độ.
- [ ] Bổ sung hướng dẫn lần đầu hoặc gợi ý rõ cách bắn, tạm dừng và nghe phát âm.

## 7. Chất lượng nội dung học

- [ ] Chốt A1–B2 là mức độ khó gameplay hay cấp độ nội dung CEFR.
- [ ] Nếu là CEFR: phân loại lại từ vựng theo cấp, tránh dùng chung bộ từ nhưng quảng bá là nội dung khác cấp.
- [ ] Rà nghĩa tiếng Việt, năm câu ví dụ, phát âm và từ loại với người có chuyên môn.
- [ ] Thay từ loại suy luận bằng dữ liệu đã kiểm duyệt ở các từ/cụm từ mơ hồ.

## 8. Thử nghiệm và phát hành

- [ ] Kiểm tra lại yêu cầu SDK, chính sách và tài sản store hiện hành tại thời điểm nộp.
- [ ] Chạy TestFlight và Google Play internal testing với người dùng thực tế.
- [ ] Nếu tài khoản Google Play cá nhân thuộc diện yêu cầu closed testing: đáp ứng số người/ngày liên tục mà Console yêu cầu; mốc đang theo dõi là ít nhất 12 người trong 14 ngày.
- [ ] Sửa lỗi, xử lý phản hồi và kiểm tra lại trước khi xin production access/nộp duyệt.
- [ ] Gửi app review kèm hướng dẫn chơi và admin disclosure.
- [ ] Chọn cách phát hành có kiểm soát theo khả năng của từng store; không giả định phased rollout của Apple áp dụng cho bản phát hành đầu tiên.
- [ ] Theo dõi crash, ANR, phản hồi và đánh giá sau phát hành.

## Trạng thái kỹ thuật đã đạt

Các kết quả dưới đây là kiểm tra phát triển, không thay thế ký thật, thử máy thật hoặc xét duyệt store:

- Gameplay checks: 41.602 kiểm tra pass.
- Admin checks: 166 kiểm tra pass.
- Mobile stability checks: 284 kiểm tra pass với target iOS.
- Android AAB build thành công; đã thử ký bằng khóa dùng một lần, không phải khóa phát hành.
- Các thư viện native Android đã kiểm tra có LOAD alignment 16 KB, chỉ ARM64.
- iOS export và Xcode Release build chưa ký thành công.
- Admin vẫn có trong production; bật/tắt bằng ba lần chạm logo trong hai giây.

## Có thể để sau bản đầu

- [ ] Lưu và khôi phục ván đang chơi khi process bị kết thúc, không cộng XP trùng.
- [ ] Bổ sung chỉ số độ chính xác, thời gian hoặc số từ bỏ lỡ.
- [ ] Thêm âm lượng phát âm riêng và tùy chọn giảm hiệu ứng.
- [ ] Chuyển dần IMGUI sang uGUI/TextMeshPro nếu đo hiệu năng hoặc nhu cầu accessibility cho thấy cần thiết.

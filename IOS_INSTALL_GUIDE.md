# Chay WordShooter tren iPhone

Huong dan nay danh cho iPhone 17 Pro Max chay iOS 27, khong dung cap USB va khong can Apple Developer Program tra phi. Mac va iPhone can cung mang Wi-Fi.

## 1. Kiem tra cong cu

- Unity Hub va project `WordShooter`.
- Unity `6000.6.7f1` voi module **iOS Build Support**.
- Xcode `27.1` va iOS SDK `27.1`.
- Apple Account mien phi dang nhap trong Xcode.

Xcode tren may da duoc cai, nhung lan kiem tra gan nhat `xcode-select` van tro vao Command Line Tools. Trong Xcode, vao **Xcode > Settings > Locations > Command Line Tools** va chon **Xcode 27.1** truoc khi build.

## 2. Ghep iPhone voi Xcode qua Wi-Fi

1. Ket noi MacBook va iPhone vao cung mang Wi-Fi.
2. Mo Xcode, chon **Xcode > Open Developer Tool > Device Hub**.
3. Nhan **+ > Pair Nearby Device**, chon iPhone va lam theo huong dan tren man hinh.
4. Chap nhan **Trust** tren iPhone neu duoc hoi. Neu iOS yeu cau, vao **Cai dat > Quyen rieng tu & Bao mat > Developer Mode**, bat len, khoi dong lai iPhone va xac nhan.
5. Kiem tra iPhone xuat hien trong Device Hub va menu run destination cua Xcode.

## 3. Build project iOS trong Unity

1. Mo project `WordShooter` bang Unity `6000.6.7f1`.
2. Mo scene `Assets/Scenes/Prototype.unity`.
3. Vao **File > Build Profiles**, chon **iOS**, roi nhan **Switch Platform** neu nut nay hien ra.
4. Mo **Player Settings**. Dat **Bundle Identifier** thanh mot ID duy nhat, vi du `com.viet.wordgalaxy.test`.
5. Giu huong man hinh **Portrait**. Project bootstrap tu dong dat huong Portrait, nhung van co the kiem tra trong **Player > iOS > Resolution and Presentation > Default Orientation**.
6. Trong Build Profiles, chon **Build** (khong can chon Build And Run). Luu vao thu muc trong rieng, vi du `Desktop/WordShooter-iOS-Build`, khong luu trong `Assets`.
7. Cho Unity tao xong project Xcode.

Scene `Prototype` da duoc dang ky trong build settings cua project.

## 4. Ky va cai game bang Xcode

1. Mo `Unity-iPhone.xcodeproj` trong thu muc build vua tao.
2. Trong Project Navigator, chon project `Unity-iPhone`; o muc **TARGETS**, chon `Unity-iPhone`.
3. Mo **Signing & Capabilities**.
4. Bat **Automatically manage signing** va chon **Personal Team** gan voi Apple Account mien phi. Neu chua co Team, dang nhap tai **Xcode > Settings > Accounts**.
5. Neu Bundle Identifier bi trung, doi thanh ID khac.
6. Tren thanh cong cu Xcode, chon iPhone 17 Pro Max lam run destination.
7. Nhan **Run** hoac **Command+R**. Xcode se build, ky va cai ung dung len iPhone.
8. Neu iPhone hien yeu cau Developer Mode hoac Trust, chap nhan theo huong dan tren may.

## 5. Sau khi cai

- Game chay truc tiep tren iPhone; co the tat MacBook sau khi cai xong.
- Personal Team mien phi khong can phi dang ky, nhung ho so development het han sau khoang 7 ngay. Khi app khong mo duoc nua, can build va cai lai tu Xcode.
- Moi lan cap nhat game, build lai va chay lai tu Xcode.

## Xu ly nhanh

- **iPhone khong hien trong Xcode:** kiem tra hai may cung Wi-Fi, mo Device Hub va pair lai; xac nhan Developer Mode neu duoc hoi.
- **Xcode bao khong tim thay developer directory:** chon Xcode 27.1 trong **Xcode > Settings > Locations > Command Line Tools**.
- **Signing error:** kiem tra Personal Team dang duoc chon va Bundle Identifier la duy nhat.
- **Unity khong co iOS trong Build Profiles:** mo Unity Hub > Installs > Unity `6000.6.7f1` > Add modules va cai **iOS Build Support**.

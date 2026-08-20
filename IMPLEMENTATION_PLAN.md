# PLAN THỰC THI ADMIN MODULE — BẢN ĐỐI CHIẾU DATABASE THẬT

## 1. Mục tiêu đã chốt

```text
Controller → Service → Repository → DbContext → SQL Server
```

Ưu tiên code dễ mở ra review. Không dùng CQRS, MediatR hoặc Clean Architecture nặng. Controller chỉ nhận request/trả response; Service xử lý nghiệp vụ; Repository truy vấn; DTO không trả password.

Chưa code module tiếp theo cho đến khi duyệt plan này.

## 2. Kết quả quét database `CloneEbayDB`

Database hiện có 17 bảng nghiệp vụ và đang kết nối được qua LocalDB. Dữ liệu hiện tại: 1 User, các bảng còn lại đang rỗng.

### Bảng chính dùng cho Admin

```text
User
  id, username, email, password, role, avatarURL

Product
  id, title, description, price, images, categoryId, sellerId,
  isAuction, auctionEndTime

OrderTable
  id, buyerId, addressId, orderDate, totalPrice, status

OrderItem
  id, orderId, productId, quantity, unitPrice

Payment
  id, orderId, userId, amount, method, status, paidAt

Dispute
  id, orderId, raisedBy, description, status, resolution

ReturnRequest
  id, orderId, userId, reason, status, createdAt

Review
  id, productId, reviewerId, rating, comment, createdAt

Feedback
  id, sellerId, averageRating, totalReviews, positiveRate

ShippingInfo
  id, orderId, carrier, trackingNumber, status, estimatedArrival
```

### Quan hệ đã xác nhận

```text
User 1—N Product
User 1—N OrderTable
User 1—N Address/Bid/Review/Message/Payment/Dispute/Store
Product 1—N OrderItem/Bid/Review/Coupon/Inventory
OrderTable 1—N OrderItem/Payment/Dispute/ReturnRequest/ShippingInfo
OrderTable N—1 User (buyer)
```

### Kết luận về `status`

Ảnh bạn gửi là `ReturnRequest.status`, không phải `User.status`.

Các cột `status` hiện có:

```text
Dispute.status
OrderTable.status
Payment.status
ReturnRequest.status
ShippingInfo.status
```

Bảng `User` hiện **chưa có cột `status`**. Vì vậy không được giả định database đã có `UserStatus`.

Để đáp ứng requirement duyệt/khóa/mở khóa user, Phase User Management sẽ tạo migration bổ sung:

```sql
ALTER TABLE [User]
ADD status nvarchar(20) NOT NULL
    CONSTRAINT DF_User_status DEFAULT 'Pending';
```

Sau khi migration được duyệt, status của User dùng:

```text
Pending → Active → Blocked → Active
```

Không sửa database trực tiếp trong lúc code; migration phải được review và chạy có kiểm soát.

### Role và seed hiện tại

```text
User.role hiện có: Admin
Tài khoản seed hiện tại: admin@gmail.com
Các bảng nghiệp vụ hiện đang rỗng.
```

Role giai đoạn đầu chỉ dùng `Admin`. Chưa triển khai `SuperAdmin` hoặc `SupportAdmin` để tránh phình scope.

## 3. Đối chiếu code local: tận dụng và tự sinh

### Có thể tận dụng

```text
Models/Address.cs, Bid.cs, Category.cs, Coupon.cs, Dispute.cs,
Feedback.cs, Inventory.cs, Message.cs, OrderItem.cs, OrderTable.cs,
Payment.cs, Product.cs, ReturnRequest.cs, Review.cs,
ShippingInfo.cs, Store.cs, User.cs

Data/ApplicationDbContext.cs — mapping scaffold theo schema thật
Data/AppDbContext.cs — context Admin đơn giản, cần thống nhất một context duy nhất
BCrypt.Net-Next — verify/hash password
Microsoft.AspNetCore.Authentication.JwtBearer — JWT
```

### Đã có từ lượt trước

```text
Controllers/AuthController.cs
Services/AuthService.cs, IAuthService.cs
Repositories/UserRepository.cs, IUserRepository.cs
DTOs/Auth/LoginRequestDto.cs, LoginResponseDto.cs
Helpers/JwtHelper.cs, JwtOptions.cs
```

### Cần tự sinh theo kiến trúc 3 tầng

```text
DashboardController/Service/Repository/DTO
AdminUserController/Service/Repository/DTO
AdminProductController/Service/Repository/DTO
AdminOrderController/Service/Repository/DTO
DisputeController/Service/Repository/DTO
AuditLogService/Repository/DTO
ExceptionMiddleware
Migration thêm User.status
Unit test và integration test
```

### Quyết định bắt buộc trước khi code tiếp

Chọn **một DbContext duy nhất**. Vì database thật dùng tên bảng `User`, `Product`, `OrderTable`, mapping phải giữ đúng tên hiện tại. Không dùng `EnsureCreated()` để thay thế schema thật; dùng migration cho thay đổi có chủ đích.

## 4. Thứ tự thực thi sau khi duyệt

### Lượt 0 — Chuẩn hóa context và schema

```text
1. Chọn AppDbContext làm context Admin duy nhất.
2. Giữ mapping bảng User/Product/OrderTable hiện có.
3. Bổ sung migration User.status.
4. Seed admin qua cấu hình, không hard-code password trong service.
5. Giữ /health và xóa hoàn toàn endpoint template.
```

**Gate 0:** `dotnet build`, migration tạo đúng `User.status`, query User/Product/OrderTable chạy được.

### Lượt 1 — Authentication + Authorization

```text
POST /api/auth/login

AuthController
  ↓
AuthService
  ↓
IUserRepository/UserRepository
  ↓
AppDbContext → User
  ↓
JwtHelper
```

JWT bắt buộc có:

```json
{
  "id": "1",
  "email": "admin@gmail.com",
  "role": "Admin"
}
```

Tiêu chí: đúng password trả 200; sai password/Pending/Blocked trả 401; token thiếu role trả 403; không trả password.

### Lượt 2 — Dashboard

```text
GET /api/admin/dashboard
```

Version đầu chỉ trả:

```json
{
  "totalUsers": 0,
  "totalProducts": 0,
  "totalOrders": 0,
  "revenue": 0
}
```

Query từ `User`, `Product`, `OrderTable`, `Payment`. Filter ngày/tháng/quý để sau khi bản cơ sở ổn định.

### Lượt 3 — User Management

```text
GET /api/admin/users
GET /api/admin/users/{id}
PUT /api/admin/users/{id}/approve
PUT /api/admin/users/{id}/block
PUT /api/admin/users/{id}/unblock
```

Dùng `User.status` sau migration. Response danh sách:

```json
{
  "page": 1,
  "pageSize": 10,
  "total": 0,
  "items": []
}
```

Approve/block/unblock phải nằm trong Service và kiểm tra transition hợp lệ.

### Lượt 4 — Product Moderation

```text
GET /api/admin/products
GET /api/admin/products/{id}
PUT /api/admin/products/{id}/hide
PUT /api/admin/products/{id}/unhide
```

Database hiện chưa có `Product.status`; cần migration bổ sung status hoặc cờ ẩn sau khi duyệt. Không xóa cứng Product.

### Lượt 5 — Order Management

```text
GET /api/admin/orders
GET /api/admin/orders/{id}
```

Đọc `OrderTable`, `OrderItem`, `Payment`, `ShippingInfo`. Chỉ cho phép transition status đã chốt; không tự suy đoán giá trị khi bảng đang rỗng.

### Lượt 6 — Dispute Management

```text
GET /api/admin/disputes
GET /api/admin/disputes/{id}
PUT /api/admin/disputes/{id}/assign
PUT /api/admin/disputes/{id}/resolve
PUT /api/admin/disputes/{id}/reject
```

Tận dụng `Dispute.status` và `Dispute.resolution` đang có. Giá trị status thực tế sẽ được chốt bằng seed/test trước khi viết transition.

### Lượt 7 — Audit Log xuyên suốt

Bắt buộc ghi các action:

```text
LOGIN_SUCCESS (có thể ghi)
APPROVE_USER
BLOCK_USER
UNBLOCK_USER
HIDE_PRODUCT
UNHIDE_PRODUCT
ASSIGN_DISPUTE
RESOLVE_DISPUTE
REJECT_DISPUTE
```

Schema AuditLog chưa có trong database hiện tại; phải tạo migration trước khi ghi log. Không ghi password, JWT hoặc secret.

### Lượt 8 — MVC

Chỉ làm sau khi API đã test qua Swagger:

```text
MVC Controller → typed HttpClient → Admin API
```

Không gọi DbContext trực tiếp từ View/MVC.

## 5. Tiêu chí duyệt từng lượt

```text
Lượt 0: schema/context/migration đúng.
Lượt 1: login + JWT + role chạy.
Lượt 2: dashboard trả số liệu đúng với DB.
Lượt 3: User status transition chạy đúng.
Lượt 4: Product hide mềm, không delete cứng.
Lượt 5: Order đọc được quan hệ thật.
Lượt 6: Dispute assign/resolve/reject đúng transition.
Lượt 7: audit truy vết được actor/action/resource.
Lượt 8: MVC chỉ gọi API và hiển thị đúng response.
```

## 6. Phạm vi code lượt tiếp theo sau khi duyệt plan

Chỉ làm **Lượt 0: chuẩn hóa AppDbContext theo schema thật và tạo migration `User.status`**. Chưa code Dashboard, User Controller, Product hay MVC.

## 7. Kết quả quét source nhóm trước

Đã tìm thấy bản clone đầy đủ trên máy:

```text
C:\Users\MSI\Desktop\ProjectNhom3RoleAdmin
Remote:
https://github.com/NAMLDHE173247/ProjectPRN232_RoleAdmin.git
Commit local:
27819ac Initial commit
```

### Nhóm trước tạo `User.status` như thế nào?

Trong `src/Domain/Entities/User.cs`, nhóm có nhiều field quản lý user:

```text
Status           = Active/Pending/Banned/Suspended
ApprovalStatus   = Approved/PendingApproval/Rejected
ApprovedBy
ApprovedAt
BannedReason
BannedBy
BannedAt
TwoFactorEnabled
```

Trong migration `src/Infrastructure/Data/Migrations/20260131170653_InitialCreate.cs`, bảng `User` được tạo với các cột `Status`, `ApprovalStatus`, `ApprovedBy`, `ApprovedAt`, `BannedReason`, `BannedBy`, `BannedAt` và các cột bảo mật. Vì vậy sơ đồ database của nhóm phải được tạo/ cập nhật từ migration này mới hiển thị đầy đủ các cột đó.

Database local hiện tại của project chúng ta có `__EFMigrationsHistory` = không tồn tại và bảng `User` chỉ có:

```text
id, username, email, password, role, avatarURL
```

Đây là lý do ảnh sơ đồ hiện tại không có `User.status`. Ảnh vẫn đúng với database hiện tại; source nhóm trước dùng schema đã migrate khác.

### Nhóm trước triển khai User Management

Endpoint group ở `src/Web/Endpoints/Users.cs` yêu cầu policy `ManageUsers` rồi chuyển request vào MediatR command/query:

```text
GET  /api/Users
GET  /api/Users/{id}
PUT  /api/Users/{id}/status
POST /api/Users/{id}/approve
POST /api/Users/{id}/reject
POST /api/Users/{id}/ban
POST /api/Users/{id}/unban
```

Business logic nằm trong các handler:

```text
ApproveUserCommandHandler
BanUserCommandHandler
UnbanUserCommandHandler
RejectUserCommandHandler
UpdateUserStatusCommandHandler
```

Ví dụ `ApproveUserCommandHandler` cập nhật `ApprovalStatus = Approved`, `Status = Active`, thời điểm/người duyệt, tạo Notification và AdminAction. `BanUserCommandHandler` kiểm tra còn giao dịch chưa kết thúc, sau đó cập nhật `Status = Banned`, lý do/người ban/thời điểm, Notification và AdminAction.

### Nhóm trước triển khai Authentication

Flow của họ là:

```text
Auth endpoint
  ↓
LoginCommandHandler
  ↓
IApplicationDbContext.Users
  ↓
BCrypt.Verify
  ↓
kiểm tra Status == Banned
  ↓
JwtService.GenerateToken
```

JWT của họ dùng các claim chính:

```text
ClaimTypes.NameIdentifier  = User.Id
ClaimTypes.Email           = User.Email
ClaimTypes.Role            = User.Role
twoFactorEnabled
```

Họ còn có 2FA, rate limiting và HttpOnly cookie. Những phần này là tham khảo; Ass2 của chúng ta chỉ lấy login + JWT + role trước, chưa copy 2FA.

### Quyết định áp dụng cho project chúng ta

1. Dùng schema nhóm trước làm **tham chiếu đã xác minh**, không nhầm `ReturnRequest.status` với `User.status`.
2. Không copy nguyên CQRS/MediatR. Chuyển cùng nghiệp vụ sang `Controller → Service → Repository`.
3. Bổ sung tối thiểu các cột User cần cho requirement:

```text
Status
ApprovalStatus
ApprovedBy
ApprovedAt
BannedReason
BannedBy
BannedAt
```

4. Có thể thêm 2FA/audit nâng cao sau MVP, không đưa vào lượt Auth đầu tiên.
5. Trước khi migration, phải backup hoặc tạo database test; không chạy migration nhóm trực tiếp lên database hiện tại vì source nhóm có nhiều bảng/phần mở rộng ngoài scope.

### Trạng thái thực thi Phase 0

Đã hoàn thành trên database local `CloneEbayDB`:

```text
Migration: 20260820000100_AddAdminUserManagementFields

User.status          nvarchar(20)
User.ApprovalStatus  nvarchar(30)
User.ApprovedBy      int nullable
User.ApprovedAt      datetime2 nullable
User.BannedReason    nvarchar(max) nullable
User.BannedBy        int nullable
User.BannedAt        datetime2 nullable
```

`User.status` dùng `Pending`, `Active`, `Banned`. `User.ApprovalStatus` dùng `PendingApproval`, `Approved`, `Rejected`.

Database đã ghi migration vào `__EFMigrationsHistory`; tài khoản `admin@gmail.com` hiện có `Status = Active`, `ApprovalStatus = Approved`.

### Trạng thái thực thi Module User Management

Đã triển khai các endpoint và tầng code:

```text
GET  /api/admin/users
GET  /api/admin/users/{id}
PUT  /api/admin/users/{id}/approve
PUT  /api/admin/users/{id}/block
PUT  /api/admin/users/{id}/unblock
```

Flow đã smoke test thành công với user test và dữ liệu test đã được xóa sau kiểm tra:

```text
Pending → Active → Banned → Active
```

AuditLog chưa ghi ở module này vì bảng AuditLog chưa được tạo; sẽ bổ sung ở Module Audit theo đúng nguyên tắc chỉ ghi sau khi transaction nghiệp vụ thành công.

### Trạng thái thực thi Audit Log Foundation

Đã hoàn thành migration và API nền:

```text
Migration: 20260820000200_AddAuditLog
GET /api/admin/audit-logs?take=50
```

Bảng `AuditLog` gồm:

```text
Id, ActorId, Action, Resource, ResourceId, Metadata, CreatedAtUtc
```

Các thao tác User Management hiện được ghi sau khi cập nhật User thành công:

```text
APPROVE_USER
BLOCK_USER
UNBLOCK_USER
```

Smoke test đã xác nhận đủ 3 log với `ActorId = 1`, sau đó user và dữ liệu test tạm đã được xóa. Build solution pass.

### Trạng thái thực thi Phase Admin Dashboard

Đã triển khai API thống kê nền theo kiến trúc 3 tầng:

```text
GET /api/admin/dashboard
AdminDashboardController
        ↓
AdminDashboardService
        ↓
DashboardRepository
        ↓
User, Product, OrderTable, Payment
```

Response hiện tại:

```json
{
  "totalUsers": 1,
  "totalProducts": 0,
  "totalOrders": 0,
  "revenue": 0
}
```

Repository dùng các truy vấn aggregate trực tiếp (`CountAsync`, `SumAsync`), doanh thu chỉ cộng Payment có `status = Paid`. API yêu cầu JWT role `Admin`; smoke test xác nhận request không token nhận `401`, Admin nhận đúng số liệu database. Chưa thêm filter ngày/tháng hoặc chart.

### Trạng thái thực thi Phase Product Moderation

Đã đối chiếu schema: bảng `Product` chưa có trạng thái. Đã thêm migration:

```text
20260820000300_AddProductStatus
Product.status nvarchar(20) NOT NULL DEFAULT 'Active'
```

Đã triển khai:

```text
GET /api/admin/products
GET /api/admin/products/{id}
PUT /api/admin/products/{id}/hide
PUT /api/admin/products/{id}/unhide
```

Flow nghiệp vụ:

```text
Active → Hidden → Active
```

Thao tác hide/unhide không xóa cứng dữ liệu và ghi AuditLog tương ứng (`HIDE_PRODUCT`, `UNHIDE_PRODUCT`) sau khi cập nhật thành công. Smoke test đã pass với sản phẩm tạm; dữ liệu test đã được xóa. Build solution pass.

### Trạng thái thực thi Phase Admin Core Management

#### Order Management

Đã triển khai trong phạm vi Admin:

```text
GET /api/admin/orders
GET /api/admin/orders/{id}
```

API danh sách hỗ trợ `status`, `from`, `to`, `buyerId`, `page`, `pageSize`. API chi tiết trả thông tin buyer, item, product, payment và shipping qua DTO riêng; không trả password hoặc dữ liệu thanh toán nhạy cảm.

Order hiện chỉ đọc vì schema và source hiện tại chưa có rule chuyển trạng thái thống nhất. Không tự tạo endpoint update status.

#### Dispute Management

Đã thêm migration:

```text
20260820000400_AddDisputeAdminWorkflow
assignedTo, assignedAt, resolvedBy, resolvedAt
```

Đã triển khai:

```text
GET /api/admin/disputes
GET /api/admin/disputes/{id}
PUT /api/admin/disputes/{id}/assign
PUT /api/admin/disputes/{id}/resolve
PUT /api/admin/disputes/{id}/reject
```

Workflow:

```text
Open → Assigned → Resolved
Open → Rejected
```

Assign chỉ nhận user có role `Admin`. Resolve và Reject yêu cầu nội dung kết luận. Transition sai trả `400`.

#### Audit Integration

Đã bổ sung:

```text
ASSIGN_DISPUTE
RESOLVE_DISPUTE
REJECT_DISPUTE
```

Không ghi log cho thao tác GET để tránh audit noise. Metadata chỉ chứa trạng thái hoặc admin được assign, không chứa JWT/password/secret.

Smoke test đã xác nhận: không token nhận `401`, role User nhận `403`, Order list/detail trả đủ item/payment/shipping, Dispute list/detail hoạt động, transition sai nhận `400`, ba action Audit được ghi đúng. Toàn bộ dữ liệu test tạm đã được xóa. Build solution pass với 0 warning, 0 error.

### Trạng thái thực thi Backend Polish + MVC Admin Panel

Backend polish đã hoàn thành:

```text
GET /api/admin/audit-logs?page=1&pageSize=20
Dashboard: activeUsers, bannedUsers, hiddenProducts, pendingDisputes
```

Audit API vẫn được bảo vệ bởi JWT role `Admin`. Enum User/Product được serialize thành chuỗi để API contract dễ đọc và MVC không phải tự suy luận trạng thái.

MVC Admin Panel đã tích hợp API qua `AdminApiClient` và session JWT:

```text
/Account/Login
/Dashboard
/Users
/Products
/Orders
/Orders/Details/{id}
/Disputes
/Disputes/Details/{id}
/AuditLogs
```

MVC không truy cập `DbContext` và không chứa business rule. Các POST từ MVC dùng anti-forgery token; API vẫn là nơi quyết định transition và authorization. Giao diện responsive, có dark/light mode, skip link và label cho form.

Kiểm tra end-to-end đã pass: login MVC, tải đủ 6 màn hình Admin, approve user qua MVC tạo đúng `APPROVE_USER`, Audit pagination và Dashboard counter trả đúng. Visual QA xác nhận trang login ở viewport desktop/mobile không bị tràn ngang. Dữ liệu test tạm đã được xóa.

### Trạng thái seed dữ liệu demo

Đã bổ sung seed có điều kiện trong `DbInitializer`: chỉ chạy một lần khi chưa có `demo.buyer@example.com`, không nhân bản dữ liệu ở các lần khởi động sau.

```text
Users:        4 (Admin, seller, buyer, pending user)
Products:     3 (2 Active, 1 Hidden)
Orders:       2
OrderItems:   3
Payments:     2 (Paid, Pending)
ShippingInfo: 2
Disputes:     2 (Assigned, Open)
AuditLogs:    2 (demo seed actions)
```

Tài khoản demo User dùng mật khẩu `Demo@123`; tài khoản Admin vẫn là `admin@gmail.com` / `Admin@123`. Dữ liệu demo phục vụ kiểm thử MVC và Dashboard, không thay thế dữ liệu nghiệp vụ thật.

### Trạng thái Phase Final — Docker + Security Hardening + Documentation

Đã hoàn thành:

- Tách connection string LocalDB (Development) và SQL Server container (Docker).
- Docker nhận `SA_PASSWORD`, `JWT_KEY` và `ADMIN_PASSWORD` qua environment variables.
- Xóa secret khỏi cấu hình production được commit; cấu hình Development chứa secret được ignore, có file `.example` để tạo lại.
- DbInitializer đọc tài khoản Admin từ cấu hình, không hard-code mật khẩu trong code.
- Bổ sung `.env.example`, README hướng dẫn chạy local/Docker và CI workflow đã có restore/build/test/docker build.

Kiểm tra:

- `dotnet build EbayClone.sln --no-restore`: pass, 0 warning, 0 error.
- API `/health`: `Healthy`.
- Login `admin@gmail.com` trả role `Admin` và JWT hợp lệ.
- `docker compose config`: pass. `docker compose up --build` chưa chạy được vì Docker Desktop daemon trên máy hiện không hoạt động.

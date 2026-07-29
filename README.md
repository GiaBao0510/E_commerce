# ShopNET

**ShopNET** là dự án thương mại điện tử được xây dựng bằng **.NET/C#**, tập trung vào phần backend theo hướng **Clean Architecture**, giúp hệ thống dễ mở rộng, dễ bảo trì và tách biệt rõ ràng giữa các tầng nghiệp vụ.

## Mục tiêu dự án

Dự án hướng tới việc xây dựng nền tảng E-Commerce có kiến trúc rõ ràng, có thể phát triển dần theo từng giai đoạn, bao gồm:

- Xác thực & phân quyền người dùng (Authentication/Authorization)
- Quản lý Access Token / Refresh Token
- Triển khai OTP + Redis cho các luồng xác minh
- Tối ưu truy vấn dữ liệu (kết hợp EF Core + Dapper)
- Hỗ trợ kiểm thử (Integration Test)
- Chuẩn bị nền tảng cho mở rộng (RabbitMQ, Kafka, Docker, Socket, Chatbot...)

---

## Cấu trúc repository

```text
ShopNET/
├─ E_Commerce.BackEnd/      # Phần backend chính
├─ E_Commerce.FrontEnd/     # Phần frontend (đang khởi tạo)
├─ Diagram/                 # Sơ đồ thiết kế (UseCase, Class)
└─ README.md
```

---

## Backend Architecture (Clean Architecture)

Thư mục `E_Commerce.BackEnd` được tổ chức theo Clean Architecture gồm các project chính:

### 1) E_commerce.Core (Domain Layer)
- Chứa **entity nghiệp vụ** cốt lõi
- Chứa **custom exception**
- Không phụ thuộc tầng bên ngoài

### 2) E_commerce.Application (Application Layer)
- Định nghĩa **interface** cho repository/service
- Chứa **DTO** cho request/response
- Điều phối use case nghiệp vụ

### 3) E_commerce.Infrastructure (Infrastructure Layer)
- Triển khai interface từ Application
- Xử lý:
  - Truy cập dữ liệu
  - Tích hợp Redis / JWT / OAuth
  - Mapping, configuration, utility, monitoring

### 4) E_commerce.Api (Presentation Layer)
- Cung cấp REST API endpoint
- Controller, middleware, filter, response model
- Là điểm giao tiếp trực tiếp với client

### 5) E_commerce.SQL
- Chứa query/script SQL
- Dùng **Dapper** cho các tác vụ cần hiệu năng truy vấn cao

### 6) E_commerce.Logging
- Ghi log tập trung (log4net)
- Hỗ trợ theo dõi và debug hệ thống

### 7) E_commerce.Test
- Integration test cho API và các thành phần liên quan
- Hỗ trợ đảm bảo chất lượng hệ thống khi phát triển

### 8) DBScripts
- Script khởi tạo cơ sở dữ liệu (ví dụ MySQL)

---

## Công nghệ sử dụng

- **Ngôn ngữ chính:** C# (.NET)
- **Kiến trúc:** Clean Architecture
- **ORM / Data Access:** EF Core + Dapper
- **Authentication:** JWT, Refresh Token, OAuth (Google)
- **Caching/OTP:** Redis
- **Logging:** log4net
- **Container hóa:** Docker / docker-compose
- **Kiểm thử:** MSTest (Integration Test)

---

## Điểm nổi bật bạn đã triển khai

- Thiết kế backend theo **đa tầng rõ ràng**, giảm phụ thuộc chéo
- Tách riêng domain model và tầng triển khai kỹ thuật
- Có định hướng mở rộng thực tế cho hệ thống production:
  - Bảo mật và xác thực
  - Hiệu năng truy vấn
  - Logging/monitoring
  - Message broker & realtime trong tương lai

---

## Tài liệu thiết kế

- `Diagram/UseCase`: Sơ đồ use case
- `Diagram/class`: Sơ đồ lớp (class diagram)

---

## Định hướng phát triển tiếp theo

- Hoàn thiện frontend để đồng bộ fullstack
- Bổ sung CI/CD và quy trình release
- Mở rộng test coverage (unit + integration + e2e)
- Tích hợp thêm message queue/realtime theo roadmap

---

## Ghi chú

Hiện tại repository thể hiện rõ trọng tâm vào phần backend. Frontend đã có cấu trúc khởi tạo và có thể được mở rộng trong các giai đoạn tiếp theo.

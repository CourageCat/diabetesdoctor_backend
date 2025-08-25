## API Endpoints

### Base URL

```
/api/v1/users
```

### 1. Tạo hồ sơ bệnh nhân

**POST** `/api/v1/users/patients`

Tạo mới hồ sơ bệnh nhân tiểu đường với đầy đủ thông tin cá nhân, chẩn đoán và tiền sử.

#### Request Body

```json
{
  "userId": "11111111-1111-1111-1111-111111111111",
  "fullName": "Nguyễn Văn A",
  "dateOfBirth": "1990-01-01T00:00:00Z",
  "gender": "Male",
  "heightCm": 170.0,
  "weightKg": 70.0,
  "diabetes": "Type2",
  "diagnosisRecency": "Recent",
  "year": 2023,
  "type2TreatmentMethod": "OralMedication",
  "controlLevel": "Good",
  "insulinInjectionFrequency": "OnceDaily",
  "complications": ["Retinopathy", "Nephropathy"],
  "otherComplicationDescription": "Mô tả biến chứng khác",
  "exerciseFrequency": "Regular",
  "eatingHabit": "Balanced",
  "usesAlcoholOrTobacco": false,
  "medicalHistories": ["Hypertension", "Dyslipidemia"]
}
```

#### Response

```json
{
  "isSuccess": true,
  "value": {
    "patientId": "22222222-2222-2222-2222-222222222222",
    "fullName": "Nguyễn Văn A",
    "dateOfBirth": "1990-01-01T00:00:00Z",
    "gender": "Male",
    "heightCm": 170.0,
    "weightKg": 70.0,
    "diabetes": "Type2"
  }
}
```

### 2. Tạo chỉ số cân nặng

**POST** `/api/v1/users/patients/records/weight`

Thêm chỉ số cân nặng mới cho bệnh nhân.

#### Request Body

```json
{
  "userId": "11111111-1111-1111-1111-111111111111",
  "value": 72.5,
  "measurementAt": "2024-01-15T08:00:00Z"
}
```

#### Response

```json
{
  "isSuccess": true,
  "value": {
    "id": "33333333-3333-3333-3333-333333333333",
    "value": 72.5,
    "measurementAt": "2024-01-15T08:00:00Z"
  }
}
```

### 3. Tạo chỉ số chiều cao

**POST** `/api/v1/users/patients/records/height`

Thêm chỉ số chiều cao mới cho bệnh nhân.

#### Request Body

```json
{
  "userId": "11111111-1111-1111-1111-111111111111",
  "value": 170.5,
  "measurementAt": "2024-01-15T08:00:00Z"
}
```

### 4. Tạo chỉ số huyết áp

**POST** `/api/v1/users/patients/records/blood-pressure`

Thêm chỉ số huyết áp mới cho bệnh nhân.

#### Request Body

```json
{
  "userId": "11111111-1111-1111-1111-111111111111",
  "systolic": 120,
  "diastolic": 80,
  "personNote": "Đo sau khi nghỉ ngơi 10 phút",
  "measurementAt": "2024-01-15T08:00:00Z"
}
```

### 5. Tạo chỉ số đường huyết

**POST** `/api/v1/users/patients/records/blood-glucose`

Thêm chỉ số đường huyết mới cho bệnh nhân.

#### Request Body

```json
{
  "userId": "11111111-1111-1111-1111-111111111111",
  "value": 120.5,
  "measureTime": "Fasting",
  "personNote": "Đo lúc 6h sáng",
  "measurementAt": "2024-01-15T06:00:00Z"
}
```

#### Các loại thời điểm đo đường huyết:

- `Fasting`: Lúc đói
- `BeforeMeal`: Trước bữa ăn
- `AfterMeal`: Sau bữa ăn

### 6. Tạo chỉ số HbA1c

**POST** `/api/v1/users/patients/records/hbA1c`

Thêm chỉ số HbA1c (đường huyết trung bình 2-3 tháng) mới cho bệnh nhân.

#### Request Body

```json
{
  "userId": "11111111-1111-1111-1111-111111111111",
  "value": 6.5,
  "personNote": "Xét nghiệm tại bệnh viện",
  "measurementAt": "2024-01-15T08:00:00Z"
}
```

### 7. Lấy hồ sơ sức khỏe

**GET** `/api/v1/users/patients/records`

Lấy danh sách các chỉ số sức khỏe của bệnh nhân theo loại và thời gian.

#### Query Parameters

- `recordTypes` (required): Danh sách loại chỉ số, phân cách bằng dấu phẩy
  - `Weight`: Cân nặng
  - `Height`: Chiều cao
  - `BloodGlucose`: Đường huyết
  - `BloodPressure`: Huyết áp
  - `HbA1c`: Chỉ số HbA1c
- `newest` (optional): Lấy bản ghi mới nhất (default: true)
- `fromDate` (optional): Ngày bắt đầu (format: yyyy-MM-dd)
- `toDate` (optional): Ngày kết thúc (format: yyyy-MM-dd)

#### Ví dụ Request

```
GET /api/v1/users/patients/records?recordTypes=Weight,BloodGlucose&newest=false&fromDate=2024-01-01&toDate=2024-01-31
```

#### Response

```json
{
  "isSuccess": true,
  "value": [
    {
      "patientId": "11111111-1111-1111-1111-111111111111",
      "recordType": "Weight",
      "healthRecord": {
        "value": 72.5,
        "measurementAt": "2024-01-15T08:00:00Z"
      },
      "mesurementAt": "2024-01-15T08:00:00Z",
      "personNote": null,
      "assistantNote": null
    },
    {
      "patientId": "11111111-1111-1111-1111-111111111111",
      "recordType": "BloodGlucose",
      "healthRecord": {
        "value": 120.5,
        "measureTime": "Fasting",
        "measurementAt": "2024-01-15T06:00:00Z"
      },
      "mesurementAt": "2024-01-15T06:00:00Z",
      "personNote": "Đo lúc 6h sáng",
      "assistantNote": null
    }
  ]
}
```

## Các Enum Values

### GenderEnum

- `Male`: Nam
- `Female`: Nữ

### DiabetesEnum

- `Type1`: Tuýp 1
- `Type2`: Tuýp 2

### DiagnosisRecencyEnum

- `Recent`: Gần đây
- `LongAgo`: Lâu rồi

### TreatmentMethodEnum

- `OralMedication`: Thuốc uống
- `Insulin`: Insulin
- `DietAndExercise`: Chế độ ăn và tập luyện
- `Combination`: Kết hợp

### ControlLevelEnum

- `Good`: Tốt
- `Fair`: Trung bình
- `Poor`: Kém

### InsulinInjectionFrequencyEnum

- `OnceDaily`: 1 lần/ngày
- `TwiceDaily`: 2 lần/ngày
- `ThreeTimesDaily`: 3 lần/ngày
- `FourTimesDaily`: 4 lần/ngày

### ExerciseFrequencyEnum

- `Never`: Không bao giờ
- `Rarely`: Hiếm khi
- `Sometimes`: Thỉnh thoảng
- `Regular`: Thường xuyên

### EatingHabitEnum

- `Unhealthy`: Không lành mạnh
- `Balanced`: Cân bằng
- `Healthy`: Lành mạnh

### ComplicationEnum

- `Retinopathy`: Bệnh võng mạc
- `Nephropathy`: Bệnh thận
- `Neuropathy`: Bệnh thần kinh
- `Cardiovascular`: Bệnh tim mạch
- `FootProblems`: Vấn đề bàn chân

### MedicalHistoryForDiabetesEnum

- `Hypertension`: Tăng huyết áp
- `Dyslipidemia`: Rối loạn lipid máu
- `Obesity`: Béo phì
- `HeartDisease`: Bệnh tim
- `KidneyDisease`: Bệnh thận

## Error Responses

API trả về lỗi với format sau:

```json
{
  "title": "Validation Error",
  "type": "VALIDATION_ERROR",
  "detail": "Thông báo lỗi chi tiết",
  "status": 400,
  "errors": [
    {
      "code": "FIELD_REQUIRED",
      "message": "Trường này là bắt buộc"
    }
  ]
}
```

## Các mã lỗi thường gặp

- `VALIDATION_ERROR`: Lỗi validation dữ liệu
- `NOT_FOUND`: Không tìm thấy tài nguyên
- `BAD_REQUEST`: Yêu cầu không hợp lệ
- `UNAUTHORIZED`: Chưa xác thực
- `FORBIDDEN`: Không có quyền truy cập

## Cài đặt và chạy

### Yêu cầu hệ thống

- .NET 8.0
- SQL Server hoặc SQLite

### Cài đặt

```bash
# Clone repository
git clone <repository-url>
cd diabetedoctor_User-Service

# Restore packages
dotnet restore

# Run migrations
dotnet ef database update --project src/UserService.Persistence --startup-project src/UserService.Api

# Run application
dotnet run --project src/UserService.Api
```

### Cấu hình

Chỉnh sửa file `src/UserService.Api/appsettings.json` để cấu hình connection string và các thiết lập khác.

## Swagger Documentation

Khi chạy ứng dụng trong môi trường Development, truy cập Swagger UI tại:

```
https://localhost:5001/swagger
```

## Lưu ý

- Tất cả các endpoint đều yêu cầu xác thực (hiện tại đang hardcode userId cho demo)
- Timestamp được lưu theo UTC
- Các giá trị số được lưu với độ chính xác 2 chữ số thập phân
- API versioning được hỗ trợ thông qua URL path

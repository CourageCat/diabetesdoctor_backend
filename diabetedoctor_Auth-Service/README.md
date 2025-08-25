# Auth Service API Documentation

## Authentication APIs

### 1. Register with Phone Number
```http
POST /api/v1/auth/patient/register-phone
```

**Request Body:**
```json
{
    "phoneNumber": "string",  // Required, format: +84xxxxxxxxx
    "password": "string"      // Required
}
```

**Response:**
```json
{
    "value": {
        "code": "AUTH_06",
        "message": "Mã xác nhận đã được gửi. Vui lòng kiểm tra tin nhắn.",
        "data": null
    },
    "isSuccess": true,
    "isFailure": false,
    "error": {
        "code": "",
        "message": ""
    }
}
```

**Possible Errors:**
- 400 Bad Request: Số điện thoại đã được sử dụng (AUTH_01)
- 400 Bad Request: Định dạng số điện thoại không hợp lệ
- 400 Bad Request: Mật khẩu không khớp (AUTH_04)

### 2. Verify OTP Registration
```http
POST /api/v1/auth/patient/verify-otp-register
```

**Request Body:**
```json
{
    "phoneNumber": "string",  // Required, format: +84xxxxxxxxx
    "otpCode": "string"       // Required, 6 digits
}
```

**Response:**
```json
{
    "value": {
        "code": "AUTH_03",
        "message": "Đăng ký thành công.",
        "data": {
            "authToken": {
                "accessToken": "string",
                "refreshToken": "string",
                "expiresAt": "2025-06-13T12:06:10.4547017Z",
                "tokenType": "Bearer"
            },
            "authUser": {
                "id": "string",
                "fullName": "string",
                "avatarUrl": "string",
                "isFirstUpdated": false,
                "roles": ["Patient"]
            }
        }
    },
    "isSuccess": true,
    "isFailure": false,
    "error": {
        "code": "",
        "message": ""
    }
}
```

**Possible Errors:**
- 400 Bad Request: Mã OTP không hợp lệ hoặc đã hết hạn (AUTH_15)
- 400 Bad Request: Mã xác nhận đã hết hạn. Vui lòng thử lại (AUTH_07)
- 400 Bad Request: Tài khoản đã tồn tại (AUTH_17)

### 3. Resend OTP Registration
```http
POST /api/v1/auth/patient/resend-otp-register
```

**Request Body:**
```json
{
    "phoneNumber": "string"  // Required, format: +84xxxxxxxxx
}
```

**Response:**
```json
{
    "value": {
        "code": "AUTH_06",
        "message": "Mã xác nhận đã được gửi. Vui lòng kiểm tra tin nhắn.",
        "data": null
    },
    "isSuccess": true,
    "isFailure": false,
    "error": {
        "code": "",
        "message": ""
    }
}
```

**Possible Errors:**
- 400 Bad Request: Mã OTP không hợp lệ hoặc đã hết hạn (AUTH_15)
- 400 Bad Request: Tài khoản đã tồn tại (AUTH_17)

### 4. Login with Phone Number
```http
POST /api/v1/auth/patient/login-phone
```

**Request Body:**
```json
{
    "phoneNumber": "string",  // Required, format: +84xxxxxxxxx
    "password": "string"      // Required
}
```

**Response:**
```json
{
    "value": {
        "code": "AUTH_05",
        "message": "Đăng nhập thành công.",
        "data": {
            "authToken": {
                "accessToken": "string",
                "refreshToken": "string",
                "expiresAt": "2025-06-13T12:06:10.4547017Z",
                "tokenType": "Bearer"
            },
            "authUser": {
                "id": "string",
                "fullName": "string",
                "avatarUrl": "string",
                "isFirstUpdated": false,
                "roles": ["Patient"]
            }
        }
    },
    "isSuccess": true,
    "isFailure": false,
    "error": {
        "code": "",
        "message": ""
    }
}
```

**Possible Errors:**
- 400 Bad Request: Tài khoản không tồn tại (AUTH_18)
- 400 Bad Request: Số điện thoại hoặc mật khẩu không chính xác (AUTH_13)
- 400 Bad Request: Tài khoản đã bị khóa (AUTH_08)

### 5. Refresh Token
```http
POST /api/v1/auth/refresh-token
```

**Request Body:**
```json
{
    "refreshToken": "string"  // Required
}
```

**Response:**
```json
{
    "value": {
        "code": "AUTH_10",
        "message": "Làm mới phiên đăng nhập thành công.",
        "data": {
            "authToken": {
                "accessToken": "string",
                "refreshToken": "string",
                "expiresAt": "2025-06-13T12:06:10.4547017Z",
                "tokenType": "Bearer"
            },
            "authUser": {
                "id": "string",
                "fullName": "string",
                "avatarUrl": "string",
                "isFirstUpdated": false,
                "roles": ["Patient"]
            }
        }
    },
    "isSuccess": true,
    "isFailure": false,
    "error": {
        "code": "",
        "message": ""
    }
}
```

**Possible Errors:**
- 400 Bad Request: Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại (AUTH_11)
- 400 Bad Request: Phiên đăng nhập đã hết hạn (AUTH_14)
- 401 Unauthorized: Token không hợp lệ

### 6. Logout
```http
DELETE /api/v1/auth/logout
```

**Headers Required:**
```
Authorization: Bearer {access_token}
X-User-Id: {user_id}
```

**Response:**
```json
{
    "value": {
        "code": "AUTH_12",
        "message": "Đăng xuất thành công.",
        "data": null
    },
    "isSuccess": true,
    "isFailure": false,
    "error": {
        "code": "",
        "message": ""
    }
}
```

**Possible Errors:**
- 401 Unauthorized: Thiếu hoặc không hợp lệ header xác thực
- 401 Unauthorized: ID người dùng không hợp lệ
- 403 Forbidden: Không có quyền truy cập

## Common Error Response Format

All endpoints may return the following error response format:

```json
{
    "value": null,
    "isSuccess": false,
    "isFailure": true,
    "error": {
        "code": "AUTH_XX",
        "message": "Thông báo lỗi"
    }
}
```

## HTTP Status Codes

- 200: Thành công
- 400: Bad Request - Dữ liệu đầu vào không hợp lệ hoặc lỗi logic nghiệp vụ
- 401: Unauthorized - Thiếu hoặc không hợp lệ xác thực
- 403: Forbidden - Không đủ quyền truy cập
- 404: Not Found - Không tìm thấy tài nguyên
- 500: Internal Server Error - Lỗi máy chủ

## Error Codes

| Code | Description |
|------|-------------|
| AUTH_01 | Số điện thoại đã được sử dụng |
| AUTH_02 | Số điện thoại chưa được đăng ký |
| AUTH_03 | Đăng ký thành công |
| AUTH_04 | Mật khẩu không khớp |
| AUTH_05 | Đăng nhập thành công |
| AUTH_06 | Mã xác nhận đã được gửi. Vui lòng kiểm tra tin nhắn |
| AUTH_07 | Mã xác nhận đã hết hạn. Vui lòng thử lại |
| AUTH_08 | Tài khoản đã bị khóa |
| AUTH_09 | Đổi mật khẩu thành công |
| AUTH_10 | Làm mới phiên đăng nhập thành công |
| AUTH_11 | Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại |
| AUTH_12 | Đăng xuất thành công |
| AUTH_13 | Số điện thoại hoặc mật khẩu không chính xác |
| AUTH_14 | Phiên đăng nhập đã hết hạn |
| AUTH_15 | Mã OTP không hợp lệ hoặc đã hết hạn |
| AUTH_16 | Xác minh số điện thoại thành công |
| AUTH_17 | Tài khoản đã tồn tại |
| AUTH_18 | Tài khoản không tồn tại | 
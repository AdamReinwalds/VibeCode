# API Endpoints Design

## Authentication Endpoints

### POST /api/auth/register

Register a new user account.

**Request:**

```json
{
  "username": "string",
  "email": "string",
  "password": "string",
  "firstName": "string",
  "lastName": "string"
}
```

**Response (201):**

```json
{
  "userId": "string",
  "username": "string",
  "email": "string",
  "firstName": "string",
  "lastName": "string"
}
```

**Errors:** 400 (validation errors), 409 (username/email exists)

### POST /api/auth/login

Authenticate user and return JWT token.

**Request:**

```json
{
  "username": "string",
  "password": "string"
}
```

**Response (200):**

```json
{
  "token": "string",
  "user": {
    "userId": "string",
    "username": "string",
    "email": "string",
    "firstName": "string",
    "lastName": "string"
  }
}
```

**Errors:** 401 (invalid credentials), 400 (validation errors)

## Basket Endpoints

### GET /api/basket

Get current user's basket with items.

**Headers:** Authorization: Bearer {token}

**Response (200):**

```json
{
  "basketId": "string",
  "items": [
    {
      "productId": "string",
      "name": "string",
      "price": 29.99,
      "quantity": 2,
      "totalPrice": 59.98
    }
  ],
  "totalAmount": 59.98
}
```

**Errors:** 401 (unauthorized), 404 (basket not found)

### POST /api/basket/add

Add product to user's basket.

**Headers:** Authorization: Bearer {token}

**Request:**

```json
{
  "productId": "string",
  "quantity": 1
}
```

**Response (200):**

```json
{
  "basketId": "string",
  "item": {
    "productId": "string",
    "name": "string",
    "price": 29.99,
    "quantity": 1,
    "totalPrice": 29.99
  },
  "totalAmount": 29.99
}
```

**Errors:** 401 (unauthorized), 404 (product not found), 400 (invalid quantity)

### DELETE /api/basket/remove/{productId}

Remove product from user's basket.

**Headers:** Authorization: Bearer {token}

**Response (200):** Empty

**Errors:** 401 (unauthorized), 404 (basket/product not found)

### DELETE /api/basket/clear

Clear all items from user's basket.

**Headers:** Authorization: Bearer {token}

**Response (200):** Empty

**Errors:** 401 (unauthorized)

## Checkout Endpoints

### POST /api/checkout

Process checkout and create order.

**Headers:** Authorization: Bearer {token}

**Request:**

```json
{
  "shippingAddress": {
    "street": "string",
    "city": "string",
    "postalCode": "string",
    "country": "string"
  },
  "paymentMethod": "string"
}
```

**Response (201):**

```json
{
  "orderId": "string",
  "totalAmount": 59.98,
  "status": "Paid",
  "createdAt": "2023-10-21T12:00:00Z"
}
```

**Errors:** 401 (unauthorized), 400 (empty basket/invalid address), 409 (payment failed)

### GET /api/orders

Get user's order history.

**Headers:** Authorization: Bearer {token}

**Query Parameters:** status (optional), page (optional), limit (optional)

**Response (200):**

```json
{
  "orders": [
    {
      "orderId": "string",
      "totalAmount": 59.98,
      "status": "Paid",
      "createdAt": "2023-10-21T12:00:00Z",
      "items": [
        {
          "productId": "string",
          "name": "string",
          "price": 29.99,
          "quantity": 2
        }
      ]
    }
  ],
  "pagination": {
    "page": 1,
    "limit": 10,
    "total": 5
  }
}
```

**Errors:** 401 (unauthorized)

## Products Endpoints (for MVP)

### GET /api/products

Get available products (for basket additions).

**Query Parameters:** category (optional), page (optional), limit (optional)

**Response (200):**

```json
{
  "products": [
    {
      "productId": "string",
      "name": "string",
      "description": "string",
      "price": 29.99,
      "imageUrl": "string",
      "stock": 100,
      "category": "string"
    }
  ],
  "pagination": {
    "page": 1,
    "limit": 10,
    "total": 50
  }
}
```

## General Error Response

All endpoints return errors in this format:

```json
{
  "error": {
    "code": "string",
    "message": "string",
    "details": {}
  }
}
```

## Security Notes

- All basket and checkout endpoints require Bearer token authentication
- JWT tokens expire after 24 hours
- Passwords are hashed using bcrypt
- Rate limiting applied to auth endpoints

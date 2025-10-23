# MongoDB Schema Design

## Collections Overview

### 1. Users Collection

```javascript
{
  "_id": ObjectId,
  "username": "string (unique)",
  "email": "string (unique)",
  "passwordHash": "string",
  "firstName": "string",
  "lastName": "string",
  "createdAt": "Date",
  "updatedAt": "Date"
}
```

### 2. Products Collection

```javascript
{
  "_id": ObjectId,
  "name": "string",
  "description": "string",
  "price": "decimal",
  "imageUrl": "string",
  "stock": "number",
  "category": "string",
  "createdAt": "Date",
  "updatedAt": "Date"
}
```

### 3. Baskets Collection

```javascript
{
  "_id": ObjectId,
  "userId": "ObjectId (reference to Users)",
  "items": [
    {
      "productId": "ObjectId (reference to Products)",
      "quantity": "number",
      "price": "decimal" // Snapshot of price at time of adding
    }
  ],
  "createdAt": "Date",
  "updatedAt": "Date"
}
```

### 4. Orders Collection

```javascript
{
  "_id": ObjectId,
  "userId": "ObjectId (reference to Users)",
  "items": [
    {
      "productId": "ObjectId (reference to Products)",
      "quantity": "number",
      "price": "decimal"
    }
  ],
  "totalAmount": "decimal",
  "status": "string (enum: Pending, Paid, Shipped, Delivered, Cancelled)",
  "shippingAddress": {
    "street": "string",
    "city": "string",
    "postalCode": "string",
    "country": "string"
  },
  "paymentMethod": "string",
  "createdAt": "Date",
  "updatedAt": "Date"
}
```

## Design Considerations

### Data Integrity

- Use references (ObjectId) instead of embedding for relationships to avoid data duplication
- Maintain referential integrity through application logic (MongoDB doesn't enforce foreign keys)

### Indexing Strategy

- Users: Unique index on username and email
- Products: Index on category for filtering
- Baskets: Index on userId for quick user basket retrieval
- Orders: Index on userId and status for querying user orders

### Transactions

- Use MongoDB transactions for checkout process to ensure atomicity between basket operations and order creation

### Schema Evolution

- Store version numbers in documents if schema changes are needed
- Use migration scripts for data transformations

## Initial Seed Data

For development/testing, we'll seed basic products and possibly a test user.

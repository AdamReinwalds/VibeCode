export interface ShippingAddressDto {
  street: string;
  city: string;
  postalCode: string;
  country: string;
}

export interface CheckoutRequest {
  shippingAddress: ShippingAddressDto;
  paymentMethod: string;
}

export interface OrderSummaryDto {
  orderId: string;
  totalAmount: number;
  status: string;
  createdAt: string;
}

export interface OrderDto {
  orderId: string;
  totalAmount: number;
  status: string;
  createdAt: string;
  items: OrderItemDto[];
}

export interface OrderItemDto {
  productId: string;
  name: string;
  price: number;
  quantity: number;
}

export interface OrdersResponse {
  orders: OrderDto[];
  pagination: PaginationDto;
}

export interface PaginationDto {
  page: number;
  limit: number;
  total: number;
}

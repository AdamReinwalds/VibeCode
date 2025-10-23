export interface BasketDto {
  basketId: string;
  items: BasketItemDto[];
  totalAmount: number;
}

export interface BasketItemDto {
  productId: string;
  name: string;
  price: number;
  quantity: number;
  totalPrice: number;
}

export interface AddToBasketRequest {
  productId: string;
  quantity: number;
}

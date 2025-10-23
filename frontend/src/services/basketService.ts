import api from "./api";
import type {
  BasketDto,
  BasketItemDto,
  AddToBasketRequest,
} from "@/types/basket";

export const basketService = {
  async getBasket(): Promise<BasketDto> {
    const response = await api.get<BasketDto>("/basket");
    return response.data;
  },

  async updateBasketItem(
    productId: string,
    quantity: number
  ): Promise<BasketItemDto> {
    const response = await api.put<{ basketItem: BasketItemDto }>(
      `/basket/update/${productId}`,
      { quantity }
    );
    return response.data.basketItem;
  },

  async addToBasket(request: AddToBasketRequest): Promise<BasketItemDto> {
    const response = await api.post<{ basketItem: BasketItemDto }>(
      "/basket/add",
      request
    );
    return response.data.basketItem;
  },

  async removeFromBasket(productId: string): Promise<void> {
    await api.delete(`/basket/remove/${productId}`);
  },

  async clearBasket(): Promise<void> {
    await api.delete("/basket/clear");
  },
};

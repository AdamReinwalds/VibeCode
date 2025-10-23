import { defineStore } from "pinia";
import { ref } from "vue";
import type { BasketDto, BasketItemDto } from "@/types/basket";

export const useBasketStore = defineStore("basket", () => {
  const basket = ref<BasketDto | null>(null);
  const loading = ref(false);

  const setBasket = (newBasket: BasketDto) => {
    basket.value = newBasket;
  };

  const addItem = (item: BasketItemDto) => {
    if (basket.value) {
      const existingItem = basket.value.items.find(
        (i) => i.productId === item.productId
      );
      if (existingItem) {
        existingItem.quantity = item.quantity;
        existingItem.totalPrice = item.totalPrice;
      } else {
        basket.value.items.push(item);
      }
      basket.value.totalAmount = basket.value.items.reduce(
        (sum, item) => sum + item.totalPrice,
        0
      );
    }
  };

  const removeItem = (productId: string) => {
    if (basket.value) {
      basket.value.items = basket.value.items.filter(
        (item) => item.productId !== productId
      );
      basket.value.totalAmount = basket.value.items.reduce(
        (sum, item) => sum + item.totalPrice,
        0
      );
    }
  };

  const clearBasket = () => {
    if (basket.value) {
      basket.value.items = [];
      basket.value.totalAmount = 0;
    }
  };

  const setLoading = (isLoading: boolean) => {
    loading.value = isLoading;
  };

  return {
    basket,
    loading,
    setBasket,
    addItem,
    removeItem,
    clearBasket,
    setLoading,
  };
});

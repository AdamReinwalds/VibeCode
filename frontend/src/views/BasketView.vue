<template>
  <div class="basket-view">
    <h2>Your Basket</h2>

    <div
      v-if="!basketStore.basket || basketStore.basket.items.length === 0"
      class="empty-basket"
    >
      <p>Your basket is empty.</p>
      <router-link to="/" class="continue-shopping"
        >Continue Shopping</router-link
      >
    </div>

    <div v-else class="basket-content">
      <div class="basket-items">
        <div
          v-for="item in basketStore.basket?.items || []"
          :key="item.productId"
          class="basket-item"
        >
          <div class="item-info">
            <h3>{{ item.name }}</h3>
            <p class="price">${{ item.price.toFixed(2) }} each</p>
          </div>

          <div class="item-controls">
            <div class="quantity-controls">
              <button
                @click="updateQuantity(item.productId, item.quantity - 1)"
                :disabled="item.quantity <= 1"
              >
                -
              </button>
              <span>{{ item.quantity }}</span>
              <button @click="increaseQuantity(item.productId)">+</button>
            </div>
            <p class="item-total">${{ item.totalPrice.toFixed(2) }}</p>
            <button @click="removeItem(item.productId)" class="remove-btn">
              Remove
            </button>
          </div>
        </div>
      </div>

      <div class="basket-summary">
        <div class="summary-row">
          <span>Total:</span>
          <span class="total-amount"
            >${{ basketStore.basket.totalAmount.toFixed(2) }}</span
          >
        </div>

        <div class="basket-actions">
          <button
            @click="clearBasket"
            class="clear-btn"
            :disabled="basketStore.loading"
          >
            Clear Basket
          </button>
          <router-link to="/checkout" class="checkout-btn"
            >Proceed to Checkout</router-link
          >
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from "vue";
import { useBasketStore } from "@/stores/basket";
import { basketService } from "@/services/basketService";

const basketStore = useBasketStore();

onMounted(async () => {
  try {
    basketStore.setLoading(true);
    const basket = await basketService.getBasket();
    basketStore.setBasket(basket);
  } catch (error) {
    console.error("Failed to load basket:", error);
  } finally {
    basketStore.setLoading(false);
  }
});

const updateQuantity = async (productId: string, newQuantity: number) => {
  if (newQuantity < 1) return;

  try {
    basketStore.setLoading(true);
    if (newQuantity === 0) {
      await removeItem(productId);
    } else {
      // Use the update endpoint if available, otherwise add the difference
      const result = await basketService.updateBasketItem(
        productId,
        newQuantity
      );
      basketStore.addItem(result);
    }
  } catch (error: any) {
    console.error("Failed to update quantity:", error);
    // Fallback to adding difference if update endpoint doesn't exist
    if (error.response?.status === 404) {
      const currentItem = basketStore.basket?.items.find(
        (i) => i.productId === productId
      );
      if (currentItem) {
        const difference = newQuantity - currentItem.quantity;
        if (difference > 0) {
          const result = await basketService.addToBasket({
            productId,
            quantity: difference,
          });
          basketStore.addItem(result);
        }
      }
    } else {
      alert("Failed to update item quantity");
    }
  } finally {
    basketStore.setLoading(false);
  }
};

const increaseQuantity = async (productId: string) => {
  const currentItem = basketStore.basket?.items.find(
    (i) => i.productId === productId
  );
  if (currentItem) {
    await updateQuantity(productId, currentItem.quantity + 1);
  }
};

const removeItem = async (productId: string) => {
  try {
    basketStore.setLoading(true);
    await basketService.removeFromBasket(productId);
    basketStore.removeItem(productId);
  } catch (error) {
    console.error("Failed to remove item:", error);
    alert("Failed to remove item from basket");
  } finally {
    basketStore.setLoading(false);
  }
};

const clearBasket = async () => {
  if (!confirm("Are you sure you want to clear your basket?")) return;

  try {
    basketStore.setLoading(true);
    await basketService.clearBasket();
    basketStore.clearBasket();
  } catch (error) {
    console.error("Failed to clear basket:", error);
    alert("Failed to clear basket");
  } finally {
    basketStore.setLoading(false);
  }
};
</script>

<style scoped>
.basket-view {
  max-width: 800px;
  margin: 2rem auto;
  padding: 0 2rem;
}

.basket-view h2 {
  color: #495057;
  margin-bottom: 2rem;
}

.empty-basket {
  text-align: center;
  padding: 3rem;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.continue-shopping {
  display: inline-block;
  background-color: #007bff;
  color: white;
  text-decoration: none;
  padding: 0.75rem 1.5rem;
  border-radius: 4px;
  margin-top: 1rem;
}

.basket-content {
  display: grid;
  gap: 2rem;
}

.basket-items {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.basket-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-bottom: 1px solid #dee2e6;
}

.basket-item:last-child {
  border-bottom: none;
}

.item-info h3 {
  margin: 0 0 0.5rem 0;
  color: #495057;
}

.price {
  color: #6c757d;
  margin: 0;
  font-size: 0.9rem;
}

.item-controls {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.quantity-controls {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.quantity-controls button {
  width: 30px;
  height: 30px;
  border: 1px solid #dee2e6;
  background: white;
  border-radius: 4px;
  cursor: pointer;
}

.quantity-controls span {
  font-weight: bold;
  min-width: 30px;
  text-align: center;
}

.item-total {
  font-weight: bold;
  color: #28a745;
  min-width: 80px;
  text-align: right;
}

.remove-btn {
  background-color: #dc3545;
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.9rem;
}

.remove-btn:hover {
  background-color: #c82333;
}

.basket-summary {
  background: white;
  border-radius: 8px;
  padding: 1.5rem;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.summary-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 1.2rem;
  font-weight: bold;
  margin-bottom: 1.5rem;
}

.total-amount {
  color: #28a745;
}

.basket-actions {
  display: flex;
  gap: 1rem;
  justify-content: space-between;
}

.clear-btn {
  background-color: #6c757d;
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 4px;
  cursor: pointer;
}

.clear-btn:hover {
  background-color: #545b62;
}

.checkout-btn {
  background-color: #28a745;
  color: white;
  text-decoration: none;
  padding: 0.75rem 1.5rem;
  border-radius: 4px;
  text-align: center;
  transition: background-color 0.2s;
}

.checkout-btn:hover {
  background-color: #218838;
}
</style>

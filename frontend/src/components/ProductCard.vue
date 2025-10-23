<template>
  <div class="product-card">
    <div class="product-info">
      <h3>{{ product.name }}</h3>
      <p class="description">{{ product.description }}</p>
      <p class="price">${{ product.price.toFixed(2) }}</p>
      <p class="stock">Stock: {{ product.stock }}</p>
    </div>

    <div class="product-actions">
      <div class="quantity-controls">
        <button @click="decreaseQuantity" :disabled="quantity <= 1">-</button>
        <span>{{ quantity }}</span>
        <button @click="increaseQuantity" :disabled="quantity >= product.stock">
          +
        </button>
      </div>
      <button
        @click="addToBasket"
        :disabled="loading || quantity > product.stock"
        class="add-btn"
      >
        {{ loading ? "Adding..." : "Add to Basket" }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, defineProps, defineEmits } from "vue";
import { useBasketStore } from "@/stores/basket";
import { basketService } from "@/services/basketService";
import type { Product } from "@/types/product";

interface Props {
  product: Product;
}

const props = defineProps<Props>();

const emit = defineEmits<{
  added: [productId: string];
}>();

const basketStore = useBasketStore();
const quantity = ref(1);
const loading = ref(false);

const increaseQuantity = () => {
  if (quantity.value < props.product.stock) {
    quantity.value++;
  }
};

const decreaseQuantity = () => {
  if (quantity.value > 1) {
    quantity.value--;
  }
};

const addToBasket = async () => {
  loading.value = true;
  try {
    const result = await basketService.addToBasket({
      productId: props.product.id,
      quantity: quantity.value,
    });

    basketStore.addItem(result);
    emit("added", props.product.id);
    quantity.value = 1; // Reset quantity after successful add
  } catch (error) {
    console.error("Failed to add to basket:", error);
    alert("Failed to add product to basket");
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
.product-card {
  background: white;
  border: 1px solid #dee2e6;
  border-radius: 8px;
  padding: 1.5rem;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.product-info {
  flex: 1;
}

.product-info h3 {
  margin: 0 0 0.5rem 0;
  color: #495057;
}

.description {
  color: #6c757d;
  margin: 0 0 1rem 0;
  font-size: 0.9rem;
}

.price {
  font-size: 1.2rem;
  font-weight: bold;
  color: #28a745;
  margin: 0 0 0.5rem 0;
}

.stock {
  color: #6c757d;
  font-size: 0.8rem;
  margin: 0;
}

.product-actions {
  display: flex;
  flex-direction: column;
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

.quantity-controls button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.quantity-controls span {
  font-weight: bold;
  min-width: 30px;
  text-align: center;
}

.add-btn {
  background-color: #007bff;
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.9rem;
}

.add-btn:disabled {
  background-color: #6c757d;
  cursor: not-allowed;
}

.add-btn:hover:not(:disabled) {
  background-color: #0056b3;
}
</style>

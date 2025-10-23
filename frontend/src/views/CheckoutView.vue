<template>
  <div class="checkout-view">
    <h2>Checkout</h2>

    <div
      v-if="!basketStore.basket || basketStore.basket.items.length === 0"
      class="empty-checkout"
    >
      <p>Your basket is empty. Add some items before checking out.</p>
      <router-link to="/" class="continue-shopping"
        >Continue Shopping</router-link
      >
    </div>

    <div v-else class="checkout-content">
      <div class="checkout-form">
        <form @submit.prevent="handleCheckout">
          <h3>Shipping Information</h3>

          <div class="form-row">
            <div class="form-group">
              <label for="firstName">First Name:</label>
              <input
                id="firstName"
                v-model="form.firstName"
                type="text"
                required
              />
            </div>

            <div class="form-group">
              <label for="lastName">Last Name:</label>
              <input
                id="lastName"
                v-model="form.lastName"
                type="text"
                required
              />
            </div>
          </div>

          <div class="form-group">
            <label for="street">Street Address:</label>
            <input id="street" v-model="form.street" type="text" required />
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="city">City:</label>
              <input id="city" v-model="form.city" type="text" required />
            </div>

            <div class="form-group">
              <label for="postalCode">Postal Code:</label>
              <input
                id="postalCode"
                v-model="form.postalCode"
                type="text"
                required
              />
            </div>
          </div>

          <div class="form-group">
            <label for="country">Country:</label>
            <input id="country" v-model="form.country" type="text" required />
          </div>

          <h3>Payment Information</h3>

          <div class="form-group">
            <label for="paymentMethod">Payment Method:</label>
            <select id="paymentMethod" v-model="form.paymentMethod" required>
              <option value="Credit Card">Credit Card</option>
              <option value="Debit Card">Debit Card</option>
              <option value="PayPal">PayPal</option>
              <option value="Bank Transfer">Bank Transfer</option>
            </select>
          </div>

          <div class="form-group">
            <label for="cardNumber">Card Number (Demo):</label>
            <input
              id="cardNumber"
              v-model="form.cardNumber"
              type="text"
              placeholder="1234 5678 9012 3456"
            />
          </div>

          <button type="submit" :disabled="loading" class="checkout-btn">
            {{
              loading
                ? "Processing..."
                : `Complete Order - $${basketStore.basket.totalAmount.toFixed(
                    2
                  )}`
            }}
          </button>
        </form>
      </div>

      <div class="order-summary">
        <h3>Order Summary</h3>

        <div class="order-items">
          <div
            v-for="item in basketStore.basket.items"
            :key="item.productId"
            class="order-item"
          >
            <div class="item-info">
              <span class="item-name">{{ item.name }}</span>
              <span class="item-quantity">Qty: {{ item.quantity }}</span>
            </div>
            <span class="item-price">${{ item.totalPrice.toFixed(2) }}</span>
          </div>
        </div>

        <div class="order-total">
          <div class="total-row">
            <span>Total:</span>
            <span class="total-amount"
              >${{ basketStore.basket.totalAmount.toFixed(2) }}</span
            >
          </div>
        </div>
      </div>
    </div>

    <div v-if="orderComplete" class="order-complete">
      <h3>Order Completed Successfully!</h3>
      <p>Order ID: {{ orderComplete.orderId }}</p>
      <p>Total: ${{ orderComplete.totalAmount.toFixed(2) }}</p>
      <router-link to="/" class="continue-shopping"
        >Continue Shopping</router-link
      >
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import { useBasketStore } from "@/stores/basket";
import { basketService } from "@/services/basketService";
import { checkoutService } from "@/services/checkoutService";
import type { OrderSummaryDto } from "@/types/checkout";

interface CheckoutForm {
  firstName: string;
  lastName: string;
  street: string;
  city: string;
  postalCode: string;
  country: string;
  paymentMethod: string;
  cardNumber: string;
}

const router = useRouter();
const basketStore = useBasketStore();

const form = ref<CheckoutForm>({
  firstName: "",
  lastName: "",
  street: "",
  city: "",
  postalCode: "",
  country: "",
  paymentMethod: "Credit Card",
  cardNumber: "",
});

const loading = ref(false);
const orderComplete = ref<OrderSummaryDto | null>(null);

onMounted(async () => {
  try {
    basketStore.setLoading(true);
    const basket = await basketService.getBasket();
    basketStore.setBasket(basket);

    if (!basket.items.length) {
      router.push("/");
      return;
    }
  } catch (error) {
    console.error("Failed to load basket:", error);
  } finally {
    basketStore.setLoading(false);
  }
});

const handleCheckout = async () => {
  loading.value = true;

  try {
    const checkoutRequest = {
      shippingAddress: {
        street: form.value.street,
        city: form.value.city,
        postalCode: form.value.postalCode,
        country: form.value.country,
      },
      paymentMethod: form.value.paymentMethod,
    };

    const orderSummary = await checkoutService.checkout(checkoutRequest);

    // Clear the basket in the store since checkout was successful
    basketStore.clearBasket();
    orderComplete.value = orderSummary;
  } catch (error: any) {
    console.error("Checkout failed:", error);
    alert(
      error.response?.data?.error?.message ||
        "Checkout failed. Please try again."
    );
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
.checkout-view {
  max-width: 1000px;
  margin: 2rem auto;
  padding: 0 2rem;
}

.checkout-view h2 {
  color: #495057;
  margin-bottom: 2rem;
  text-align: center;
}

.empty-checkout {
  text-align: center;
  padding: 3rem;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.checkout-content {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 2rem;
}

.checkout-form {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.checkout-form h3 {
  color: #495057;
  margin-bottom: 1.5rem;
  padding-bottom: 0.5rem;
  border-bottom: 1px solid #dee2e6;
}

.form-row {
  display: flex;
  gap: 1rem;
}

.form-group {
  margin-bottom: 1rem;
  flex: 1;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: bold;
  color: #495057;
}

input,
select {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #dee2e6;
  border-radius: 4px;
  font-size: 1rem;
}

.checkout-btn {
  width: 100%;
  background-color: #28a745;
  color: white;
  border: none;
  padding: 1rem;
  border-radius: 4px;
  font-size: 1.1rem;
  font-weight: bold;
  cursor: pointer;
  margin-top: 2rem;
}

.checkout-btn:disabled {
  background-color: #6c757d;
  cursor: not-allowed;
}

.order-summary {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  height: fit-content;
}

.order-summary h3 {
  color: #495057;
  margin-bottom: 1.5rem;
  text-align: center;
}

.order-items {
  margin-bottom: 2rem;
}

.order-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem 0;
  border-bottom: 1px solid #f8f9fa;
}

.order-item:last-child {
  border-bottom: none;
}

.item-info {
  display: flex;
  flex-direction: column;
}

.item-name {
  font-weight: bold;
  color: #495057;
}

.item-quantity {
  font-size: 0.9rem;
  color: #6c757d;
}

.item-price {
  font-weight: bold;
  color: #28a745;
}

.order-total {
  border-top: 2px solid #dee2e6;
  padding-top: 1rem;
}

.total-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 1.2rem;
  font-weight: bold;
}

.total-amount {
  color: #28a745;
}

.order-complete {
  text-align: center;
  padding: 3rem;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.order-complete h3 {
  color: #28a745;
  margin-bottom: 1rem;
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

@media (max-width: 768px) {
  .checkout-content {
    grid-template-columns: 1fr;
  }
}
</style>

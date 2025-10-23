<template>
  <div class="home-view">
    <div class="welcome-section">
      <h2>Welcome, {{ authStore.user?.firstName }}!</h2>
      <p>You are successfully logged into the E-Commerce MVP application.</p>
    </div>

    <div class="features-section">
      <h3>Available Features:</h3>
      <div class="feature-grid">
        <div class="feature-card">
          <h4>👕 Browse Products</h4>
          <p>Explore our collection of clothing and accessories.</p>
          <router-link to="/products" class="feature-link"
            >Browse Products</router-link
          >
        </div>

        <div class="feature-card">
          <h4>� Basket Management</h4>
          <p>Add products to your basket, view items, and manage quantities.</p>
          <router-link to="/basket" class="feature-link"
            >Go to Basket</router-link
          >
        </div>

        <div class="feature-card">
          <h4>💳 Checkout Process</h4>
          <p>Complete your purchase with our secure checkout system.</p>
          <router-link to="/checkout" class="feature-link"
            >Start Checkout</router-link
          >
        </div>

        <div class="feature-card">
          <h4>📦 Order History</h4>
          <p>View your previous orders and purchase history.</p>
          <button @click="viewOrders" class="feature-link">View Orders</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from "@/stores/auth";
import { useRouter } from "vue-router";

const authStore = useAuthStore();
const router = useRouter();

const viewOrders = () => {
  // For now, we'll just show an alert. In a full implementation, this would navigate to an orders page
  alert("Order history feature will be available soon!");
};
</script>

<style scoped>
.home-view {
  max-width: 800px;
  margin: 2rem auto;
  padding: 0 2rem;
}

.welcome-section {
  text-align: center;
  margin-bottom: 3rem;
}

.welcome-section h2 {
  color: #495057;
  margin-bottom: 1rem;
}

.features-section h3 {
  color: #495057;
  margin-bottom: 2rem;
  text-align: center;
}

.feature-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 2rem;
}

.feature-card {
  background: white;
  border: 1px solid #dee2e6;
  border-radius: 8px;
  padding: 1.5rem;
  text-align: center;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.feature-card h4 {
  color: #495057;
  margin-bottom: 1rem;
}

.feature-card p {
  color: #6c757d;
  margin-bottom: 1.5rem;
  line-height: 1.5;
}

.feature-link {
  display: inline-block;
  background-color: #007bff;
  color: white;
  text-decoration: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  transition: background-color 0.2s;
}

.feature-link:hover {
  background-color: #0056b3;
}
</style>

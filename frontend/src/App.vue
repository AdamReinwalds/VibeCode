<script setup lang="ts">
import { RouterLink, RouterView } from "vue-router";
import { useAuthStore } from "@/stores/auth";
import { onMounted } from "vue";

const authStore = useAuthStore();

onMounted(() => {
  authStore.initializeAuth();
});
</script>

<template>
  <header v-if="authStore.isAuthenticated">
    <div class="wrapper">
      <h1>E-Commerce MVP</h1>

      <nav>
        <RouterLink to="/">Home</RouterLink>
        <RouterLink to="/products">Products</RouterLink>
        <RouterLink to="/basket">Basket</RouterLink>
        <RouterLink to="/checkout">Checkout</RouterLink>
        <button @click="authStore.logout()" class="logout-btn">Logout</button>
      </nav>
    </div>
  </header>

  <main>
    <RouterView />
  </main>
</template>

<style scoped>
header {
  background-color: #f8f9fa;
  padding: 1rem 0;
  border-bottom: 1px solid #dee2e6;
}

.wrapper {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

h1 {
  margin: 0;
  color: #495057;
}

nav {
  display: flex;
  gap: 1rem;
  align-items: center;
}

nav a {
  color: #007bff;
  text-decoration: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  transition: background-color 0.2s;
}

nav a:hover,
nav a.router-link-active {
  background-color: #e9ecef;
}

.logout-btn {
  background-color: #dc3545;
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
}

.logout-btn:hover {
  background-color: #c82333;
}

main {
  min-height: calc(100vh - 80px);
}
</style>

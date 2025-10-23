<template>
  <div class="products-view">
    <h2>Our Products</h2>

    <div class="category-filters">
      <button
        @click="selectedCategory = null"
        :class="{ active: selectedCategory === null }"
        class="filter-btn"
      >
        All Products
      </button>
      <button
        v-for="category in categories"
        :key="category"
        @click="selectedCategory = category"
        :class="{ active: selectedCategory === category }"
        class="filter-btn"
      >
        {{ category }}
      </button>
    </div>

    <div v-if="loading" class="loading">
      <p>Loading products...</p>
    </div>

    <div v-else-if="error" class="error">
      <p>{{ error }}</p>
      <button @click="loadProducts" class="retry-btn">Retry</button>
    </div>

    <div v-else class="products-grid">
      <ProductCard
        v-for="product in filteredProducts"
        :key="product.id"
        :product="product"
        @added="onProductAdded"
      />
    </div>

    <div
      v-if="!loading && !error && filteredProducts.length === 0"
      class="no-products"
    >
      <p>No products found in this category.</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import ProductCard from "@/components/ProductCard.vue";
import { productService } from "@/services/productService";
import type { Product } from "@/types/product";

const products = ref<Product[]>([]);
const loading = ref(false);
const error = ref<string | null>(null);
const selectedCategory = ref<string | null>(null);

const categories = computed(() => {
  const uniqueCategories = new Set(products.value.map((p) => p.category));
  return Array.from(uniqueCategories).sort();
});

const filteredProducts = computed(() => {
  if (selectedCategory.value === null) {
    return products.value;
  }
  return products.value.filter((p) => p.category === selectedCategory.value);
});

const loadProducts = async () => {
  loading.value = true;
  error.value = null;

  try {
    const fetchedProducts = await productService.getAllProducts();
    products.value = fetchedProducts;
  } catch (err) {
    console.error("Failed to load products:", err);
    error.value =
      "Failed to load products. Please check the console for details.";
  } finally {
    loading.value = false;
  }
};

const onProductAdded = (productId: string) => {
  console.log(`Product ${productId} added to basket`);
  // You could show a success message here
};

onMounted(() => {
  loadProducts();
});
</script>

<style scoped>
.products-view {
  max-width: 1200px;
  margin: 2rem auto;
  padding: 0 2rem;
}

.products-view h2 {
  color: #495057;
  margin-bottom: 2rem;
  text-align: center;
}

.category-filters {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  flex-wrap: wrap;
  justify-content: center;
}

.filter-btn {
  background: white;
  border: 1px solid #dee2e6;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s;
}

.filter-btn:hover {
  background-color: #f8f9fa;
}

.filter-btn.active {
  background-color: #007bff;
  color: white;
  border-color: #007bff;
}

.loading,
.error,
.no-products {
  text-align: center;
  padding: 3rem;
  color: #6c757d;
}

.error {
  color: #dc3545;
}

.retry-btn {
  background-color: #dc3545;
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  margin-top: 1rem;
}

.retry-btn:hover {
  background-color: #c82333;
}

.products-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(400px, 1fr));
  gap: 2rem;
}

@media (max-width: 768px) {
  .products-grid {
    grid-template-columns: 1fr;
  }

  .category-filters {
    justify-content: flex-start;
  }
}
</style>

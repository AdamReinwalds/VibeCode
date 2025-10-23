<template>
  <div class="about-view">
    <h2>About E-Commerce MVP</h2>
    <p>
      This is a minimum viable product (MVP) for an e-commerce application
      featuring:
    </p>

    <ul>
      <li>User authentication (login/register)</li>
      <li>Basket management (add/remove items)</li>
      <li>Checkout process with simulated payment</li>
      <li>Order history</li>
    </ul>

    <h3>Technology Stack</h3>
    <div class="tech-stack">
      <div class="tech-category">
        <h4>Backend</h4>
        <ul>
          <li>.NET 8 Web API</li>
          <li>MongoDB</li>
          <li>JWT Authentication</li>
          <li>xUnit for testing</li>
        </ul>
      </div>

      <div class="tech-category">
        <h4>Frontend</h4>
        <ul>
          <li>Vue 3 + TypeScript</li>
          <li>Pinia for state management</li>
          <li>Vue Router</li>
          <li>Vitest for unit testing</li>
          <li>Playwright for E2E testing</li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// No script needed for this static page
</script>

<style scoped>
.about-view {
  max-width: 800px;
  margin: 2rem auto;
  padding: 0 2rem;
}

.about-view h2 {
  color: #495057;
  margin-bottom: 1rem;
}

.about-view ul {
  margin: 1rem 0 2rem 2rem;
}

.tech-stack {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 2rem;
  margin-top: 2rem;
}

.tech-category {
  background: white;
  border: 1px solid #dee2e6;
  border-radius: 8px;
  padding: 1.5rem;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.tech-category h4 {
  color: #495057;
  margin-bottom: 1rem;
  text-align: center;
}

.tech-category ul {
  margin: 0 0 0 1.5rem;
}
</style>

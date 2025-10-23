<template>
  <div class="register-form">
    <h2>Register</h2>
    <form @submit.prevent="handleRegister">
      <div class="form-row">
        <div class="form-group">
          <label for="firstName">First Name:</label>
          <input id="firstName" v-model="form.firstName" type="text" required />
        </div>

        <div class="form-group">
          <label for="lastName">Last Name:</label>
          <input id="lastName" v-model="form.lastName" type="text" required />
        </div>
      </div>

      <div class="form-group">
        <label for="username">Username:</label>
        <input id="username" v-model="form.username" type="text" required />
      </div>

      <div class="form-group">
        <label for="email">Email:</label>
        <input id="email" v-model="form.email" type="email" required />
      </div>

      <div class="form-group">
        <label for="password">Password:</label>
        <input
          id="password"
          v-model="form.password"
          type="password"
          required
          minlength="6"
        />
      </div>

      <button type="submit" :disabled="loading">
        {{ loading ? "Registering..." : "Register" }}
      </button>
    </form>

    <p v-if="error" class="error">{{ error }}</p>

    <router-link to="/login">Already have an account? Login</router-link>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "@/stores/auth";
import { authService } from "@/services/authService";
import type { RegisterRequest } from "@/types/auth";

const router = useRouter();
const authStore = useAuthStore();

const form = ref<RegisterRequest>({
  username: "",
  email: "",
  password: "",
  firstName: "",
  lastName: "",
});

const loading = ref(false);
const error = ref<string | null>(null);

const handleRegister = async () => {
  loading.value = true;
  error.value = null;

  try {
    const response = await authService.register(form.value);
    authStore.login(response);
    router.push("/");
  } catch (err: any) {
    error.value = err.response?.data?.error?.message || "Registration failed";
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
.register-form {
  max-width: 400px;
  margin: 2rem auto;
  padding: 2rem;
  border: 1px solid #ddd;
  border-radius: 8px;
}

.form-row {
  display: flex;
  gap: 1rem;
}

.form-row .form-group {
  flex: 1;
}

.form-group {
  margin-bottom: 1rem;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: bold;
}

input {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
}

button {
  width: 100%;
  padding: 0.75rem;
  background-color: #28a745;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 1rem;
  cursor: pointer;
}

button:disabled {
  background-color: #6c757d;
  cursor: not-allowed;
}

.error {
  color: #dc3545;
  margin-top: 1rem;
  text-align: center;
}

a {
  display: block;
  text-align: center;
  margin-top: 1rem;
  color: #007bff;
  text-decoration: none;
}
</style>

import { defineStore } from "pinia";
import { ref, computed } from "vue";
import type { AuthResponse, UserResponse } from "@/types/auth";

export const useAuthStore = defineStore("auth", () => {
  const token = ref<string | null>(localStorage.getItem("authToken"));
  const user = ref<UserResponse | null>(null);

  const isAuthenticated = computed(() => !!token.value && !!user.value);

  const login = (authResponse: AuthResponse) => {
    token.value = authResponse.token;
    user.value = authResponse.user;
    localStorage.setItem("authToken", authResponse.token);
  };

  const logout = () => {
    token.value = null;
    user.value = null;
    localStorage.removeItem("authToken");
  };

  const initializeAuth = () => {
    const savedToken = localStorage.getItem("authToken");
    if (savedToken) {
      token.value = savedToken;
      // In a real app, you might validate the token here
    }
  };

  return {
    token,
    user,
    isAuthenticated,
    login,
    logout,
    initializeAuth,
  };
});

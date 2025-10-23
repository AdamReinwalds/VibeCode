import { describe, it, expect, vi, beforeEach } from "vitest";
import { mount } from "@vue/test-utils";
import { createTestingPinia } from "@pinia/testing";
import LoginForm from "../LoginForm.vue";
import { useAuthStore } from "@/stores/auth";
import { authService } from "@/services/authService";

// Mock the auth service
vi.mock("@/services/authService", () => ({
  authService: {
    login: vi.fn(),
  },
}));

describe("LoginForm", () => {
  let authStore: ReturnType<typeof useAuthStore>;

  beforeEach(() => {
    vi.clearAllMocks();
  });

  const mountComponent = () => {
    return mount(LoginForm, {
      global: {
        plugins: [
          createTestingPinia({
            createSpy: vi.fn,
          }),
        ],
      },
    });
  };

  it("renders login form correctly", () => {
    const wrapper = mountComponent();

    expect(wrapper.find("h2").text()).toBe("Login");
    expect(wrapper.find('input[type="text"]').exists()).toBe(true);
    expect(wrapper.find('input[type="password"]').exists()).toBe(true);
    expect(wrapper.find("button").text()).toBe("Login");
  });

  it("updates form data when inputs change", async () => {
    const wrapper = mountComponent();

    const usernameInput = wrapper.find('input[type="text"]');
    const passwordInput = wrapper.find('input[type="password"]');

    await usernameInput.setValue("testuser");
    await passwordInput.setValue("password123");

    expect((usernameInput.element as HTMLInputElement).value).toBe("testuser");
    expect((passwordInput.element as HTMLInputElement).value).toBe(
      "password123"
    );
  });

  it("calls login service and updates store on successful login", async () => {
    const wrapper = mountComponent();
    authStore = useAuthStore();

    const mockResponse = {
      token: "mock-token",
      user: {
        userId: "123",
        username: "testuser",
        email: "test@example.com",
        firstName: "Test",
        lastName: "User",
      },
    };

    vi.mocked(authService.login).mockResolvedValue(mockResponse);

    await wrapper.find('input[type="text"]').setValue("testuser");
    await wrapper.find('input[type="password"]').setValue("password123");
    await wrapper.find("form").trigger("submit.prevent");

    expect(authService.login).toHaveBeenCalledWith({
      username: "testuser",
      password: "password123",
    });

    expect(authStore.login).toHaveBeenCalledWith(mockResponse);
  });

  it("shows error message on login failure", async () => {
    const wrapper = mountComponent();

    const mockError = {
      response: {
        data: {
          error: {
            message: "Invalid credentials",
          },
        },
      },
    };

    vi.mocked(authService.login).mockRejectedValue(mockError);

    await wrapper.find('input[type="text"]').setValue("testuser");
    await wrapper.find('input[type="password"]').setValue("wrongpassword");
    await wrapper.find("form").trigger("submit.prevent");

    expect(wrapper.text()).toContain("Invalid credentials");
  });

  it("disables submit button during loading", async () => {
    const wrapper = mountComponent();

    vi.mocked(authService.login).mockImplementation(
      () => new Promise((resolve) => setTimeout(resolve, 100))
    );

    await wrapper.find('input[type="text"]').setValue("testuser");
    await wrapper.find('input[type="password"]').setValue("password123");
    await wrapper.find("form").trigger("submit.prevent");

    expect(wrapper.find("button").text()).toBe("Logging in...");
    expect(wrapper.find("button").attributes("disabled")).toBeDefined();
  });
});

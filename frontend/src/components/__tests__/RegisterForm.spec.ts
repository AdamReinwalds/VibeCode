import { describe, it, expect, vi, beforeEach } from "vitest";
import { mount } from "@vue/test-utils";
import { createTestingPinia } from "@pinia/testing";
import RegisterForm from "../RegisterForm.vue";
import { authService } from "@/services/authService";

vi.mock("@/services/authService");

describe("RegisterForm", () => {
  let wrapper: any;

  beforeEach(() => {
    wrapper = mount(RegisterForm, {
      global: {
        plugins: [
          createTestingPinia({
            createSpy: vi.fn,
          }),
        ],
        stubs: ["router-link"],
      },
    });
  });

  it("renders registration form correctly", () => {
    expect(wrapper.find("h2").text()).toBe("Register");
    expect(wrapper.find("input#firstName").exists()).toBe(true);
    expect(wrapper.find("input#lastName").exists()).toBe(true);
    expect(wrapper.find("input#username").exists()).toBe(true);
    expect(wrapper.find("input#email").exists()).toBe(true);
    expect(wrapper.find("input#password").exists()).toBe(true);
    expect(wrapper.find("button[type='submit']").text()).toBe("Register");
  });

  it("updates form data when inputs change", async () => {
    const firstNameInput = wrapper.find("input#firstName");
    const lastNameInput = wrapper.find("input#lastName");
    const usernameInput = wrapper.find("input#username");
    const emailInput = wrapper.find("input#email");
    const passwordInput = wrapper.find("input#password");

    await firstNameInput.setValue("John");
    await lastNameInput.setValue("Doe");
    await usernameInput.setValue("johndoe");
    await emailInput.setValue("john.doe@example.com");
    await passwordInput.setValue("password123");

    expect(wrapper.vm.form.firstName).toBe("John");
    expect(wrapper.vm.form.lastName).toBe("Doe");
    expect(wrapper.vm.form.username).toBe("johndoe");
    expect(wrapper.vm.form.email).toBe("john.doe@example.com");
    expect(wrapper.vm.form.password).toBe("password123");
  });

  it("calls register service and updates store on successful registration", async () => {
    const mockResponse = {
      token: "fake-token",
      user: {
        id: "user-id",
        username: "johndoe",
        email: "john.doe@example.com",
        firstName: "John",
        lastName: "Doe",
      },
    };

    (authService.register as any).mockResolvedValue(mockResponse);

    await wrapper.find("input#firstName").setValue("John");
    await wrapper.find("input#lastName").setValue("Doe");
    await wrapper.find("input#username").setValue("johndoe");
    await wrapper.find("input#email").setValue("john.doe@example.com");
    await wrapper.find("input#password").setValue("password123");

    await wrapper.find("form").trigger("submit.prevent");

    expect(authService.register).toHaveBeenCalledWith({
      firstName: "John",
      lastName: "Doe",
      username: "johndoe",
      email: "john.doe@example.com",
      password: "password123",
    });
  });

  it("shows error message on registration failure", async () => {
    const errorMessage = "Registration failed";
    (authService.register as any).mockRejectedValue({
      response: {
        data: {
          error: { message: errorMessage },
        },
      },
    });

    await wrapper.find("input#firstName").setValue("John");
    await wrapper.find("input#lastName").setValue("Doe");
    await wrapper.find("input#username").setValue("johndoe");
    await wrapper.find("input#email").setValue("john.doe@example.com");
    await wrapper.find("input#password").setValue("password123");

    await wrapper.find("form").trigger("submit.prevent");

    expect(wrapper.vm.error).toBe(errorMessage);
    expect(wrapper.find(".error").text()).toBe(errorMessage);
  });

  it("disables submit button during loading", async () => {
    const button = wrapper.find("button[type='submit']");

    expect(button.attributes("disabled")).toBeUndefined();

    await wrapper.find("input#firstName").setValue("John");
    await wrapper.find("input#lastName").setValue("Doe");
    await wrapper.find("input#username").setValue("johndoe");
    await wrapper.find("input#email").setValue("john.doe@example.com");
    await wrapper.find("input#password").setValue("password123");

    wrapper.find("form").trigger("submit.prevent");

    await wrapper.vm.$nextTick();
    expect(button.attributes("disabled")).toBeDefined();
    expect(button.text()).toBe("Registering...");
  });
});

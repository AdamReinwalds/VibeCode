import { test, expect } from "@playwright/test";

test.describe("Authentication", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto("/");
  });

  test("should allow user to register and login", async ({ page }) => {
    // Navigate to register page
    await page.click("text=Register");

    // Fill registration form
    await page.fill('input[id="firstName"]', "John");
    await page.fill('input[id="lastName"]', "Doe");
    await page.fill('input[id="username"]', "johndoe");
    await page.fill('input[id="email"]', "john.doe@example.com");
    await page.fill('input[id="password"]', "password123");

    // Submit registration
    await page.click('button[type="submit"]');

    // Should redirect to login page first (since the app redirects unauthenticated users to login)
    await expect(page).toHaveURL("/login");

    // Now login with the registered credentials
    await page.fill('input[id="username"]', "johndoe");
    await page.fill('input[id="password"]', "password123");
    await page.click('button[type="submit"]');

    // Should redirect to home page after successful login
    await expect(page).toHaveURL("/");

    // Check if user is logged in (should see logout or user info)
    await expect(page.locator("text=Logout")).toBeVisible();
  });

  test("should allow user to login", async ({ page }) => {
    // Navigate to login page
    await page.click("text=Login");

    // Fill login form
    await page.fill('input[id="username"]', "johndoe");
    await page.fill('input[id="password"]', "password123");

    // Submit login
    await page.click('button[type="submit"]');

    // Should redirect to home page after successful login
    await expect(page).toHaveURL("/");

    // Check if user is logged in
    await expect(page.locator("text=Logout")).toBeVisible();
  });

  test("should show error on invalid login", async ({ page }) => {
    // Navigate to login page
    await page.click("text=Login");

    // Fill login form with invalid credentials
    await page.fill('input[id="username"]', "invaliduser");
    await page.fill('input[id="password"]', "wrongpassword");

    // Submit login
    await page.click('button[type="submit"]');

    // Should show error message
    await expect(page.locator(".error")).toBeVisible();
    await expect(page.locator(".error")).toContainText("Login failed");
  });
});

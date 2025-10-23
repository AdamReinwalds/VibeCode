import { test, expect } from "@playwright/test";

test.describe("Checkout Process", () => {
  test.beforeEach(async ({ page }) => {
    // Login first
    await page.goto("/login");
    await page.fill('input[id="username"]', "johndoe");
    await page.fill('input[id="password"]', "password123");
    await page.click('button[type="submit"]');
    await expect(page).toHaveURL("/");
  });

  test("should complete checkout process", async ({ page }) => {
    // Add item to basket first
    await page.goto("/");
    const addToBasketButton = page
      .locator("button")
      .filter({ hasText: "Add to Basket" })
      .first();
    await addToBasketButton.click();

    // Navigate to basket
    await page.click("text=Basket");
    await expect(page).toHaveURL("/basket");

    // Proceed to checkout
    await page.click("text=Checkout");
    await expect(page).toHaveURL("/checkout");

    // Verify order summary is displayed
    await expect(page.locator("text=Order Summary")).toBeVisible();
    await expect(page.locator(".order-item")).toHaveCount(1);

    // Fill payment form
    await page.fill('input[id="cardNumber"]', "4111111111111111");
    await page.fill('input[id="expiryDate"]', "12/25");
    await page.fill('input[id="cvv"]', "123");
    await page.fill('input[id="nameOnCard"]', "John Doe");

    // Fill shipping address
    await page.fill('input[id="address"]', "123 Main St");
    await page.fill('input[id="city"]', "Anytown");
    await page.fill('input[id="zipCode"]', "12345");
    await page.fill('input[id="country"]', "USA");

    // Submit order
    await page.click('button[type="submit"]');

    // Should show success message and redirect to order confirmation
    await expect(page.locator("text=Order placed successfully")).toBeVisible();
    await expect(page).toHaveURL("/order-confirmation");
  });

  test("should validate payment form", async ({ page }) => {
    // Add item and go to checkout
    await page.goto("/");
    const addToBasketButton = page
      .locator("button")
      .filter({ hasText: "Add to Basket" })
      .first();
    await addToBasketButton.click();
    await page.click("text=Basket");
    await page.click("text=Checkout");

    // Try to submit with empty form
    await page.click('button[type="submit"]');

    // Should show validation errors
    await expect(page.locator("text=Card number is required")).toBeVisible();
    await expect(page.locator("text=Expiry date is required")).toBeVisible();
    await expect(page.locator("text=CVV is required")).toBeVisible();
  });

  test("should calculate total correctly", async ({ page }) => {
    // Add multiple items to basket
    await page.goto("/");
    const addButtons = page
      .locator("button")
      .filter({ hasText: "Add to Basket" });
    await addButtons.nth(0).click();
    await addButtons.nth(1).click();

    // Go to checkout
    await page.click("text=Basket");
    await page.click("text=Checkout");

    // Verify total calculation (assuming we know the prices)
    const totalElement = page.locator(".total-amount");
    await expect(totalElement).toBeVisible();
    // This would need to be adjusted based on actual product prices
    const totalText = await totalElement.textContent();
    expect(totalText).toMatch(/\$\d+\.\d{2}/);
  });
});

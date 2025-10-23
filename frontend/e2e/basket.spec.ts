import { test, expect } from "@playwright/test";

test.describe("Basket Management", () => {
  test.beforeEach(async ({ page }) => {
    // Login first (assuming user exists from previous tests)
    await page.goto("/login");
    await page.fill('input[id="username"]', "johndoe");
    await page.fill('input[id="password"]', "password123");
    await page.click('button[type="submit"]');
    await expect(page).toHaveURL("/");
  });

  test("should add items to basket", async ({ page }) => {
    // Navigate to home/products page
    await page.goto("/");

    // Assuming there are products displayed, click "Add to Basket" on first product
    const addToBasketButton = page
      .locator("button")
      .filter({ hasText: "Add to Basket" })
      .first();
    await expect(addToBasketButton).toBeVisible();
    await addToBasketButton.click();

    // Check basket count/badge updates
    const basketBadge = page.locator(".basket-count");
    await expect(basketBadge).toContainText("1");

    // Navigate to basket page
    await page.click("text=Basket");
    await expect(page).toHaveURL("/basket");

    // Verify item is in basket
    await expect(page.locator(".basket-item")).toHaveCount(1);
  });

  test("should remove items from basket", async ({ page }) => {
    // Assuming basket has items from previous test
    await page.goto("/basket");

    // Click remove button on first item
    const removeButton = page
      .locator("button")
      .filter({ hasText: "Remove" })
      .first();
    await removeButton.click();

    // Verify basket is empty
    await expect(page.locator(".basket-item")).toHaveCount(0);
    await expect(page.locator("text=Your basket is empty")).toBeVisible();
  });

  test("should update item quantities in basket", async ({ page }) => {
    // Add item to basket first
    await page.goto("/");
    const addToBasketButton = page
      .locator("button")
      .filter({ hasText: "Add to Basket" })
      .first();
    await addToBasketButton.click();

    // Go to basket
    await page.goto("/basket");

    // Increase quantity
    const increaseButton = page
      .locator("button")
      .filter({ hasText: "+" })
      .first();
    await increaseButton.click();

    // Verify quantity updated
    const quantityInput = page.locator('input[type="number"]').first();
    await expect(quantityInput).toHaveValue("2");

    // Decrease quantity
    const decreaseButton = page
      .locator("button")
      .filter({ hasText: "-" })
      .first();
    await decreaseButton.click();

    // Verify quantity back to 1
    await expect(quantityInput).toHaveValue("1");
  });
});

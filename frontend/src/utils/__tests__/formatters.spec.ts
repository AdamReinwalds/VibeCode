import { describe, it, expect } from "vitest";
import { formatCurrency, formatDate } from "../formatters";

describe("formatters", () => {
  describe("formatCurrency", () => {
    it("formats number to currency string", () => {
      expect(formatCurrency(29.99)).toBe("$29.99");
      expect(formatCurrency(0)).toBe("$0.00");
      expect(formatCurrency(1234.56)).toBe("$1,234.56");
    });

    it("handles negative numbers", () => {
      expect(formatCurrency(-29.99)).toBe("-$29.99");
    });

    it("rounds to 2 decimal places", () => {
      expect(formatCurrency(29.999)).toBe("$30.00");
      expect(formatCurrency(29.994)).toBe("$29.99");
    });
  });

  describe("formatDate", () => {
    it("formats date to readable string", () => {
      const date = new Date("2023-10-21T12:00:00Z");
      expect(formatDate(date)).toBe("October 21, 2023");
    });

    it("handles different date formats", () => {
      const date = new Date("2023-01-01T00:00:00Z");
      expect(formatDate(date)).toBe("January 1, 2023");
    });
  });
});

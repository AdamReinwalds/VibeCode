import api from "./api";
import type { CheckoutRequest, OrderSummaryDto } from "@/types/checkout";

export const checkoutService = {
  async checkout(request: CheckoutRequest): Promise<OrderSummaryDto> {
    const response = await api.post<OrderSummaryDto>("/checkout", request);
    return response.data;
  },
};

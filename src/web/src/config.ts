function parseRefreshInterval(rawValue: string | undefined): number {
  const parsedValue = Number(rawValue);

  if (!Number.isFinite(parsedValue) || parsedValue <= 0) {
    return 5000;
  }

  return parsedValue;
}

export const appConfig = {
  // Use a same-origin API base so Vite proxy can avoid browser CORS issues in local development.
  apiBaseUrl: import.meta.env.VITE_API_BASE_URL || "/api",
  // Keep the function key configurable to support AuthorizationLevel.Function endpoints.
  functionsKey: import.meta.env.VITE_FUNCTIONS_KEY || "",
  // Polling cadence is configurable to balance freshness and backend load.
  refreshIntervalMs: parseRefreshInterval(import.meta.env.VITE_LOGS_REFRESH_MS)
};

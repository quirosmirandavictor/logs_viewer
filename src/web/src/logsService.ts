import { appConfig } from "./config";

export type LogRecord = Record<string, unknown>;

export async function fetchLogs(): Promise<LogRecord[]> {
  const headers: Record<string, string> = {
    Accept: "application/json"
  };

  // Send x-functions-key only when configured; this keeps local anonymous mode compatible.
  if (appConfig.functionsKey.trim()) {
    headers["x-functions-key"] = appConfig.functionsKey.trim();
  }

  const endpoint = `${appConfig.apiBaseUrl}/logs`;
  const response = await fetch(endpoint, {
    method: "GET",
    headers
  });

  if (!response.ok) {
    const errorBody = await response.text();
    console.error("[logsService] GetLogs request failed", {
      endpoint,
      status: response.status,
      statusText: response.statusText,
      body: errorBody
    });

    if (response.status === 401) {
      throw new Error(
        "GetLogs returned 401 Unauthorized. Set VITE_FUNCTIONS_KEY with a valid function key."
      );
    }

    throw new Error(`GetLogs failed with status ${response.status}.`);
  }

  const data = (await response.json()) as LogRecord[];
  console.log("[logsService] GetLogs response", {
    endpoint,
    records: data.length,
    payload: data
  });

  return data;
}

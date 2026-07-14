import { FormEvent, useEffect, useState } from "react";
import { isAuthenticated, login, logout } from "./auth";
import { appConfig } from "./config";
import { fetchLogs, type LogRecord } from "./logsService";
import { MetricCards } from "./MetricCards";

export function App() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [error, setError] = useState("");
  const [logs, setLogs] = useState<LogRecord[]>([]);
  const [logsError, setLogsError] = useState("");
  const [isLoadingLogs, setIsLoadingLogs] = useState(false);
  const [lastUpdated, setLastUpdated] = useState("");

  useEffect(() => {
    setIsLoggedIn(isAuthenticated());
  }, []);

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (!username.trim() || !password.trim()) {
      setError("Please enter username and password.");
      return;
    }

    const ok = login(username.trim(), password.trim());

    if (!ok) {
      setError("Invalid credentials for demo login.");
      return;
    }

    setError("");
    setPassword("");
    setIsLoggedIn(true);
  }

  function handleLogout() {
    logout();
    setIsLoggedIn(false);
    setUsername("");
    setPassword("");
    setError("");
    setLogs([]);
    setLogsError("");
    setIsLoadingLogs(false);
    setLastUpdated("");
  }

  useEffect(() => {
    if (!isLoggedIn) {
      return;
    }

    // Fetch immediately after login so the dashboard starts with real data.
    let isActive = true;

    async function loadLogs() {
      try {
        setIsLoadingLogs(true);
        setLogsError("");

        const payload = await fetchLogs();

        if (!isActive) {
          return;
        }

        setLogs(payload);
        setLastUpdated(new Date().toLocaleTimeString());
        console.log("[App] Logs updated in state", {
          count: payload.length,
          refreshIntervalMs: appConfig.refreshIntervalMs
        });
      } catch (fetchError) {
        if (!isActive) {
          return;
        }

        const message =
          fetchError instanceof Error
            ? fetchError.message
            : "Unexpected error while retrieving logs.";

        setLogsError(message);
      } finally {
        if (isActive) {
          setIsLoadingLogs(false);
        }
      }
    }

    void loadLogs();

    // Keep polling while the protected dashboard is mounted.
    const timerId = window.setInterval(() => {
      void loadLogs();
    }, appConfig.refreshIntervalMs);

    return () => {
      isActive = false;
      window.clearInterval(timerId);
    };
  }, [isLoggedIn]);

  return (
    <main className="page-shell">
      <section className="hero-panel">
        <h1>Logs Viewer</h1>
        <p>React UI bootstrap for the next dashboard iteration.</p>
      </section>

      {!isLoggedIn ? (
        <section className="login-card" aria-label="Demo login form">
          <h2>Demo Login</h2>
          <p className="hint">
            Use the configured demo credentials to access the placeholder protected screen.
          </p>

          <form onSubmit={handleSubmit}>
            <label htmlFor="username">Username</label>
            <input
              id="username"
              type="text"
              autoComplete="username"
              value={username}
              onChange={(event) => setUsername(event.target.value)}
              placeholder="demo"
            />

            <label htmlFor="password">Password</label>
            <input
              id="password"
              type="password"
              autoComplete="current-password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              placeholder="demo123"
            />

            {error && <p className="error-message">{error}</p>}

            <button type="submit">Sign in</button>
          </form>
        </section>
      ) : (
        <section className="protected-card" aria-label="Protected area">
          <h2>Protected Demo Area</h2>
          <p>
            Log data is loaded from AppFunction GetLogs and refreshed automatically.
          </p>
          <p className="hint">Refresh interval: {appConfig.refreshIntervalMs} ms</p>

          {isLoadingLogs && <p className="hint">Loading logs...</p>}
          {logsError && <p className="error-message">{logsError}</p>}

          <MetricCards logs={logs} />

          <div className="logs-console-preview" aria-label="Logs preview">
            <h3>Latest Logs ({logs.length})</h3>
            <p className="hint">Last updated: {lastUpdated || "Not updated yet"}</p>

            {logs.length === 0 ? (
              <p className="hint">No logs available yet.</p>
            ) : (
              <pre>{JSON.stringify(logs, null, 2)}</pre>
            )}
          </div>

          <button className="secondary" onClick={handleLogout}>
            Sign out
          </button>
        </section>
      )}
    </main>
  );
}

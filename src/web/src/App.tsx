import { FormEvent, useEffect, useState } from "react";
import { isAuthenticated, login, logout } from "./auth";

export function App() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [error, setError] = useState("");

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
  }

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
            Login flow is active. API consumption for AppFunction logs will be added in the next stage.
          </p>
          <button className="secondary" onClick={handleLogout}>
            Sign out
          </button>
        </section>
      )}
    </main>
  );
}

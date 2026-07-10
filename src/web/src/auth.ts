const SESSION_KEY = "logsviewer-authenticated";

const demoUser = import.meta.env.VITE_DEMO_USER || "demo";
const demoPassword = import.meta.env.VITE_DEMO_PASSWORD || "demo123";

export function login(username: string, password: string): boolean {
  const isValid = username === demoUser && password === demoPassword;

  if (isValid) {
    localStorage.setItem(SESSION_KEY, "true");
  }

  return isValid;
}

export function logout(): void {
  localStorage.removeItem(SESSION_KEY);
}

export function isAuthenticated(): boolean {
  return localStorage.getItem(SESSION_KEY) === "true";
}

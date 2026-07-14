import { defineConfig, loadEnv } from "vite";

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), "");
  const proxyTarget = env.VITE_PROXY_TARGET || "http://localhost:7071";

  return {
    server: {
      host: "0.0.0.0",
      port: 5173,
      proxy: {
        // Proxy API requests through Vite to keep browser calls same-origin in local development.
        "/api": {
          target: proxyTarget,
          changeOrigin: true
        }
      }
    },
    preview: {
      host: "0.0.0.0",
      port: 5173
    }
  };
});

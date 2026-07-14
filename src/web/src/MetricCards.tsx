import { useMemo } from "react";

export type LogItem = Record<string, unknown>;

export type MetricCardsProps = {
  logs: LogItem[];
};

// Aggregated counters displayed by the dashboard cards.
type LogMetrics = {
  totalLogs: number;
  netErrors: number;
  pythonErrors: number;
};

// Visual variants are semantic and reusable across future cards.
type MetricVariant = "total" | "success" | "error";
type TrendDirection = "up" | "down";

// Read a string value from multiple candidate keys to support mixed JSON naming conventions.
function getStringValue(record: LogItem, keys: string[]): string {
  for (const key of keys) {
    const value = record[key];
    if (typeof value === "string" && value.trim()) {
      return value.trim();
    }
  }

  return "";
}

export function calculateLogMetrics(logs: LogItem[]): LogMetrics {
  return logs.reduce<LogMetrics>(
    (accumulator, log) => {
      // Accept both PascalCase and camelCase properties to support different serializer settings.
      const source = getStringValue(log, ["Source", "source"]).toLowerCase();
      const level = getStringValue(log, ["Level", "level"]).toLowerCase();

      // Count explicit Error level entries; if level is missing, keep counting to avoid dropping partial records.
      const isError = level === "error" || level === "";

      accumulator.totalLogs += 1;

      if (source === "nlog" && isError) {
        accumulator.netErrors += 1;
      }

      if (source === "python" && isError) {
        accumulator.pythonErrors += 1;
      }

      return accumulator;
    },
    { totalLogs: 0, netErrors: 0, pythonErrors: 0 }
  );
}

// Apply compact formatting only to large totals so cards remain readable on small screens.
function formatMetricValue(value: number, variant: MetricVariant): string {
  if (variant === "total" && value >= 1000) {
    return `${(value / 1000).toFixed(1)}k`;
  }

  return value.toLocaleString("es-CR");
}

// Provide a lightweight inline sparkline shape for each visual variant.
function getSparklinePath(variant: MetricVariant): string {
  if (variant === "success") {
    return "M0 42 C18 34, 32 40, 46 32 C60 22, 76 30, 90 20 C106 10, 124 18, 140 12 C156 8, 176 14, 196 10";
  }

  if (variant === "error") {
    return "M0 44 C16 40, 32 44, 50 36 C66 28, 84 34, 102 26 C118 20, 136 30, 154 22 C172 16, 186 26, 196 20";
  }

  return "M0 46 C14 42, 28 48, 44 38 C60 26, 78 36, 96 24 C112 14, 130 28, 146 18 C162 8, 178 26, 196 18";
}

// Present a single metric value card in the protected dashboard area.
function MetricCard({
  title,
  value,
  trendDirection,
  trendLabel,
  variant
}: {
  title: string;
  value: number;
  trendDirection: TrendDirection;
  trendLabel: string;
  variant: MetricVariant;
}) {
  const isDownTrend = trendDirection === "down";

  return (
    <article className={`metric-card metric-card-${variant}`} aria-label={title}>
      <header className="metric-card-head">
        <p className="metric-card-title">{title}</p>
        <p className={`metric-card-trend ${isDownTrend ? "is-down" : "is-up"}`}>
          <span className="metric-card-trend-arrow" aria-hidden="true">
            {isDownTrend ? "▼" : "▲"}
          </span>
          <span className="metric-card-trend-label">{trendLabel}</span>
        </p>
      </header>

      <div className="metric-card-value-row">
        <p className="metric-card-value">{formatMetricValue(value, variant)}</p>
        <span className="metric-card-dot" aria-hidden="true" />
      </div>

      <div className="metric-card-sparkline" aria-hidden="true">
        <svg viewBox="0 0 196 52" preserveAspectRatio="none" role="presentation">
          <path d={getSparklinePath(variant)} className="sparkline-stroke" />
        </svg>
      </div>
    </article>
  );
}

export function MetricCards({ logs }: MetricCardsProps) {
  // Recompute summary counters only when the source logs array changes.
  const metrics = useMemo(() => calculateLogMetrics(logs), [logs]);

  return (
    <section className="metric-cards-grid" aria-label="Log metrics summary">
      <MetricCard
        title="Total logs"
        value={metrics.totalLogs}
        trendDirection="up"
        trendLabel="live"
        variant="total"
      />
      <MetricCard
        title="Errores .NET"
        value={metrics.netErrors}
        trendDirection="up"
        trendLabel="source=nlog"
        variant="success"
      />
      <MetricCard
        title="Errores Python"
        value={metrics.pythonErrors}
        trendDirection="down"
        trendLabel="source=python"
        variant="error"
      />
    </section>
  );
}

export default MetricCards;

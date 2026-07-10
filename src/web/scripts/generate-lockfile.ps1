Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent $PSScriptRoot

Write-Host "Generating pnpm-lock.yaml using a disposable Node container..."

Push-Location $projectRoot
try {
  docker run --rm `
    -v "${projectRoot}:/workspace" `
    -w /workspace `
    node:20-alpine `
    sh -lc "corepack enable; corepack prepare pnpm@9.12.1 --activate; pnpm install --lockfile-only"
}
finally {
  Pop-Location
}

Write-Host "Done. Commit src/web/pnpm-lock.yaml to keep installs immutable."

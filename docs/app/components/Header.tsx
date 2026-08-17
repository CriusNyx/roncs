"use client"

import { Select } from "antd"
import { useCodeThemeService } from "../services/CodeThemeService"

const themes = [
  "dark-plus",
  "dracula-soft",
  "dracula",
  "github-dark",
  "github-dark-dimmed",
  "github-from-css",
  "github-light",
  "light-plus",
  "material-darker",
  "material-default",
  "material-from-css",
  "material-lighter",
  "material-ocean",
  "material-palenight",
  "min-dark",
  "min-light",
  "monokai",
  "nord",
  "one-dark-pro",
  "poimandres",
  "slack-dark",
  "slack-ochin",
  "solarized-dark",
  "solarized-light",
] as const

export function Header() {
  const codeThemeService = useCodeThemeService()

  return null

  return (
    <div className="flex flex-row justify-between">
      <div></div>
      <div className="flex flex-row gap-2">
        <span>Theme:</span>
        <Select
          value={codeThemeService.codeTheme}
          options={themes.map((name) => ({ value: name, label: name }))}
          onChange={(theme) => codeThemeService.setCodeTheme(theme)}
        />
      </div>
    </div>
  )
}

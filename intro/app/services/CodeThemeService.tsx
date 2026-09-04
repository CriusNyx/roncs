"use client"
import { createContext, PropsWithChildren, useContext, useState } from "react"
import { Theme } from "codehike/code"

interface CodeThemeContext {
  codeTheme: Theme
  setCodeTheme: (codeTheme: Theme) => void
}

const CodeThemeContext = createContext<CodeThemeContext>({} as CodeThemeContext)

export function useCodeThemeService() {
  return useContext(CodeThemeContext)
}

export function CodeThemeService(props: PropsWithChildren) {
  const [codeTheme, setCodeTheme] = useState<Theme>("dark-plus")
  return (
    <CodeThemeContext.Provider value={{ codeTheme, setCodeTheme }}>
      {props.children}
    </CodeThemeContext.Provider>
  )
}

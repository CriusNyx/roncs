"use client"
import { Pre, RawCode, highlight } from "codehike/code"
import { callout } from "./annotations/callout"
import { useEffect, useState } from "react"
import { useCodeThemeService } from "../services/CodeThemeService"
import { tokenTransitions } from "./annotations/token-transitions"

function useAsync<T>(promise: () => Promise<T>, deps: any[] = []) {
  const [state, setState] = useState<T | undefined>(undefined)
  useEffect(() => {
    setState(undefined)
    promise().then(setState)
  }, deps)
  return state
}

export function Code({ codeblock }: { codeblock: RawCode }) {
  const codeTheme = useCodeThemeService()

  const result = useAsync(
    () => highlight(codeblock, codeTheme.codeTheme as any),
    [codeTheme.codeTheme, codeblock],
  )

  if (result) {
    return (
      <Pre
        code={result}
        handlers={[callout, tokenTransitions]}
        className="border border-zinc-800"
      />
    )
  }
}

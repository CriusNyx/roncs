import { Pre, RawCode, highlight } from "codehike/code"
import { callout } from "./annotations/callout"
import { tokenTransitions } from "./annotations/token-transitions"

interface CSCodeProps {
  source: string
}

export function CSCode(props: CSCodeProps) {
  return <Code codeblock={{ value: props.source, lang: "cs", meta: "" }} />
}

export async function Code({ codeblock }: { codeblock: RawCode }) {
  const tokens = await highlight(codeblock, "dark-plus")

  return (
    <Pre
      code={tokens}
      handlers={[callout, tokenTransitions]}
      className="border border-zinc-800"
    />
  )
}

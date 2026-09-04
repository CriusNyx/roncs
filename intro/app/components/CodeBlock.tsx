import { PropsWithChildren } from "react"

interface CodeBlockProps extends PropsWithChildren {
  lang?: string
}

export function CodeBlock(props: CodeBlockProps) {
  // This comment is here because Next is not caching correctly.
  return (
    <div className="flex flex-col w-full [&>pre]:h-full">
      {props.lang && <h3 className="pl-2">{props.lang}</h3>}
      {props.children}
    </div>
  )
}

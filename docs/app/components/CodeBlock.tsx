import { PropsWithChildren } from "react"

export function CodeBlock(props: PropsWithChildren) {
  return (
    <div className="flex flex-col w-full [&>pre]:h-full">{props.children}</div>
  )
}

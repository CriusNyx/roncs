import { PropsWithChildren } from "react"

export function SideBySide(props: PropsWithChildren) {
  return (
    <div className="flex flex-row justify-stretch gap-5 w-full">
      {props.children}
    </div>
  )
}

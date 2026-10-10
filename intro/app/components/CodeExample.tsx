import { PropsWithChildren } from "react"

export function CodeExample(props: PropsWithChildren) {
  return (
    <div className="flex flex-col md:flex-row gap-2 justify-stretch md:[&>*]:w-1/2">
      {props.children}
    </div>
  )
}

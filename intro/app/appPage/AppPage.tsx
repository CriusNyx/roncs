import { PropsWithChildren } from "react"
import { Navigation } from "../navigation"
import { Header } from "../components/Header"

export function AppPage(props: PropsWithChildren) {
  return (
    <div className="flex flex-row w-full relative h-full">
      <div className="flex flex-col w-full min-h-screen h-screen overflow-clip">
        <Header />
        <div className="grow w-full overflow-y-scroll">
          <div className="flex flex-col mx-auto max-w-5xl py-5 px-10">
            {props.children}
          </div>
        </div>
      </div>
      <div className="absolute top-0 left-0 bottom-0 bg-zinc-900">
        <Navigation />
      </div>
    </div>
  )
}

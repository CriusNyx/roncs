import { SearchLoader } from "./SearchLoader"

export function Header() {
  return (
    <div className="flex flex-row justify-center bg-zinc-900 p-2 w-full">
      <div className="w-[60rem]">
        <SearchLoader />
      </div>
    </div>
  )
}

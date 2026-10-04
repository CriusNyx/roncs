"use client"

import { Input } from "antd"
import { useEffect, useMemo, useRef, useState } from "react"
import Minisearch from "minisearch"
import { useRouter } from "next/navigation"
import z from "zod"
import XMLToReact from "xml-to-react"
import { SearchSignature } from "./SearchSignature"
import Link from "next/link"

export const searchCacheElementSchema = z.object({
  name: z.string(),
  declaringType: z.string().optional().nullable(),
  shortName: z.string(),
  signature: z.string(),
  signatureXml: z.string(),
  summary: z.string().optional(),
})

export type SearchCacheElement = z.output<typeof searchCacheElementSchema>

export type SearchCacheMap = { [id: string]: SearchCacheElement }

interface SearchProps {
  values?: SearchCacheMap
}

export function Search(props: SearchProps) {
  const miniSearch = useMemo(() => {
    var miniSearch = new Minisearch({
      fields: ["name", "signature", "shortName", "summary"],
      searchOptions: { fuzzy: 0.5 },
    })
    miniSearch.addAll(Object.values(props.values ?? {}))
    return miniSearch
  }, [])

  const [search, setSearch] = useState("")
  const [visible, setVisible] = useState(false)

  const inputRef = useRef<HTMLElement>(null)

  useEffect(() => {
    document.addEventListener("click", (e) => {
      if (e.target !== inputRef.current) setVisible(false)
    })
  }, [])

  const results = search && miniSearch.search(search)

  useEffect(() => {
    if (results) {
      setVisible(true)
    }
  }, [search])

  function linkUrl(id: string) {
    const element = props.values?.[id]
    if (!element?.declaringType || element?.declaringType === element?.name) {
      return encodeURIComponent(element?.name ?? "")
    } else {
      const typePath = encodeURIComponent(element?.declaringType ?? "")
      const memberPath = encodeURIComponent(element?.name)
      return `${typePath}#${memberPath}`
    }
  }

  return (
    <div className="w-full relative">
      <Input
        className="w-full"
        placeholder="search"
        value={search}
        onFocus={() => {
          setVisible(true)
        }}
        onChange={(e) => setSearch(e.target.value)}
        ref={(value) => {
          inputRef.current = value?.nativeElement ?? null
        }}
      />
      {visible && results && (
        <div className="absolute top-10 left-0 right-0 bg-zinc-800 flex flex-col rounded-md py-3 z-10 shadow-2xl shadow-black">
          {results.slice(0, 10).map((x) => (
            <Link
              className="text-white w-full cursor-pointer hover:bg-zinc-700 px-5 no-underline"
              href={`/types/${linkUrl(x.id)}`}
              key={x.id}
              onClick={() => setSearch("")}
            >
              <SearchSignature
                signatureXml={props.values?.[x.id].signatureXml ?? ""}
              />
            </Link>
          ))}
        </div>
      )}
    </div>
  )
}

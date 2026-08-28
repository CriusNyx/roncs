"use client"
import { useCookie } from "@reactuses/core"
import _ from "lodash"

import { ChevronRight, ChevronLeft } from "react-feather"

function useCookieBool(key: string, value: boolean = false) {
  const [state, setState] = useCookie(
    key,
    { expires: 1, path: "/" },
    value ? "true" : "",
  )

  function setBool(value: boolean) {
    setState(value ? "true" : "")
  }

  return [
    state === undefined ? value : !!state,
    setBool,
    state === undefined,
  ] as const
}

type Route = {
  name: string
  href: string
  children: Route[]
}

function Route(name: string, href: string, ...children: Route[]): Route {
  return { name, href, children }
}

const Root: Route[] = [
  Route("Home", "/"),
  Route("Getting Started", "/docs/gettingStarted"),
  Route(
    "API",
    "/docs/api",
    Route("Serialize", "/docs/api#serialize"),
    Route("Deserialize", "/docs/api#deserialize"),
    Route("RegisterType", "/docs/api#registertype"),
    Route("RegisterTypeConverter", "/docs/api#registertypeconverter"),
    Route("RegisterListType", "/docs/api#registerlisttype"),
    Route("RegisterDictionaryType", "/docs/api#registerdictionarytype"),
    Route("RegisterTupleConverter", "/docs/api#registertupleconverter"),
    Route("RegisterProxyType", "/docs/api#registerproxytype"),
  ),
  Route(
    "Attributes",
    "/docs/serializationAttributes",
    Route("RonInclude", "/docs/serializationAttributes#ronexcluderoninclude"),
    Route("RonExclude", "/docs/serializationAttributes#ronexcluderoninclude"),
    Route("RonInto", "/docs/serializationAttributes#roninto"),
    Route("RonFrom", "/docs/serializationAttributes#ronfrom"),
    Route("RonList", "/docs/serializationAttributes#ronlist"),
    Route("RonMap", "/docs/serializationAttributes#ronmap"),
    Route("RonProxy", "/docs/serializationAttributes#ronproxy"),
    Route("RonTuple", "/docs/serializationAttributes#rontuple"),
  ),
]

export function Navigation() {
  const [expanded, setExpanded] = useCookieBool("nav-expanded")

  return (
    <div className="flex flex-row h-full">
      <div
        className={`flex flex-col ${expanded ? "w-60" : "w-0"} overflow-clip transition-all`}
      >
        <div className="flex flex-col overflow-clip w-[500px] py-5 pl-5">
          {Root.map((x, i) => (
            <RouteButton key={`route-${i}`} route={x} isRoot />
          ))}
        </div>
      </div>

      <div
        className="w-8 h-full flex flex-col justify-center items-center cursor-pointer"
        onClick={() => setExpanded(!expanded)}
      >
        {expanded ? <ChevronLeft /> : <ChevronRight />}
      </div>
    </div>
  )
}

export function ExpandButton(props: {
  expanded: boolean
  setExpanded: (value: boolean) => void
}) {
  return (
    <div
      className="cursor-pointer w-5 flex flex-col items-center justify-start"
      onClick={() => props.setExpanded(!props.expanded)}
    >
      {props.expanded ? "-" : "+"}
    </div>
  )
}

export function RouteButton(props: { route: Route; isRoot?: boolean }) {
  const [expanded, setExpanded] = useCookieBool(`expanded-${props.route.name}`)

  return (
    <div className="flex flex-col w-full select-none">
      <div className="flex flex-row items-center">
        {(props.isRoot && !_.isEmpty(props.route.children) && (
          <ExpandButton
            expanded={!!expanded}
            setExpanded={() => setExpanded(!expanded)}
          />
        )) || <div className="w-5" />}
        <a href={props.route.href} className={props.isRoot ? "" : "text-sm"}>
          {props.route.name}
        </a>
      </div>

      {!_.isEmpty(props.route.children) && (
        <div
          className={`${expanded ? "max-h-screen" : "max-h-0"} transition-all overflow-hidden `}
        >
          <div className="flex flex-col gap-1 py-3">
            {props.route.children.map((x, i) => (
              <RouteButton key={`route-${i}`} route={x} />
            ))}
          </div>
        </div>
      )}
    </div>
  )
}

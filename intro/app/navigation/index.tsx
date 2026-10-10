"use client"
import { useCookie } from "@reactuses/core"
import _ from "lodash"
import { ChevronRight, ChevronLeft } from "react-feather"
import { Pages } from "../pages"
import Link from "next/link"
import typeRoutes from "../../generated/RonCS.routeCache.json"
import useIsMobile from "../util/hooks"
import { createContext, useContext } from "react"

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
  Route("Home", `${Pages._path}`),
  Route("Getting Started", Pages.docs.gettingStarted._path),
  Route(
    "API",
    Pages.docs.api._path,
    Route("Serialize", `${Pages.docs.api._path}#serialize`),
    Route("Deserialize", `${Pages.docs.api._path}#deserialize`),
    Route("RegisterType", `${Pages.docs.api._path}#registertype`),
    Route(
      "RegisterTypeConverter",
      `${Pages.docs.api._path}#registertypeconverter`,
    ),
    Route("RegisterListType", `${Pages.docs.api._path}#registerlisttype`),
    Route(
      "RegisterDictionaryType",
      `${Pages.docs.api._path}#registerdictionarytype`,
    ),
    Route(
      "RegisterTupleConverter",
      `${Pages.docs.api._path}#registertupleconverter`,
    ),
    Route("RegisterProxyType", `${Pages.docs.api._path}#registerproxytype`),
  ),
  Route(
    "Attributes",
    Pages.docs.serializationAttributes._path,
    Route(
      "RonInclude",
      `${Pages.docs.serializationAttributes._path}#ronexcluderoninclude`,
    ),
    Route(
      "RonExclude",
      `${Pages.docs.serializationAttributes._path}#ronexcluderoninclude`,
    ),
    Route("RonInto", `${Pages.docs.serializationAttributes._path}#roninto`),
    Route("RonFrom", `${Pages.docs.serializationAttributes._path}#ronfrom`),
    Route("RonList", `${Pages.docs.serializationAttributes._path}#ronlist`),
    Route("RonMap", `${Pages.docs.serializationAttributes._path}#ronmap`),
    Route("RonProxy", `${Pages.docs.serializationAttributes._path}#ronproxy`),
    Route("RonTuple", `${Pages.docs.serializationAttributes._path}#rontuple`),
  ),
  Route("Types", Pages.types._path, ...(typeRoutes as any)),
]

interface NavigationContext {
  setExpanded: (value: boolean) => void
}

const navigationContext = createContext<NavigationContext>({
  setExpanded: () => {},
})

export function Navigation() {
  const [expanded, setExpanded] = useCookieBool("nav-expanded")

  return (
    <navigationContext.Provider value={{ setExpanded }}>
      <div className="flex flex-row h-screen z-10">
        <div
          className={`flex flex-col ${expanded ? "w-[380px]" : "w-0"} overflow-clip transition-all`}
        >
          <div className="flex flex-col overflow-y-scroll scrollbar-thin w-full h-screen py-5 pl-5 pr-2">
            {Root.map((x, i) => (
              <RouteButton key={`route-${i}`} route={x} isRoot />
            ))}
          </div>
        </div>

        <div
          className={`h-full flex flex-col justify-center items-end cursor-pointer pr-1 transition-all ${expanded ? "pl-0" : "pl-1"}`}
          onClick={() => setExpanded(!expanded)}
        >
          {expanded ? <ChevronLeft /> : <ChevronRight />}
        </div>
      </div>
    </navigationContext.Provider>
  )
}

export function ExpandButton(props: {
  expanded: boolean
  setExpanded: (value: boolean) => void
}) {
  return (
    <div
      className="cursor-pointer min-w-5 flex flex-col items-center justify-start"
      onClick={() => props.setExpanded(!props.expanded)}
    >
      {props.expanded ? "-" : "+"}
    </div>
  )
}

export function RouteButton(props: { route: Route; isRoot?: boolean }) {
  const [expanded, setExpanded] = useCookieBool(`expanded-${props.route.name}`)
  const { isMobile } = useIsMobile()
  const navContext = useContext(navigationContext)

  return (
    <div className="flex flex-col w-full max-w-full select-none overflow-clip">
      <div className="flex flex-row w-full items-center">
        {(props.isRoot && !_.isEmpty(props.route.children) && (
          <ExpandButton
            expanded={!!expanded}
            setExpanded={() => setExpanded(!expanded)}
          />
        )) || <div className="min-w-5" />}
        <Link
          href={props.route.href}
          className={`text-clip text-nowrap ${props.isRoot ? "" : "text-sm"}`}
          onClick={() => {
            if (isMobile) {
              navContext.setExpanded(false)
            }
          }}
        >
          {props.route.name}
        </Link>
      </div>

      {!_.isEmpty(props.route.children) && (
        <div
          className={`${expanded ? "" : "max-h-0"} transition-all overflow-hidden `}
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

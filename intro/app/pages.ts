import _ from "lodash"

type Page = {
  _name: string
} & (
  | {
      [childPath: string]: Page
    }
  | {}
)

type ThreadedPage<T extends Page> = Readonly<
  {
    [K in keyof T]: T[K] extends Page ? ThreadedPage<T[K]> : T[K]
  } & { _path: string }
>

function threadPages<T extends Page>(
  page: T,
  path: string = "/",
): ThreadedPage<T> {
  let output = { _name: page._name, _path: path } as any
  for (const [key, value] of Object.entries(page)) {
    if (_.isObject(value)) {
      output[key] = threadPages(value, path + key + "/")
    }
  }
  return output
}

export const Pages = threadPages({
  _name: "Home",
  docs: {
    _name: "Docs",
    api: {
      _name: "API",
    },
    gettingStarted: {
      _name: "Getting Started",
    },
    serializationAttributes: {
      _name: "Attributes",
    },
  },
  types: {
    _name: "Types",
  },
})

import "../csDocs.css"
import _ from "lodash"
import XMLToReact from "xml-to-react"
import { DOMParser } from "@xmldom/xmldom"
import xpath from "xpath"
import { DocComponents } from "./DocComponents"
import * as fs from "node:fs/promises"

const ENV = process.env.NODE_ENV

const xmlFile = await fs.readFile("./generated/RonCS.xml")
const xmlDocs = xmlFile.toString()

const components = createConverters(DocComponents)

const xmlToReact = new XMLToReact(components)

function camel(source: string) {
  return source[0].toLocaleLowerCase() + source.substring(1)
}

function createConverters(source: Record<string, React.FC<any>>) {
  return Object.entries(source).reduce(
    (prev, curr) => ({
      ...prev,
      [camel(curr[0])]: (attrs: any, data: any) => ({
        type: curr[1],
        props: { ...attrs, ...data },
      }),
    }),
    {},
  )
}

export default async function CSDocs(props: {
  params: Promise<{ slug: string }>
}) {
  const { slug } = await props.params

  const reactTree = xmlToReact.convert(xmlDocs, { slug })

  return <div className="flex flex-col pt-5">{reactTree}</div>
}

export async function generateStaticParams() {
  const doc = new DOMParser().parseFromString(xmlDocs, "text/xml")

  var declaringTypes = (
    xpath.select("//member[@declaringType]/@declaringType", doc as any) as any
  ).map((x: any) => x.value)

  const ids = _.uniq(declaringTypes) as string[]

  if (ENV === "production") {
    return ids.map((x) => ({ slug: x }))
  } else {
    return ids.map((x) => ({ slug: encodeURIComponent(x) }))
  }
}

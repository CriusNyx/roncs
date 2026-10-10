import * as fs from "node:fs/promises"
import "./csDocs.css"
import _ from "lodash"
import XMLToReact from "xml-to-react"
import { PropsWithChildren } from "react"
import { SearchSignature } from "../components/SearchSignature"
import Link from "next/link"

const xmlFile = await fs.readFile("./generated/RonCS.xml")
const xml = xmlFile.toString()

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

interface MemberProps {
  memberType: string
  declaringType?: string
  name: string
  searchSignatureXml: string
}

function linkUrl(declaringType: string | undefined, name: string) {
  if (!declaringType || declaringType === name) {
    return encodeURIComponent(name ?? "")
  } else {
    const typePath = encodeURIComponent(declaringType ?? "")
    const memberPath = encodeURIComponent(name)
    return `${typePath}#${memberPath}`
  }
}

const docComponents = createConverters({
  Doc(props: PropsWithChildren) {
    return props.children
  },
  Members(props: PropsWithChildren) {
    return props.children
  },
  Member(props: PropsWithChildren<MemberProps>) {
    const href = `/types/${linkUrl(props.declaringType, props.name)}`
    const content = (
      <Link className="no-underline" href={href}>
        <SearchSignature signatureXml={props.searchSignatureXml} />
      </Link>
    )
    if (props.memberType === "type") {
      return <div>{content}</div>
    } else {
      return <div className="pl-5">{content}</div>
    }
  },
})

const docXmlRenderer = new XMLToReact(docComponents)

export default async function CSDocs() {
  return (
    <div className="flex flex-col justify-start">
      {docXmlRenderer.convert(xml)}
    </div>
  )
}

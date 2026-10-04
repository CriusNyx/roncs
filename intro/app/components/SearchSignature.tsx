import { PropsWithChildren } from "react"
import XMLToReact from "xml-to-react"

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

const componentSource = {
  Signature(props: PropsWithChildren) {
    return props.children
  },
  Type(props: PropsWithChildren) {
    return <span className="text-[#4EC9B0]">{props.children}</span>
  },
  Variable(props: PropsWithChildren) {
    return <span className="text-[#9CDCFE]">{props.children}</span>
  },
  Method(props: PropsWithChildren) {
    return <span className="text-[#DCDCAA]">{props.children}</span>
  },
  Space() {
    return " "
  },
} as const

const components = createConverters(componentSource)

const xmlToReact = new XMLToReact(components)

interface SearchSignatureProps {
  signatureXml: string
}

export function SearchSignature(props: SearchSignatureProps) {
  return xmlToReact.convert(props.signatureXml)
}

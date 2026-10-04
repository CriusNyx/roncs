import _ from "lodash"
import Link from "next/link"
import React from "react"
import { PropsWithChildren } from "react"

export interface MemberProps {
  name: string
  signature: string
  declaringType: string
  memberType: string
  slug: string
}

export interface ParamProps {
  name: string
}

export interface InheritdocProps {
  baseImpl?: string
  baseImplUrl?: string
}

interface SignatureProps {
  type: string
}

export function TypeMember(props: PropsWithChildren<MemberProps>) {
  const id = decodeURIComponent(props.slug)
  const elementId = props.name

  if (props.declaringType !== id) return null

  const children = React.Children.toArray(props.children)

  const signatureIndex = children.findIndex(
    (x) =>
      typeof x === "object" &&
      "type" in x &&
      _.isFunction(x.type) &&
      x.type.name === "Signature",
  )

  const signature = children[signatureIndex]
  const rest = children.slice(signatureIndex + 1)

  return (
    <div className="type" id={elementId}>
      {signature}
      <div className="member">{rest}</div>
    </div>
  )
}

export function NonTypeMember(props: PropsWithChildren<MemberProps>) {
  const id = decodeURIComponent(props.slug)
  const elementId = props.name

  if (props.declaringType !== id) return null

  return (
    <div className={"member"} id={elementId}>
      {props.children}
    </div>
  )
}

export const DocComponents = {
  // XDoc components

  Doc(props: PropsWithChildren) {
    return <div>{props.children}</div>
  },
  Name(props: PropsWithChildren) {
    return <h1 className="text-wrap">{props.children}</h1>
  },
  Members(props: PropsWithChildren) {
    return <div className="flex flex-col">{props.children}</div>
  },
  Member(props: PropsWithChildren<MemberProps>) {
    if (props.memberType === "type") {
      return <TypeMember {...props} />
    } else {
      return <NonTypeMember {...props} />
    }
  },
  Summary(props: PropsWithChildren) {
    return <p className="summary">{props.children}</p>
  },
  Inheritdoc(props: InheritdocProps) {
    if (props.baseImplUrl) {
      const match = /([^#]*)#(.*)/.exec(props.baseImplUrl)
      const left = match?.[1] ?? ""
      const right = match?.[2] ?? ""
      const encodedImplUri =
        encodeURIComponent(left) + "#" + encodeURIComponent(right)

      return (
        <Link className="flex flex-row" href={`/types/${encodedImplUri}`}>
          See base for more info.
        </Link>
      )
    }
  },
  Param(props: PropsWithChildren<ParamProps>) {
    return (
      <div className="flex flex-row gap-1">
        <b>{props.name}:</b> {props.children}
      </div>
    )
  },
  Typeparam(props: PropsWithChildren<ParamProps>) {
    return (
      <div className="flex flex-row gap-1">
        <b>{props.name}</b> {props.children}
      </div>
    )
  },
  Returns(props: PropsWithChildren) {
    return (
      <div className="flex flex-row gap-1">
        <b>returns:</b> {props.children}
      </div>
    )
  },
  Exception(props: PropsWithChildren) {
    return (
      <div className="flex flex-row gap-1">
        <b>throw:</b>
        {props.children}
      </div>
    )
  },

  // Signature

  Signature(props: PropsWithChildren<SignatureProps>) {
    switch (props.type) {
      case "type":
        return (
          <h1 className="flex flex-row font-mono text-2xl">{props.children}</h1>
        )
      default:
        return (
          <h3 className="flex flex-row font-mono text-wrap text-base flex-wrap">
            {props.children}
          </h3>
        )
    }
  },
  Keyword(props: PropsWithChildren) {
    return <span className="text-[#569CD6]">{props.children}</span>
  },
  Type(props: TypeProps) {
    if (props.refUrl) {
      const encodedUrl = encodeURIComponent(props.refUrl)
      return (
        <Link href={`/types/${encodedUrl}`}>
          <span className="text-[#4EC9B0]">{props.children}</span>
        </Link>
      )
    }
    return <span className="text-[#4EC9B0]">{props.children}</span>
  },
  Variable(props: PropsWithChildren) {
    return <span className="text-[#9CDCFE]">{props.children}</span>
  },
  Method(props: PropsWithChildren) {
    return <span className="text-[#DCDCAA]">{props.children}</span>
  },

  // Formatting components

  Space() {
    return <>&nbsp;</>
  },
  Break() {
    return <span className="break" />
  },
  Tab() {
    return <span className="w-5" />
  },
}

interface TypeProps extends PropsWithChildren {
  refUrl?: string
}

import React from "react"

import _ from "lodash"
import { Code } from "../components/code"

interface XElementProps<T> {
  element: T
}

export function XElement(props: XElementProps<XElement>) {
  const element = props.element
  if ("assembly" in props.element) {
    return null
  }
  if ("doc" in props.element) {
    return <Doc element={props.element} />
  }
  if ("members" in props.element) {
    return <Members element={props.element} />
  }
  if ("member" in props.element) {
    return <Member element={props.element} />
  }
  if ("summary" in props.element) {
    return <Summary element={props.element} />
  }
  if ("param" in props.element) {
    return <Param element={props.element} />
  }
  if ("returns" in props.element) {
    return <Returns element={props.element} />
  }
  if ("inheritdoc" in props.element) {
    return <InheritDoc element={props.element} />
  }
  if ("exception" in props.element) {
    return <Exception element={props.element} />
  }
  if ("typeparam" in props.element) {
    return <TypeParam element={props.element} />
  }
  if ("#text" in props.element) {
    return <Text element={props.element} />
  }
  return <pre>{JSON.stringify(props.element, undefined, 2)}</pre>
}

function renderChildren(children: XElement[] | undefined) {
  return children?.map((x, i) => <XElement key={i} element={x} />)
}

export function Doc(props: XElementProps<Doc>) {
  return renderChildren(props.element.doc)
}

export function Members(props: XElementProps<Members>) {
  return renderChildren(props.element.members)
}

export function Member(props: XElementProps<Member>) {
  const signature = props.element[":@"]?.["@_signature"] ?? ""

  return (
    <div className="flex flex-col">
      <h1>{signature}</h1>
      {renderChildren(props.element.member)}
    </div>
  )
}

export function Summary(props: XElementProps<Summary>) {
  return (
    <>
      <div className="flex flex-col bg-slate-800 p-2 rounded-lg my-5">
        {renderChildren(props.element.summary)}
      </div>
    </>
  )
}

export function Text(props: XElementProps<XElement>) {
  return <span>{props.element["#text"]}</span>
}

export function Param(props: XElementProps<Param>) {
  return <div>{renderChildren(props.element.param)}</div>
}

export function Returns(props: XElementProps<Returns>) {
  return (
    <div>
      <b>Returns: </b>
      {renderChildren(props.element.returns)}
    </div>
  )
}

export function InheritDoc(props: XElementProps<InheritDoc>) {
  return (
    <div>
      <p>
        <b>Reflection information needed for inheritdoc</b>
      </p>
      {renderChildren(props.element.inheritdoc)}
    </div>
  )
}
export function Exception(props: XElementProps<Exception>) {
  return (
    <>
      <p>
        <b>Throws: </b>
        {props.element[":@"]?.["@_cref"]}
      </p>
      {renderChildren(props.element.exception)}
    </>
  )
}

export function TypeParam(props: XElementProps<Typeparam>) {
  return (
    <>
      <p>
        <b>Typeparam:</b> {props.element[":@"]?.["@_name"]}
      </p>
      {renderChildren(props.element.typeparam)}
    </>
  )
}

const type = ["fieldOrProp", "method", "type"] as const

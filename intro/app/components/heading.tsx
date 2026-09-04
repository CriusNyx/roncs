import _ from "lodash"
import { PropsWithChildren, ReactNode } from "react"

function headingSlug(children: ReactNode | ReactNode[]): string | undefined {
  if (_.isArray(children)) {
    return children
      .map(headingSlug)
      .filter(_.isString)
      .reduce((prev, curr) => `${prev}_${curr}`)
  }
  if (_.isString(children)) {
    return children.toLowerCase().replaceAll(/[^a-z*]/g, "")
  }
  return undefined
}

type HeadingType = "h1" | "h2" | "h3" | "h4" | "h5" | "h6"

interface HeadingProps extends PropsWithChildren {
  type: HeadingType
}

export function Heading(props: HeadingProps) {
  const Component = props.type
  return (
    <Component id={headingSlug(props.children)}>{props.children}</Component>
  )
}

const createHeadingComponent =
  (type: HeadingType) => (props: PropsWithChildren) => {
    return <Heading type={type}>{props.children}</Heading>
  }

export const HeadingComponents = {
  h1: createHeadingComponent("h1"),
  h2: createHeadingComponent("h2"),
  h3: createHeadingComponent("h3"),
  h4: createHeadingComponent("h4"),
  h5: createHeadingComponent("h5"),
  h6: createHeadingComponent("h6"),
}

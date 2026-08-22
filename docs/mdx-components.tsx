import type { MDXComponents } from "mdx/types"
import { Code } from "./app/components/code"
import { SideBySide } from "./app/components/annotations/SideBySide"
import { CodeBlock } from "./app/components/CodeBlock"
import { CodeExample } from "./app/components/CodeExample"
import { HeadingComponents } from "./app/components/heading"

export function useMDXComponents(components: MDXComponents): MDXComponents {
  return {
    ...HeadingComponents,
    ...components,
    Code,
    SideBySide,
    CodeBlock,
    CodeExample,
  }
}

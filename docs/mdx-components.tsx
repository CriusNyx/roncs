import type { MDXComponents } from "mdx/types"
import { Code } from "./app/components/code"
import { SideBySide } from "./app/components/annotations/SideBySide"
import { CodeBlock } from "./app/components/CodeBlock"

export function useMDXComponents(components: MDXComponents): MDXComponents {
  return {
    ...components,
    Code,
    SideBySide,
    CodeBlock,
  }
}

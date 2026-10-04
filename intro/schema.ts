type DocElement<Name extends string> = {
  [K in Name]?: XElement[]
} & {
  "#text"?: string | boolean
  ":@"?: {
    "@_name"?: string
    "@_cref"?: string
    "@_version"?: string
    "@_signature"?: string
    "@_baseClass"?: string
    "@_declaringType"?: string
  }
}

type Doc = DocElement<"doc">
type Assembly = DocElement<"assembly">
type Name = DocElement<"name">
type Members = DocElement<"members">
type Member = DocElement<"member">
type Param = DocElement<"param">
type Typeparam = DocElement<"typeparam">
type Summary = DocElement<"summary">
type InheritDoc = DocElement<"inheritdoc">
type Returns = DocElement<"returns">
type Exception = DocElement<"exception">
type Example = DocElement<"example">
type Code = DocElement<"code">
type XElement =
  | Doc
  | Assembly
  | Name
  | Members
  | Member
  | Param
  | Typeparam
  | Summary
  | InheritDoc
  | Returns
  | Exception
  | Example
  | Code

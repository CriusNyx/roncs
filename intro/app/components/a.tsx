const env = process.env.NODE_ENV

export function a(props: React.HTMLProps<"a">) {
  if (env === "production") {
    return <a href={`${props.href}.html`}>{props.children}</a>
  } else {
    return <a href={props.href}>{props.children}</a>
  }
}

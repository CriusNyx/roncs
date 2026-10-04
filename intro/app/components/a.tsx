import Link from "next/link"

const env = process.env.NODE_ENV

export function a(props: React.ComponentProps<typeof Link>) {
  return <Link href={props.href}>{props.children}</Link>
}

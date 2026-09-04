import type { Metadata } from "next"
import { Inter } from "next/font/google"
import "./globals.css"
import { ConfigProvider, theme } from "antd"
import { ServiceProvider } from "./services"
import { PropsWithChildren } from "react"
import { Page } from "./appPage"

const inter = Inter({ subsets: ["latin"] })

export const metadata: Metadata = {
  title: "Ron CS",
  description: "Doc site of RON CS",
}

export default function RootLayout({ children }: PropsWithChildren) {
  return (
    <html lang="en" className="dark bg-zinc-950 prose prose-invert">
      <body className={`${inter.className} w-[100vw] h-[100vh]`}>
        <ServiceProvider>
          <ConfigProvider theme={{ algorithm: theme.darkAlgorithm }}>
            <Page>{children}</Page>
          </ConfigProvider>
        </ServiceProvider>
      </body>
    </html>
  )
}

import type { Metadata } from "next"
import { Inter } from "next/font/google"
import "./globals.css"
import { ConfigProvider, theme } from "antd"
import { Header } from "./components/Header"
import { ServiceProvider } from "./services"

const inter = Inter({ subsets: ["latin"] })

export const metadata: Metadata = {
  title: "Ron CS",
  description: "Doc site of RON CS",
}

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode
}>) {
  return (
    <html
      lang="en"
      className="dark bg-zinc-950 prose prose-invert mx-auto py-10 max-w-5xl px-4"
    >
      <body className={inter.className}>
        <ServiceProvider>
          <ConfigProvider theme={{ algorithm: theme.darkAlgorithm }}>
            <Header />
            {children}
          </ConfigProvider>
        </ServiceProvider>
      </body>
    </html>
  )
}

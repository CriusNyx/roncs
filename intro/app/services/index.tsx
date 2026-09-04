"use client"

import { PropsWithChildren } from "react"
import { CodeThemeService } from "./CodeThemeService"

const Services: React.ComponentType<PropsWithChildren>[] = [CodeThemeService]

const ServiceHost = Services.reduceRight(
  (Prev, Curr) =>
    ({ children }: PropsWithChildren) => (
      <Prev>
        <Curr>{children}</Curr>
      </Prev>
    ),
)

export function ServiceProvider({ children }: PropsWithChildren) {
  return <ServiceHost>{children}</ServiceHost>
}

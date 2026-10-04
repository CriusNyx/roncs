import * as fs from "node:fs/promises"
import { Search } from "./Search"
import z from "zod"
import _ from "lodash"

export const searchCacheElementSchema = z.object({
  name: z.string(),
  declaringType: z.string().optional().nullable(),
  shortName: z.string(),
  signature: z.string(),
  signatureXml: z.string(),
  summary: z.string().optional(),
})

const searchCacheFile = await fs.readFile(
  "./generated/RonCS.searchCache.json",
  "ascii",
)
const searchCacheJson = JSON.parse(searchCacheFile)
const searchCache = z
  .array(searchCacheElementSchema)
  .parse(searchCacheJson)
  .map((x) => ({ ...x, id: x.name }))

const searchMap = _.keyBy(searchCache, (x) => x.name)

export function SearchLoader() {
  return <Search values={searchMap} />
}

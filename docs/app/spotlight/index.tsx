import { z } from "zod"
import {
  Selection,
  Selectable,
  SelectionProvider,
} from "codehike/utils/selection"
import { Block, CodeBlock, parseRoot } from "codehike/blocks"
import { Code } from "../components/code"

const Schema = Block.extend({
  steps: z.array(Block.extend({ code: CodeBlock })),
})

export default function Spotlight({ content }: { content: React.FC }) {
  const { steps } = parseRoot(content, Schema)

  return (
    <SelectionProvider className="flex flex-col">
      <div className="flex flex-row spotlight-selectable">
        {steps.map((step, i) => (
          <Selectable
            key={i}
            index={i}
            selectOn={["click"]}
            className="border border-zinc-700 data-[selected=true]:border-blue-400 px-5 py-2 mb-4 rounded bg-zinc-900 cursor-pointer hover:bg-zinc-800 transition-colors duration-200 ease-in-out"
          >
            <span>{step.title}</span>
          </Selectable>
        ))}
      </div>
      <div>
        <Selection
          from={steps.map((step) => (
            <Code codeblock={step.code} />
          ))}
        />
      </div>
    </SelectionProvider>
  )
}

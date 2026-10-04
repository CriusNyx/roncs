import z from "zod";

const paramsSchema = z.strictObject({
  "@_name": z.string().optional(),
  "@_cref": z.string().optional(),
  "@_version": z.string().optional(),
  "@_signature": z.string().optional(),
  "@_baseClass": z.string().optional(),
  "@_declaringType": z.string().optional(),
  "@_memberType": z.string().optional(),
  "@_friendlyName": z.string().optional(),
});

const xmlElementSchema = z.strictObject({
  get ["?xml"]() {
    return xmlElementSchema.array().optional();
  },
  get doc() {
    return xmlElementSchema.array().optional();
  },
  get assembly() {
    return xmlElementSchema.array().optional();
  },
  get name() {
    return xmlElementSchema.array().optional();
  },
  get members() {
    return xmlElementSchema.array().optional();
  },
  get member() {
    return xmlElementSchema.array().optional();
  },
  get param() {
    return xmlElementSchema.array().optional();
  },
  get typeparam() {
    return xmlElementSchema.array().optional();
  },
  get summary() {
    return xmlElementSchema.array().optional();
  },
  get inheritdoc() {
    return xmlElementSchema.array().optional();
  },
  get returns() {
    return xmlElementSchema.array().optional();
  },
  get exception() {
    return xmlElementSchema.array().optional();
  },
  get example() {
    return xmlElementSchema.array().optional();
  },
  get code() {
    return xmlElementSchema.array().optional();
  },
  "#text": z.union([z.string(), z.boolean()]).optional(),
  ":@": paramsSchema.optional(),
});

const csDocumentationSchema = xmlElementSchema.array();

export const schemas = { csDocumentationSchema } as const;

export type CsDocumentation = z.infer<typeof csDocumentationSchema>;
export type XMLElement = z.infer<typeof xmlElementSchema>;

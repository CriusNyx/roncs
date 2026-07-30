# TODO

## Parser

Consider copying the EBNF file directrly for already implemented parsers instead
of trying to be clever.

- [?] Extension Parser
  - [ ] I'm not sure what this means
- [x] String Parser
  - [ ] Tests
- [?] Check Char Lexer
  - [ ] Tests
- [x] ByteString Lexer
- [x] Numeric Lexers
  - [x] Int
  - [x] Float
  - [x] Unsigned
  - [x] Byte
- [x] Boolean Parser
- [x] Optional Parser
- [x] Range Parser
- [x] List Parser
- [x] Struct Parser
- [x] Enum (N/A for C#) Parser
- [x] Identifier Parser
- [ ] Fuzz Test

## AST Printer

- [ ] Implement AST Printer CLI

## Serializer/Deserializer

- [ ] Implement Serializer
- [x] Implement Prototype Deserializer
- [ ] Implement Property Deserializer

## Serializer/Deserializer Attributes

- [x] RonFromAttribute
- [x] RonIntoAttribute
- [x] Memoize RonFrom RonInto
- [ ] Specify Ron type coercion
- [ ] Attribute to control serialization of fields
- [ ] RonProxy Attribute
- [ ] RonField
- [ ] RonProperty (what for?)
- [ ] Implement circular serialization check

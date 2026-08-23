# TODO

## Parser

Consider copying the EBNF file directrly for already implemented parsers instead
of trying to be clever.

- [?] Extension Parser
  - [ ] Push parser extensions to Superpower
  - [ ] I'm not sure what this means
- [x] String Parser
  - [x] Tests
- [x] Check Char Lexer
  - [x] Tests
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
- [ ] Add trivia (problem for another time)
- [ ] Fuzz Test

## AST Printer

- [ ] Implement AST Printer CLI

## Serializer/Deserializer

- [x] Implement Serializer
- [x] Implement Prototype Deserializer
- [ ] Implement Property Deserializer

## Serializer/Deserializer Attributes

- [x] RonFromAttribute
- [x] RonIntoAttribute
- [x] Memoize RonFrom RonInto
- [ ] Specify AST (It doesn't match the RON RS AST exactly)
- [ ] Specify Ron type coercion
- [ ] Attribute to control serialization of fields
- [x] RonProxy Attribute
- [ ] RonField
- [x] RonList Attribute
- [x] RonDict Attribute
- [ ] RonProperty (what for?)
- [ ] Implement circular serialization check

# TODO

## Parser

Consider copying the EBNF file directrly for already implemented parsers instead
of trying to be clever.

- [ ] Add trivia for round tripping (problem for another time)
- [ ] Fuzz Test

## AST Printer

- [x] Implement AST Printer CLI

## Serializer/Deserializer

- [x] Implement Serializer
- [x] Implement Prototype Deserializer
  - [ ] Optimize deserializer
- [x] Implement Property Deserializer

## Serializer/Deserializer Attributes

- [x] RonFromAttribute
- [x] RonIntoAttribute
- [x] Memoize RonFrom RonInto
- [ ] Specify AST (It doesn't match the RON RS AST exactly)
- [ ] Specify Ron type coercion
- [ ] Attribute to control serialization of fields
- [x] RonProxy Attribute
- [n/a] RonField
  - [x] RonInclude/RonExclude
- [x] RonList Attribute
- [x] RonDict Attribute
- [x] RonMap Attribute
- [ ] Implement circular serialization check

## Other Goals

- [ ] Remove dependencies

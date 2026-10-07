# RonCS

Ron CS is a serializer/deserializer library for Ron and C#, letting you convert
C# objects into Ron files, and Ron files back into C# objects.

Compared to JSON and XML, Ron is more human readable and has built in type
names.

For most types they're public fields/properties and be serialized using
`Ron.Serialize(o)` and deserialized with `Ron.Deserialize<MyType>(ronString)`.

```ts
// A file encoded as RON.
Sequence(
  children: [
    FindFood(
      variableName: "food"
    ),
    NavigateTo(
      target: "food"
    ),
    Acquire(
      target: "food"
    )
  ]
)
```

See the docs website for information on how to use ron.

- [Docs](https://RonCS.net)
  - [Getting Started Guide](https://roncs.net/docs/gettingStarted)
  - [API quick start guide](https://roncs.net/docs/api)
  - [Full API docs](https://roncs.net/types)

# Getting Started

## Installation

Run `dotnet add package RonCS`

## Serializing and Deserializing

Convert an object to a ron string.

```cs
// C# source code
User user = new User {
  username = "foo@bar.com",
  name = "Foo"
};

string ron = Ron.Serialize(user);
```

```ts
// Ron document
User(
  username: "foo@bar.com",
  name: "Foo"
)
```

Converting an object back to C#

```cs
User original = Ron.Deserialize<User>(ron);

// Original will be the same as the user that was originally constructed
```

## Deserializing polymorphic types

To deserialize polymorphic types they have to be registered first so that the
deserializer knows about them.

```rs
// Example Ron document
Sequence(
  children: [
    FindFood(
      variableName: "food"
    ),
    NavigateTo(
      target: "food"
    )
    Acquire(
      target: "food"
    )
  ]
)
```

```cs
// Register polymorphic types
Ron.RegisterTypes(
  typeof(Sequence), 
  typeof(FindFood), 
  typeof(NavigateTo), 
  typeof(Acquire)
);

// Deserialize ron document.
BehaviorTree behaviorTree = Ron.Deserialize<BehaviorTree>(ronString);
```

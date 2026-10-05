# RonCS

Serialize and Deserialize C# objects as ron files.

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

<!-- TODO Link docs website -->

- Docs
  - Getting Started Guide
  - API quick start guide
  - Full API docs

# Getting Started

## Installation

TBD

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

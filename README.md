[![C# Version](https://img.shields.io/badge/C%23-6.0-green.svg)](https://msdn.microsoft.com/en-us/magazine/dn802602.aspx)
[![.Net Build Status](https://img.shields.io/appveyor/ci/rkoeninger/KitchenSink/master.svg?label=.Net%20Build)](https://ci.appveyor.com/project/rkoeninger/kitchensink/branch/master)
[![Mono Build Status](https://img.shields.io/travis/rkoeninger/KitchenSink/master.svg?label=Mono%20Build)](https://travis-ci.org/rkoeninger/KitchenSink)

# KitchenSink

A library for multi-paradigm programming in C#.

Not everything in this library is meant to be taken completely seriously. Some facilities are almost sarcastic in their inclusion.

### Basic Data Types

  * `Unit` - has only one meaningully unique value
  * `Void` - no value can be constructed; is always null
  * `Maybe<A>` - may or may not have a value and maps functions over missing value
  * `NewType<A>` - wraps existing types to distinguish on the type level

### Convenient Operators and Extensions

  * `ListOf`, `SeqOf`, `DictOf` - concise collection creation: `DictOf("one", 1, "two" 2, "three", 3)`
  * `Cmp` - sets up expressive bound comparisons: `0 <= Cmp(x) < 10`
  * `Eq`, `Same`, `Str`, `Hash` - null-safe basic object operations
  * `Apply` - partially apply functions
  * `Split` - splits strings by `Regex`
  * `A.IsIn(IEnumerable<A>)`, `A.IsIn(params A[])` - reversed contains check
  * `IEnumerable.AsStream`, `Stream.AsEnumerable` - converts `IEnumerable`s to/from `Stream`s

### Collection Enhancements

  * `Dictionary<K1, K2, V>` - `Dictionary`s that use `Tuple` for aggregate keys

### Composable Control Structures

  * `Cond` - builds a list of clauses and conditionally evaluates consequents
  * `Case` - like `Cond`, but clauses are applied to a key value

### Dynamic Scoping Emulation

  * `Scope.Push`, `Scope.Get` - controlled, thread local, global variables that are only defined farther down the call chain

### Dependency Injection

  * `Needs` - a minimal-configuration IoC container that can search assemblies and parent types and failover to other IoC containers
  * `SingleUse` - an attribute indicating that a dependency implementation is not threadsafe or can only be used once

### Testing and Validation

  * `Expect` - testing for exceptions and invalid syntax; companion to Assert
  * `Check` - property based testing
  * `Verify` - structural examination of data
  * `Validation` - appies a series of validation predicates to a value, collecting exceptions
  * `Rand` - produces random test data
  * `Sample` - provides common and edge-case test values
  * `All` - enumerates all values of certain types

### Pure Functional Programming Facilities

  * `IO` - a type for modeling and composing I/O operations and side effects
  * `Lens` - composable get and set pair for immutable types
  * `HList` - allows for abstraction over arity

### Questionable File Path Building Facility using Operator Overloading

  * Looks weird
  * But looks cool
  * Uses overloaded `/` operator to build paths
  * Works on .Net/Windows and Mono/Linux
  
```csharp
Drive.C / "Folder1" / "Folder2" / "File.txt"
    "C:\Folder1\Folder2\File.txt"

Folder.AppData / "MyApp" / "Config.xml"
    "C:\Users\Me\AppData\MyApp\Config.xml"
	"/users/me/.config/"
```

### Crazy XML Building Facility using Operator Overloading

  * This was clearly a mistake
  * I am so sorry
  * Uses overloaded `<`, `>`, `<=` and `>=` operators to build XML
  * Lets you do this:

```csharp
Xml.Doc < "catalog"
    < "book" >= "id" <= "bk101"
        < "author" <= "Gambardella, Matthew"
        < "title" <= "XML Developer's Guide"
        < "price" >= "currency" <= "USD" <= "44.95" > Xml.End
    < "book" >= "id" <= "bk102"
        < "author" <= "Ralls, Kim"
        < "title" <= "Midnight Rain"
        < "price" >= "currency" <= "USD" <= "5.95" > Xml.EndDoc
```

which generates

```xml
<catalog>
  <book id="bk101">
    <author>Gambardella, Matthew</author>
    <title>XML Developer's Guide</title>
    <price currency="USD">44.95</price>
  </book>
  <book id="bk102">
    <author>Ralls, Kim</author>
    <title>Midnight Rain</title>
    <price currency="USD">5.95</price>
  </book>
</catalog>
```

### Slang, an External Expression Language

  * Parsed into expression trees
  * Allows snippets of code to be declared in config files or database tables
  * Doesn't support declaring classes or methods
  * Type inference and collection literals:

```csharp
new List<int> { 1, 2, 3, 4, 5 }
new Dictionary<string, int> {
  {"one", 1}, {"two", 2}, {"three", 3}
}
```

```
[1 2 3 4 5]
{"one" 1 "two" 2 "three" 3}
```

  * Largely uses symbolic expression syntax and variadic functions that allow more terse expression of code:

```csharp
x + y + z + w
(x >= y) && (y >= z) && (z >= w)
(x == y) && (x == z) && (x == w)
```

```
(+ x y z)
(>= x y z)
(== x y z)
```

  * Nestable string characters: `« »`

[![.Net Build Status](https://img.shields.io/appveyor/ci/rkoeninger/KitchenSink/master.svg?label=.Net%20Build)](https://ci.appveyor.com/project/rkoeninger/kitchensink/branch/master)
[![Mono Build Status](https://img.shields.io/travis/rkoeninger/KitchenSink/master.svg?label=Mono%20Build)](https://travis-ci.org/rkoeninger/KitchenSink)

# KitchenSink

A library for multi-paradigm programming in C#/.Net.

### Basic Data Types

  * `Maybe<A>` - may or may not have a value and maps functions over missing value
  * `Unit` - has only one meaningully unique value
  * `Void` - no value can be constructed; is always null
  * `ISequence<A>` - lazy, persistent, one-time iteration of values

### Pure Functional Programming Facilities

  * `IO` - a type for modeling and composing I/O operations and side effects
  * `Lens` - composable get and set pair for immutable types

### Testing and Validation

  * `Expect` - testing for exceptions and invalid syntax; companion to Assert
  * `Check` - property based testing
  * `Verify` - structural examination of data
  * `Validation` - appies a series of validation predicates to a value, collecting exceptions
  * `Rand` - produces random test data
  * `Sample` - provides common and edge-case test values
  * `All` - enumerates all values of certain types

### Dependency Injection

  * `Needs` - a basic IoC container that uses the following attributes
  * `DefaultImplementationAttribute` - placed on an interface to indicate the default implementation so an IoC container doesn't have to be explicitly configured with it
  * `DefaultImplementationOfAttribute` - placed on a class to indicate that it is the default implementation of the specified interface
  * Mockable interfaces to common components: IConsole, IFileSystem
  * Standard "live" implementations of those interfaces that pass-through to Console, File, Directory, etc.
  * Useful mock implementations of those interfaces like ScriptedConsole and VirtualFileSystem

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

### Bizarre Range Comparison Facility using Operator Overloading

  * Can use `<`, `>`, `<=` and `>=`
  * Not type safe, as you can't specify type parameters to operators
  * Aside from the first `-` operator, looks pretty clean

```csharp
Z.Cmp- 0 <= x < 10
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

[![Build Status](https://travis-ci.org/rkoeninger/KitchenSink.svg?branch=master)](https://travis-ci.org/rkoeninger/KitchenSink)

# KitchenSink

Dumping ground for multi-paradigm programming ideas in C#.

Below is a summary of what this repo contains. Note that not everything in this library is meant to be taken completely seriously. Some facilities are almost sarcastic in their inclusion.

### Basic Data Types

  * `Unit` - has only one meaningully unique value
  * `Void` - no value can be constructed; is always null
  * `Maybe<A>` - may or may not have a value and maps functions over missing value
  * `Either<A, B>` - represents a distinction between two possible types of values
  * `NewType<A>` - wraps existing types to distinguish on the type level
  * `Monoid<A>` - appends values of a particular type into combined values of the same type
  * `Functor<FA, FB, A, B>` - lifts a function `A => B` into the space of `F`

### Convenient Operators and Extensions

  * `ListOf`, `SeqOf`, `DictOf` - concise collection creation: `DictOf("one", 1, "two", 2, "three", 3)`
  * `Cmp` - sets up expressive bound comparisons: `0 <= Cmp(x) < 10`
  * `Eq`, `Same`, `Str`, `Hash` - null-safe basic object operations
  * `Apply` - partially apply functions
  * `Split` - splits strings by `Regex`
  * `A.IsIn(IEnumerable<A>)`, `A.IsIn(params A[])` - reversed contains check
  * `IEnumerable.AsStream`, `Stream.AsEnumerable` - converts `IEnumerable`s to/from `Stream`s
  * `Batch` - split sequence into subsequences of consecutive values
  * `Buffer` - eagerly evaluate sequence ahead of time in arbitrary sized groups
  * `CrossJoin` - zip every element in sequence with every element in another sequence
  * `Cycle` - repeat elements of a sequence infinitely
  * `Deal` - split sequence into subsequences of every nth value
  * `Flatten` - combine values of subsequences into one long sequence
  * `Forever` - create lazy sequence by repeatedly calling an impure function
  * `Interleave` - create a sequence of the values of multiple sequences round-robin
  * `Intersperse` - create a sequence of elements from one sequence with all the elements of another between each one
  * `OverlappingPairs` - creates a sequence of tuple pairs of overlapping adjacent pairs in a sequence
  * `Shuffle` - randomize ordering of a sequence
  * `ZipExact` - zip two sequences, raising an error if one runs out before the other

### Optimally Specialized Collections

  * `BankersQueue<A>` - persistent queue made from two `ConsList`s
  * `BitmappedTrie<A>` - persistent vector with tree structure
  * `ComputedList<A>` - list that generates values based on index instead of storing in memory
  * `ConsList<A>` - an immutable, singly-linked list
  * `DefaultingDictionary<A, Z>` - dictionary that defers to another dictionary when key is missing
  * `Dictionary<A, B, Z>` - `Dictionary`s that use `Tuple` for aggregate keys
  * `FingerTree<A>` - a persistent dequeue implemented as 2,3-finger tree
  * `PairingHeap<A>` - a self-balancing, persistent, ordered heap
  * `RadixDictionary<A>` - mutable dictionary optimized for string keys
  * `RoseTree<A>` - mutable tree data structure that braches arbitrarily

### Neat Math Operations

  * `Factorial` - factorial as an extension method on `int`
  * `PermutationCount` - computes number of permutations for given collection size and subsequence size
  * `Permutations` - creates sequence of permutations of given size
  * `AllPermutations` - creates sequence of permutations of all sizes
  * `CombinationCount` - computes number of combinations for given collection size and subset size
  * `Combinations` - creates sequence of combinations of elements of given size
  * `AllCombinations` - creates sequence of combinations of all sizes

### Abstracted File System

  * `IFileSystem` - interface representing minimal set of file system operations
  * `RealFileSystem` - forwards operations to `System.IO` classes
  * `VirtualFileSystem` - in-memory file system data structure
  * `ResilientFileSystem` - `IFileSystem` decorator that adds retry logic to all operations

### Multiple Dispatch Mechanism

  * `MultiMethod` - group of functions that execute based on argument type or arbitrary predicate

### Powerful Concurrency Primitives

  * `Atom<A>` - mutually exclusive reference cell with synchronous updates that is both divisible and composable
  * `Lock` - exclusive locking primitive built on `Monitor.Enter`/`.Exit` that is composible

### Composable Control Structures

  * `Cond` - builds a list of clauses and conditionally evaluates consequents
  * `Case` - like `Cond`, but clauses are applied to a key value

### Flexible Resiliency Mechanisms

  * `Retry.Exponential` - waits double time between each successive attempt of an operation
  * `Retry.Fractal` - subdivides workload of batch operations, retrying as series of smaller batches
  * `Retry.Sequential` - attempts a series of alternate arguments to parameterized operation

### Easy Cache Implementation

  * `Batcher` - accumulates arguments to `Push` method until limit is reached or `Flush` is called
  * `Cache` - uses code generation to build wrapper class around interface implementation that caches all methods
  * `Debounce` - returns new version of an action that only passes through call after a time has passed since last call
  * `Memo` - returns new version of a function that caches return value for inputs, with optional expiration time
  * `Sprinkler` - splits items from batches passed to `Push` spread out by time delay

### Dynamic Scoping Emulation

  * `Scope.Push`, `Scope.Get` - controlled, thread local, global variables that are only defined farther down the call chain

### Simple Dependency Injection

  * `Needs` - a minimal-configuration IoC container that can search assemblies and parent types and failover to other IoC containers
  * `SingleUse` - an attribute indicating that a dependency implementation is not threadsafe or can only be used once

### Precise Timekeeping Representations

  * `DateSpan` - region of time between two dates

### Testing and Validation

  * `Expect` - testing for exceptions and property based testing; companion to Assert
  * `Rand` - produces random test data
  * `Sample` - provides common and edge-case test values
  * `All` - enumerates all values of certain types

### Pure Functional Programming Facilities

  * `Io` - a type for modeling and composing I/O operations and side effects
  * `Lens` - composable get and set pair for immutable types

### Questionable File Path Building Facility using Operator Overloading

  * Looks weird
  * But looks cool
  * Uses overloaded `/` operator to build paths
  * Works on .Net/Windows and Mono/Linux
 
```csharp
Drive.C / "Folder1" / "Folder2" / "File.txt"
    "C:\\Folder1\\Folder2\\File.txt" (Windows)

Folder.AppData / "MyApp" / "Config.xml"
    "C:\\Users\\Me\\AppData\\MyApp\\Config.xml" (Windows)
    "/users/me/.config/MyApp/Config.xml"        (Linux)
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

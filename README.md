# zed-sharp

A library for multi-paradigm programming in C#/.Net

### Basic Data Types

  * Maybe - may or may not have a value and maps functions over a hypothetical value
  * Unit - has only one meaningully unique value
  * Void - has no values
  * Idiot - every value is unique

### Function Composition

  * Match - extensible, immutable control-flow structure that branches on arbitrary predicates
  * MultiMethod - mutable set of method overloads that are dispatched on the types of multiple arguments
  * GenericMethod - mutable set of method overloads that are dispatched on arbitrary predicates

### Pure Functional Programming Facilities

  * IO - a type for modeling and composing I/O and side effects
  * Lens - composable get and set pair for immutable types

### Testing and Validation

  * Expect - testing for exceptions and invalid syntax; companion to Assert
  * Check - property based testing
  * Verify - structural examination of data
  * Validation - appies a series of validation predicates to a value, collecting exceptions
  * Rand - produces random test data
  * Sample - provides common and edge-case test values
  * All - enumerates all values of certain types

### Relational Data Structures

  * Table - a list/set of Rows
  * Row - a typed series of named values in a Table
  * Column - a typed, named list/set of values in a Table
  * Projection - a conversion from one Table to another

### Improved Date and Time Facilities

  * ZonedDateTime - DateTime with explicit TimeZoneInfo
  * DateTimeRange - a span of DateTimes that describes an entire day instead of midight the morning of that day
  * Clock - overridable access to the system clock

### Dependency Injection

  * __DefaultImplementation__Attribute - placed on an interface to indicate the default implementation so an IoC container doesn't have to explicitly configured with it
  * __DefaultImplementationOf__Attribute - placed on a class to indicate that it is the default implementation of the specified interface
  * ConsoleDI, FileSystemDI - interfaces to Console and FileSystem that can be mocked out

### Crazy XML Building Facility Based on Operator Overloading

  * This was probably a mistake
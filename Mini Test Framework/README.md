# MiniTest Framework

## Overview
MiniTest is a lightweight unit testing framework built from scratch. It consists of two main components:

1. **MiniTest Library** – Provides test attributes for marking classes and methods as test containers and includes assertion methods.
2. **MiniTestRunner** – A console application that dynamically loads assemblies containing tests, executes them, and presents the results in the console.

---

## Features
### Test Attributes
MiniTest provides the following attributes to define test structures and execution order:

- **`TestClassAttribute`**: Marks a class as a test container.
- **`TestMethodAttribute`**: Marks a method as a unit test.
- **`BeforeEachAttribute`**: Defines a method to run before each test method.
- **`AfterEachAttribute`**: Defines a method to run after each test method.
- **`PriorityAttribute`**: Sets an execution priority for test methods (lower values run first).
- **`DataRowAttribute`**: Enables parameterized testing by supplying multiple test inputs.
- **`DescriptionAttribute`**: Adds a description to a test class or test method.

### Assertions
A static `Assert` class provides methods for test validation:

- **`ThrowsException<TException>(Action action, string message = "")`**: Ensures a specific exception is thrown.
- **`AreEqual<T>(T? expected, T? actual, string message = "")`**: Verifies two values are equal.
- **`AreNotEqual<T>(T? notExpected, T? actual, string message = "")`**: Ensures two values are not equal.
- **`IsTrue(bool condition, string message = "")`**: Confirms that a condition is true.
- **`IsFalse(bool condition, string message = "")`**: Confirms that a condition is false.
- **`Fail(string message = "")`**: Forces a test failure with a custom error message.

If an assertion fails, an `AssertionException` is thrown.

### Exception Handling
The framework includes a custom exception, `AssertionException`, to provide clear failure messages.

---

## MiniTestRunner
### How It Works
1. Loads test assemblies dynamically using `AssemblyLoadContext`.
2. Discovers test classes (`TestClassAttribute`) and test methods (`TestMethodAttribute`).
3. Executes tests in order based on `PriorityAttribute` (lower values run first).
4. Runs `BeforeEach` and `AfterEach` setup/teardown methods.
5. Handles parameterized tests (`DataRowAttribute`).
6. Outputs test results with color-coded formatting.

### Console Output Formatting
- ✅ **Green** – Passed tests
- ❌ **Red** – Failed tests
- ⚠ **Yellow** – Warnings (e.g., missing constructor, invalid attributes)

Example output:
```
Running tests in MyTests.TestClass...

TestMethod1................................................ : PASSED
TestMethod2................................................ : FAILED
Expected: 5. Actual: 3.

******************************
* Test passed:    1 / 2      *
* Failed:        1           *
******************************
```

---
### Requirements
- .NET 6 or later
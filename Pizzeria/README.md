# Pizzeria Simulation

## Overview
This project is a multithreaded simulation of a pizzeria that models the interactions between chefs preparing pizzas and deliverers distributing them. The core logic is based on the producer-consumer problem, where chefs (producers) create pizzas and place them in a synchronized queue, and deliverers (consumers) retrieve and deliver them.

## How It Works
### 1. **ParallelQueue<T> (Thread-Safe Queue)**
The `ParallelQueue<T>` class is a custom thread-safe queue implementation that:
- Uses semaphores to manage enqueueing and dequeueing operations safely.
- Ensures a fixed-size queue where chefs can only enqueue pizzas if there is space available.
- Supports both blocking (`Enqueue`, `Dequeue`) and non-blocking (`TryEnqueueAsync`, `TryDequeueAsync`) operations with timeouts.
- Implements the `IParallelQueue<T>` and `IPausableQueue<T>` interfaces.

### 2. **Pausable Queue Operations**
The queue supports pausing and resuming operations:
- `PauseEnqueue()` and `PauseDequeue()` stop adding and removing items, respectively.
- `ResumeEnqueue()` and `ResumeDequeue()` allow operations to continue.
- These mechanisms use `ManualResetEventSlim` to efficiently manage thread synchronization.

### 3. **Pizzeria Simulation**
The `Pizzeria` class orchestrates the entire simulation by:
- Creating and managing chefs and deliverers using asynchronous tasks.
- Using a `Barrier` to synchronize end-of-day reporting.
- Maintaining daily income records using `ConcurrentDictionary<string, decimal>`.
- Listening for user input to pause/resume operations and sum up daily earnings.

### 4. **Chefs (Producers)**
- A fixed number of chefs are spawned.
- Each chef generates pizzas using `GeneratePizzaOrder()`.
- If the queue is full, the chef waits up to one second before discarding the order.
- Baking time is simulated with a random delay between 1000-2000 ms.

### 5. **Deliverers (Consumers)**
- A fixed number of deliverers retrieve pizzas from the queue.
- If no pizza is available, they wait up to one second before retrying.
- Delivering a pizza takes between 1000-2000 ms.
- Each successful delivery updates the income record.

### 6. **Shift System and End-of-Day Summary**
- Each worker must process `_dailyPizzaOrders` before finishing their shift.
- At the end of each shift, all workers wait at a `Barrier` before summing up the income.
- The user can press `b` to view the daily earnings.

### 7. **User Controls**
The simulation can be controlled using the following keys:
```
  p - Pause pizza preparation
  r - Resume pizza preparation
  k - Pause delivery
  l - Resume delivery
  b - Sum up daily income
  q - Exit
```

## Dependencies
- `System.Threading.Tasks`
- `System.Threading`
- `System.Collections.Concurrent`

## Running the Simulation
You can start the simulation by running the `Program` class. The number of chefs, deliverers, and queue capacity can be specified as command-line arguments or defaults to half of the processor count for chefs and deliverers, and a queue size of 10.

Example usage:
```bash
PizzeriaSimulation.exe 4 4 10
```
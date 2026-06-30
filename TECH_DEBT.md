# Technical Debt

This file tracks technical debt in the TodoList.Api project.

## 1. Use case and controller relationship with DTO
### Problem

- Sometimes use cases receive DTOs or entities as input.
- Sometimes use cases return DTOs or entities as output.
- The use case should not depend on the controller or the DTOs.
- The controller should not depend on the use case or the entities.

### Solution (Maybe)
- Maybe let use cases depends on DTOs (in/out) but should not use DTO Mappers and manually map them.
- Maybe let use cases depends on entities (in/out) and the controller use DTO Mappers.

### Problem Place
- [Auth Controller](./Features/Auth/Presentation/Controller/AuthController.cs)
- [Auth Use Case](./Features/Auth/Domain/UseCase)
- [Todo Controller](./Features/Todo/Presentation/Controller/TodoController.cs)
- [Todo Use Case](./Features/Todo/Domain/UseCase)

---


## 2. Retrive curret authenticated user from the request
### Problem
- The current authenticated user is retrieved from the request in the controller with specific method.
- That method should be in a common place and not in the controller, so that it can be reused in other places.

### Solution (Maybe)
- Not sure for now.

### Problem Place
- [Todo Controller](./Features/Todo/Presentation/Controller/TodoController.cs)

---

## 3. Too much code in Program.cs
### Problem
- The Program.cs file has too much code and is hard to read.
- The configuration of the application should be in a separate class or classes.

### Solution (Maybe)
- Not sure for now.

### Problem Place
- [Program.cs](./Program.cs)
module ToDoAPI.Tests.ToDoControllerTests

open System
open Xunit
open ToDoAPI
open ToDoAPI.Controllers
open Microsoft.AspNetCore.Mvc

/// <summary>
/// Fake repository for unit testing ToDoController.
/// </summary>
type FakeToDoRepository(items: ToDoItemEntity list) =
    let mutable store = items
    interface IDisposable with member _.Dispose() = ()
    interface IToDoRepository with
        member _.GetAll() = store
        member _.TryGetById(id) = store |> List.tryFind (fun x -> x.Id = id)
        member _.Add(item) = store <- item :: store
        member _.Update(item) = store <- item :: (store |> List.filter (fun x -> x.Id <> item.Id))
        member _.Delete(id) = store <- store |> List.filter (fun x -> x.Id <> id)

[<Fact>]
let ``GetAll returns all items`` () =
    // Arrange
    let items = [
        { Id = Guid.NewGuid(); Title = "Test 1"; IsCompleted = false }
        { Id = Guid.NewGuid(); Title = "Test 2"; IsCompleted = true }
    ]
    let repo = FakeToDoRepository(items) :> IToDoRepository
    let controller = ToDoController(repo)
    // Act
    let result = controller.GetAll()
    // Assert
    let okResult = Assert.IsType<ActionResult<ToDoItemEntity list>>(result)
    let value = okResult.Value
    Assert.Equal(2, List.length value)

[<Fact>]
let ``GetById returns item when found`` () =
    // Arrange
    let id = Guid.NewGuid()
    let item = { Id = id; Title = "Test"; IsCompleted = false }
    let repo = FakeToDoRepository([item]) :> IToDoRepository
    let controller = ToDoController(repo)
    // Act
    let result = controller.GetById(id)
    // Assert
    let okResult = Assert.IsType<ActionResult<ToDoItemEntity>>(result)
    Assert.Equal(item.Id, okResult.Value.Id)

[<Fact>]
let ``GetById returns NotFound when item does not exist`` () =
    // Arrange
    let repo = FakeToDoRepository([])
    let controller = ToDoController(repo)
    // Act
    let result = controller.GetById(Guid.NewGuid())
    // Assert
    Assert.Null(result.Value)
    Assert.IsType<NotFoundObjectResult>(result.Result)

[<Fact>]
let ``Add adds item and returns it`` () =
    // Arrange
    let repo = FakeToDoRepository([])
    let controller = ToDoController(repo)
    let item = { Id = Guid.NewGuid(); Title = "New"; IsCompleted = false }
    // Act
    let result = controller.Add(item)
    // Assert
    let okResult = Assert.IsType<ActionResult<ToDoItemEntity>>(result)
    Assert.Equal(item.Id, okResult.Value.Id)
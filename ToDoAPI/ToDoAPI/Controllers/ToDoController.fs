namespace ToDoAPI.Controllers

open Microsoft.AspNetCore.Mvc
open ToDoAPI

[<ApiController>]
[<Route("api/[controller]")>]
type ToDoController(repository: IToDoRepository) =
    inherit ControllerBase()

    [<HttpGet>]
    member _.GetAll() : ActionResult<ToDoItemEntity list> =
        let items = repository.GetAll()
        ActionResult<ToDoItemEntity list>(items)

    [<HttpGet("{id}")>]
    member _.GetById(id: System.Guid) : ActionResult<ToDoItemEntity> =
        match repository.TryGetById(id) with
        | Some item -> ActionResult<ToDoItemEntity>(item)
        | None -> ActionResult<ToDoItemEntity>(base.NotFound("Item not found"))

    [<HttpPost>]
    member this.Add([<FromBody>] item: ToDoItemEntity) : ActionResult<ToDoItemEntity> =
        if not this.ModelState.IsValid then
            ActionResult<ToDoItemEntity>(this.BadRequest(this.ModelState))
        else
            repository.Add(item)
            ActionResult<ToDoItemEntity>(item)
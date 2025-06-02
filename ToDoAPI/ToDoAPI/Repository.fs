namespace ToDoAPI

open System
open System.Linq
open Microsoft.EntityFrameworkCore

/// <summary>
/// Provides database-backed storage and management for to-do items using Entity Framework Core.
/// </summary>
type IToDoRepository =
    abstract member GetAll : unit -> ToDoItemEntity list
    abstract member TryGetById : Guid -> ToDoItemEntity option
    abstract member Add : ToDoItemEntity -> unit
    abstract member Update : ToDoItemEntity -> unit
    abstract member Delete : Guid -> unit

type ToDoRepository(ctx: ToDoDbContext) =
    interface IToDoRepository with
        /// <summary>
        /// Retrieves all to-do items from the database.
        /// </summary>
        member _.GetAll() : ToDoItemEntity list =
            ctx.ToDoItems.AsNoTracking().ToList() |> List.ofSeq

        /// <summary>
        /// Retrieves a to-do item by its unique identifier from the database.
        /// </summary>
        member _.TryGetById(id: Guid) : ToDoItemEntity option =
            ctx.ToDoItems.AsNoTracking().FirstOrDefault(fun x -> x.Id = id)
            |> Option.ofObj

        /// <summary>
        /// Adds a new to-do item to the database.
        /// </summary>
        member _.Add(item: ToDoItemEntity) : unit =
            ctx.ToDoItems.Add(item) |> ignore
            ctx.SaveChanges() |> ignore

        /// <summary>
        /// Updates an existing to-do item in the database.
        /// </summary>
        member _.Update(item: ToDoItemEntity) : unit =
            ctx.ToDoItems.Update(item) |> ignore
            ctx.SaveChanges() |> ignore

        /// <summary>
        /// Deletes a to-do item by its unique identifier from the database.
        /// </summary>
        member _.Delete(id: Guid) : unit =
            match ctx.ToDoItems.Find(id) |> Option.ofObj with
            | None -> ()
            | Some entity ->
                ctx.ToDoItems.Remove(entity) |> ignore
                ctx.SaveChanges() |> ignore
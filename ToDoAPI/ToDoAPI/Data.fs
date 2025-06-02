namespace ToDoAPI

open System
open System.ComponentModel.DataAnnotations
open Microsoft.EntityFrameworkCore

/// <summary>
/// Entity Framework Core entity for ToDo items.
/// </summary>
[<CLIMutable>]
type ToDoItemEntity = {
    Id: Guid
    [<Required>]
    [<StringLength(100)>]
    Title: string
    IsCompleted: bool
}

type ToDoDbContext(options: DbContextOptions<ToDoDbContext>) =
    inherit DbContext(options)

    [<DefaultValue>]
    val mutable toDoItems : DbSet<ToDoItemEntity>
    member this.ToDoItems
        with get() = this.toDoItems
        and set v = this.toDoItems <- v

    override _.OnModelCreating(modelBuilder: ModelBuilder) =
        modelBuilder.Entity<ToDoItemEntity>().ToTable("ToDoItems") |> ignore
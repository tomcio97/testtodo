namespace ToDoAPI
#nowarn "20"
open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.HttpsPolicy
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.EntityFrameworkCore

module Program =
    let exitCode = 0

    let seedData (ctx: ToDoAPI.ToDoDbContext) =
        if not (ctx.ToDoItems.Any()) then
            ctx.ToDoItems.AddRange(
                [|
                    { ToDoAPI.ToDoItemEntity.Id = Guid.NewGuid(); Title = "Sample Task 1"; IsCompleted = false }
                    { ToDoAPI.ToDoItemEntity.Id = Guid.NewGuid(); Title = "Sample Task 2"; IsCompleted = true }
                |]
            ) |> ignore
            ctx.SaveChanges() |> ignore

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)

        // Add controllers and Swagger services
        builder.Services.AddControllers() |> ignore
        builder.Services.AddEndpointsApiExplorer() |> ignore
        builder.Services.AddSwaggerGen() |> ignore

        // Add EF Core with PostgreSQL
        let connStr = builder.Configuration.GetConnectionString("DefaultConnection")
        builder.Services.AddDbContext<ToDoAPI.ToDoDbContext>(fun options ->
            options.UseNpgsql(connStr) |> ignore
        ) |> ignore

        // Add scoped ToDoRepository service
        builder.Services.AddScoped<IToDoRepository, ToDoRepository>() 

        let app = builder.Build()

        // Seed data
        use scope = app.Services.CreateScope()
        let db = scope.ServiceProvider.GetRequiredService<ToDoAPI.ToDoDbContext>()
        db.Database.EnsureCreated() |> ignore
        seedData db

        // Enable Swagger middleware
        app.UseSwagger() |> ignore
        app.UseSwaggerUI() |> ignore

        app.UseHttpsRedirection()

        app.UseAuthorization()
        app.MapControllers()

        app.Run()

        exitCode

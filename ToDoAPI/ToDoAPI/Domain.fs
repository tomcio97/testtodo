namespace ToDoAPI

/// <summary>
/// Represents a single to-do item in the application.
/// </summary>
type ToDoItem = {
    /// Unique identifier for the to-do item.
    Id: System.Guid
    /// Title or short description of the to-do item.
    Title: string
    /// Indicates whether the to-do item is completed.
    IsCompleted: bool
}
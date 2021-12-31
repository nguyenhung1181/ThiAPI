var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<EmployeeDb>(opt => opt.UseInMemoryDatabase("baithi"));
var app = builder.Build();


app.MapGet("/employee", async (EmployeeDb db) =>
    await db.Employees.ToListAsync());

app.MapGet("/employee/{id}", async (int id, EmployeeDb db) =>
    await db.Employees.FindAsync(id)
        is Employee employee
            ? Results.Ok(employee)
            : Results.NotFound());

app.MapPost("/employee", async (Employee employee, EmployeeDb db) =>
{
    db.Employees.Add(employee);
    await db.SaveChangesAsync();

    return Results.Created($"/employee/{employee.Id}", employee);
});

app.MapPut("/employee/{id}", async (int id, Employee inputTodo, EmployeeDb db) =>
{
    var employee = await db.Employees.FindAsync(id);

    if (employee is null) return Results.NotFound();

    employee.Name = inputTodo.Name;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/employee/{id}", async (int id, EmployeeDb db) =>
{
    if (await db.Todos.FindAsync(id) is Employee employee)
    {
        db.Todos.Remove(employee);
        await db.SaveChangesAsync();
        return Results.Ok(employee);
    }

    return Results.NotFound();
});

app.Run();

class Employee
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
}

class EmployeeDb : DbContext
{
    public EmployeeDb(DbContextOptions<EmployeeDb> options)
        : base(options) { }

    public DbSet<Employee> Employees => Set<Employee>();
}
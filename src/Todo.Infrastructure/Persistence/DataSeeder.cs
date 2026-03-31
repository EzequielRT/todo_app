using Microsoft.AspNetCore.Identity;
using Todo.Domain.Entities;
using Todo.Infrastructure.Persistence.Models;

namespace Todo.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        if (!userManager.Users.Any())
        {
            var user = new ApplicationUser
            {
                UserName = "admin@todo.com",
                Email = "admin@todo.com",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(user, "Admin123!");

            if (!context.TodoItems.Any())
            {
                context.TodoItems.AddRange(
                    TodoItem.Create("Organizar mesa de trabalho", "Limpar a poeira e organizar os cabos do setup", DateTime.UtcNow.AddDays(1), user.Id),
                    TodoItem.Create("Comprar mantimentos", "Ir ao mercado comprar café, leite e frutas", DateTime.UtcNow.AddSeconds(3600), user.Id),
                    TodoItem.Create("Treino de musculação", "Focar em membros superiores hoje", DateTime.UtcNow.AddDays(1), user.Id),
                    TodoItem.Create("Leitura matinal", "Ler 10 páginas do livro atual", DateTime.UtcNow.AddDays(2), user.Id),
                    TodoItem.Create("Planejamento semanal", "Definir as metas para a próxima semana", DateTime.UtcNow.AddDays(3), user.Id)
                );

                await context.SaveChangesAsync();
            }
        }
    }
}

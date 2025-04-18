using QAProphet.Data;
using QAProphet.Domain;

namespace QAProphet.Features.Tags.Seed;

public class Seed
{
    private readonly AppDbContext _dbContext;

    public Seed(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedTags()
    {
        List<Tag> tags =
        [
            new Tag
            {
                Title = "C#",
                Description = "Вопросы по языку программирования C# и платформе .NET"
            },
            new Tag
            {
                Title = "JavaScript",
                Description = "Вопросы по языку программирования JavaScript и связанным технологиям"
            },
            new Tag
            {
                Title = "Python",
                Description = "Вопросы по языку программирования Python и его экосистеме"
            },
            new Tag
            {
                Title = "SQL",
                Description = "Вопросы по работе с базами данных и языку запросов SQL"
            },
            new Tag
            {
                Title = "ASP.NET",
                Description = "Вопросы по веб-фреймворку ASP.NET и его компонентам"
            },
            new Tag
            {
                Title = "React",
                Description = "Вопросы по библиотеке React и разработке пользовательских интерфейсов"
            },
            new Tag
            {
                Title = "Алгоритмы",
                Description = "Вопросы по алгоритмам, структурам данных и сложности вычислений"
            },
            new Tag
            {
                Title = "Git",
                Description = "Вопросы по системе контроля версий Git и платформам типа GitHub/GitLab"
            },
            new Tag
            {
                Title = "Docker",
                Description = "Вопросы по контейнеризации приложений и работе с Docker"
            },
            new Tag
            {
                Title = "DevOps",
                Description = "Вопросы по методологиям DevOps и связанным инструментам"
            },
            new Tag
            {
                Title = "Тестирование",
                Description = "Вопросы по тестированию программного обеспечения"
            },
            new Tag
            {
                Title = "HTML/CSS",
                Description = "Вопросы по вёрстке и стилизации веб-страниц"
            },
            new Tag
            {
                Title = "Базы данных",
                Description = "Общие вопросы по проектированию и работе с базами данных"
            },
            new Tag
            {
                Title = "Linux",
                Description = "Вопросы по операционной системе Linux и администрированию"
            },
            new Tag
            {
                Title = "Безопасность",
                Description = "Вопросы по информационной безопасности и защите данных"
            }
        ];

        if (_dbContext.Tags.Any()) return;
        
        _dbContext.Tags.AddRange(tags);
        await _dbContext.SaveChangesAsync();
    }
}
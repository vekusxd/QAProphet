using Elastic.Clients.Elasticsearch;
using Microsoft.EntityFrameworkCore;
using QAProphet.Data.ElasticSearch;
using QAProphet.Domain;
using QAProphet.Features.Tags.GetTagDetails;

namespace QAProphet.Data.EntityFramework;

public class Seed
{
    private readonly AppDbContext _dbContext;
    private readonly ElasticsearchClient _client;

    public Seed(AppDbContext dbContext, ElasticsearchClient client)
    {
        _dbContext = dbContext;
        _client = client;
    }

    public async Task SeedTags()
    {
        List<Tag> tags =
        [
            new()
            {
                Title = "C#",
                Description = "Вопросы по языку программирования C# и платформе .NET"
            },
            new()
            {
                Title = "JavaScript",
                Description = "Вопросы по языку программирования JavaScript и связанным технологиям"
            },
            new()
            {
                Title = "Python",
                Description = "Вопросы по языку программирования Python и его экосистеме"
            },
            new()
            {
                Title = "SQL",
                Description = "Вопросы по работе с базами данных и языку запросов SQL"
            },
            new()
            {
                Title = "ASP.NET",
                Description = "Вопросы по веб-фреймворку ASP.NET и его компонентам"
            },
            new()
            {
                Title = "React",
                Description = "Вопросы по библиотеке React и разработке пользовательских интерфейсов"
            },
            new()
            {
                Title = "Алгоритмы",
                Description = "Вопросы по алгоритмам, структурам данных и сложности вычислений"
            },
            new()
            {
                Title = "Git",
                Description = "Вопросы по системе контроля версий Git и платформам типа GitHub/GitLab"
            },
            new()
            {
                Title = "Docker",
                Description = "Вопросы по контейнеризации приложений и работе с Docker"
            },
            new()
            {
                Title = "DevOps",
                Description = "Вопросы по методологиям DevOps и связанным инструментам"
            },
            new()
            {
                Title = "Тестирование",
                Description = "Вопросы по тестированию программного обеспечения"
            },
            new()
            {
                Title = "HTML/CSS",
                Description = "Вопросы по вёрстке и стилизации веб-страниц"
            },
            new()
            {
                Title = "Базы данных",
                Description = "Общие вопросы по проектированию и работе с базами данных"
            },
            new()
            {
                Title = "Linux",
                Description = "Вопросы по операционной системе Linux и администрированию"
            },
            new()
            {
                Title = "Безопасность",
                Description = "Вопросы по информационной безопасности и защите данных"
            }
        ];

        if (_dbContext.Tags.Any()) return;

        _dbContext.Tags.AddRange(tags);
        await _dbContext.SaveChangesAsync();

        tags = await _dbContext.Tags.ToListAsync();

        var urls = tags.Select(t => $"/api/tags/{t.Id}").ToList();

        var entries = tags.Zip(urls, (tag, url) => new IndexEntry
        {
            Id = tag.Id,
            Title = tag.Title,
            Type = nameof(Tag),
            Url = url ?? throw new ArgumentException(nameof(url))
        }).ToList();

       await _client.IndexManyAsync(entries);
    }

    public async Task SeedComplaintCategories()
    {
        List<QuestionComplaintCategory> questionComplaintCategories =
        [
            new()
            {
                Title = "Это вообще не вопрос"
            },
            new()
            {
                Title = "Вопрос содержит оскорбления"
            },
            new()
            {
                Title = "Вопрос нарушает правила сообщества"
            },
            new()
            {
                Title = "Вопрос содержит спам или рекламу"
            },
            new()
            {
                Title = "Вопрос не по теме"
            },
            new()
            {
                Title = "Другое"
            }
        ];

        List<AnswerComplaintCategory> answerComplaintCategories =
        [
            new()
            {
                Title = "Это вообще не ответ"
            },
            new()
            {
                Title = "Ответ содержит оскорбления"
            },
            new()
            {
                Title = "Ответ не соответствует вопросу"
            },
            new()
            {
                Title = "Ответ содержит спам или рекламу"
            },
            new()
            {
                Title = "Ответ нарушает правила сообщества"
            },
            new()
            {
                Title = "Другое"
            }
        ];

        if (!_dbContext.QuestionComplaintCategories.Any())
            _dbContext.QuestionComplaintCategories.AddRange(questionComplaintCategories);

        if (!_dbContext.AnswerComplaintCategories.Any())
            _dbContext.AnswerComplaintCategories.AddRange(answerComplaintCategories);

        await _dbContext.SaveChangesAsync();
    }
}
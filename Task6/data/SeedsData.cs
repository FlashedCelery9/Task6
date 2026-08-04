using Bogus;
using Task6.Models;

namespace Task6.data;

public class SeedsData
{
    public static void Initialize(MeetingsDBContext context)
    {

        // Якщо дані вже є — не додаємо
        if (context.Meetings.Any()) return;

        var faker = new Faker<Meeting>()
                .RuleFor(m => m.Title, f => f.Commerce.ProductName())
                .RuleFor(m => m.Description, f => f.Lorem.Sentence())
                .RuleFor(m => m.StartTime, f => f.Date.Future()) // правильний DateTime
            ;
        var meetings = faker.Generate(50);

        context.Meetings.AddRange(meetings);
        context.SaveChanges();            
    }
}
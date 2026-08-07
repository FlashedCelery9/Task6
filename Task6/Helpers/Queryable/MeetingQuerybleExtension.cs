using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Task6.DTO_s.Clients;
using Task6.Helpers.QueryParameters;
using Task6.Models;

namespace Task6.Helpers.Queryable;

public static class MeetingQueryableExtension
{
    public static IQueryable<Meeting> ApplyFilters(this IQueryable<Meeting> query, MeetingQueryParameters parameters)
    {
        // Фільтр: тільки StartTime
        if (!string.IsNullOrWhiteSpace(parameters.StartTime) &&
            string.IsNullOrWhiteSpace(parameters.EndTime))
        {
            DateTime startTime = DateTime.Parse(parameters.StartTime);
            query = query.Where(m => m.StartTime.Date == startTime.Date);
        }

        // Фільтр: пошук по слову
        if (!string.IsNullOrWhiteSpace(parameters.Search_word))
        {
            string word = parameters.Search_word.ToLower();
            query = query.Where(m => m.Description != null &&
                                     m.Description.ToLower().Contains(word));
        }

        // Фільтр: StartTime + EndTime
        if (!string.IsNullOrWhiteSpace(parameters.StartTime) &&
            !string.IsNullOrWhiteSpace(parameters.EndTime))
        {
            DateTime startTime = DateTime.Parse(parameters.StartTime);
            DateTime endTime = DateTime.Parse(parameters.EndTime);

            if (startTime > endTime)
            {
                // ❗ Повертаємо порожній набір, але НЕ null і НЕ весь список
                return query.Where(m => false);
            }

            query = query.Where(m => m.StartTime >= startTime &&
                                     m.StartTime <= endTime);
        }

        return query;
    }

    
    public static IQueryable<Meeting> ApplySort(this IQueryable<Meeting> query, MeetingQueryParameters parameters){
     
            return parameters.Sort switch
            {
                "start_time_desc" => query.OrderBy(m => m.StartTime),
                _ => query.OrderBy(m => m.Id)

            };
    }
}
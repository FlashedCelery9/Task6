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
        if (parameters.StartTime != null && parameters.EndTime == null)
        {
            query = query.Where(m => m.StartTime.Date == parameters.StartTime);
        }
        if (parameters.Search_word != null)
        {
            query = query.Where(m => m.Description != null &&
                             m.Description.ToLower().Contains(parameters.Search_word));
            
        }

        if (!parameters.StartTime.Equals(null) && !parameters.EndTime.Equals(null))
        {
            if (parameters.StartTime < parameters.EndTime)
            {
                return null;
            }
            query = query.Where(m => m.StartTime >= parameters.StartTime && m.StartTime <= parameters.EndTime);
            
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
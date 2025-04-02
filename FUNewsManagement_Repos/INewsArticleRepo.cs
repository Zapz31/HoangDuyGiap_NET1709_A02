using FUNewsManagement_BOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FUNewsManagement_DAOs.NewsArticleDAO;

namespace FUNewsManagement_Repos
{
    public interface INewsArticleRepo
    {
        Task<NewsArticleResponse> GetList(string searchTerm, int pageIndex, int pageSize, int roleID);
        Task AddNewsArticle(NewsArticle newsArticle, List<Tag> tagsAttach);
        Task UpdateNewsAricle(NewsArticle newsArticle);
        Task<NewsArticle> GetNewsArticleById(string id);
        Task DeleteNewsArticle(string id);
        Task<List<NewsReportStatistic>> GetReportStatisticsAsync(DateTime startDate, DateTime endDate);
        Task<String> GetNextNewsArticleID();
        public Task<List<NewsArticleDto>> GetNewsArticlesByAccount(short accountId);
    }
}

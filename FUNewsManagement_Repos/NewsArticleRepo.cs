using FUNewsManagement_BOs;
using FUNewsManagement_DAOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FUNewsManagement_DAOs.NewsArticleDAO;

namespace FUNewsManagement_Repos
{
    public class NewsArticleRepo : INewsArticleRepo
    {
        public Task AddNewsArticle(NewsArticle newsArticle, List<Tag> tagsAttach)
        {
            return NewsArticleDAO.Instance.AddNewsArticle(newsArticle, tagsAttach);
        }

        public Task<NewsArticleDAO.NewsArticleResponse> GetList(string searchTerm, int pageIndex, int pageSize, int roleID)
        {
            return NewsArticleDAO.Instance.GetList(searchTerm, pageIndex, pageSize, roleID);
        }

        public Task UpdateNewsAricle(NewsArticle newsArticle)
        {
            return NewsArticleDAO.Instance.UpdateNewsAricle(newsArticle);
        }
        public Task<NewsArticle> GetNewsArticleById(string id)
        {
            return NewsArticleDAO.Instance.GetNewsArticleById(id);
        }

        public Task DeleteNewsArticle(string id)
        {
            return NewsArticleDAO.Instance.DeleteNewsArticle(id);
        }

        public async Task<List<NewsReportStatistic>> GetReportStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            var reportStatistics = NewsArticleDAO.Instance.GetReportStatistics(startDate, endDate);

            return await reportStatistics
                .OrderByDescending(r => r.TotalArticles)
                .Select(r => new NewsReportStatistic
                {
                    CategoryName = r.CategoryName,
                    TotalArticles = r.TotalArticles,
                    EarliestArticleDate = r.EarliestArticleDate,
                    LatestArticleDate = r.LatestArticleDate,
                    TopTags = r.TopTags
                })
                .ToListAsync();
        }

        public Task<String> GetNextNewsArticleID()
        {
            return NewsArticleDAO.Instance.GetNextNewsArticleIdAsync();
        }

        public Task<List<NewsArticleDto>> GetNewsArticlesByAccount(short accountId)
        {
            return NewsArticleDAO.Instance.GetNewsArticlesByAccount(accountId);
        }
    }
}

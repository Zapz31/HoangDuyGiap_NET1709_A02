using FUNewsManagement_BOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagement_DAOs
{
    public class NewsArticleDAO
    {
        private readonly FunewsManagementContext _context;
        private static NewsArticleDAO instance = null;


        private NewsArticleDAO()
        {
            _context = new FunewsManagementContext();
        }

        public static NewsArticleDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    return new NewsArticleDAO();
                }
                return instance;
            }
        }

        public async Task<NewsArticleResponse> GetList(string searchTerm, int pageIndex, int pageSize, int roleID)
        {
            var query = _context.NewsArticles.Include(x => x.Tags).Include(x => x.CreatedBy).Include(x => x.Category).OrderByDescending(x => x.CreatedDate).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                if (roleID == 2)
                {
                    // execute condition 1
                    query = query.Where(x =>
                        (x.NewsTitle.ToLower().Contains(searchTerm.ToLower()) ||
                         x.Headline.ToLower().Contains(searchTerm.ToLower()))
                        && x.NewsStatus == true);
                }
                else
                {
                    // execute condition 2
                    query = query.Where(x =>
                        x.NewsTitle.ToLower().Contains(searchTerm.ToLower()) ||
                        x.Headline.ToLower().Contains(searchTerm.ToLower()));
                }
            }
            else
            {
                if (roleID == 2)
                {
                    // execute condition 3
                    query = query.Where(x => x.NewsStatus == true);
                }
            }

            int count = await query.CountAsync(); //11
            int totalPages = (int)Math.Ceiling(count / (double)pageSize); //3
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new NewsArticleResponse
            {
                NewsArticles = await query.ToListAsync(),
                TotalPages = totalPages,
                PageIndex = pageIndex
            };
        }

        public class NewsArticleResponse
        {
            public List<NewsArticle> NewsArticles { get; set; }
            public int TotalPages { get; set; }
            public int PageIndex { get; set; }
        }

        public async Task<NewsArticle> GetNewsArticleById(string id)
        {
            return await _context.NewsArticles.Include(m => m.CreatedBy).Include(m => m.Category).Include(m => m.Tags).FirstOrDefaultAsync(m => m.NewsArticleId.Equals(id));
        }

        public async Task AddNewsArticle(NewsArticle newsArticle, List<Tag> tagsAttach)
        {
            try
            {
                var existingNewsArticle = await GetNewsArticleById(newsArticle.NewsArticleId);
                if (existingNewsArticle != null)
                {
                    throw new Exception("NewsArticle already exists");
                }
                var category = await CategoryDAO.Instance.getCategoryById(newsArticle.CategoryId);
                if (category == null)
                {
                    throw new Exception("Category does not exist");
                }
                var createdBy = await _context.SystemAccounts.AsNoTracking().FirstOrDefaultAsync(x => x.AccountId.Equals(newsArticle.CreatedById));
                if (createdBy == null)
                {
                    throw new Exception("Category does not exist");
                }
                newsArticle.Tags = new List<Tag>();
                foreach (var tag in tagsAttach)
                {
                    _context.Attach(tag); // Đảm bảo EF không insert mới
                    newsArticle.Tags.Add(tag);
                }
                _context.NewsArticles.Add(newsArticle); 
                await _context.SaveChangesAsync();
            }

            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task UpdateNewsAricle(NewsArticle newsArticleUpdate)
        {
            try
            {
                var existingNewsArticle = await GetNewsArticleById(newsArticleUpdate.NewsArticleId);
                if (existingNewsArticle == null)
                {
                    throw new Exception("News Article not exist");
                }

                existingNewsArticle.NewsTitle = newsArticleUpdate.NewsTitle;
                existingNewsArticle.Headline = newsArticleUpdate.Headline;
                existingNewsArticle.NewsContent = newsArticleUpdate.NewsContent;
                existingNewsArticle.NewsSource = newsArticleUpdate.NewsSource;
                existingNewsArticle.CategoryId = newsArticleUpdate.CategoryId;
                var updateBy = await _context.SystemAccounts.FirstOrDefaultAsync(s => s.AccountId == newsArticleUpdate.UpdatedById);
                if (updateBy == null)
                {
                    throw new Exception("Update by does not exist");
                }
                existingNewsArticle.UpdatedById = newsArticleUpdate.UpdatedById;
                existingNewsArticle.NewsStatus = newsArticleUpdate.NewsStatus;
                existingNewsArticle.ModifiedDate = newsArticleUpdate?.ModifiedDate;

                _context.NewsArticles.Update(existingNewsArticle);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task DeleteNewsArticle(string id)
        {
            try
            {
                var existNewsArticle = _context.NewsArticles.Include(m => m.Tags).FirstOrDefault(m => m.NewsArticleId == id);
                if (existNewsArticle == null)
                {
                    throw new Exception("Art not found");
                }
                existNewsArticle.Tags.Clear();
                await _context.SaveChangesAsync();

                _context.NewsArticles.Remove(existNewsArticle);
                await _context.SaveChangesAsync();

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IQueryable<NewsReportStatistic> GetReportStatistics(DateTime startDate, DateTime endDate)
        {
            return _context.NewsArticles
                // Additional null and status checks
                .Where(na => na.CreatedDate.HasValue &&
                             na.CreatedDate.Value >= startDate &&
                             na.CreatedDate.Value <= endDate &&
                             na.Category != null &&
                             na.NewsStatus == true)  // Assuming active news articles

                .GroupBy(na => na.Category.CategoryName)

                .Select(g => new NewsReportStatistic
                {
                    CategoryName = g.Key ?? "Uncategorized",

                    TotalArticles = g.Count(),

                    EarliestArticleDate = g.Min(na => na.CreatedDate) ?? DateTime.MinValue,

                    LatestArticleDate = g.Max(na => na.CreatedDate) ?? DateTime.MaxValue,

                    TopTags = g.SelectMany(na => na.Tags)
                               .Where(t => !string.IsNullOrWhiteSpace(t.TagName))
                               .GroupBy(t => t.TagName)
                               .OrderByDescending(t => t.Count())
                               .Take(3)
                               .Select(t => t.Key)
                               .ToList()
                });
        }

        public class NewsReportStatistic
        {
            public string CategoryName { get; set; }
            public int TotalArticles { get; set; }
            public DateTime? EarliestArticleDate { get; set; }
            public DateTime? LatestArticleDate { get; set; }
            public List<string> TopTags { get; set; }
        }

        public async Task<string> GetNextNewsArticleIdAsync()
        {
            int maxId = await _context.NewsArticles
            .Where(na => EF.Functions.Like(na.NewsArticleId, "%[^0-9]%") == false) // Ensure only numeric values
            .Select(na => (int?)Convert.ToInt32(na.NewsArticleId)) // Convert to int
            .MaxAsync() ?? 0; // Default to 0 if no valid IDs

            int nextId = maxId + 1;
            return nextId.ToString();
        }

        public class NewsArticleDto
        {
            public string? NewsTitle { get; set; }
            public string Headline { get; set; } = string.Empty;
            public DateTime? CreatedDate { get; set; }
            public string? NewsContent { get; set; }
            public string? NewsSource { get; set; }
            public string CategoryName { get; set; } = string.Empty;
            public bool? NewsStatus { get; set; }
            public List<string?> Tags { get; set; } = new List<string?>();
        }

        public async Task<List<NewsArticleDto>> GetNewsArticlesByAccount(short accountId)
        {
                var articles = await _context.NewsArticles
                    .Where(na => na.CreatedById == accountId)
                    .Select(na => new NewsArticleDto
                    {
                        NewsTitle = na.NewsTitle,
                        Headline = na.Headline,
                        CreatedDate = na.CreatedDate,
                        NewsContent = na.NewsContent,
                        NewsSource = na.NewsSource,
                        NewsStatus = na.NewsStatus,
                        // Retrieve the CategoryName via the navigation property (if exists)
                        CategoryName = na.Category != null ? na.Category.CategoryName : string.Empty,
                        // Get the Tag names associated with the article
                        Tags = na.Tags.Select(t => t.TagName).ToList()
                    })
                    .ToListAsync();

                return articles;
        }
    }
}

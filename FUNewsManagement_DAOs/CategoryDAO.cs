using FUNewsManagement_BOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FUNewsManagement_DAOs.SystemAccountDAO;

namespace FUNewsManagement_DAOs
{
    public class CategoryDAO
    {
        private readonly FunewsManagementContext _context;
        private static CategoryDAO instance = null;

        private CategoryDAO()
        {
            _context = new FunewsManagementContext();
        }

        public static CategoryDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CategoryDAO();
                }
                return instance;
            }
        }

        public async Task<List<Category>> GetList()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category?> getCategoryById(short? id)
        {
            return await _context.Categories.FirstOrDefaultAsync(m => m.CategoryId == id);
        }

        public async Task<Category?> getCategoryByName(string name)
        {
            return await _context.Categories.FirstOrDefaultAsync(m => m.CategoryName.ToLower().Equals(name.ToLower()));
        }

        public async Task<CategoriesResponse> GetListWithPagination(string searchTerm, int pageIndex, int pageSize)
        {
            var query = _context.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x => x.CategoryName.ToLower().Contains(searchTerm.ToLower()) || x.CategoryDesciption.ToLower().Contains(searchTerm.ToLower()));
            }

            int count = await query.CountAsync(); //11
            int totalPages = (int)Math.Ceiling(count / (double)pageSize); //3

            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new CategoriesResponse
            {
                Categories = await query.ToListAsync(),
                TotalPages = totalPages,
                PageIndex = pageIndex
            };
        }

        public class CategoriesResponse
        {
            public List<Category> Categories { get; set; }
            public int TotalPages { get; set; }
            public int PageIndex { get; set; }
        }

        public async Task AddCategory(Category category)
        {
            try
            {
                var existingCategory = await getCategoryByName(category.CategoryName);
                if (existingCategory != null)
                {
                    throw new Exception("Category with this name has already exist");
                }
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<short> GetNextCategoryIdAsync()
        {
            var latestCategoryId = await _context.Categories
                .OrderByDescending(c => c.CategoryId)
                .Select(c => c.CategoryId)
                .FirstOrDefaultAsync();

            return (short)(latestCategoryId + 1);
        }

        public async Task UpdateCategory(Category category)
        {
            try
            {
               Category? existingCategory = await getCategoryById(category.CategoryId);
                if (existingCategory == null)
                {
                    throw new Exception("Does not exist");
                }
                existingCategory.CategoryName = category.CategoryName;
                existingCategory.CategoryDesciption = category.CategoryDesciption;
                existingCategory.IsActive = category.IsActive;
                _context.Categories.Update(existingCategory);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task DeleteCategory(short id)
        {
            try
            {
                var existCategory = _context.Categories.Include(m => m.NewsArticles).FirstOrDefault(m => m.CategoryId == id);
                if (existCategory == null)
                {
                    throw new Exception("Category not found");
                }
                if (existCategory.NewsArticles.Any())
                {
                    throw new Exception("Cannot delete category because it has associated NewsArticles.");
                }

                _context.Categories.Remove(existCategory);
                await _context.SaveChangesAsync();

            }
            catch (Exception e)
            {
                throw e;
            }
        }


    }
}

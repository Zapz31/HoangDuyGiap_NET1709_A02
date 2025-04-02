using FUNewsManagement_BOs;
using FUNewsManagement_DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FUNewsManagement_DAOs.CategoryDAO;

namespace FUNewsManagement_Repos
{
    public class CategoryRepo : ICategoryRepo
    {
        public Task AddCategory(Category category)
        {
            return CategoryDAO.Instance.AddCategory(category);
        }

        public Task<List<Category>> GetList()
        {
            return CategoryDAO.Instance.GetList();
        }

        public Task<CategoriesResponse> GetListWithPagination(string searchTerm, int pageIndex, int pageSize)
        {
            return CategoryDAO.Instance.GetListWithPagination(searchTerm, pageIndex, pageSize);
        }

        public Task<short> GetNextCategoryIdAsync()
        {
            return CategoryDAO.Instance.GetNextCategoryIdAsync();
        }

        public Task UpdateCategory(Category category)
        {
            return CategoryDAO.Instance.UpdateCategory(category);
        }

        public Task<Category?> getCategoryById(short? id)
        {
            return CategoryDAO.Instance.getCategoryById(id);
        }

        public Task DeleteCategory(short id)
        {
            return CategoryDAO.Instance.DeleteCategory(id);
        }
    }
}

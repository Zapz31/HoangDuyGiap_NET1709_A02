using FUNewsManagement_BOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FUNewsManagement_DAOs.CategoryDAO;

namespace FUNewsManagement_Repos
{
    public interface ICategoryRepo
    {
        Task<List<Category>> GetList();
        Task<CategoriesResponse> GetListWithPagination(string searchTerm, int pageIndex, int pageSize);
        Task AddCategory(Category category);
        Task<short> GetNextCategoryIdAsync();
        Task UpdateCategory(Category category);
        Task<Category?> getCategoryById(short? id);
        Task DeleteCategory(short id);
    }
}

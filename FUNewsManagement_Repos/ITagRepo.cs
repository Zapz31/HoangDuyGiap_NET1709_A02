using FUNewsManagement_BOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagement_Repos
{
    public interface ITagRepo
    {
        Task<List<Tag>> GetList();
        Task<List<Tag>> GetTagsByIds(List<int> tagIds);
    }
}

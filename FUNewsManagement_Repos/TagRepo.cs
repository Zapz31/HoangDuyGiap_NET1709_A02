using FUNewsManagement_BOs;
using FUNewsManagement_DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagement_Repos
{
    public class TagRepo : ITagRepo
    {
        public Task<List<Tag>> GetList()
        {
            return TagDAO.Instance.GetList();
        }

        public Task<List<Tag>> GetTagsByIds(List<int> tagIds)
        {
            return TagDAO.Instance.GetTagsByIds(tagIds);
        }
    }
}

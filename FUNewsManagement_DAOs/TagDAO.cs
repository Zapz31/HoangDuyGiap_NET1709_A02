using FUNewsManagement_BOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagement_DAOs
{
    public class TagDAO
    {
        private readonly FunewsManagementContext _context;
        private static TagDAO instance = null;

        private TagDAO()
        {
            _context = new FunewsManagementContext();
        }

        public static TagDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TagDAO();
                }
                return instance;
            }
        }

        public async Task<List<Tag>> GetList()
        {
            return await _context.Tags.ToListAsync();
        }

        public async Task<List<Tag>> GetTagsByIds(List<int> tagIds)
        {
            return await _context.Tags.Where(t => tagIds.Contains(t.TagId)).ToListAsync();
        }
    }
}

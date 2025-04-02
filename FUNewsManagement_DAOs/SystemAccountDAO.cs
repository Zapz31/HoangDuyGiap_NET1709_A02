using FUNewsManagement_BOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagement_DAOs
{
	public class SystemAccountDAO
	{
        private readonly FunewsManagementContext _context;
        private static SystemAccountDAO instance = null;

        private SystemAccountDAO()
        {
            _context = new FunewsManagementContext();
        }

        public static SystemAccountDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SystemAccountDAO();
                }
                return instance;
            }
        }

        public async Task<SystemAccount?> Login(string email, string password)
        {
            return await _context.SystemAccounts.FirstOrDefaultAsync(m => m.AccountEmail.Equals(email) && m.AccountPassword.Equals(password));
        }

        public async Task<List<SystemAccount>> GetList()
        {
            return await _context.SystemAccounts.ToListAsync();
        }

        public async Task<SystemAccount> GetAccountById(int id)
        {
            return await _context.SystemAccounts.FirstOrDefaultAsync(m => m.AccountId == id);
        }

        public async Task<SystemAccount> GetAccountByEmail(string email)
        {
            return await _context.SystemAccounts.FirstOrDefaultAsync(m => m.AccountEmail.Equals(email));
        }

        public async Task<SystemAccountsResponse> GetList(string searchTerm, int pageIndex, int pageSize)
        {
            var query = _context.SystemAccounts.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x => x.AccountName.ToLower().Contains(searchTerm.ToLower()) || x.AccountEmail.ToLower().Contains(searchTerm.ToLower()));
            }

            int count = await query.CountAsync(); //11
            int totalPages = (int)Math.Ceiling(count / (double)pageSize); //3

            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new SystemAccountsResponse
            {
                SystemAccounts = await query.ToListAsync(),
                TotalPages = totalPages,
                PageIndex = pageIndex
            };
        }

        public class SystemAccountsResponse
        {
            public List<SystemAccount> SystemAccounts { get; set; }
            public int TotalPages { get; set; }
            public int PageIndex { get; set; }
        }

        public async Task UpdateSystemAccount(SystemAccount systemAccountReq)
        {
            try
            {
                

                var existingAccount = await GetAccountById(systemAccountReq.AccountId);
                if (existingAccount == null)
                {
                    throw new Exception("Does not exist");
                }

                var emailAccount = await GetAccountByEmail(systemAccountReq.AccountEmail);
                if (emailAccount != null && emailAccount.AccountId != existingAccount.AccountId)
                {
                    throw new Exception("Email has already exists");
                }

                existingAccount.AccountName = systemAccountReq.AccountName;
                existingAccount.AccountEmail = systemAccountReq.AccountEmail;
                existingAccount.AccountRole = systemAccountReq.AccountRole;
                existingAccount.AccountPassword = systemAccountReq.AccountPassword;

                

                _context.SystemAccounts.Update(existingAccount);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task AddAccount(SystemAccount systemAccountCreate)
        {
            try
            {
                var existingSystemAccount = await GetAccountByEmail(systemAccountCreate.AccountEmail);
                if (existingSystemAccount != null)
                {
                    throw new Exception("Email has already exists ");
                }
                
                _context.SystemAccounts.Add(systemAccountCreate);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<short> GetNextAccountIdAsync()
        {
            var latestAccountId = await _context.SystemAccounts
                .OrderByDescending(a => a.AccountId)
                .Select(a => a.AccountId)
                .FirstOrDefaultAsync();

            return (short)(latestAccountId + 1);
        }

        public async Task DeleteAccount(short id)
        {
            try
            {
                var existAccount = _context.SystemAccounts
                    .Include(sa => sa.NewsArticles)
                    .FirstOrDefault(sa => sa.AccountId == id);
                if (existAccount == null)
                {
                    throw new Exception("Art not found");
                }
                var newsArticles = existAccount.NewsArticles.ToList();
                if (newsArticles.Any())
                {
                    throw new Exception("This account has news article in system, so cannot be deleted !");
                }


                _context.SystemAccounts.Remove(existAccount);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}

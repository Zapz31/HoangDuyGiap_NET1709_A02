using FUNewsManagement_BOs;
using FUNewsManagement_DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagement_Repos
{
    public interface ISystemAccountRepo
    {
        Task<SystemAccount?> Login(string username, string password);
        Task<List<SystemAccount>> GetList();
        Task<SystemAccount> GetAccountByID(int id);
        Task<SystemAccountDAO.SystemAccountsResponse> GetList(string searchTerm, int pageIndex, int pageSize);
        Task UpdatePainting(SystemAccount systemAccount);
        Task CreateAccount(SystemAccount systemAccount);
        Task<short> GetNextAccountId();
        Task DeleteAccount(short id);
    }
}


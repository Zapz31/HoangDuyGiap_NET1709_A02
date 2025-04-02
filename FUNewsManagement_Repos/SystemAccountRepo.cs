using FUNewsManagement_BOs;
using FUNewsManagement_DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagement_Repos
{
    public class SystemAccountRepo : ISystemAccountRepo
    {
        public Task<SystemAccount?> Login(string username, string password)
        {
            return SystemAccountDAO.Instance.Login(username, password);
        }

        public Task<List<SystemAccount>> GetList()
        {
            return SystemAccountDAO.Instance.GetList();
        }

        public Task<SystemAccount> GetAccountByID(int id)
        {
            return SystemAccountDAO.Instance.GetAccountById(id);
        }

        public Task<SystemAccountDAO.SystemAccountsResponse> GetList(string searchTerm, int pageIndex, int pageSize)
        {
            return SystemAccountDAO.Instance.GetList(searchTerm, pageIndex, pageSize);
        }

        public Task UpdatePainting(SystemAccount systemAccount)
        {
            return SystemAccountDAO.Instance.UpdateSystemAccount(systemAccount);
        }

        public Task CreateAccount(SystemAccount systemAccount)
        {
            return SystemAccountDAO.Instance.AddAccount(systemAccount);
        }

        public Task<short> GetNextAccountId()
        {
            return SystemAccountDAO.Instance.GetNextAccountIdAsync();
        }

        public Task DeleteAccount(short id)
        {
            return SystemAccountDAO.Instance.DeleteAccount(id);
        }

    }
}

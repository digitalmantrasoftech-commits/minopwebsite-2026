using PayTimeWebClient.Models;
using System.Collections.Generic;
using System.Data;

namespace PayTimeWebClient.Helper
{

    public interface IAccountRestClint
    {
        MRespo Register(LoginModel model);
        MRespo Login(LoginModel model);
        MRespo EmpLogin(LoginModel model);
        MRespo SubDomainLogin(LoginModel model);
        MRespo DeveloperLogin(LoginModel model);
        MRespo GetEmailPassword();
    }
    public interface IMasterRepository<T>
    {
        T Get(int id);
        IEnumerable<T> GetAll();
        T Add(T serverData);
        bool Delete(T serverData);
        bool Update(T serverData);
    }

   


}

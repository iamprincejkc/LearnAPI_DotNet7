using LearnAPI.DB.Models;
using LearnAPI.Helper;
using LearnAPI.Model;

namespace LearnAPI.Services
{
    public interface ICustomerService
    {
        Task<List<CustomerModel>> GetAll();
        Task<CustomerModel> GetByCode(int code);
        Task<APIResponse> RemoveByCode(int code);
        Task<APIResponse> Create(CustomerModel customerModel);
        Task<APIResponse> Update(CustomerModel customerModel);
    }
}

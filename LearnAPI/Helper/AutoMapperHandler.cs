using AutoMapper;
using LearnAPI.DB.Models;
using LearnAPI.Model;

namespace LearnAPI.Helper
{
    public class AutoMapperHandler : Profile
    {
        public AutoMapperHandler()
        {
            CreateMap<TblCustomer, CustomerModel>().ForMember(item => item.StatusName,
                opt => opt.MapFrom(
                    item => (item.IsActive ?? false && item.IsActive.Value) ? "Active" : "InActive")).ReverseMap();

        }
    }
}

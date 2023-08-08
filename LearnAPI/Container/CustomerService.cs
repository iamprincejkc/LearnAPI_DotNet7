using AutoMapper;
using LearnAPI.DB;
using LearnAPI.DB.Models;
using LearnAPI.Helper;
using LearnAPI.Model;
using LearnAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace LearnAPI.Container
{
    public class CustomerService : ICustomerService
    {
        private readonly LearnApiDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerService> _logger;
        public int MyProperty { get; set; }
        public CustomerService(LearnApiDbContext context, IMapper mapper,ILogger<CustomerService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;

        }
        public async Task<List<CustomerModel>> GetAll()
        {
            List<CustomerModel> customerModels = new();
            var _data = await this._context.TblCustomers.ToListAsync();
            if (_data != null)
            {
                customerModels = this._mapper.Map<List<TblCustomer>, List<CustomerModel>>(_data);
            }
            return customerModels;
        }

        public async Task<CustomerModel> GetByCode(int code)
        {
            CustomerModel customerModels = new();
            var _data = await this._context.TblCustomers.FindAsync(code);
            if (_data != null)
            {
                customerModels = this._mapper.Map<TblCustomer, CustomerModel>(_data);
            }
            return customerModels;
        }

        public async Task<APIResponse> RemoveByCode(int code)
        {
            APIResponse apiResponse = new();
            try
            {
                var data = await _context.TblCustomers.FindAsync(code);
                if (data != null)
                {
                    _context.TblCustomers.Remove(data);
                    await _context.SaveChangesAsync();

                    apiResponse.ResponseCode = 201;
                    apiResponse.Data = code.ToString();
                    apiResponse.Message = "Success";
                }
                else
                {
                    apiResponse.ResponseCode = 404;
                    apiResponse.Message = "Data Not Found";
                }
            }
            catch (Exception ex)
            {
                apiResponse.ResponseCode = 400;
                apiResponse.Data = "";
                apiResponse.Message = ex.Message;   
            }
            return apiResponse;
        }

        public async Task<APIResponse> Create(CustomerModel customerModel)
        {
            APIResponse apiResponse = new();
            try
            {
                _logger.LogInformation("Create Customer Starting");
                TblCustomer _customer = this._mapper.Map<CustomerModel, TblCustomer>(customerModel);
                _context.TblCustomers.AddAsync(_customer);
                await _context.SaveChangesAsync();
                apiResponse.ResponseCode = 201;
                apiResponse.Data = customerModel.Code.ToString();
                apiResponse.Message = "Success";
            }
            catch (Exception ex)
            {
                apiResponse.ResponseCode = 400;
                apiResponse.Data = "";
                apiResponse.Message = ex.Message;
                _logger.LogError(ex,"Error in Create Customer");
            }
            return apiResponse;
        }

        public async Task<APIResponse> Update(CustomerModel customerModel)
        {
            APIResponse apiResponse = new();
            try
            {
                var data = await _context.TblCustomers.FindAsync(customerModel.Code);
                if (data != null)
                {
                    _context.Entry(data).CurrentValues.SetValues(customerModel);
                    await _context.SaveChangesAsync();

                    apiResponse.ResponseCode = 201;
                    apiResponse.Data = customerModel.Code.ToString();
                    apiResponse.Message = "Success";
                }
                else
                {
                    apiResponse.ResponseCode = 404;
                    apiResponse.Message = "Data Not Found";
                }
            }
            catch (Exception ex)
            {
                apiResponse.ResponseCode = 400;
                apiResponse.Data = "";
                apiResponse.Message = ex.Message;
            }
            return apiResponse;
        }
    }
}

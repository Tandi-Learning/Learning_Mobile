using GloboTicket.Admin.Mobile.Models;

namespace GloboTicket.Admin.Mobile.Services;

public interface ICategoryService
{
    Task<List<CategoryModel>> GetCategories();
}

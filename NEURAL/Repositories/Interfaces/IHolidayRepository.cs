using NEURAL.Models.Entities;
using NEURAL.Models.ViewModel;
using System.Net;

namespace NEURAL.Repositories.Interfaces
{
    public interface IHolidayRepository
    {
        Task<List<HolidaySummaryViewModel>> GetSummary();
        Task<List<HOLIDAY_T>> GetDetailByFilter(int holidayYear,long jobsiteId, string mbrVersion);
        Task<HttpStatusCode> CallHolidayApi();
    }
}

using TripMaster.Models;

namespace TripMaster.Services
{
    public interface IDataLoggerService
    {
        void SaveRun(RunRecord record);
    }
}
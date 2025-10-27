using System.Text.Json;
using TripMaster.Models;

namespace TripMaster.Services
{
    public class DataLoggerService : IDataLoggerService
    {
        private readonly List<RunRecord> _runs = new();
        public void SaveRun(RunRecord record)
        {
            _runs.Add(record);
            // Optionally persist to file
            var json = JsonSerializer.Serialize(_runs);
            // File saving omitted for brevity
        }
    }
}
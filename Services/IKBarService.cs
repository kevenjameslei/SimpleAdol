using StockAnalysiserAPI.Models;
using System.Numerics;

namespace StockAnalysiserAPI.Services
{
    public interface IKBarService
    {
        Task<List<KBar>> GetKLinesAsync(string symbol, string period, DateTime? start, DateTime? end, int limit);
    }
}

using StockAnalysiserAPI.Models;
using System.Numerics;

namespace StockAnalysiserAPI.Repositories
{
    public interface IKBarRepository
    {
        Task<List<KBar>> QueryKLinesAsync(string symbol, string period, DateTime? start, DateTime? end, int limit);
    }
}
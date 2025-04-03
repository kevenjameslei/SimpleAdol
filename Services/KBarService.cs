using StockAnalysiserAPI.Models;
using StockAnalysiserAPI.Repositories;
using StockAnalysiserAPI.Services;
using System.Numerics;

namespace KLineApi.Services
{
    public class KBarService : IKBarService
    {
        private readonly IKBarRepository _repo;

        public KBarService(IKBarRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<KBar>> GetKLinesAsync(string symbol, string period, DateTime? start, DateTime? end, int limit)
        {
            return await _repo.QueryKLinesAsync(symbol, period, start, end, limit);
        }
    }
}
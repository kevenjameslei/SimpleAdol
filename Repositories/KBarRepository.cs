using Dapper;
using StockAnalysiserAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Text;


namespace StockAnalysiserAPI.Repositories
{
    public class KLineRepository : IKBarRepository
    {
        private readonly IConfiguration _config;

        public KLineRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<List<KBar>> QueryKLinesAsync(string symbol, string period, DateTime? start, DateTime? end, int limit)
        {
            var connStr = _config.GetConnectionString("DefaultConnection");
            using var conn = new SqlConnection(connStr);

            // 查询数据库中实际存在的 KDaily_xxxx 表
            var existingTables = await conn.QueryAsync<string>(
                @"SELECT [name] FROM sys.tables WHERE [name] LIKE 'KDaily_19%' OR [name] LIKE 'KDaily_20%'"
            );

            var sb = new StringBuilder();
            foreach (var table in existingTables)
            {
                sb.AppendLine($@"
            SELECT * FROM {table} with (nolock)
            WHERE Id = @Symbol
            {(start != null ? "AND TradeDate >= @Start" : "")}
            {(end != null ? "AND TradeDate <= @End" : "")}
            UNION ALL
        ");
            }

            if (sb.Length == 0)
                return new List<KBar>();

            // 移除最后的 UNION ALL
            var unionSql = sb.ToString();
            unionSql = unionSql.Substring(0, unionSql.LastIndexOf("UNION ALL"));

            // 包装外层：排序 + Limit
            var finalSql = $@"
        SELECT TOP (@Limit) * FROM (
            {unionSql}
        ) AS AllData
        ORDER BY TradeDate DESC";

            var result = (await conn.QueryAsync<KBar>(finalSql, new
            {
                Symbol = symbol,
                Period = period,
                //Timestamp = TradeDate,
                Start = start,
                End = end,
                Limit = limit
            })).ToList();

            result.ForEach(bar => bar.KLevel = KBarLevel.Day);

            return result;
        }
    }
}
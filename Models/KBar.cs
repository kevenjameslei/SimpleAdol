namespace StockAnalysiserAPI.Models
{
    // K线数据结构
    public class KBar
    {
        public DateTime TradeDate { get; set; }     // 时间
        public string Id { get; set; }              // 股票代码
        public KBarLevel KLevel { get; set; }      // K线级别
        public double OpenPrice { get; set; }       // 开盘价
        public double HighPrice { get; set; }       // 最高价
        public double LowPrice { get; set; }        // 最低价
        public double ClosePrice { get; set; }      // 收盘价
        public float PreClosePrice { get; set; }    // 前收盘价
        public float Change { get; set; }           // 涨跌金额
        public float Percent_Change { get; set; }   // 涨跌幅
        public long Volumn { get; set; }            // 成交量
        public float Amount { get; set; }           // 成交金额
        public bool IsMerged { get; set; }          // 是否被合并处理
    }


}
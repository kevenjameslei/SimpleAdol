using StockAnalysiserAPI.Models;
using System.Numerics;

namespace StockAnalysiserAPI
{
    // 分型结构
    public class Fractal
    {
        public FractalType Type { get; set; }  // 顶分型/底分型
        public int Index { get; set; }         // 在合并后K线序列中的位置
        public KBar AnchorKBar { get; set; } // 分型锚定K线（中间K线）
    }
}
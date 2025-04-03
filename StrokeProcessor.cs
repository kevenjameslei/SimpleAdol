using StockAnalysiserAPI.Models;
using System.Numerics;

namespace StockAnalysiserAPI
{
    // 缠论处理器
    public class StrokeProcessor
    {
        private List<KBar> mergedKBars = new List<KBar>();

        // 处理包含关系并合并K线
        public void ProcessContains(List<KBar> rawKBars)
        {
            mergedKBars.Clear();
            if (rawKBars.Count == 0) return;

            KBar current = rawKBars[0];
            bool isUpTrend = true; // 初始趋势假设为上升

            for (int i = 1; i < rawKBars.Count; i++)
            {
                KBar next = rawKBars[i];

                // 判断包含关系
                if (current.HighPrice >= next.HighPrice && current.LowPrice <= next.LowPrice)
                {
                    // 合并K线
                    current = MergeKBar(current, next, isUpTrend);
                }
                else
                {
                    // 无包含关系，记录当前K线并更新趋势
                    mergedKBars.Add(current);
                    isUpTrend = next.HighPrice > current.HighPrice; // 趋势方向判断
                    current = next;
                }
            }
            mergedKBars.Add(current); // 添加最后一个K线
        }

        // 合并包含关系的K线
        private KBar MergeKBar(KBar a, KBar b, bool isUpTrend)
        {
            return new KBar
            {
                TradeDate = a.TradeDate,
                HighPrice = isUpTrend ? Math.Max(a.HighPrice, b.HighPrice) : Math.Min(a.HighPrice, b.HighPrice),
                LowPrice = isUpTrend ? Math.Max(a.LowPrice, b.LowPrice) : Math.Min(a.LowPrice, b.LowPrice),
                IsMerged = true
            };
        }

        // 识别分型（顶分型 & 底分型）
        public List<Fractal> DetectFractal()
        {
            List<Fractal> Fractals = new List<Fractal>();
            if (mergedKBars.Count < 3) return Fractals;

            for (int i = 1; i < mergedKBars.Count - 1; i++)
            {
                KBar prev = mergedKBars[i - 1];
                KBar current = mergedKBars[i];
                KBar next = mergedKBars[i + 1];

                // 顶分型条件
                if (current.HighPrice > prev.HighPrice && current.HighPrice > next.HighPrice &&
                    current.LowPrice > prev.LowPrice && current.LowPrice > next.LowPrice)
                {
                    Fractals.Add(new Fractal
                    {
                        Type = FractalType.Top,
                        Index = i,
                        AnchorKBar = current
                    });
                }
                // 底分型条件
                else if (current.LowPrice < prev.LowPrice && current.LowPrice < next.LowPrice &&
                         current.HighPrice < prev.HighPrice && current.HighPrice < next.HighPrice)
                {
                    Fractals.Add(new Fractal
                    {
                        Type = FractalType.Bottom,
                        Index = i,
                        AnchorKBar = current
                    });
                }
            }
            return FilterInvalidFractal(Fractals);
        }

        // 过滤无效分型（示例简化）
        private List<Fractal> FilterInvalidFractal(List<Fractal> raw)
        {
            List<Fractal> valid = new List<Fractal>();
            for (int i = 0; i < raw.Count; i++)
            {
                // 简单示例：至少间隔2根K线
                if (i > 0 && raw[i].Index - raw[i - 1].Index < 2) continue;
                valid.Add(raw[i]);
            }
            return valid;
        }

        // 生成笔（简化的笔识别）
        public List<Tuple<int, int>> GenerateStoke(List<Fractal> Fractals)
        {
            List<Tuple<int, int>> bis = new List<Tuple<int, int>>();
            Fractal last = null;

            foreach (var fx in Fractals)
            {
                if (last == null)
                {
                    last = fx;
                    continue;
                }

                // 笔方向判断
                if (last.Type == FractalType.Bottom && fx.Type == FractalType.Top)
                {
                    // 上升笔：底分型 → 顶分型
                    if (fx.AnchorKBar.HighPrice > last.AnchorKBar.HighPrice)
                    {
                        bis.Add(Tuple.Create(last.Index, fx.Index));
                        last = fx;
                    }
                }
                else if (last.Type == FractalType.Top && fx.Type == FractalType.Bottom)
                {
                    // 下降笔：顶分型 → 底分型
                    if (fx.AnchorKBar.LowPrice < last.AnchorKBar.LowPrice)
                    {
                        bis.Add(Tuple.Create(last.Index, fx.Index));
                        last = fx;
                    }
                }
            }
            return bis;
        }
    }
}
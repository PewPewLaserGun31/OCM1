using Microsoft.FSharp.Collections;
using Plotly.NET;
using Plotly.NET.LayoutObjects;

public class Program
{
    private static List<float> _randomValues;
    private static int _intervalsCount;

    public static void Main()
    {
        int randomValuesCount = 3000;
        _intervalsCount = 30;

        float s = 1;
        float a = 1;

        _randomValues = GenerateRandomValues(randomValuesCount, s, a);

        List<int> histogram = new List<int>(_intervalsCount);

        for (var i = 1; i <= _intervalsCount; i++)
            histogram.Add(_randomValues.Count(x => x < GetRange(i) && x > GetRange(i - 1)));

        float step = GetRange(1) - GetRange(0);
        IEnumerable<float> range = Enumerable.Range(0, histogram.Count).Select(x => (x - 12) * step);
        List<float> idealHist = range
            .Select(x => (1 / (s * MathF.Sqrt(MathF.PI * 2)) * MathF.Exp(-MathF.Pow(x - a, 2) / (2 * s * s)))).ToList();
        float histSum = idealHist.Sum();
        idealHist = idealHist.Select(x => x / histSum * randomValuesCount).ToList();


        var zgody = new List<float>();
        for (int i = 0; i < _intervalsCount; i++)
            zgody.Add(MathF.Pow(histogram[i] - idealHist[i], 2) / idealHist[i]);

        PrintValues(_randomValues.Min(), _randomValues.Max(), _randomValues.Average(), _randomValues, zgody,
            randomValuesCount);
        ShowHistogram(histogram);
    }

    private static float GetRange(int i)
    {
        return (_randomValues.Max() - _randomValues.Min()) / _intervalsCount * i + _randomValues.Min();
    }

    private static List<float> GenerateRandomValues(int randomValuesCount, float s, float a)
    {
        Random random = new Random();
        List<float> randomValues = new List<float>(randomValuesCount);

        for (int i = 0; i < randomValuesCount; i++)
            randomValues.Add(s * Enumerable.Range(1, 13).Sum(_ => random.NextSingle() - 6) + a);
        return randomValues;
    }
    
    /// <summary>
    /// Малює графік-гістограму
    /// </summary>
    /// <param name="histogram"></param>
    private static void ShowHistogram(List<int> histogram)
    {
        var layout = new Layout();
        layout.SetValue("xaxis", new LinearAxis());
        layout.SetValue("yaxis", new LinearAxis());
        layout.SetValue("plot_bgcolor", "#e5ecf6");
        layout.SetValue("showlegend", true);
        
        
        var trace = new Trace("bar");
        trace.SetValue("x", Enumerable.Range(0, histogram.Count).Select(GetRange));
        trace.SetValue("y", histogram);

        var fig = GenericChart.Figure.create(ListModule.OfSeq(new[] { trace }), layout);
        GenericChart.fromFigure(fig).Show();
    }

    /// <summary>
    /// Виводить в консоль отримані значення
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="avg"></param>
    /// <param name="randomValues"></param>
    /// <param name="zgody"></param>
    /// <param name="randomValuesCount"></param>
    private static void PrintValues(float min, float max, float avg, List<float> randomValues, IEnumerable<float> zgody,
        int randomValuesCount)
    {
        Console.WriteLine($"Мiнiмальне значення: {min}");
        Console.WriteLine($"Максимальне значення: {max}");
        Console.WriteLine($"Середнє значення: {avg}");
        Console.WriteLine(
            $"Дисперсiя: {(randomValues.Sum(x => x * x) / randomValues.Count) - (Math.Pow(randomValues.Sum() / randomValues.Count, 2))}");
        Console.WriteLine($"Критерiй згоди: {zgody.Sum() / randomValuesCount}");
    }
}
namespace GoodsHierarchy;

public class Processor
{
    private const int MinCores = 1;
    private const double MinFrequencyGHz = 0.1;

    public Processor(string model, int cores, double frequencyGHz)
    {
        Validation.NotEmpty(model, nameof(model));
        Validation.AtLeast(cores, MinCores, nameof(cores));
        Validation.AtLeast(frequencyGHz, MinFrequencyGHz, nameof(frequencyGHz));
        Model = model;
        Cores = cores;
        FrequencyGHz = frequencyGHz;
    }

    public string Model { get; }

    public int Cores { get; }

    public double FrequencyGHz { get; }

    public override string ToString() =>
        $"{Model}, {Cores} ядер, {Validation.Number(FrequencyGHz)} ГГц";
}

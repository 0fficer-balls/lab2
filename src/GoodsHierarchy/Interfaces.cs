namespace GoodsHierarchy;

// устройство с электропитанием
public interface IPowered
{
    int PowerWatts { get; }

    bool IsOn { get; }

    string TurnOn();

    string TurnOff();
}

public interface IPrinter
{
    string Print(string document, int copies);
}

public interface IScanner
{
    string Scan(string document);
}

public interface IPortable
{
    int BatteryLifeHours { get; }

    double ScreenSizeInch { get; }
}

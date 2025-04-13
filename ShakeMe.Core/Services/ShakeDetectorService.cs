using Microsoft.Maui.Devices.Sensors;

namespace ShakeMe.Core.Services;

public class ShakeDetectorService
{
    private const double ShakeThresholdX = 1.5; // Seuil uniquement sur l'axe X
    private DateTime _lastShakeTime = DateTime.MinValue;

    public event Action? ShakeDetected;

    public void Start()
    {
        Console.WriteLine("▶️ ShakeDetectorService.Start()");

        if (Accelerometer.IsMonitoring)
        {
            Console.WriteLine("⚠️ Accelerometer already running");
            return;
        }

        Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
        Accelerometer.Start(SensorSpeed.Game);
        Console.WriteLine("✅ Accelerometer started");
    }


    public void Stop()
    {
        if (Accelerometer.IsMonitoring)
        {
            Accelerometer.ReadingChanged -= Accelerometer_ReadingChanged;
            Accelerometer.Stop();
        }
    }

    private void Accelerometer_ReadingChanged(object? sender, AccelerometerChangedEventArgs e)
    {
        var x = e.Reading.Acceleration.X;

        if (Math.Abs(x) > 0.5)
        {
            _lastShakeTime = DateTime.Now;
            Console.WriteLine("🎯 Shake detected!");
            ShakeDetected?.Invoke();
        }
    }

}

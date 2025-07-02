using System.Numerics;
using Microsoft.Maui.Devices.Sensors;

namespace ShakeMe.Services;

public class ShakeDetectorService
{
    // private const double ShakeThreshold = 1.8;
    private const double ShakeThreshold = 0.1; 
    private DateTime _lastShakeTime = DateTime.MinValue;
    private Vector3? _lastAcceleration;
    public bool IsRunning => Accelerometer.IsMonitoring;


    public event Action? ShakeDetected;

    public void Start()
    {
        Console.WriteLine("▶️ ShakeDetectorService.Start()");

        if (Accelerometer.IsMonitoring)
        {
            Console.WriteLine("⚠️ Accelerometer already running");
            return;
        }

        Accelerometer.ReadingChanged += OnAccelerometerReadingChanged;
        Accelerometer.Start(SensorSpeed.Game);
        Console.WriteLine("✅ Accelerometer started");
    }

    public void Stop()
    {
        if (Accelerometer.IsMonitoring)
        {
            Accelerometer.ReadingChanged -= OnAccelerometerReadingChanged;
            Accelerometer.Stop();
            Console.WriteLine("⏹️ Accelerometer stopped");
        }
    }

    private void OnAccelerometerReadingChanged(object? sender, AccelerometerChangedEventArgs e)
    {
        var current = e.Reading.Acceleration;

        if (_lastAcceleration == null)
        {
            _lastAcceleration = current;
            return;
        }

        var deltaX = Math.Abs(current.X - _lastAcceleration.Value.X);
        var deltaY = Math.Abs(current.Y - _lastAcceleration.Value.Y);
        var deltaZ = Math.Abs(current.Z - _lastAcceleration.Value.Z);

        // var totalDelta = deltaX + deltaY + deltaZ;
        var totalDelta = deltaX;
        _lastAcceleration = current;

        if (totalDelta > ShakeThreshold && DateTime.Now - _lastShakeTime > TimeSpan.FromSeconds(1.5))
        {
            _lastShakeTime = DateTime.Now;
            Console.WriteLine($"🎯 Shake détecté ! force = {totalDelta:F2}");
            ShakeDetected?.Invoke();
        }
    }
}

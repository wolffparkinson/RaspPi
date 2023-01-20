using System.Device.Gpio;
using Microsoft.AspNetCore.Components;

namespace RaspPi.Pages
{

  public partial class AutoToggle
  {
    private readonly int Pin = 11;
    public bool Enabled { get; set; } = false;
    public int Interval { get; set; } = 1;

    private PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(1));


    public void OnInvervalInput(ChangeEventArgs args)
    {
      Interval = int.Parse(args.Value.ToString());
      Console.WriteLine($"Interval : {Interval}s");
      timer.Dispose();
      timer = new PeriodicTimer(TimeSpan.FromSeconds(Interval));
      StartTimer();
    }

    public void OnChange(ChangeEventArgs args)
    {
      var Enabled = (bool)args.Value;
      Console.WriteLine($"Enabled : {Enabled}");

      return; // TODO: Comment when connected to Pi
      using (var controller = new GpioController())
      {
        controller.OpenPin(Pin, PinMode.Output);
        controller.Write(Pin, Enabled ? PinValue.High : PinValue.Low);
      }
    }


    protected override async Task OnInitializedAsync()
    {
      StartTimer();
    }


    public async ValueTask DisposeAsync()
    {
      timer.Dispose();
    }

    public async void StartTimer()
    {
      UpdateStatus();
      while ((await timer.WaitForNextTickAsync()))
      {
        UpdateStatus();
      }
    }

    private void UpdateStatus()
    {
      var status = GetStatus();
      Console.WriteLine($"Status : {status}");
      Enabled = status;
    }

    private bool GetStatus()
    {
      return false; // TODO: Comment when connected to Pi
      using (var controller = new GpioController())
      {
        controller.OpenPin(Pin, PinMode.Input);
        var value = controller.Read(Pin);
        if (value.Equals(PinValue.High))
        {
          return true;
        }
        else
        {
          return false;
        }
      }
    }
  }
}
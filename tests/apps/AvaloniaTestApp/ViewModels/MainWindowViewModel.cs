using System;
using ReactiveUI;

namespace AvaloniaTestApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private string _greeting = "Welcome to Avalonia!";
    public string Greeting
    {
        get => _greeting;
        set
        {
            if (value.Contains("hi"))
                throw new ArgumentException("No hi allowed");
            this.RaiseAndSetIfChanged(ref _greeting, value);
        }
    }

    readonly string[] gretings = ["Hello", "Hi", "What's up?", "How are you?"];
    readonly Random random = new();
    public void ClickCommand()
    {        
        Greeting = gretings[random.Next(gretings.Length-1)];
    }
}

using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShakeMe.Core.Models;

namespace ShakeMe.ViewModels;

public partial class ChatPageViewModel : ObservableObject
{
    public ObservableCollection<MessageModel> Messages { get; } = new();

    [ObservableProperty]
    private string newMessage;

    public ChatPageViewModel()
    {
        Console.WriteLine("✅ ChatPageViewModel instancié");
        // Message fictif au démarrage
        Messages.Add(new MessageModel { Sender = "Alice", Text = "Salut !", Timestamp = DateTime.Now, IsMine = false });
        Messages.Add(new MessageModel { Sender = "Moi", Text = "Yo 👋", Timestamp = DateTime.Now, IsMine = true });
    }

    [RelayCommand]
    private void SendMessage()
    {
        if (string.IsNullOrWhiteSpace(NewMessage))
            return;

        Messages.Add(new MessageModel
        {
            Sender = "Moi",
            Text = NewMessage,
            Timestamp = DateTime.Now,
            IsMine = true
        });

        NewMessage = string.Empty;
    }
}
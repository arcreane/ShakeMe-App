using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShakeMe.Core.Models;
using ShakeMe.Services;

namespace ShakeMe.ViewModels;

public partial class ChatPageViewModel : ObservableObject
{
    private readonly WebSocketService _webSocketService;
    private readonly ConversationStateService _conversationState;

    [ObservableProperty]
    private string newMessage = string.Empty;
    
    [ObservableProperty]
    private string userDisplayName = "Utilisateur";

    [ObservableProperty]
    private string partnerDisplayName = "Partenaire";

    [ObservableProperty]
    private string iceBreaker = string.Empty;

    [ObservableProperty]
    private bool showIceBreaker = false;

    public ObservableCollection<MessageModel> Messages { get; } = new();

    private string _userPseudo = "";

    // Propriété pour activer/désactiver le bouton d'envoi
    public bool CanSendMessage => !string.IsNullOrWhiteSpace(NewMessage) && _webSocketService.IsConnected;

    public ChatPageViewModel(WebSocketService webSocketService, ConversationStateService conversationState)
    {
        _webSocketService = webSocketService;
        _conversationState = conversationState;
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        // Récupérer ou générer un pseudo unique
        var storedPseudo = await SecureStorage.GetAsync("user_pseudo");
        if (string.IsNullOrEmpty(storedPseudo))
        {
            // Générer un pseudo unique basé sur l'heure et un nombre aléatoire
            var random = new Random();
            _userPseudo = $"User_{DateTime.Now.Ticks % 10000}_{random.Next(100, 999)}";
            await SecureStorage.SetAsync("user_pseudo", _userPseudo);
            Console.WriteLine($"✅ Nouveau pseudo généré : {_userPseudo}");
        }
        else
        {
            _userPseudo = storedPseudo;
            Console.WriteLine($"✅ Pseudo existant récupéré : {_userPseudo}");
        }
        UserDisplayName = _userPseudo;
        Console.WriteLine("✅ ChatPageViewModel initialisé sans reconnexion");

        // S'assurer qu'on ne s'abonne qu'une fois
        _webSocketService.OnMessageReceived -= HandleIncomingMessage;
        _webSocketService.OnMessageReceived += HandleIncomingMessage;
    }

    public void OnPageAppearing()
    {
        Console.WriteLine("📄 ChatPage.OnPageAppearing appelé");

        if (!string.IsNullOrWhiteSpace(_conversationState.IceBreaker))
        {
            Console.WriteLine($"🎯 IceBreaker injecté depuis ConversationState : {_conversationState.IceBreaker}");
            IceBreaker = _conversationState.IceBreaker;
            ShowIceBreaker = true;
            _conversationState.IceBreaker = null;
        }

        // Récupérer le pseudo du partenaire
        if (!string.IsNullOrWhiteSpace(_conversationState.PartnerPseudo))
        {
            Console.WriteLine($"👤 Pseudo partenaire récupéré : {_conversationState.PartnerPseudo}");
            PartnerDisplayName = _conversationState.PartnerPseudo;
            // NE PAS nettoyer _conversationState.PartnerPseudo ici pour éviter de perdre l'info
        }
        else
        {
            Console.WriteLine("⚠️ Aucun pseudo partenaire trouvé dans ConversationState");
        }
    }

    partial void OnNewMessageChanged(string value)
    {
        // Notifier le changement de CanSendMessage quand NewMessage change
        OnPropertyChanged(nameof(CanSendMessage));
    }

    private void HandleIncomingMessage(string json)
    {
        Console.WriteLine($"📥 Message WebSocket reçu : {json}");
        
        try
        {
            var document = JsonDocument.Parse(json);
            if (!document.RootElement.TryGetProperty("type", out var typeProp)) 
            {
                Console.WriteLine("❌ Pas de propriété 'type' dans le message");
                return;
            }

            var messageType = typeProp.GetString();
            Console.WriteLine($"🔍 Type de message : {messageType}");

            if (messageType == "message")
            {
                var sender = document.RootElement.GetProperty("sender").GetString();
                var content = document.RootElement.GetProperty("content").GetString();
                var sentAt = document.RootElement.GetProperty("sentAt").GetDateTime();

                Console.WriteLine($"📨 Message de '{sender}' vs mon pseudo '{_userPseudo}'");
                Console.WriteLine($"📝 Contenu : {content}");

                // Vérifier si c'est notre propre message
                if (sender == _userPseudo)
                {
                    Console.WriteLine("📤 Message reçu de moi-même, ignoré.");
                    return;
                }

                // Mettre à jour le pseudo du partenaire si nécessaire
                if (!string.IsNullOrEmpty(sender) && 
                    (PartnerDisplayName == "Partenaire" || string.IsNullOrEmpty(PartnerDisplayName)))
                {
                    Console.WriteLine($"👤 Mise à jour du pseudo partenaire : {sender}");
                    PartnerDisplayName = sender;
                }

                var msg = new MessageModel
                {
                    Sender = sender ?? "?",
                    Content = content ?? "",
                    SentAt = sentAt,
                    IsMine = false
                };

                Console.WriteLine($"✅ Ajout du message à la liste : {msg.Content}");
                MainThread.BeginInvokeOnMainThread(() => 
                {
                    Messages.Add(msg);
                    Console.WriteLine($"📊 Nombre total de messages : {Messages.Count}");
                });
            }
            else if (messageType == "info")
            {
                Console.WriteLine("ℹ️ Message info reçu dans ChatPageViewModel");

                var infoMessage = document.RootElement.GetProperty("message").GetString();

                var msg = new MessageModel
                {
                    Sender = "ShakeMeBot 🤖",
                    Content = infoMessage ?? "Information.",
                    SentAt = DateTime.UtcNow,
                    IsMine = false
                };

                Console.WriteLine($"ℹ️ Ajout message info : {msg.Content}");
                MainThread.BeginInvokeOnMainThread(() => Messages.Add(msg));
            }
            else
            {
                Console.WriteLine($"❓ Type de message non géré : {messageType}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur parsing message WebSocket : {ex.Message}");
            Console.WriteLine($"📄 JSON reçu : {json}");
        }
    }

    [RelayCommand]
    private async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(NewMessage))
        {
            Console.WriteLine("❌ Message vide, envoi annulé");
            return;
        }

        if (!_webSocketService.IsConnected)
        {
            Console.WriteLine("❌ WebSocket non connecté, envoi annulé");
            return;
        }

        Console.WriteLine($"📤 Envoi du message : '{NewMessage}' par {_userPseudo}");

        var msg = new MessageModel
        {
            Sender = _userPseudo,
            Content = NewMessage,
            SentAt = DateTime.UtcNow,
            IsMine = true
        };

        // Ajouter le message à notre liste locale immédiatement
        Messages.Add(msg);
        Console.WriteLine($"✅ Message ajouté localement : {msg.Content}");

        var messageToSend = NewMessage;
        NewMessage = string.Empty; // Vider le champ immédiatement

        try
        {
            var payload = new
            {
                type = "message",
                sender = msg.Sender,
                content = messageToSend
            };

            Console.WriteLine($"🔄 Envoi WebSocket : {JsonSerializer.Serialize(payload)}");
            await _webSocketService.SendAsync(payload);
            Console.WriteLine("✅ Message envoyé via WebSocket");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur envoi message : {ex.Message}");
            // Optionnel : marquer le message comme non envoyé ou le supprimer
        }
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        try
        {
            Console.WriteLine("🔙 Navigation retour");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur navigation retour : {ex.Message}");
        }
    }

    [RelayCommand]
    private void StartConversation()
    {
        Console.WriteLine("🎬 Début de conversation");
        
        if (!string.IsNullOrWhiteSpace(IceBreaker))
        {
            Messages.Add(new MessageModel
            {
                Sender = "ShakeMeBot 🤖",
                Content = IceBreaker,
                SentAt = DateTime.UtcNow,
                IsMine = false
            });
            Console.WriteLine($"🤖 IceBreaker ajouté : {IceBreaker}");
        }
        
        ShowIceBreaker = false;
        IceBreaker = string.Empty;
    }

    public void ResetConversation()
    {
        Console.WriteLine("🔄 Reset de la conversation");
        MainThread.BeginInvokeOnMainThread(() => 
        {
            Messages.Clear();
            ShowIceBreaker = false;
            IceBreaker = string.Empty;
            PartnerDisplayName = "Partenaire"; // Reset du nom du partenaire
        });
        NewMessage = string.Empty;
    }
}
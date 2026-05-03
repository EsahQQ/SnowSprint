using System;
using System.Collections.Generic;
using _Project.Scripts.Features.Network.Lobby;
using _Project.Scripts.Features.Network.Server.Auth.Data;
using _Project.Scripts.Features.Network.Server.Email;
using _Project.Scripts.Features.Network.Server.ServerDatabase;
using _Project.Scripts.Features.Player.Services;
using _Project.Scripts.Features.UI.Shop.Settings;
using Cysharp.Threading.Tasks;
using Mirror;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Features.Network.Server.Auth
{
    public class GameAuthenticator : NetworkAuthenticator
{
    
    private IServerDatabase _db;
    private IEmailService _emailService;
    private ShopDatabase _shopDatabase;
    
    private readonly Dictionary<int, string> _authenticatedUsers = new();

    public event Action<AuthResponseMessage> OnAuthResponseReceived;

    [Inject]
    public void Construct(IServerDatabase db, IEmailService emailService, ShopDatabase shopDatabase)
    {
        _db = db;
        _emailService = emailService;
        _shopDatabase = shopDatabase;
    }

    public override void OnStartServer()
    {
        Debug.Log($"[Auth] OnStartServer. DB={_db != null}, Email={_emailService != null}");

        if (_db == null || _emailService == null)
        {
            Debug.LogError("[Auth] Зависимости не инжектированы!");
            return;
        }

        NetworkServer.RegisterHandler<AuthRequestMessage>(OnServerReceiveRequest, false);
        NetworkServer.RegisterHandler<GetProfileRequest>(OnGetProfileRequest);
        NetworkServer.RegisterHandler<BuyUpgradeRequest>(OnBuyUpgradeRequest);
        
        NetworkServer.OnDisconnectedEvent += OnPlayerDisconnected;
    }

    public override void OnStopServer()
    {
        NetworkServer.OnDisconnectedEvent -= OnPlayerDisconnected;
        _authenticatedUsers.Clear();
    }
    
    private void OnPlayerDisconnected(NetworkConnectionToClient conn)
    {
        if (_authenticatedUsers.Remove(conn.connectionId))
            Debug.Log($"[Auth] Игрок отключился (connId={conn.connectionId}), удалён из аутентифицированных");
    }

    private void OnServerReceiveRequest(NetworkConnectionToClient conn, AuthRequestMessage msg)
    {
        switch (msg.Type)
        {
            case AuthRequestType.Register:
                HandleRegister(conn, msg);
                break;

            case AuthRequestType.Verify:
                HandleVerify(conn, msg);
                break;

            case AuthRequestType.Login:
                HandleLogin(conn, msg);
                break;
        }
    }

    private void HandleRegister(NetworkConnectionToClient conn, AuthRequestMessage msg)
    {
        if (_db.IsEmailExists(msg.Email))
        {
            conn.Send(new AuthResponseMessage
            {
                Type = msg.Type, Success = false, Message = "Email уже занят"
            });
        }
        else
        {
            string code = _db.CreateUserAndGetCode(msg.Email, msg.Username, msg.Password);
            _emailService.SendVerificationEmail(msg.Email, code);
            conn.Send(new AuthResponseMessage
            {
                Type = msg.Type, Success = true, Message = "Код отправлен"
            });
        }
    }

    private void HandleVerify(NetworkConnectionToClient conn, AuthRequestMessage msg)
    {
        bool ok = _db.TryVerify(msg.Email, msg.Code);
        conn.Send(new AuthResponseMessage
        {
            Type = msg.Type,
            Success = ok,
            Message = ok ? "Аккаунт активирован" : "Неверный код"
        });
    }

    private void HandleLogin(NetworkConnectionToClient conn, AuthRequestMessage msg)
    {
        var user = _db.TryLogin(msg.Email, msg.Password);

        if (user == null)
        {
            conn.Send(new AuthResponseMessage
            {
                Type = msg.Type, Success = false, Message = "Неверный логин или пароль"
            });
            return;
        }

        if (!user.IsActivated)
        {
            conn.Send(new AuthResponseMessage
            {
                Type = msg.Type, Success = false, Message = "Аккаунт не активирован"
            });
            return;
        }

        // Сохраняем маппинг до ServerAccept
        _authenticatedUsers[conn.connectionId] = msg.Email;
        Debug.Log($"[Auth] Игрок {msg.Email} аутентифицирован (connId={conn.connectionId})");

        conn.Send(new AuthResponseMessage
        {
            Type = msg.Type, Success = true, Message = user.Username
        });

        ServerAccept(conn);
    }

    private void OnGetProfileRequest(NetworkConnectionToClient conn, GetProfileRequest msg)
    {
        if (!_authenticatedUsers.TryGetValue(conn.connectionId, out var email))
        {
            Debug.LogWarning($"[Server] GetProfileRequest от неаутентифицированного соединения {conn.connectionId}");
            return;
        }

        var user = _db.GetUserByEmail(email);
        if (user == null)
        {
            Debug.LogWarning($"[Server] Пользователь {email} не найден в БД");
            return;
        }

        conn.Send(new ProfileDataResponse
        {
            Coins = user.Coins,
            UnlockedUpgrades = user.UnlockedUpgrades.ToArray()
        });

        Debug.Log($"[Server] Профиль отправлен: {email}, монеты={user.Coins}");
    }

    private void OnBuyUpgradeRequest(NetworkConnectionToClient conn, BuyUpgradeRequest msg)
    {
        /*if (!_authenticatedUsers.TryGetValue(conn.connectionId, out var email))
        {
            Debug.LogWarning($"[Server] BuyUpgradeRequest от неаутентифицированного соединения");
            return;
        }

        var user = _db.GetUserByEmail(email);
        if (user == null) return;
        
        var shopItem = _shopDatabase.GetItemById(msg.UpgradeId);
        if (shopItem == null)
        {
            Debug.LogWarning($"[Server] Неизвестный апгрейд: {msg.UpgradeId}");
            return;
        }

        if (_db.IsUpgradeOwned(email, msg.UpgradeId))
        {
            Debug.LogWarning($"[Server] Апгрейд уже куплен: {msg.UpgradeId}");
            return;
        }

        if (user.Coins < shopItem.Price)
        {
            Debug.LogWarning($"[Server] Недостаточно монет: {user.Coins} < {shopItem.Price}");
            return;
        }

        int newCoins = user.Coins - shopItem.Price;
        _db.UpdateCoins(email, newCoins);
        _db.AddUserUpgrade(email, msg.UpgradeId);

        Debug.Log($"[Server] Апгрейд {msg.UpgradeId} куплен для {email}. Монеты: {user.Coins} → {newCoins}");*/
    }

    // Остальные методы остаются без изменений...
    public override void OnStartClient()
    {
        NetworkClient.RegisterHandler<AuthResponseMessage>(OnClientReceiveResponse, false);
    }

    private void OnClientReceiveResponse(AuthResponseMessage msg)
    {
        if (msg.Type == AuthRequestType.Login && msg.Success)
            ClientAccept();

        OnAuthResponseReceived?.Invoke(msg);
    }

    public override void OnServerAuthenticate(NetworkConnectionToClient conn) { }
    public override void OnClientAuthenticate() { }
    
}
}
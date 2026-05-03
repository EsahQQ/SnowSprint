using System;
using _Project.Scripts.Features.Network.Server.Auth.Data;
using _Project.Scripts.Features.Network.Server.Email;
using _Project.Scripts.Features.Network.Server.ServerDatabase;
using Mirror;
using Zenject;

namespace _Project.Scripts.Features.Network.Server.Auth
{
    public class GameAuthenticator : NetworkAuthenticator
    {
        private IServerDatabase _db;
        private IEmailService _emailService;
        
        [Inject]
        public void Construct(IServerDatabase db, IEmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        public override void OnStartServer() => NetworkServer.RegisterHandler<AuthRequestMessage>(OnServerReceiveRequest, false);

        public override void OnServerAuthenticate(NetworkConnectionToClient conn) { }

        public override void OnClientAuthenticate() { }

        private void OnServerReceiveRequest(NetworkConnectionToClient conn, AuthRequestMessage msg)
        {
            switch (msg.Type)
            {
                case AuthRequestType.Register:
                    if (_db.IsEmailExists(msg.Email))
                    {
                        conn.Send(new AuthResponseMessage { Type = msg.Type, Success = false, Message = "Email уже занят" });
                    }
                    else
                    {
                        string code = _db.CreateUserAndGetCode(msg.Email, msg.Username, msg.Password);
                        _emailService.SendVerificationEmail(msg.Email, code);
                        conn.Send(new AuthResponseMessage { Type = msg.Type, Success = true, Message = "Код отправлен" });
                    }
                    break;

                case AuthRequestType.Verify:
                    if (_db.TryVerify(msg.Email, msg.Code))
                        conn.Send(new AuthResponseMessage { Type = msg.Type, Success = true, Message = "Аккаунт активирован" });
                    else
                        conn.Send(new AuthResponseMessage { Type = msg.Type, Success = false, Message = "Неверный код" });
                    break;

                case AuthRequestType.Login:
                    var user = _db.TryLogin(msg.Email, msg.Password);
                    if (user == null)
                    {
                        conn.Send(new AuthResponseMessage { Type = msg.Type, Success = false, Message = "Неверный логин или пароль" });
                    }
                    else if (!user.IsActivated)
                    {
                        conn.Send(new AuthResponseMessage { Type = msg.Type, Success = false, Message = "Аккаунт не активирован" });
                    }
                    else
                    {
                        conn.Send(new AuthResponseMessage { Type = msg.Type, Success = true, Message = user.Username });
                        ServerAccept(conn);
                    }
                    break;
            }
        }
    }
}
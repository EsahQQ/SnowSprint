using System;
using _Project.Scripts.Features.Network.Server.Auth.Data;

namespace _Project.Scripts.Features.Network.Server.Auth.Controller
{
    public interface IClientAuthController
    {
        void HandlePlayClicked();
        void HandleLogout();
        void HandleAuthAction(AuthData input);
    }
}
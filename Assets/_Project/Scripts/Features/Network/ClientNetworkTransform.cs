using Unity.Netcode.Components;
using UnityEngine;

namespace _Project.Scripts.Features.Network
{
    [DisallowMultipleComponent]
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative() => false;
    }
}
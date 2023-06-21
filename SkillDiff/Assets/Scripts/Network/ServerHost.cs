using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ServerHost : MonoBehaviour
{
   public void StartServer()
   {
        NetworkManager.Singleton.StartServer();
   }

     public void StartClient()
   {
        NetworkManager.Singleton.StartClient();
   }
}

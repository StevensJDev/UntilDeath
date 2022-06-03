using UnityEngine;
using MarsFPSKit.UI;
using Photon.Realtime;
using Steamworks;
using System.Text;

namespace MarsFPSKit
{
    namespace Integrations
    {
        namespace Steam
        {
            public class Kit_SteamLoginNew : Kit_MenuLoginBase
            {
                /// <summary>
                /// Refer to the Photon documentation on "Photon Steam Authentication"!
                /// </summary>
                public bool useSteamAuthentication;

                private HAuthTicket authenticationTicket;

                public override void Initialize(Kit_MenuManager mm)
                {
                    //Set name
                    Kit_GameSettings.userName = SteamFriends.GetPersonaName();
                    //Call
                    mm.LoggedIn(SteamFriends.GetPersonaName());
                    //Set
                    PlayerPrefs.SetString("previousUsername", SteamFriends.GetPersonaName());
                }

                public override AuthenticationValues GetAuthenticationValues()
                {
                    if (useSteamAuthentication)
                    {
                        AuthenticationValues av = new AuthenticationValues();
                        av.UserId = SteamUser.GetSteamID().ToString();
                        av.AuthType = CustomAuthenticationType.Steam;
                        av.AddAuthParameter("ticket", GetSteamAuthTicket(out authenticationTicket));

                        return av;
                    }
                    else
                    {
                        AuthenticationValues av = new AuthenticationValues();
                        av.UserId = SteamFriends.GetPersonaName();

                        //Just return our username as userid
                        return av;
                    }
                }

                // hAuthTicket should be saved so you can use it to cancel the ticket as soon as you are done with it
                public string GetSteamAuthTicket(out HAuthTicket hAuthTicket)
                {
                    byte[] ticketByteArray = new byte[1024];
                    uint ticketSize;
                    hAuthTicket = SteamUser.GetAuthSessionTicket(ticketByteArray, ticketByteArray.Length, out ticketSize);
                    System.Array.Resize(ref ticketByteArray, (int)ticketSize);
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < ticketSize; i++)
                    {
                        sb.AppendFormat("{0:x2}", ticketByteArray[i]);
                    }
                    return sb.ToString();
                }

                public override void OnConnectedToMaster(Kit_MenuManager mm)
                {
                    if (useSteamAuthentication)
                    {
                        SteamUser.CancelAuthTicket(authenticationTicket);
                    }
                }
            }
        }
    }
}
using UnityEngine;

public class RandomMatchmaker : Photon.PunBehaviour
{
	
	public string charaname;
    private PhotonView myPhotonView;

    // Use this for initialization
    public void Start()
    {
        PhotonNetwork.ConnectUsingSettings("0.1");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("JoinRandom");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnConnectedToMaster()
    {
        // when AutoJoinLobby is off, this method gets called when PUN finished the connection (instead of OnJoinedLobby())
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnPhotonRandomJoinFailed()
    {
        PhotonNetwork.CreateRoom(null);
    }

    public override void OnJoinedRoom()
    {
		GameObject monster = PhotonNetwork.Instantiate(charaname, Vector3.zero, Quaternion.identity, 0);
        myPhotonView = monster.GetComponent<PhotonView>();
    }

    public void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

        if (PhotonNetwork.inRoom)
        {
            bool shoutMarco = GameLogic.playerWhoIsIt == PhotonNetwork.player.ID;

            if (shoutMarco && GUILayout.Button("Marco!"))
            {
                myPhotonView.RPC("Marco", PhotonTargets.All);
            }
            if (!shoutMarco && GUILayout.Button("Polo!"))
            {
                myPhotonView.RPC("Polo", PhotonTargets.All);
            }
        }
    }
}

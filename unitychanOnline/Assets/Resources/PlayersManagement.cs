using UnityEngine;
using UnityEngine.UI;
public class PlayersManagement : Photon.MonoBehaviour {
	public bool[] Players = new bool[4];
	private bool[] CorrectPlayers = new bool[4];

	public Text[] texts = new Text[4];
	public int[] Scores = new int[4];
	private int[] CorrectScores = new int[4];
	public int[] Deaths = new int[4];
	private int[] CorrectDeaths = new int[4];
	// Update is called once per frame
	void Awake()
	{
		for (int i = 0; i < 4; i++) {
			string s = "Text" + i.ToString ();
			texts [i] = GameObject.Find (s).GetComponent<Text>();
		}
	}
	void Update ()
	{
		if (!photonView.isMine)
		{
			for (int i = 0; i < 4; i++) {
				Players [i] = CorrectPlayers[i];
				Scores [i] = CorrectScores [i];	
				Deaths [i] = CorrectDeaths [i];
			}
		}
		for (int i = 0; i < 4; i++) {
			texts[i].text = "Kill:" + Scores[i].ToString() +" Death:" + Deaths[i].ToString();
		}
	}
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			// We own this player: send the others our data
			for (int i = 0; i < 4; i++) {
				stream.SendNext(Players [i]);
				stream.SendNext (Scores [i]);
				stream.SendNext (Deaths [i]);
			}
		}
		else
		{
			// Network player, receive data
			for (int i = 0; i < 4; i++) {
				CorrectPlayers [i] = (bool)stream.ReceiveNext();
				CorrectScores [i] = (int)stream.ReceiveNext ();	
				CorrectDeaths [i] = (int)stream.ReceiveNext ();
			}
		}
	}
	[PunRPC]
	void ChangeToTrue(int i){
		Players [i] = true;
	}
	[PunRPC]
	void ChangeToFalse(int i){
		Players [i] = false;
	}

	[PunRPC]
	void ScoreChanger(int i){
		Scores [i]++;
		Debug.Log("Score");
	}
	[PunRPC]
	void DeathChanger(int i){
		Deaths [i]++;
		Debug.Log("Death");
	}
}

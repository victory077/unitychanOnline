using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ScoreManager : Photon.MonoBehaviour
{
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
			Scores [i] = 0;
			Deaths [i] = 0;
		}
	}
	void Update()
	{
		for (int i = 0; i < 4; i++) {
			Scores [i] = CorrectScores [i];	
			Deaths [i] = CorrectDeaths [i];
			texts[i].text = "Kill:" + Scores[i].ToString() +" Death:" + Deaths[i].ToString();
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			for (int i = 0; i < 4; i++) {
				stream.SendNext (Scores [i]);
				stream.SendNext (Deaths [i]);
			}
		}
		else
		{
			// Network player, receive data
			for (int i = 0; i < 4; i++) {
				CorrectScores [i] = (int)stream.ReceiveNext ();	
				CorrectDeaths [i] = (int)stream.ReceiveNext ();
			}
		}
	}
}
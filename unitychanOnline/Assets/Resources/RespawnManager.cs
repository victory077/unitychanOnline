using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class RespawnManager : MonoBehaviour {
	public bool life = false;
	public string charaname;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.R) && life)
		{
			PhotonNetwork.Instantiate(charaname, new Vector3(Random.Range(-48,48), 1.0f, Random.Range(-48,48)), transform.rotation,0);
			life = false;
		}
	}
}

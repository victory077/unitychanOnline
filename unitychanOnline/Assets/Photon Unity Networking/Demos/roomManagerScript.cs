using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class roomManagerScript : Photon.MonoBehaviour
{
	//Script系
	public string charaname;
	public GameObject[] pos = new GameObject[4];

	//photon系変数
	RoomInfo[] roomInfo = new RoomInfo[0];
	private string playerName;
	private string roomName;
	private bool connectFailed = false;
	private PhotonView myPhotonView;
	private int number;

	public GameObject myPlayer;
	public GameObject friend;

	private PhotonView masterPhotonView;

	PlayersManagement PM;





	public void Awake ()
	{
		// マスタークライアントのsceneと同じsceneを部屋に入室した人もロードする。
		PhotonNetwork.automaticallySyncScene = true;
		// もしまだ接続していない状態ならば
		// Photonネットワークに接続する
		if (PhotonNetwork.connectionStateDetailed == ClientState.PeerCreated) {
			// PhotonServerSettingsの設定に従ってPhotonNetwork（マスターサーバー）に接続する。
			PhotonNetwork.ConnectUsingSettings ("0.1");
		}
	}

	[SerializeField]
	private GUISkin guiSkin;
	private int GUIMode;
	[SerializeField]
	private float GUIHeight;

	void Start ()
	{

		//GUI系
		guiSkin = Resources.Load<GUISkin> ("roomManageGUISkin");

		GUIMode = 0;
		GUIHeight = Screen.height / 24 * 1.5f;
		int fontSize = GetFontSize (GUIHeight * 4 / 5);
		guiSkin.label.fontSize = fontSize;
		guiSkin.button.fontSize = fontSize;
		guiSkin.textField.fontSize = fontSize;
		playerName = "Player";
	}
		

	public void OnJoinedLobby ()
	{
		Debug.Log ("Joined Lobby");
	}

	public void OnReceivedRoomListUpdate ()
	{
		Debug.Log ("Updated rooms information");
		roomInfo = PhotonNetwork.GetRoomList ();
//		for(int i = 0; i<roomInfo.Length; i++){
//			Debug.Log(roomInfo[i].name);
//			Debug.Log(roomInfo[i].playerCount);
//			Debug.Log(roomInfo[i].maxPlayers);
//		}
	}

	//roomにJoinする時の処理
	public void OnJoinedRoom ()
	{
		Debug.Log ("OnJoinedRoom");
		//Photonにプレイヤー名を登録
		PhotonNetwork.playerName = this.playerName; 
		// Roomに参加しているプレイヤー情報を配列で取得.
		PhotonPlayer[] playerArray = PhotonNetwork.playerList;
		// プレイヤー名とIDを表示.
		for (int i = 0; i < playerArray.Length; i++) {
			Debug.Log ((i).ToString () + " : " + playerArray [i].name + " ID = " + playerArray [i].ID);
		}
		StartCoroutine("OnJoinedRoomC");
	}

	//部屋作成に成功したときにコールs
	public void OnCreatedRoom ()
	{
		Debug.Log ("OnCreatedRoom");
	}

	//接続が切断されたときにコール
	public void OnDisconnectedFromPhoton ()
	{
		Debug.Log ("Disconnected from Photon.");
	}

	//接続失敗時にコール
	public void OnFailedToConnectToPhoton (object parameters)
	{
		this.connectFailed = true;
		Debug.Log ("OnFailedToConnectToPhoton. StatusCode: " + parameters + " ServerAddress: " + PhotonNetwork.networkingPeer.ServerAddress);
	}

	int GetFontSize (float value)
	{
		GUIStyle s = new GUIStyle ();
		int size = 0;
		int width = Screen.width;
		int height = Screen.height;
		Vector2 vec = Vector2.zero;

		height = Mathf.CeilToInt (value);

		// フォントサイズ1から増やしていきCalcSize()で描画時の画素数を取得
		// 描画の画素数がmodeにより示す最大値を超えないフォントサイズを決定
		for (int i = 1;; i++) {
			s.fontSize = i;
			Vector2 v = s.CalcSize (new GUIContent ("A"));
			if (v.x < width && v.y < height) {
				size = i; // フォントサイズ
				vec = v; // 描画の画素数
			} else {
				break;
			}
		}
		return size;
	}

	//GUIを生成する
	public void OnGUI ()
	{
		GUI.skin = guiSkin;
		GUILayout.BeginVertical ("box");

		switch (GUIMode) {
		case 0:
			GUILayout.BeginHorizontal (GUILayout.Width (Screen.width / 3));
			if (GUILayout.Button ("Settings On", GUILayout.Height (GUIHeight))) {
				GUIMode = 1;
			}
			GUILayout.EndHorizontal ();
			break;
		case 1:
			//1行目:設定オフボタン
			GUILayout.BeginHorizontal ();
			{
				if (GUILayout.Button ("Settings Off", GUILayout.Height (GUIHeight))) {
					GUIMode = 0;
				}
			}
			GUILayout.EndHorizontal ();
			//2行目:photonStatusを表示
			GUILayout.BeginHorizontal ("box");
			{
				GUILayout.Label ("PhotonStatus ==> " + PhotonNetwork.connectionStateDetailed.ToString (), GUILayout.Height (GUIHeight));
			}
			GUILayout.EndHorizontal ();
			//3行目:プレイヤー名を入力(defaultは"Player")
			GUILayout.BeginHorizontal ("box");
			{
				GUILayout.Label ("Player Name:", GUILayout.Height (GUIHeight), GUILayout.Width (Screen.width / 6));
				this.playerName = GUILayout.TextField (this.playerName, GUILayout.Height (GUIHeight), GUILayout.Width (Screen.width / 6));
			}
			GUILayout.EndHorizontal ();
			//4行目:roomを選択する
			GUILayout.BeginHorizontal ();
			{
				for (int i = 0; i < 5; i++) {
					//作成されたroomがある場合
					//Debug.Log(roomInfo.Length);
					if (roomInfo.Length > 0) {
						for (int j = 0; j < roomInfo.Length; j++) {
							//ルームが1つでも存在して、roomが作成されるいる部屋のボタンを生成
							if (roomInfo [j].name == "Room#" + i.ToString ()) {
								if (GUILayout.Button ("Room#" + i.ToString () + "\n" + roomInfo [j].playerCount + "/" + roomInfo [j].maxPlayers, GUILayout.Height (GUIHeight * 2))) {
									this.roomName = "Room#" + i.ToString ();
									//入室。引数はルーム名
									PhotonNetwork.JoinRoom (this.roomName);
									GUIMode = 2;
									break;
								}
							}
							//ルームが1つでも存在して、roomが作成されていない部屋のボタンを生成
							else if (GUILayout.Button ("Room#" + i.ToString () + "\n" + "0/4", GUILayout.Height (GUIHeight * 2))) {
								this.roomName = "Room#" + i.ToString ();
								//ルームを作成。引数はルーム名
								RoomOptions roomOptions = new RoomOptions ();
								roomOptions.MaxPlayers = 4;
								PhotonNetwork.CreateRoom (this.roomName, roomOptions, null);
								GUIMode = 2;
							}
						}
					}
					//作成されたroomが１つもない場合
					else {
						if (GUILayout.Button ("Room#" + i.ToString () + "\n" + "0/4", GUILayout.Height (GUIHeight * 2))) {
							this.roomName = "Room#" + i.ToString ();
							//ルームを作成。引数はルーム名
							RoomOptions roomOptions = new RoomOptions ();
							roomOptions.MaxPlayers = 4;
							PhotonNetwork.CreateRoom (this.roomName, roomOptions, null);
							GUIMode = 2;
						}
					}
				}
			}
			GUILayout.EndHorizontal ();
			break;
		case 2:
			GUILayout.BeginHorizontal (GUILayout.Width (Screen.width / 3));
			//ルームを退出するボタン
			if (GUILayout.Button ("Exit Room", GUILayout.Height (GUIHeight))) {
				//退出。引数はルーム名
				masterPhotonView.RPC ("ChangeToFalse", PhotonPlayer.Find(masterPhotonView.ownerId), number);
				PhotonNetwork.LeaveRoom ();
				Destroy(GameObject.Find("FriendTarget(Clone)"));
				GUIMode = 0;
			}
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal (GUILayout.Width (Screen.width / 3));
			//一人デバッグモード
			if (GUILayout.Button ("Debug Mode", GUILayout.Height (GUIHeight))) {

			}
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal();
			GUILayout.EndHorizontal();
			break;
		}
		GUILayout.EndVertical ();
	}

	IEnumerator OnJoinedRoomC(){
		if (PhotonNetwork.isMasterClient) {
			PhotonNetwork.InstantiateSceneObject ("PlayerManager", Vector3.zero, transform.rotation, 0, null);
		}

		GameObject cha = PhotonNetwork.Instantiate(charaname, Vector3.zero, Quaternion.identity, 0);

		yield return StartCoroutine ("SearchPM");

		UnitychanNetWork un = cha.GetComponent<UnitychanNetWork> ();
		un.masterView = masterPhotonView;
		int i = 0;
		for ( ; ; ) {
			if (!PM.Players [i]) {
				un.Number = i;
				Debug.Log ("Login as P " + i);
				number = i;
				if (PhotonNetwork.isMasterClient) {
					PM.Players [i] = true;
				} else {
					masterPhotonView.RPC ("ChangeToTrue", PhotonPlayer.Find(masterPhotonView.ownerId), i);
				}
				break;
			}
			i++;
		}
		myPhotonView = cha.GetComponent<PhotonView>();
	}
	IEnumerator SearchPM(){
		bool b = true;
		while (b) {
			if (FindObjectOfType<PlayersManagement>() != null) {
				PM = FindObjectOfType<PlayersManagement> () as PlayersManagement;
				masterPhotonView = PM.gameObject.GetComponent<PhotonView> ();
				b = false;
			}
			yield return new WaitForSeconds (0.1f);
		}
	}
}

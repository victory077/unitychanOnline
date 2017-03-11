using UnityEngine;

public class UnitychanNetWork : Photon.MonoBehaviour
{
    private Vector3 correctPlayerPos = Vector3.zero; // We lerp towards this
    private Quaternion correctPlayerRot = Quaternion.identity; // We lerp towards this
	private int correctHP;

	private RespawnManager rm;
	public ScoreManager SM;

	public int HP;
	public string messenger;
    
	public int Number;

	public PhotonView masterView;
	// Update is called once per frame
	void Awake()
	{
		if (photonView.isMine) {
			this.GetComponent<CapsuleCollider> ().enabled = false;
			Invoke ("SwitchCollider", 5);
			Invoke ("SwitchFire", 5);

			CameraCtrl cc = GetComponent<CameraCtrl> ();
			cc.enabled = true;
			cc.isControlable = true;

			GetComponent<UnityChanControlScriptWithRgidBody> ().enabled = true;

			GameObject g = GameObject.Find (messenger);

			rm = g.GetComponent<RespawnManager> ();
			rm.life = false;
		}
	}
    void Update()
	{
		if (!photonView.isMine) {
			transform.position = Vector3.Lerp (transform.position, this.correctPlayerPos, Time.deltaTime * 5);
			transform.rotation = Quaternion.Lerp (transform.rotation, this.correctPlayerRot, Time.deltaTime * 5);
			HP = correctHP;
		} 
	}
	[PunRPC]//命令をネットワーク化する。ないと自分の画面内でのみ消えてて他の画面では消えない。
	void Damage(float f,int p)
	{
		HP -= Mathf.RoundToInt (f);
		if (HP <= 0) {
			if (photonView.isMine) {
				rm.life = true;
			}
			masterView.RPC ("ScoreChanger", PhotonPlayer.Find(masterView.ownerId), p);
			masterView.RPC ("DeathChanger", PhotonPlayer.Find(masterView.ownerId), Number);
			PhotonNetwork.Destroy (gameObject);
		}
	}

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
			stream.SendNext (HP);
        }
        else
        {
            // Network player, receive data
            this.correctPlayerPos = (Vector3)stream.ReceiveNext();
            this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
			this.correctHP = (int)stream.ReceiveNext();
        }
    }
	void SwitchFire(){
		MonsterFire mf = GetComponent<MonsterFire> ();
		mf.enabled = true;
		mf.pn = Number;
	}

	void SwitchCollider(){
		GetComponent<CapsuleCollider> ().enabled = true;
	}
}

using UnityEngine;

public class bullet : Photon.MonoBehaviour
{
    private Vector3 correctPlayerPos = Vector3.zero; // We lerp towards this
    private Quaternion correctPlayerRot = Quaternion.identity; // We lerp towards this
	public float atk;
	public int number;
    // Update is called once per frame
	void Awake()
	{
		if (photonView.isMine) {
			GetComponent<Rigidbody>().isKinematic = false;
		}
	}
    void Update()
    {
		if (!photonView.isMine) {
			transform.position = Vector3.Lerp (transform.position, this.correctPlayerPos, Time.deltaTime * 5);
			transform.rotation = Quaternion.Lerp (transform.rotation, this.correctPlayerRot, Time.deltaTime * 5);
		} else {
			atk -= 0.05f;
			if (atk < 0f) {
				PhotonNetwork.Destroy (gameObject);
			}
		}
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(transform.position);//これでサーバーにtransform.pos送っている
            stream.SendNext(transform.rotation);
        }
        else
        {
            // Network player, receive data
            this.correctPlayerPos = (Vector3)stream.ReceiveNext();
            this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
        }
	}
	void OnTriggerEnter(Collider other)
	{
		if (photonView.isMine) {
			PhotonView myView = gameObject.GetComponent<PhotonView> ();

			if (other.gameObject.tag == "Player") {
				PhotonView otherView = other.gameObject.GetComponent<PhotonView> ();

				if (otherView.ownerId == myView.ownerId) {
					return;
				}
				if (myView.ownerId == PhotonNetwork.player.ID) {
					otherView.RPC ("Damage", PhotonPlayer.Find (otherView.ownerId),atk,number);
				}
			}
			if (other.gameObject.tag == "Finish") {
				if (myView.isMine) {
					PhotonNetwork.Destroy (gameObject);
				}
			}
		}
	}
}

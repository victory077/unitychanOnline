using UnityEngine;
using System.Collections;

public class CameraCtrl : MonoBehaviour {
	public GameObject Camera;
	private float CameraHeight = 2;

	public bool isControlable = false; 

	public Sprite sp;
	private int HP;
	private int MaxHP;
	public UnitychanNetWork player;
	public GameObject bar;
	// Use this for initialization

	void Start () {
		if (isControlable) {
			SpriteRenderer sr = bar.GetComponent<SpriteRenderer> ();
			sr.sprite = sp;
		}
		Camera = GameObject.Find("Main Camera");
		MaxHP = player.HP;
		HP = MaxHP;
	}

	// Update is called once per frame
	void Update () {
		if (isControlable) {
			CameraMoving ();
		}

		HP = player.HP;
		bar.transform.LookAt (Camera.transform);
		bar.transform.localScale = new Vector3 (5 * HP / MaxHP, 0.3f, 1);
	}
	void CameraMoving(){
		Vector3 s = new Vector3 (0,CameraHeight,0);
		Camera.transform.position = transform.position + transform.forward * -3 + s;
		Vector3 t = new Vector3 (0, 1, 0);
		Camera.transform.LookAt (this.transform.position + t);
		if (Input.GetKey("i")){
			CameraHeight -= 0.03f;
		}
		if (Input.GetKey("k")){
			CameraHeight += 0.03f;
		}
	}
}

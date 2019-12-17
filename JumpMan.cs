using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpMan : MonoBehaviour {
  public float maxChargeTime = 1f;
  public float maxChargeForce = 250f;
  public float chargePercent = 0f;
  [HideInInspector]
  public bool charging = false;
  public float sensitivity;
  private float startTime = 0f;
  private float chargeTime = 0f;
  private GameManager dm;
  public ParticleSystem chargeParticleFX;
  public ParticleSystem maxChargeParticleFX;

	void Start() {
		dm = FindObjectOfType<GameManager> ();
	}

	void Update() { //Called every frame.
		if (!dm.paused) { //If the game is not paused.
            #if UNITY_EDITOR
                //if we're in editor, use A and D keys.
                transform.Translate(Input.GetAxis("Horizontal") * 0.5f * sensitivity, 0, 0);
            #endif
            //Normally, we'll use the device accelerometer.
            transform.Translate (Input.acceleration.x * 0.5f * sensitivity, 0, 0);
			if (transform.position.x < -3.75f) {
				transform.position = new Vector3 (-3.75f, transform.position.y, transform.position.z);
			} else if (transform.position.x > 3.75f) {
				transform.position = new Vector3 (3.75f, transform.position.y, transform.position.z);
			}
			if (charging) {
        //Character's jumping logic here.
				chargeTime = Time.time - startTime;
				chargePercent = Mathf.Clamp (chargeTime / maxChargeTime, 0f, 1f);
                if(chargePercent >= 1f && !maxChargeParticleFX.isPlaying)
                {
                    maxChargeParticleFX.Play();
                }
			} else {
				chargeTime = 0f;
			}
		}

    //Offsets the scale of the GameObject's transfrom in the Y direction so that it appears to scrunch down as the jump charges.
		gameObject.transform.localScale = new Vector3(1, Mathf.Clamp ((1f - chargePercent), 0.25f, 1.0f), 1); 

		if (transform.position.y <= -5f) { //This GameObject is off the map.
			dm.GameOver ();
			ResetPosition ();
		}
	}

	public bool IsGrounded() { //Returns true if this GameObject is near the ground.
		return Physics.Raycast(transform.position, -Vector3.up, (gameObject.transform.localScale.y/2f + 0.1f));
	}

	public void ChargeJump() { //Begins charging a jump.
		if (IsGrounded()) {
			startTime = Time.time;
			charging = true;
            chargeParticleFX.Play();
        }
	}

	public void ReleaseJump() { //Stops charging a jump, and jumps character upwards.
		charging = false;
		gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * (chargePercent) * maxChargeForce);
		chargePercent = 0f;
        chargeParticleFX.Stop();
        maxChargeParticleFX.Stop();
    }

	public void ResetPosition() { //Resets the GameObject's transform to a pred-determined position.
		GetComponent<Rigidbody> ().velocity = Vector3.zero;
		transform.rotation = new Quaternion();
		transform.position = new Vector3 (0f, 0.65f, 0f);
	}

	public void SetSensitivity(float f){ //Changes the player's input sensitivity.
		sensitivity = f;
	}

}

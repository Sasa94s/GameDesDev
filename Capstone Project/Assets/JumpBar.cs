using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBar : MonoBehaviour {

    /// the healthbar's foreground bar
    public Transform ForegroundBar;
    /// the color when at max fuel
    public Color MaxFuelColor = new Color(36 / 255f, 199 / 255f, 238 / 255f);
    /// the color for min fuel
    public Color MinFuelColor = new Color(24 / 255f, 164 / 255f, 198 / 255f);

    private CharacterController2D _character;

    private float jumpPercent;
    // Use this for initialization
    void Start ()
    {
        if (GameManager.gm.player != null) _character = GameManager.gm.player;
        if (_character == null)
        {
            _character = GetComponent<CharacterController2D>();
        }
        if (ForegroundBar == null) // if ForegroundSprite is missing
            Debug.LogError("Foreground Sprite component missing from this gameobject");
    }
	
	// Update is called once per frame
	void Update () {
	    if (_character == null)
	        return;

        jumpPercent = _character.jumpTimeCounter / _character.jumpTime;
	    if (_character.isGrounded) jumpPercent = 1;
        ForegroundBar.localScale = new Vector3(jumpPercent, 1, 1);
    }
}

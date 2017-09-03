using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

    /// the healthbar's foreground sprite
    public Transform ForegroundSprite;
    /// the color when at max health
    public Color MaxHealthColor = new Color(255 / 255f, 63 / 255f, 63 / 255f);
    /// the color for min health
    public Color MinHealthColor = new Color(64 / 255f, 137 / 255f, 255 / 255f);

    private CharacterController2D _character;

    // Use this for initialization
    void Start () {
        if (GameManager.gm.player != null) _character = GameManager.gm.player;
        if (_character == null)
        {
            _character = GetComponent<CharacterController2D>();
        }
        if (ForegroundSprite == null) // if ForegroundSprite is missing
            Debug.LogError("Foreground Sprite component missing from this gameobject");
    }
	
	// Update is called once per frame
	void Update () {
	    if (_character == null)
	        return;

	    float healthPercent = _character.playerHealth;
	    ForegroundSprite.localScale = new Vector3(healthPercent, 1, 1);
    }
}

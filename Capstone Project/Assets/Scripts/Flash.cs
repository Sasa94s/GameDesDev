using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    // player animation flash
    [Range(0.2f, 1.0f)] // create a slider in the editor and set limits on transparency
    public float[] keyFramesAlpha;
    public float AlphaDelay = 0.1f;

    private SpriteRenderer _renderer;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        if (_renderer == null) // if renderer is missing
            Debug.LogError("Sprite Renderer component missing from this gameobject");

    }

    void Start ()
    {
        StartCoroutine(InvisibleFlash(keyFramesAlpha,AlphaDelay));
    }
	
    IEnumerator InvisibleFlash(float[] keyframes, float samples)
    {
        for (int i = 0; i < keyframes.Length; ++i)
        {
            UpdateColor(keyframes[i]);
            yield return new WaitForSeconds(samples);
        }
        yield return new WaitForSeconds(samples * Time.deltaTime);
        StartCoroutine(InvisibleFlash(keyframes, samples));
        UpdateColor(samples*2);
    }
    void UpdateColor(float alpha)
    {
        _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, alpha);
    }
}

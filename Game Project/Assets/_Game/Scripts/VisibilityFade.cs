using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class VisibilityFade : MonoBehaviour
{

    public float alpha = 0.5f;
    public float fadeIn = 0.2f;// Fade in duration
    public float fadeOut = 1.0f;// Fade out duration
    private Renderer[] renderers;

    private float timer = 1.0f;
    private bool fadingIn = false;

    // Use this for initialization
    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        if (fadingIn)
            timer += Time.deltaTime / fadeIn;
        else
            timer -= Time.deltaTime / fadeOut;

        timer = Mathf.Clamp01(timer);

        foreach (Renderer rend in renderers)
        {
            Color matcolor = rend.material.color;
            matcolor.a = Mathf.Lerp(1.0f, alpha, timer);

            rend.material.SetColor("_Color", matcolor);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
            fadingIn = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
            fadingIn = false;

    }
}

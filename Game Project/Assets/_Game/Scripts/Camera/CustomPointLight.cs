using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class CustomPointLight : CustomLight
{
    public Color Color = Color.white;
    public float Intensity = 1.0f;
    public float Range = 10.0f;

    public float Constant = 1.0f;
    public float Linear = 1.0f;
    public float Cuadratic = 2.0f;
    public float Power = 0.5f;


    public void OnEnable()
    {
        CustomLightSystem.Instance.AddLight(this);
    }

    public void Start()
    {
        CustomLightSystem.Instance.AddLight(this);
    }

    public void OnDisable()
    {
        CustomLightSystem.Instance.RemoveLight(this);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color;
        Gizmos.DrawIcon(transform.position, "PointLight Gizmo", true);
    }
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.1f, 0.7f, 1.0f, 0.6f);

        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.DrawWireSphere(transform.position, Range);
    }

}

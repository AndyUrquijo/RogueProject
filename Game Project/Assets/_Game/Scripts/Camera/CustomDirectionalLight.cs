using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class CustomDirectionalLight : CustomLight
{
    public Color Color = Color.white;
    public float Intensity = 1.0f;

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
        Gizmos.DrawIcon(transform.position, "DirectionalLight Gizmo", true);
    }
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.1f, 0.7f, 1.0f, 0.6f);

        Gizmos.matrix = Matrix4x4.identity;

        float radius = 2;
        float length = 10;


        Vector3 displace = transform.forward * length;
        Vector3 up = transform.up * radius;
        Vector3 right = transform.right * radius;
        Vector3 center = transform.position;

        Gizmos.DrawWireSphere(center, radius);
        Gizmos.DrawLine(center + up, center + up + displace);
        Gizmos.DrawLine(center - up, center - up + displace);
        Gizmos.DrawLine(center + right, center + right + displace);
        Gizmos.DrawLine(center - right, center - right + displace);
    }

}

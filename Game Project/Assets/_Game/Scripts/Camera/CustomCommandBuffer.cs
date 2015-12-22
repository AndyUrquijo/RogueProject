using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using System.Collections.Generic;
public class CustomLight : MonoBehaviour
{
}

public class CustomLightSystem
{
    public HashSet<CustomLight> lights = new HashSet<CustomLight>();

    static CustomLightSystem instance;
    static public CustomLightSystem Instance
    {
        get
        {
            if (instance == null)
                instance = new CustomLightSystem();
            return instance;
        }
    }

    public bool AddLight(CustomLight light)
    {
        return lights.Add(light);
    }

    public bool RemoveLight(CustomLight light)
    {
        return lights.Remove(light);
    }

}


[ExecuteInEditMode]
public class CustomCommandBuffer : MonoBehaviour
{
    struct CommandSet
    {
        public CommandBuffer lightCommand; // Renders Lights
        public CommandBuffer applyCommand; // Applies light data

    }
    private Dictionary<Camera, CommandSet> m_Cameras = new Dictionary<Camera, CommandSet>();

    public Shader pointLightShader;
    Material pointLightMaterial;

    public Shader directionalLightShader;
    Material directionalLightMaterial;

    public Shader applyShader;
    Material applyMaterial;

    public Mesh sphereMesh;

    public Color Ambient;
    public float AmbientIntensity;

    private void Cleanup()
    {
        foreach (var pair in m_Cameras)
        {
            Camera cam = pair.Key;
            if (!cam)
                continue;
            cam.RemoveAllCommandBuffers();
        }
        m_Cameras.Clear();
        Object.DestroyImmediate(pointLightMaterial);
        Object.DestroyImmediate(directionalLightMaterial);
        Object.DestroyImmediate(applyMaterial);
    }

    public void OnEnable()
    {
        Cleanup();
    }

    public void OnDisable()
    {
        Cleanup();
    }

    public void OnWillRenderObject()
    {
        var act = gameObject.activeInHierarchy && enabled;
        if (!act)
        {
            Cleanup();
            return;
        }

        var cam = Camera.current;
        if (!cam)
            return;


        if (!pointLightMaterial)
            pointLightMaterial = new Material(pointLightShader);
        if (!directionalLightMaterial)
            directionalLightMaterial = new Material(directionalLightShader);

        // A new set of commands is created with the latest light information
        // NOTE: Dumb&easy. Every command set is recreated on each call. 
        // Ideally, existing command lists could be reused and cleared only when needed.
        cam.RemoveAllCommandBuffers();
        CommandSet comSet = m_Cameras[cam] = new CommandSet();

        comSet.lightCommand = new CommandBuffer();
        comSet.lightCommand.name = "Light Commands";
        cam.AddCommandBuffer(CameraEvent.AfterLighting, comSet.lightCommand);

        comSet.applyCommand = new CommandBuffer();
        comSet.applyCommand.name = "Apply Commands";
        cam.AddCommandBuffer(CameraEvent.BeforeForwardAlpha, comSet.applyCommand);

        int propLightTexture = Shader.PropertyToID("_LightTexture");

        int propLightColor = Shader.PropertyToID("_CustomLightColor");
        int propLightData = Shader.PropertyToID("_CustomLightData");
        int propAttenuation = Shader.PropertyToID("_CustomAttenuation");
        
        comSet.lightCommand.GetTemporaryRT(propLightTexture, -1, -1, 0, FilterMode.Bilinear,RenderTextureFormat.ARGB32);
        comSet.lightCommand.SetRenderTarget(propLightTexture);
        Color ambientColor = Color.Lerp(Color.black, Ambient, AmbientIntensity);
        ambientColor.a = AmbientIntensity;
        
        comSet.lightCommand.ClearRenderTarget(false,true, ambientColor);
        //comSet.lightCommand.ClearRenderTarget(false,true, Color.clear);
        //comSet.lightCommand.SetRenderTarget(propLightTexture);

        var lights = CustomLightSystem.Instance.lights;
        foreach (CustomLight light in lights)
        {
            CustomPointLight pointLight = light as CustomPointLight;
            if (pointLight)
            {
                comSet.lightCommand.SetGlobalColor(propLightColor, pointLight.Color);

                Vector4 lightData = Vector4.zero;
                lightData.x = pointLight.Intensity;
                lightData.y = pointLight.Range;
                comSet.lightCommand.SetGlobalVector(propLightData, lightData);

                Vector4 attenuation;
                attenuation.x = pointLight.Constant;
                attenuation.y = pointLight.Linear;
                attenuation.z = pointLight.Cuadratic;
                attenuation.w = pointLight.Power;
                comSet.lightCommand.SetGlobalVector(propAttenuation, attenuation);

                Matrix4x4 lightMatrix = Matrix4x4.TRS(light.transform.position, Quaternion.identity, Vector3.one * pointLight.Range * 2);
                comSet.lightCommand.DrawMesh(sphereMesh, lightMatrix, pointLightMaterial, 0, 0);

                continue;
            }

            CustomDirectionalLight directionalLight = light as CustomDirectionalLight;
            if (directionalLight)
            {
                comSet.lightCommand.SetGlobalColor(propLightColor, directionalLight.Color);

                Vector4 lightData = Vector4.zero;
                lightData.x = directionalLight.transform.forward.x;
                lightData.y = directionalLight.transform.forward.y;
                lightData.z = directionalLight.transform.forward.z;
                lightData.w = directionalLight.Intensity;
                comSet.lightCommand.SetGlobalVector(propLightData, lightData);


                //Matrix4x4 lightMatrix = Matrix4x4.TRS(light.transform.position, Quaternion.identity, Vector3.one * 1000);
                //comSet.lightCommand.DrawMesh(sphereMesh, lightMatrix, directionalLightMaterial, 0, 0);
                comSet.lightCommand.Blit(BuiltinRenderTextureType.None, propLightTexture, directionalLightMaterial,0);
                //comSet.lightCommand.Blit(BuiltinRenderTextureType.None, propLightTexture, directionalLightMaterial,0);

                continue;
            }
        }
       
        if (!applyMaterial)
            applyMaterial = new Material(applyShader);

        comSet.applyCommand.SetGlobalTexture("_LightTexture", propLightTexture);
        comSet.applyCommand.Blit(BuiltinRenderTextureType.CurrentActive, BuiltinRenderTextureType.CurrentActive, applyMaterial);
        comSet.lightCommand.ReleaseTemporaryRT(propLightTexture);

    }

}

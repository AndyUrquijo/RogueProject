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

	Material pointLightMaterial;
	Material directionalLightMaterial;
	Material applyMaterial;

	
    public Shader pointLightShader;
    public Shader directionalLightShader;
	public Shader applyShader;


	RenderTexture lightRT;
	RenderTexture backgroundRT;

    public Mesh sphereMesh; // used by point lights

    public Color Background;
    public Color Ambient;
    public float AmbientIntensity;

	public Texture2D RampTexture;

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

		if( lightRT == null )
		{
			lightRT = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			lightRT.Create();
		}

		if( backgroundRT == null )
		{
			backgroundRT = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
			backgroundRT.Create();
		}

        // A new set of commands is created with the latest light information
        // NOTE: Dumb&easy. Every command set is recreated on each call. 
        // Ideally, existing command lists could be reused and cleared only when needed.
        cam.RemoveAllCommandBuffers();
        CommandSet comSet = m_Cameras[cam] = new CommandSet();


		// Create Command Buffers

        comSet.lightCommand = new CommandBuffer();
        comSet.lightCommand.name = "Light Commands";
        cam.AddCommandBuffer(CameraEvent.AfterLighting, comSet.lightCommand);

        comSet.applyCommand = new CommandBuffer();
        comSet.applyCommand.name = "Apply Commands";
        cam.AddCommandBuffer(CameraEvent.BeforeForwardAlpha, comSet.applyCommand);


        int propLightColor = Shader.PropertyToID("_CustomLightColor");
        int propLightData = Shader.PropertyToID("_CustomLightData");
        int propAttenuation = Shader.PropertyToID("_CustomAttenuation");
        
		// Render Background Texture TODO: try simply rendering to active RT

		Color backgroundColor = Background;
		backgroundColor.a = 0;
		comSet.lightCommand.SetRenderTarget(backgroundRT);
		comSet.lightCommand.ClearRenderTarget(false,true, backgroundColor);


        comSet.lightCommand.SetRenderTarget(lightRT);
		Color ambientColor = Color.Lerp(Color.black, Ambient, AmbientIntensity);
        ambientColor.a = AmbientIntensity;
        
        comSet.lightCommand.ClearRenderTarget(false,true, ambientColor);

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

                comSet.lightCommand.Blit(BuiltinRenderTextureType.None, lightRT, directionalLightMaterial,0);
                
				continue;
            }
        }
       
        if (!applyMaterial)
		{
            applyMaterial = new Material(applyShader);
		}
		applyMaterial.SetTexture("_RampTexture",RampTexture);
        comSet.applyCommand.SetGlobalTexture("_LightTexture", lightRT);
        comSet.applyCommand.SetGlobalTexture("_EmissiveTexture", backgroundRT);


        comSet.applyCommand.Blit(BuiltinRenderTextureType.CurrentActive, BuiltinRenderTextureType.CurrentActive, applyMaterial);
		
	}

}

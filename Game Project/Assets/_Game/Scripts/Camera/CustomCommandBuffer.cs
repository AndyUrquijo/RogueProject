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

	void CreateRenderTexture( ref RenderTexture renderTexture, int width, int height, int depth, RenderTextureFormat format, RenderTextureReadWrite readwrite)
	{
		if( renderTexture == null || renderTexture.width != width || renderTexture.height != height )
		{
			renderTexture = new RenderTexture(width, height, depth, format, readwrite);
            //renderTexture.filterMode = FilterMode.Point;
            //renderTexture.antiAliasing = 0;
            //renderTexture.anisoLevel = 0;
            renderTexture.Create();
		}
	}

	void CreateCommand( ref CommandBuffer comBuf, Camera camera, string name, CameraEvent timeSlot)
	{
		comBuf = new CommandBuffer();
		comBuf.name = name;
		camera.AddCommandBuffer(timeSlot, comBuf);
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

		CreateRenderTexture(ref lightRT, cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
		CreateRenderTexture(ref backgroundRT, cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);



        // A new set of commands is created with the latest light information
        // NOTE: Dumb&easy. Every command set is recreated on each call. 
        // Ideally, existing command lists could be reused and cleared only when needed.
        cam.RemoveAllCommandBuffers();
        CommandSet comSet = m_Cameras[cam] = new CommandSet();

		CreateCommand(ref comSet.lightCommand,cam, "Light Command", CameraEvent.AfterLighting);
		CreateCommand(ref comSet.applyCommand,cam, "Apply Commands", CameraEvent.BeforeForwardAlpha);


		// Light Commands
		
        int propLightColor = Shader.PropertyToID("_CustomLightColor");
        int propLightData = Shader.PropertyToID("_CustomLightData");
        int propAttenuation = Shader.PropertyToID("_CustomAttenuation");
        

		// Render Background Texture
		Color backgroundColor = Background;
		backgroundColor.a = 0;
		comSet.lightCommand.SetRenderTarget(backgroundRT); //TODO: try simply rendering to active RT
		comSet.lightCommand.ClearRenderTarget(false,true, backgroundColor);


        comSet.lightCommand.SetRenderTarget(lightRT);
		Color ambientColor = Ambient;
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
       
		// Apply Commands
		
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

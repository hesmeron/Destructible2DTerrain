using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class ProceduralDrawFeature : ScriptableRendererFeature
{
    [SerializeField] 
    private Material material;
    [SerializeField]
    private Mesh mesh;
    DrawPass renderPass;

    /// <inheritdoc/>
    public override void Create()
    {
        renderPass = new DrawPass(material, mesh);
        renderPass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }
    
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(renderPass);
    }
}

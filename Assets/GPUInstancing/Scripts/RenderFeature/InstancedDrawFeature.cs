using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class InstancedDrawFeature : ScriptableRendererFeature
{
    [SerializeField] 
    private Material material;
    [SerializeField]
    private Mesh mesh;
    InstancedDrawPass renderPass;

    /// <inheritdoc/>
    public override void Create()
    {
        renderPass = new InstancedDrawPass(material, mesh);
        renderPass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }
    
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(renderPass);
    }
}

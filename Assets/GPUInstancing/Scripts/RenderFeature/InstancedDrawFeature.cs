using UnityEngine;
using UnityEngine.Rendering.Universal;

public class InstancedDrawFeature : ScriptableRendererFeature
{
    [SerializeField] 
    private ComputeShader cullingShader;    
    [SerializeField] 
    private Material material;
    [SerializeField]
    private Mesh mesh;
    
    InstancedDrawPass renderPass;
    private CullingPass cullingPass;

    /// <inheritdoc/>
    public override void Create()
    {
        cullingPass = new CullingPass(cullingShader);
        cullingPass.renderPassEvent = RenderPassEvent.BeforeRendering;
        renderPass = new InstancedDrawPass(material, mesh);
        renderPass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }
    
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(cullingPass);
        renderer.EnqueuePass(renderPass);
    }
}

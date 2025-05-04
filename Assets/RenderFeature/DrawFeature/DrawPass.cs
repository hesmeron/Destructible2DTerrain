using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

class DrawPass : ScriptableRenderPass
{
    private Material _material;
    private Mesh _mesh;

    public DrawPass(Material material, Mesh mesh)
    {
        _material = material;
        _mesh = mesh;
    }

    private class PassData
    {
        public Material material;
        public Mesh mesh;
    }
    
    static void ExecutePass(PassData data, RasterGraphContext context)
    {
        int shaderPass = data.material.FindPass("ForwardLit");
        context.cmd.DrawMesh(data.mesh, Matrix4x4.identity, data.material, 0, shaderPass);

    }
    
    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        const string passName = "Render Custom Pass";
            
        using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
        {
            passData.material = _material;
            passData.mesh = _mesh;
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
            builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture);
            builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
        }
    }
}
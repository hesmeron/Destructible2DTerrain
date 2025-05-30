using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

class InstancedDrawPass : ScriptableRenderPass
{
    private Material _material;
    private Mesh _mesh;

    public InstancedDrawPass(Material material, Mesh mesh)
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
        int shaderPass = data.material.FindPass("Unlit");
        for (int x = 0; x < 100; x++)
        {
            for (int z = 0; z < 100; z++)
            {
                Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(x*1.2f, 0, z*1.5f), Quaternion.identity, Vector3.one);
                context.cmd.DrawMesh(data.mesh, matrix, data.material, 0, shaderPass);
            }
        }

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
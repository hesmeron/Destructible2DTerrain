using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

class InstancedDrawPass : ScriptableRenderPass
{
    private static readonly int TransformationMatrices = Shader.PropertyToID("_TransformationMatrices");
    private static readonly int CulledMatrices = Shader.PropertyToID("_CulledMatrices");
    
    private Material _material;
    private Mesh _mesh;

    public InstancedDrawPass(Material material, Mesh mesh)
    {
        _material = material;
        _mesh = mesh;
    }

    private class PassData
    {
        public CullingFrameData cullingFrameData;
        public Material material;
        public Mesh mesh;
    }

    static void ExecutePass(PassData data, RasterGraphContext context)
    {
        GraphicsBuffer counterCopyBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, 1, sizeof(uint));
        GraphicsBuffer.CopyCount(data.cullingFrameData.CulledMatricesBuffer, counterCopyBuffer, 0);
        uint[] counterValueArray = new uint[1];
        counterCopyBuffer.GetData(counterValueArray);
        Debug.Log("Buffer counter " + counterValueArray[0]);
        int counterValue = (int) counterValueArray[0];
        int shaderPass = data.material.FindPass("Unlit");

        if (counterValue > 0)
        {
            MaterialPropertyBlock block = context.renderGraphPool.GetTempMaterialPropertyBlock();
            block.SetBuffer(TransformationMatrices, data.cullingFrameData.AllMatricesBuffer);
            block.SetBuffer(CulledMatrices, data.cullingFrameData.CulledMatricesBuffer);
            context.cmd.DrawMeshInstancedProcedural(data.mesh, 
                0, data.material,
                shaderPass,
                (int) counterValueArray[0],
                block );
        }
    }
    
    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        const string passName = "Render Custom Pass";
            
        using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
        {
            CullingFrameData cullingFrameData = frameData.Get<CullingFrameData>();
            passData.cullingFrameData = cullingFrameData;
            passData.material = _material;
            passData.mesh = _mesh;
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            builder.UseBuffer(passData.cullingFrameData.CulledMatricesBuffer);
            builder.UseBuffer(passData.cullingFrameData.AllMatricesBuffer);
            builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
            builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture);
            builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
        }
    }
}
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

class InstancedDrawPass : ScriptableRenderPass
{
    private static readonly int TransformationMatrices = Shader.PropertyToID("_TransformationMatrices");
    private static readonly int CulledMatrices = Shader.PropertyToID("_CulledMatrices");
    private static GraphicsBuffer _counterCopyBuffer;
    private static int _frameIndex = -1;
    
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
        public int Count;
    }

    static void ExecutePass(PassData data, RasterGraphContext context)
    {      
        _frameIndex = (_frameIndex + 1) % 2;
        GraphicsBuffer culledBuffer = InstancedDrawSystem.GetMatrixBuffer(_frameIndex);
        _counterCopyBuffer= new GraphicsBuffer(GraphicsBuffer.Target.Raw, 1, sizeof(uint));
        GraphicsBuffer.CopyCount(culledBuffer, _counterCopyBuffer, 0);
        uint[] counterValueArray = new uint[1];
        _counterCopyBuffer.GetData(counterValueArray);
//        Debug.Log("Buffer counter " + counterValueArray[0]);
        int counterValue = (int) counterValueArray[0];

        int shaderPass = data.material.FindPass("Unlit");

        if (counterValue > 0)
        {
            MaterialPropertyBlock block = context.renderGraphPool.GetTempMaterialPropertyBlock();
            block.SetBuffer(TransformationMatrices, data.cullingFrameData.AllMatricesBuffer);
            block.SetBuffer(CulledMatrices, culledBuffer);
            context.cmd.DrawMeshInstancedProcedural(data.mesh, 
                0, data.material,
                shaderPass,
                counterValue,
                block );
        }
    }
    
    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        const string passName = "Render Custom Pass";
        CullingFrameData cullingFrameData = frameData.Get<CullingFrameData>();

        
        using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
        {

            passData.cullingFrameData = cullingFrameData;
            passData.material = _material;
            passData.mesh = _mesh;
            //passData.Count = counterValue;
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            builder.UseBuffer(passData.cullingFrameData.CulledMatricesBuffer);
            builder.UseBuffer(passData.cullingFrameData.AllMatricesBuffer);
            builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
            builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture);
            builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
        }
    }
}
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class CullingPass : ScriptableRenderPass
{
    private class PassData
    {
        
    }
    
    static void ExecutePass(PassData data, ComputeGraphContext context)
    {

    }
    
    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        const string passName = "Culling Pass";
        Matrix4x4[] matrices = new Matrix4x4[100 * 100];
        
        for (int x = 0; x < 100; x++)
        {
            for (int z = 0; z < 100; z++)
            {
                Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(x*1.2f, 0, z*1.5f), Quaternion.identity, Vector3.one);
                matrices[x * 100 + z] = matrix;
            }
        }

        CullingFrameData data = frameData.Create<CullingFrameData>();
        GraphicsBuffer matrixBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured | GraphicsBuffer.Target.IndirectArguments, 
            matrices.Length, sizeof(float)*16);
        matrixBuffer.SetData(matrices);
        BufferHandle matrixBufferHandle = renderGraph.ImportBuffer(matrixBuffer);
        data.CulledMatricesBuffer = matrixBufferHandle;
        data.InstanceCount = matrices.Length;
        using (var builder = renderGraph.AddComputePass<PassData>(passName, out var passData))
        {
            builder.SetRenderFunc((PassData data, ComputeGraphContext context) => ExecutePass(data, context));
        }
    }
}

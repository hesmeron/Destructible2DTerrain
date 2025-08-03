//using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class CullingPass : ScriptableRenderPass
{
    private static readonly int InMatrices = Shader.PropertyToID("IN_Matrices");
    private static readonly int OutCulledMatrices = Shader.PropertyToID("OUT_CulledMatrices");
    private ComputeShader _cullingShader;
    private ComputeShader _resetCounterShader;
    private class PassData
    {
        public ComputeShader CullingShader;
        public BufferHandle MatricesToCullBuffer;        
        public BufferHandle CulledMatricesBuffer;
    }

    public CullingPass(ComputeShader cullingShader)
    {
        _cullingShader = cullingShader;
    }
    
    static void ExecutePass(PassData data, ComputeGraphContext context)
    {
        Debug.Log("Execute culling pass");
        context.cmd.SetBufferCounterValue(data.CulledMatricesBuffer, 0);
        ComputeShader shader = data.CullingShader;
        shader.SetBuffer(0, InMatrices, data.MatricesToCullBuffer);
        shader.SetBuffer(0, OutCulledMatrices, data.CulledMatricesBuffer);
        
        //context.cmd.d
        context.cmd.DispatchCompute(shader, 0, 10000, 1, 1);
        GraphicsBuffer counterCopyBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, 1, sizeof(uint));
        GraphicsBuffer.CopyCount(data.CulledMatricesBuffer, counterCopyBuffer, 0);
        uint[] counterValueArray = new uint[1];
        counterCopyBuffer.GetData(counterValueArray);
        Debug.Log("Buffer counter A" + counterValueArray[0]);
    }
    
    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {        
        Debug.Log("Record culling pass");
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
        
        GraphicsBuffer inputMatricesBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured | GraphicsBuffer.Target.IndirectArguments, 
            matrices.Length, sizeof(float)*16);
        inputMatricesBuffer.SetData(matrices);
        inputMatricesBuffer.name = "InputMatrixBuffer";

        GraphicsBuffer outputBuffer = InstancedDrawSystem.GetMatrixBuffer();
        BufferHandle inputBufferHandle = renderGraph.ImportBuffer(inputMatricesBuffer);
        BufferHandle outputBufferHandle = renderGraph.ImportBuffer(outputBuffer);
        CullingFrameData cullingFrameData = frameData.Create<CullingFrameData>();
        cullingFrameData.CulledMatricesBuffer = outputBufferHandle;
        cullingFrameData.AllMatricesBuffer = inputBufferHandle;
        cullingFrameData.InstanceCount = 10000;//matrices.Length;


        using (var builder = renderGraph.AddComputePass<PassData>(passName, out var passData))
        {
            passData.CullingShader = _cullingShader;
            passData.MatricesToCullBuffer = inputBufferHandle;
            passData.CulledMatricesBuffer = outputBufferHandle;
            builder.UseBuffer(passData.MatricesToCullBuffer);
            builder.UseBuffer(passData.CulledMatricesBuffer, AccessFlags.ReadWrite);
            builder.SetRenderFunc((PassData passData, ComputeGraphContext context) => ExecutePass(passData, context));
        }
    }
}

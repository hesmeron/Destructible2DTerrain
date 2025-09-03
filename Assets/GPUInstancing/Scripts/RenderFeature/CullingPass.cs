//using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class CullingPass : ScriptableRenderPass
{
    private static readonly int InMatrices = Shader.PropertyToID("IN_Matrices");
    private static readonly int InFrustumPlaness = Shader.PropertyToID("IN_FrustumPlanes");
    private static readonly int InRadius = Shader.PropertyToID("IN_Radius");
    private static readonly int OutCulledMatrices = Shader.PropertyToID("OUT_CulledMatrices");
    private ComputeShader _cullingShader;
    private ComputeShader _resetCounterShader;
    private static ComputeBuffer planesBuffer;
    private static GraphicsBuffer inputMatricesBuffer;
    private static int _frameIndex = 0;
    
    
    private class PassData
    {
        public CullingFrameData frameData;
        public ComputeShader CullingShader;
        public BufferHandle MatricesToCullBuffer;        
        public BufferHandle CulledMatricesBuffer;
    }

    public CullingPass(ComputeShader cullingShader)
    {
        Debug.Log("Create culling pass");
        _cullingShader = cullingShader;
        planesBuffer = new ComputeBuffer(6, sizeof(float) * 4);
        inputMatricesBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured | GraphicsBuffer.Target.IndirectArguments, 
            10000, sizeof(float)*16);
    }
    
    static void ExecutePass(PassData data, ComputeGraphContext context)
    {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        Vector4[] planeVectors = new Vector4[6];
        for (int i = 0; i < 6; i++)
        {
            Plane p = frustumPlanes[i];
            planeVectors[i] = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        }


        planesBuffer.SetData(planeVectors);
        
        Debug.Log("Execute culling pass");
        context.cmd.SetBufferCounterValue(data.CulledMatricesBuffer, 0);
        ComputeShader shader = data.CullingShader;
        int kernel = shader.FindKernel("CSMain");
        shader.SetFloat(InRadius, 3);
        shader.SetBuffer(kernel, InFrustumPlaness, planesBuffer);
        shader.SetBuffer(kernel, InMatrices, data.MatricesToCullBuffer);
        shader.SetBuffer(kernel, OutCulledMatrices, data.CulledMatricesBuffer);
        context.cmd.DispatchCompute(shader, 0, 10000, 1, 1);
        
        GraphicsBuffer _counterCopyBuffer= new GraphicsBuffer(GraphicsBuffer.Target.Raw, 1, sizeof(uint));
        GraphicsBuffer.CopyCount(data.CulledMatricesBuffer, _counterCopyBuffer, 0);
        uint[] counterValueArray = new uint[1];
        _counterCopyBuffer.GetData(counterValueArray);
        //Debug.Log("Buffer counter " + counterValueArray[0]);
        int counterValue = (int) counterValueArray[0];
        data.frameData.Count = counterValue;
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
        
        inputMatricesBuffer.SetData(matrices);
        inputMatricesBuffer.name = "InputMatrixBuffer";
        _frameIndex = (_frameIndex + 1) % 2;
        GraphicsBuffer outputBuffer = InstancedDrawSystem.GetMatrixBuffer(_frameIndex);
        Assert.IsNotNull(outputBuffer);
        //GraphicsBuffer outputBuffer = InstancedDrawSystem.GetMatrixBuffer();
        BufferHandle inputBufferHandle = renderGraph.ImportBuffer(inputMatricesBuffer);
        BufferHandle outputBufferHandle = renderGraph.ImportBuffer(outputBuffer);
        
        CullingFrameData cullingFrameData = frameData.GetOrCreate<CullingFrameData>();
        cullingFrameData.CulledMatricesBuffer = outputBufferHandle;
        cullingFrameData.AllMatricesBuffer = inputBufferHandle;
        //cullingFrameData.InstanceCount = 10000;//matrices.Length;


        using (var builder = renderGraph.AddComputePass<PassData>(passName, out var passData))
        {
            passData.CullingShader = _cullingShader;
            passData.frameData = cullingFrameData;
            passData.MatricesToCullBuffer = inputBufferHandle;
            passData.CulledMatricesBuffer = outputBufferHandle;
            builder.UseBuffer(passData.MatricesToCullBuffer);
            builder.UseBuffer(passData.CulledMatricesBuffer, AccessFlags.ReadWrite);
            builder.SetRenderFunc((PassData passData, ComputeGraphContext context) => ExecutePass(passData, context));
        }
    }
}

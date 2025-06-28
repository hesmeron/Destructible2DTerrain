using UnityEngine;

public static class InstancedDrawSystem
{
    public static GraphicsBuffer GetMatrixBuffer()
    {
        Matrix4x4[] matrices = new Matrix4x4[100 * 100];
        
        for (int x = 0; x < 100; x++)
        {
            for (int z = 0; z < 100; z++)
            {
                Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(x*1.2f, 0, z*1.5f), Quaternion.identity, Vector3.one);
                matrices[x * 100 + z] = matrix;
            }
        }

        GraphicsBuffer matrixBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured | GraphicsBuffer.Target.IndirectArguments, 
            matrices.Length, sizeof(float)*16);
        matrixBuffer.SetData(matrices);
        matrixBuffer.SetCounterValue((uint) matrices.Length);
        return matrixBuffer;
    }    
    
    public static ComputeBuffer GetMatrixComputeBuffer()
    {
        Matrix4x4[] matrices = new Matrix4x4[100 * 100];
        
        for (int x = 0; x < 100; x++)
        {
            for (int z = 0; z < 100; z++)
            {
                Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(x*1.2f, 0, z*1.5f), Quaternion.identity, Vector3.one);
                matrices[x * 100 + z] = matrix;
            }
        }

        ComputeBuffer matrixBuffer = new ComputeBuffer(matrices.Length, sizeof(float)*16, ComputeBufferType.IndirectArguments);
        matrixBuffer.SetData(matrices);
        matrixBuffer.SetCounterValue((uint) matrices.Length);
        return matrixBuffer;
    }
}

using UnityEngine;

public static class InstancedDrawSystem
{
    private static GraphicsBuffer _matrixBuffer;
    public static GraphicsBuffer GetMatrixBuffer()
    {
        if (_matrixBuffer == null)
        {
            _matrixBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured 
                                               | GraphicsBuffer.Target.Append, 
                                        10000, sizeof(int));
            _matrixBuffer.name = "MatrixBuffer";
        }
        return _matrixBuffer;
    }
}

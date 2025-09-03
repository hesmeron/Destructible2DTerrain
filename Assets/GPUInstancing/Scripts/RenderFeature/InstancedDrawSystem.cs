using Unity.VisualScripting;
using UnityEngine;

public static class InstancedDrawSystem
{
    
    private static GraphicsBuffer _matrixBuffer;
    private static GraphicsBuffer[] _swapChain;
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
    
    public static GraphicsBuffer GetMatrixBuffer(int index)
    {
        if (_swapChain == null)
        {
            _swapChain = new GraphicsBuffer[2];
        }

        if (_swapChain[index] == null)
        {
            GraphicsBuffer buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured 
                                               | GraphicsBuffer.Target.Append, 
                10000, sizeof(int));
            buffer.name = "MatrixBuffer" + index;
            _swapChain[index] = buffer;
        }
        return _swapChain[index];
    }
}

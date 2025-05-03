using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

class DrawPass : ScriptableRenderPass
{
    // This class stores the data needed by the RenderGraph pass.
    // It is passed as a parameter to the delegate function that executes the RenderGraph pass.
    private class PassData
    {
    }

    // This static method is passed as the RenderFunc delegate to the RenderGraph render pass.
    // It is used to execute draw commands.
    static void ExecutePass(PassData data, RasterGraphContext context)
    {
        
    }

    // RecordRenderGraph is where the RenderGraph handle can be accessed, through which render passes can be added to the graph.
    // FrameData is a context container through which URP resources can be accessed and managed.
    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        const string passName = "Render Custom Pass";
            
        using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
        {

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
            builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
        }
    }
}
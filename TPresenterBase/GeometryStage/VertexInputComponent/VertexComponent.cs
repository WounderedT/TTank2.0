using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Render
{
    internal abstract class MyComponent
    {
        private static int NextIndex(Dictionary<string, int> dict, string name)
        {
            int val = 0;
            if (dict.TryGetValue(name, out val))
                dict[name] = val + 1;
            else
                dict[name] = 1;

            return val;
        }

        public static ShaderMacro[] GetComponentMacros(string declaration, string transferCode)
        {
            return new ShaderMacro[] { new ShaderMacro("VERTEX_COMPONENTS_DECLARATIONS", declaration), new ShaderMacro("TRANSFER_VERTEX_COMPONENTS", transferCode) };
        }

        protected static void AddSingle(string name, string variable, Format format, VertexInputComponent component, 
            List<InputElement> list, Dictionary<string, int> dict, StringBuilder declaraton)
        {
            var classification = component.Freq == VertexInputComponentFreq.PER_VERTEX ? InputClassification.PerVertexData : InputClassification.PerInstanceData;
            var freq = component.Freq == VertexInputComponentFreq.PER_VERTEX ? 0 : 1;
            var index = NextIndex(dict, name);

            list.Add(new InputElement(name, index, format, InputElement.AppendAligned, component.Slot, classification, freq));
            declaraton.AppendFormat("{0} : {1}{2};\\\n", variable, name, index);
            //declaraton.AppendFormat("{0} : {1};\\\n", variable, name);
        }

        internal abstract void AddComponent(VertexInputComponent component, List<InputElement> list, Dictionary<string, int> dict, StringBuilder declaration, StringBuilder code);
    }
    
    internal sealed class BlendIndicesComponent : MyComponent
    {
        internal override void AddComponent(VertexInputComponent component, List<InputElement> list, Dictionary<string, int> dict, StringBuilder declaration, StringBuilder code)
        {
            AddSingle("BLENDINDICES", "uint4 blend_indices0", Format.R32G32B32A32_UInt, component, list, dict, declaration);

            code.Append("__blend_indices0 = input.blend_indices0;\\\n");
        }
    }

    internal sealed class BlendWeightsComponent : MyComponent
    {
        internal override void AddComponent(VertexInputComponent component, List<InputElement> list, Dictionary<string, int> dict, StringBuilder declaration, StringBuilder code)
        {
            AddSingle("BLENDWEIGHT", "float4 blend_weights0", Format.R32G32B32A32_Float, component, list, dict, declaration);
            AddSingle("BLENDWEIGHT", "float4 blend_weights1", Format.R32G32B32A32_Float, component, list, dict, declaration);

            code.Append("__blend_weights0 = input.blend_weights0;\\\n");
            code.Append("__blend_weights1 = input.blend_weights1;\\\n");
        }
    }

    internal sealed class Color4Component : MyComponent
    {
        internal override void AddComponent(VertexInputComponent component, List<InputElement> list, Dictionary<string, int> dict, StringBuilder declaration, StringBuilder code)
        {
            AddSingle("COLOR", "float4 color", Format.R8G8B8A8_UNorm, component, list, dict, declaration);
            code.Append("__color = input.color;\\\n");
        }
    }

    internal sealed class NormalComponent : MyComponent
    {
        internal override void AddComponent(VertexInputComponent component, List<InputElement> list, Dictionary<string, int> dict, StringBuilder declaration, StringBuilder code)
        {
            AddSingle("NORMAL", "float3 normal", Format.R32G32B32_Float, component, list, dict, declaration);
            code.Append("__normal = input.normal;\\\n");
        }
    }

    internal sealed class Position3Component : MyComponent
    {
        internal override void AddComponent(VertexInputComponent component, List<InputElement> list, Dictionary<string, int> dict, StringBuilder declaration, StringBuilder code)
        {
            AddSingle("SV_Position", "float4 position", Format.R32G32B32_Float, component, list, dict, declaration);
            code.Append("__position_object = float4(input.position, 1);\\\n");
        }
    }

    internal sealed class TextureCoordinate0Component : MyComponent
    {
        internal override void AddComponent(VertexInputComponent component, List<InputElement> list, Dictionary<string, int> dict, StringBuilder declaration, StringBuilder code)
        {
            AddSingle("TEXCOORD", "float2 textcoord0", Format.R32G32_Float, component, list, dict, declaration);
            code.Append("__textcoord0 = input.textcoord0;\\\n");
        }
    }

    partial class MyVertexInputLayout
    {
        internal static Dictionary<VertexInputComponentType, MyComponent> mapComponent = new Dictionary<VertexInputComponentType, MyComponent>();

        static void InitComponentsMap()
        {
            mapComponent[VertexInputComponentType.BLEND_INDICES] = new BlendIndicesComponent();
            mapComponent[VertexInputComponentType.BLEND_WEIGHTS] = new BlendWeightsComponent();
            mapComponent[VertexInputComponentType.COLOR_4] = new Color4Component();
            mapComponent[VertexInputComponentType.POSITION3] = new Position3Component();
            mapComponent[VertexInputComponentType.NORMAL] = new NormalComponent();
            mapComponent[VertexInputComponentType.TEXCOORD0] = new TextureCoordinate0Component();
        }
    }
}

using SharpDX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Render
{
    class ShaderBundle
    {
        public VertexShaderId VertexShader { get; private set; }
        public PixelShaderId PixelShader { get; private set; }
        public InputLayoutId InputLayout { get; private set; }

        public ShaderBundle(VertexShaderId vs, PixelShaderId ps, InputLayoutId il)
        {
            VertexShader = vs;
            PixelShader = ps;
            InputLayout = il;
        }
    }

    static class ShaderResolver
    {
        static Dictionary<StringId, ShaderBundle> bundlesCache = new Dictionary<StringId, ShaderBundle>();

        public static ShaderBundle GetShaderBundle(string shaderBundleName, bool isAnimated, MyShaderFlags flags)
        {
            StringId key = StringId.GetOrCompute(shaderBundleName);
            if (bundlesCache.ContainsKey(key))
                return bundlesCache[key];

            string vsFile = GetShaderPath(ShaderType.SHADER_TYPE_VERTEX);
            string psFile = GetShaderPath(ShaderType.SHADER_TYPE_PIXEL);

            List<ShaderMacro> macros = new List<ShaderMacro>();
            MyMaterialShader.AddMaterialShaderFlagMacro(macros, flags);

            VertexLayoutId vl = MyVertexLayouts.GetLayout(GetShaderVertexInputComponents(isAnimated));

            VertexShaderId vs = MyShaders.CreateVertexShader(vsFile, macros.ToArray());
            PixelShaderId ps = MyShaders.CreatePixelShader(psFile, macros.ToArray());
            InputLayoutId il = MyShaders.CreateInputLayout(vs.Bytecode, vl);

            ShaderBundle bundle = new ShaderBundle(vs, ps, il);
            bundlesCache.Add(key, bundle);
            return bundle;
        }

        enum ShaderType
        {
            SHADER_TYPE_VERTEX,
            SHADER_TYPE_PIXEL,
        }

        static string GetShaderPath(ShaderType type)
        {
            switch (type)
            {
                case ShaderType.SHADER_TYPE_VERTEX:
                    return "Geometry\\VertexShader.hlsl";
                case ShaderType.SHADER_TYPE_PIXEL:
                    return "Geometry\\BlinnPhong.hlsl";
                default:
                    return string.Empty;
            }
        }

        static VertexInputComponent[] GetShaderVertexInputComponents(bool isAnimated)
        {
            List<VertexInputComponent> ic = new List<VertexInputComponent>();
            if (isAnimated)
            {
                ic.Add(new VertexInputComponent(VertexInputComponentType.POSITION3));
                ic.Add(new VertexInputComponent(VertexInputComponentType.NORMAL));
                ic.Add(new VertexInputComponent(VertexInputComponentType.TEXCOORD0));
                ic.Add(new VertexInputComponent(VertexInputComponentType.BLEND_INDICES));
                ic.Add(new VertexInputComponent(VertexInputComponentType.BLEND_WEIGHTS));
            }
            else
            {
                ic.Add(new VertexInputComponent(VertexInputComponentType.POSITION3));
                ic.Add(new VertexInputComponent(VertexInputComponentType.NORMAL));
                ic.Add(new VertexInputComponent(VertexInputComponentType.TEXCOORD0));
            }
            return ic.ToArray();
        }
    }
}

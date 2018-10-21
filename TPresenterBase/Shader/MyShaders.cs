using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Windows;
using SharpDX.DXGI;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;

using TPresenter.Filesystem;

using Buffer = SharpDX.Direct3D11.Buffer;
using TPresenter;
using System.IO;
using SharpDX.Direct3D;

namespace TPresenter.Render
{
    internal enum MyShaderProfile
    {
        vs_5_0,
        ps_5_0,

        count
    }

    public class MyShaders : SharpDX.Toolkit.Component
    {
        private struct MyBytecode
        {
            internal byte[] Bytecode;
        }

        private struct InputLayoutInfo
        {
            internal ShaderBytecodeId MyBytecode;
            internal VertexLayoutId VertexLayoutId;
        }

        static InputLayout[] InputLayoutObjects = new InputLayout[64];
        static MyList<InputLayoutInfo> InputLayouts = new MyList<InputLayoutInfo>(64);

        static MyList<MyBytecode> Bytecodes = new MyList<MyBytecode>(512);
        internal static Dictionary<ShaderBytecodeId, MyShaderCompilationInfo> Shaders = new Dictionary<ShaderBytecodeId, MyShaderCompilationInfo>();

        static VertexShader[] VertexShaderObjects = new VertexShader[128];
        internal static MyList<MyShaderInfo> VertexShaders = new MyList<MyShaderInfo>(128);

        static PixelShader[] PixelShaderObjects = new PixelShader[128];
        internal static MyList<MyShaderInfo> PixelShaders = new MyList<MyShaderInfo>(128);

        public static string ShadersPath
        {
            get
            {
                return FileProvider.ShadersPath;
            }
        }

        internal static VertexShaderId CreateVertexShader(string file, ShaderMacro[] macros = null)
        {
            var bytecode = new ShaderBytecodeId { Index = Bytecodes.Allocate() };
            var id = new VertexShaderId { Index = VertexShaders.Allocate() };
            Shaders[bytecode] = new MyShaderCompilationInfo
            {
                File = StringId.GetOrCompute(file),
                Profile = MyShaderProfile.vs_5_0,
#if DEBUG
                Flags = ShaderFlags.Debug,
#else
                Flags = ShaderFlags.None,
#endif
                Macros = macros,
            };

            VertexShaders.Data[id.Index] = new MyShaderInfo
            {
                Bytecode = bytecode,
            };

            if (VertexShaderObjects[id.Index] != null)
                VertexShaderObjects[id.Index].Dispose();

            Compile(bytecode);
            VertexShaderObjects[id.Index] = new VertexShader(Render11.Direct3DDevice, GetBytecode(bytecode));
            VertexShaderObjects[id.Index].DebugName = file;

            return id;
        }

        internal static PixelShaderId CreatePixelShader(string file, ShaderMacro[] macros = null)
        {
            var bytecode = new ShaderBytecodeId { Index = Bytecodes.Allocate() };
            var id = new PixelShaderId { Index = PixelShaders.Allocate() };
            Shaders[bytecode] = new MyShaderCompilationInfo
            {
                File = StringId.GetOrCompute(file),
                Profile = MyShaderProfile.ps_5_0,
#if DEBUG
                Flags = ShaderFlags.Debug,
#else
                Flags = ShaderFlags.None,
#endif
                Macros = macros,
            };

            PixelShaders.Data[id.Index] = new MyShaderInfo
            {
                Bytecode = bytecode,
            };

            if (PixelShaderObjects[id.Index] != null)
                PixelShaderObjects[id.Index].Dispose();

            Compile(bytecode);

            PixelShaderObjects[id.Index] = new PixelShader(Render11.Direct3DDevice, GetBytecode(bytecode));
            PixelShaderObjects[id.Index].DebugName = file;

            return id;
        }

        internal static InputLayoutId CreateInputLayout(ShaderBytecodeId bytecode, VertexLayoutId layout)
        {
            var id = new InputLayoutId { Index = InputLayouts.Allocate() };

            InputLayouts.Data[id.Index] = new InputLayoutInfo
            {
                MyBytecode = bytecode,
                VertexLayoutId = layout,
            };

            if (InputLayoutObjects[id.Index] != null)
                InputLayoutObjects[id.Index].Dispose();

            InputLayoutObjects[id.Index] = new InputLayout(Render11.Direct3DDevice, GetBytecode(bytecode), MyVertexLayouts.GetElements(layout));

            return id;
        }

        internal static void Compile(ShaderBytecodeId bytecode)
        {
            var info = Shaders[bytecode];
            var result = ShaderBytecode.CompileFromFile(
                Path.Combine(ShadersPath, info.File.String),
                ProfileToEntryPoint(info.Profile),
                ProfileToString(info.Profile),
                info.Flags,
                defines: info.Macros,
                include: new MyIncludeProcessor(info.File.String)
                );
            if (result.HasErrors)
            {
                //log compilation error.
            }
            Bytecodes.Data[bytecode.Index].Bytecode = result.Bytecode.Data;
        }

        internal static byte[] GetBytecode(ShaderBytecodeId id)
        {
            return Bytecodes.Data[id.Index].Bytecode;
        }

        internal static InputLayout GetInputLayout(InputLayoutId id)
        {
            return InputLayoutObjects[id.Index];
        }

        internal static ShaderBytecode GetShaderBytecode(ShaderBytecodeId id)
        {
            return new ShaderBytecode(Bytecodes.Data[id.Index].Bytecode);
        }

        internal static VertexShader GetVertexShader(VertexShaderId id)
        {
            return VertexShaderObjects[id.Index];
        }

        internal static PixelShader GetPixelShader(PixelShaderId id)
        {
            return PixelShaderObjects[id.Index];
        }

        internal static string ProfileToString(MyShaderProfile profile)
        {
            switch (profile)
            {
                case MyShaderProfile.ps_5_0:
                    return "ps_5_0";
                case MyShaderProfile.vs_5_0:
                    return "vs_5_0";
            }
            return null;
        }

        internal static string ProfileToEntryPoint(MyShaderProfile profile)
        {
            switch (profile)
            {
                case MyShaderProfile.ps_5_0:
                    return "pixel_shader";
                case MyShaderProfile.vs_5_0:
                    return "vertex_shader";
            }
            return null;
        }

        private class MyIncludeProcessor : Include
        {
            private string m_basePath;

            internal MyIncludeProcessor(string filepath)
            {
                string basePath = null;
                if (filepath != null)
                    basePath = Path.GetDirectoryName(filepath);

                m_basePath = basePath;
            }

            public void Close(Stream stream)
            {
                stream.Close();
            }

            public Stream Open(IncludeType type, string fileName, Stream parentStream)
            {
                string fullFileName;
                if(m_basePath != null)
                {
                    fullFileName = Path.Combine(ShadersPath, m_basePath, fileName);
                    if(!File.Exists(fullFileName))
                        fullFileName = Path.Combine(ShadersPath, fileName);
                }
                else
                    fullFileName = Path.Combine(ShadersPath, fileName);

                return new FileStream(fullFileName, FileMode.Open, FileAccess.Read);
            }

            public void Dispose()
            {
            }

            public IDisposable Shadow { get; set; }
        }
    }


}

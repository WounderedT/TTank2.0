using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Render
{
    struct MyShaderInfo
    {
        internal ShaderBytecodeId Bytecode;
        internal string File;
    }

    struct MyShaderCompilationInfo
    {
        internal StringId File;
        internal MyShaderProfile Profile;
        internal ShaderFlags Flags;
        internal ShaderMacro[] Macros;
    }

    struct ShaderBytecodeId
    {
        internal int Index;

        public static bool operator ==(ShaderBytecodeId x, ShaderBytecodeId y)
        {
            return x.Index == y.Index;
        }

        public static bool operator !=(ShaderBytecodeId x, ShaderBytecodeId y)
        {
            return x.Index != y.Index;
        }

        internal static readonly ShaderBytecodeId NULL = new ShaderBytecodeId { Index = -1 };
    }

    struct InputLayoutId
    {
        internal int Index;

        public static bool operator ==(InputLayoutId x, InputLayoutId y)
        {
            return x.Index == y.Index;
        }

        public static bool operator !=(InputLayoutId x, InputLayoutId Y)
        {
            return x.Index != Y.Index;
        }

        internal static readonly InputLayoutId NULL = new InputLayoutId { Index = -1 };

        public static implicit operator InputLayout(InputLayoutId id)
        {
            return MyShaders.GetInputLayout(id);
        }
    }

    struct VertexShaderId
    {
        internal int Index;

        public static bool operator ==(VertexShaderId x, VertexShaderId y)
        {
            return x.Index == y.Index;
        }

        public static bool operator !=(VertexShaderId x, VertexShaderId Y)
        {
            return x.Index != Y.Index;
        }

        internal static readonly VertexShaderId NULL = new VertexShaderId { Index = -1 };

        public static implicit operator VertexShader(VertexShaderId id)
        {
            return MyShaders.GetVertexShader(id);
        }

        internal ShaderBytecodeId Bytecode { get { return MyShaders.VertexShaders.Data[Index].Bytecode; } }
    }

    struct PixelShaderId
    {
        internal int Index;

        public static bool operator ==(PixelShaderId x, PixelShaderId y)
        {
            return x.Index == y.Index;
        }

        public static bool operator !=(PixelShaderId x, PixelShaderId Y)
        {
            return x.Index != Y.Index;
        }

        internal static readonly PixelShaderId NULL = new PixelShaderId { Index = -1 };

        public static implicit operator PixelShader(PixelShaderId id)
        {
            return MyShaders.GetPixelShader(id);
        }

        internal ShaderBytecodeId Bytecode { get { return MyShaders.PixelShaders.Data[Index].Bytecode; } }
    }
}

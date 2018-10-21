using SharpDX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter
{
    [Flags]
    enum MyShaderFlags
    {
        NONE = 0,
        USE_SKINNING = 1 << 1,
    }

    static class MyMaterialShader
    {
        internal static void AddMaterialShaderFlagMacro(List<ShaderMacro> list, MyShaderFlags flags)
        {
            if ((flags & MyShaderFlags.USE_SKINNING) > 0)
                list.Add(new ShaderMacro("USE_SKINNING", null));
        }
    }
}

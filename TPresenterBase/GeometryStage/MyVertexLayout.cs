using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Render;

namespace TPresenter.Render
{
    public enum VertexInputComponentType
    {
        COLOR_4,

        NORMAL,
        POSITION3,

        TEXCOORD0,

        BLEND_INDICES,
        BLEND_WEIGHTS,
    }

    public enum VertexInputComponentFreq
    {
        PER_VERTEX,
        PER_INSTANCE
    }

    struct VertexInputComponent
    {
        internal VertexInputComponentType Type;
        internal VertexInputComponentFreq Freq;
        internal int Slot;

        internal VertexInputComponent(VertexInputComponentType type)
        {
            Type = type;
            Slot = 0;
            Freq = VertexInputComponentFreq.PER_VERTEX;
        }

        internal VertexInputComponent(VertexInputComponentType type, VertexInputComponentFreq freq)
        {
            Type = type;
            Slot = 0;
            Freq = freq;
        }

        internal VertexInputComponent(VertexInputComponentType type, int slot)
        {
            Type = type;
            Slot = slot;
            Freq = VertexInputComponentFreq.PER_VERTEX;
        }

        internal VertexInputComponent(VertexInputComponentType type, VertexInputComponentFreq freq, int slot)
        {
            Type = type;
            Slot = slot;
            Freq = freq;
        }

        public override string ToString()
        {
            return String.Format("<{0}, {1}, {2}>", Type, Slot, Freq);
        }

        public int CompareTo(VertexInputComponent item)
        {
            if (Type == item.Type)
            {
                if (Slot == item.Slot)
                    return Freq - item.Freq;
                else
                    return Slot - item.Slot;
            }
            else
                return Type - item.Type;
        }
    }

    struct VertexLayoutId
    {
        internal int Index;

        public static bool operator ==(VertexLayoutId x, VertexLayoutId y)
        {
            return x.Index == y.Index;
        }

        public static bool operator !=(VertexLayoutId x, VertexLayoutId Y)
        {
            return x.Index != Y.Index;
        }

        internal static readonly VertexLayoutId NULL = new VertexLayoutId { Index = -1 };

        public override int GetHashCode()
        {
            return Index;
        }

        internal InputElement[] Elements { get { return MyVertexLayouts.GetElements(this); } }
        internal MyVertexLayoutInfo Info { get { return MyVertexLayouts.Layouts.Data[Index]; } }
        internal bool HasBonesInfo { get { return MyVertexLayouts.Layouts.Data[Index].HasBonesInfo; } }
    }

    struct MyVertexLayoutInfo
    {
        internal VertexInputComponent[] Components;
        internal InputElement[] Elements;
        internal ShaderMacro[] Macros;
        internal bool HasBonesInfo;
    }

    static class MyVertexLayouts
    {
        static Dictionary<int, VertexLayoutId> HashIndex = new Dictionary<int, VertexLayoutId>();
        internal static MyList<MyVertexLayoutInfo> Layouts = new MyList<MyVertexLayoutInfo>(64);

        internal static VertexLayoutId Empty;

        internal static InputElement[] GetElements(VertexLayoutId id)
        {
            return Layouts.Data[id.Index].Elements;
        }

        static MyVertexLayouts()
        {
            var id = new VertexLayoutId { Index = Layouts.Allocate() };
            HashIndex[0] = id;
            Layouts.Data[id.Index] = new MyVertexLayoutInfo()
            {
                Elements = new InputElement[0],
                Macros = new ShaderMacro[0],
            };

            Empty = id;
        }

        internal static VertexLayoutId GetLayout(params VertexInputComponentType[] components)
        {
            return GetLayout(components.Select(c => new VertexInputComponent(c)).ToArray());
        }

        internal static VertexLayoutId GetLayout(VertexLayoutId firstLayout, VertexLayoutId secondLayout)
        {
            VertexLayoutId combinedLayout = VertexLayoutId.NULL;
            List<VertexInputComponent> firstComponents = new List<VertexInputComponent>(firstLayout.Info.Components);
            VertexInputComponent[] secondComponents = secondLayout.Info.Components;

            firstComponents.AddArray(secondComponents);

            Debug.Assert(firstComponents.Count == firstComponents.Capacity);
            firstComponents.Capacity = firstComponents.Count;

            combinedLayout = GetLayout(firstComponents.GetInternalArray());
            return combinedLayout;
        }

        internal static VertexLayoutId GetLayout(params VertexInputComponent[] components)
        {
            if (components == null || components.Length == 0)
                return Empty;

            var hash = 0;
            foreach (var component in components)
                HashHelpers.Combine(ref hash, component.GetHashCode());

            if (HashIndex.ContainsKey(hash))
                return HashIndex[hash];

            var id = new VertexLayoutId { Index = Layouts.Allocate() };
            HashIndex[hash] = id;

            var declarationBuilder = new StringBuilder();
            var sourceBuilder = new StringBuilder();
            var semanticDict = new Dictionary<string, int>();

            var elemensList = new List<InputElement>(components.Length);

            foreach(var component in components)
                MyVertexInputLayout.mapComponent[component.Type].AddComponent(component, elemensList, semanticDict, declarationBuilder, sourceBuilder);

            elemensList.Capacity = elemensList.Count;

            Layouts.Data[id.Index] = new MyVertexLayoutInfo
            {
                Components = components,
                Elements = elemensList.GetInternalArray(),
                Macros = MyComponent.GetComponentMacros(declarationBuilder.ToString(), sourceBuilder.ToString()),
                HasBonesInfo = components.Any(c => c.Type == VertexInputComponentType.BLEND_INDICES)
            };

            return id;
        }
    }

    internal partial class MyVertexInputLayout
    {
        VertexInputComponent[] components = new VertexInputComponent[0];
        int hash;
        int id;

        private InputElement[] elements;
        private ShaderMacro[] macros;

        static readonly Dictionary<int, MyVertexInputLayout> cached = new Dictionary<int, MyVertexInputLayout>();

        internal static MyVertexInputLayout Empty { get { return cached[0]; } }

        static MyVertexInputLayout()
        {
            var empty = new MyVertexInputLayout();
            empty.Build();
            cached[0] = empty;

            InitComponentsMap();
        }

        private MyVertexInputLayout Append(VertexInputComponent component)
        {
            return Append(component.Type, component.Slot, component.Freq);
        }

        internal MyVertexInputLayout Append(VertexInputComponentType type, int slot, VertexInputComponentFreq freq)
        {
            VertexInputComponent component = new VertexInputComponent
            {
                Type = type,
                Slot = slot,
                Freq = freq,
            };

            int nextHash = HashHelpers.Combine(hash, component.GetHashCode());

            MyVertexInputLayout next;
            if (cached.TryGetValue(hash, out next))
                return next;

            next = new MyVertexInputLayout
            {
                hash = nextHash,
                id = cached.Count,
                components = components.Concat(component.Yield()).ToArray()
            };

            next.Build();

            cached[nextHash] = next;
            return next;
        }

        private void Build()
        {
            var declarationBuilder = new StringBuilder();
            var sourceBuilder = new StringBuilder();
            var elementsList = new List<InputElement>();
            var semanticDict = new Dictionary<string, int>();

            foreach (var component in components)
                mapComponent[component.Type].AddComponent(component, elementsList, semanticDict, declarationBuilder, sourceBuilder);

            elements = elementsList.ToArray();
            macros = MyComponent.GetComponentMacros(declarationBuilder.ToString(), sourceBuilder.ToString());
        }
    }
}

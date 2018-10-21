using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Render.Resources;

namespace TPresenter.Render
{
    struct Mapping
    {
        private MyRenderContext renderContext;
        private Resource resource;
        private int bufferSize;
        private DataBox dataBox;
        private IntPtr dataPointer;

        #region Static Methods.

        internal static Mapping MapDiscard(IBuffer buffer)
        {
            return MapDiscard(Render11.Direct3DContext, buffer);
        }

        internal static Mapping MapDiscard(MyRenderContext rc, IBuffer buffer)
        {
            return MapDiscard(rc, buffer, buffer.Description.SizeInBytes);
        }

        internal static Mapping MapDiscard(MyRenderContext rc, IResource resource, int bufferSize)
        {
            Mapping mapping;
            mapping.renderContext = rc;
            mapping.resource = resource.Resource;
            mapping.bufferSize = bufferSize;
            mapping.dataBox = rc.MapSubresource(resource, 0, MapMode.WriteDiscard, MapFlags.None);

            if (mapping.dataBox.IsEmpty)
                throw new Exception("Resource mapping failed!");    //We should thrown custom exception here.
            mapping.dataPointer = mapping.dataBox.DataPointer;

            return mapping;
        }

        internal static Mapping MapDiscard(IResource resource)
        {
            var mapping = MapDiscard(Render11.Direct3DContext, resource, 0);
            if (mapping.dataBox.SlicePitch != 0)
                mapping.bufferSize = mapping.dataBox.SlicePitch;
            //else
            //{
            //    mapping.bufferSize = mapping.dataBox.RowPitch;
            //}
            return mapping;
        }

        internal static Mapping MapRead(MyRenderContext rc, IResource resource, int bufferSize)
        {
            Mapping mapping;
            mapping.renderContext = rc;
            mapping.resource = resource.Resource;
            mapping.bufferSize = bufferSize;
            mapping.dataBox = rc.MapSubresource(resource, 0, MapMode.Read, MapFlags.None);

            if(mapping.dataBox.IsEmpty)
                throw new Exception("Resource mapping failed!");    //We should thrown custom exception here.
            mapping.dataPointer = mapping.dataBox.DataPointer;

            return mapping;
        }

        #endregion

        #region Methods

        internal void ReadAndPosition<T>(ref T data) where T : struct
        {
            dataPointer = Utilities.ReadAndPosition(dataPointer, ref data);
            Debug.Assert((dataPointer.ToInt64() - dataBox.DataPointer.ToInt64()) <= bufferSize);
        }

        internal void WriteAndPosition<T>(ref T data) where T : struct
        {
            dataPointer = Utilities.WriteAndPosition(dataPointer, ref data);
            Debug.Assert((dataPointer.ToInt64() - dataBox.DataPointer.ToInt64()) <= bufferSize);
        }

        internal void WriteAndPosition<T>(T[] data, int count, int offset = 0) where T : struct
        {
            dataPointer = Utilities.Write(dataPointer, data, offset, count);
            Debug.Assert((dataPointer.ToInt64() - dataBox.DataPointer.ToInt64()) <= bufferSize);
        }

        internal void WriteAndPositionByRow<T>(T[] data, int count, int offset = 0) where T : struct
        {
            Debug.Assert(count <= dataBox.RowPitch);
            Utilities.Write(dataPointer, data, offset, count);
            dataPointer += dataBox.RowPitch;
            Debug.Assert((dataPointer.ToInt64() - dataBox.DataPointer.ToInt64()) <= bufferSize);
        }

        internal void Unmap()
        {
            renderContext.UnmapSubresource(resource, 0);
        }

        #endregion

    }
}

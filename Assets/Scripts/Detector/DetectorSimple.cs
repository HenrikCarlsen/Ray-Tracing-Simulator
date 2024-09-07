using Unity.Entities;

using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
namespace Detector
{

    public struct DetectorSimple
    {

    }
    public struct DetectorPixel : IBufferElementData
    {
        public double count;
    }

    [ChunkSerializable]
    public struct DetectorGrid : IComponentData
    {
        // https://www.youtube.com/watch?v=Mt4HmReBFsE

        public int MaterialID;
        // readonly public NativeArray<DetectorPixel> All() { return pixels; }

        public int2 PixelCount { get; private set; }

        public enum Scale { linear, log }
        public Scale scale;

        public double2 Range { get; set; }


        public int totalCount; // TODO not impemented yet

        public double TotalCountCalculator(NativeArray<DetectorPixel> pixels)
        {
            double totalCount = 0;
            for (int i = 0; i < pixels.Length; i++)
            {
                totalCount += pixels[i].count;
            }
            return totalCount;
        }

        readonly private int Flatten(int2 coord)
        {
            return PixelCount.y * coord.x + coord.y;
        }

        // private int2 Expand(int i)
        // {
        //     int x = i / PixelCount.y;
        //     int y = i % PixelCount.y;
        //     return new int2(x, y);
        // }

        readonly public double Get(int2 coord, DynamicBuffer<DetectorPixel> pixels)
        {
            return pixels[Flatten(coord)].count;
        }

        public void Set(int2 coord, double pixel, DynamicBuffer<DetectorPixel> pixels)
        {
            pixels[Flatten(coord)] = new DetectorPixel { count = pixel };
        }

        public DetectorGrid(int2 size, Scale scale = Scale.linear)
        {
            this.MaterialID = -1;
            this.Range = new double2(0, 0);
            this.scale = scale;
            this.totalCount = 0;
            this.PixelCount = size;
            // this.pixels = new NativeArray<DetectorPixel>(size.x * size.y, Allocator.Persistent);
        }
    }
}
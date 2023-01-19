using Unity.Entities;

using Unity.Mathematics;
using Unity.Collections;

using UnityEngine;

public struct DetectorPixel : IComponentData
{
    public double count;
}

public struct DetectorGrid : IComponentData
{
    // https://www.youtube.com/watch?v=Mt4HmReBFsE
    private NativeArray<DetectorPixel> pixels;

    public int MaterialID;

    public NativeArray<DetectorPixel> all() { return pixels; }

    public int2 pixelCount { get; private set; }

    public enum Scale { linear, log }
    public Scale scale;

    public double2 range { get; set; }


    public int totalCount; // TODO not impemented yet

    public double totalCountCalculator()
    {
        double totalCount = 0;
        for (int i = 0; i < pixels.Length; i++)
        {
            totalCount += pixels[i].count;
        }
        return totalCount;
    }

    private int flatten(int2 coord)
    {
        return pixelCount.y * coord.x + coord.y;
    }

    private int2 expand(int i)
    {
        int x = i / pixelCount.y;
        int y = i % pixelCount.y;
        return new int2(x, y);
    }

    public double get(int2 coord)
    {
        return pixels[flatten(coord)].count;
    }
    public void set(int2 coord, double pixel)
    {
        pixels[flatten(coord)] = new DetectorPixel { count = pixel };
    }

    public DetectorGrid(int2 size, Scale scale = Scale.linear)
    {
        this.MaterialID = -1;
        this.range = new double2(0, 0);
        this.scale = scale;
        this.totalCount = 0;
        this.pixelCount = size;
        this.pixels = new NativeArray<DetectorPixel>(size.x * size.y, Allocator.Persistent);
    }
}

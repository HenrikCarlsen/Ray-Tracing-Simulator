using Unity.Entities;
public struct GeneratorHolder : IComponentData
{
    public Entity generator;
    public EntityArchetype archetype;
}
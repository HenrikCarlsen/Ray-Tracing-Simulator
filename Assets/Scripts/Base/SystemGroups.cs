using Unity.Entities;
using UnityEngine.PlayerLoop;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class CreateParticles : ComponentSystemGroup { }
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(CreateParticles))]
public partial class CreateParticlesEnd : ComponentSystemGroup { }

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(CreateParticlesEnd))]
public partial class MoveParticles : ComponentSystemGroup { }
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(MoveParticles))]
public partial class MoveParticlesEnd : ComponentSystemGroup { }

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(MoveParticlesEnd))]
public partial class InteractionOfParticles : ComponentSystemGroup { }
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(InteractionOfParticles))]
public partial class InteractionOfParticlesEnd : ComponentSystemGroup { }

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(InteractionOfParticlesEnd))]
public partial class HistoryOfParticles : ComponentSystemGroup { }

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(HistoryOfParticles))]
public partial class CleanupParticles : ComponentSystemGroup { }

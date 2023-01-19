# Physics Ray Tracing Simulator
This is a novel scientific ray tracer which aims to leverage the benefits of data-oriented design (DOD) to deliver state of the art simulation of physics instruments' background.
Modularity, performance and ease of use is the focus while keeping the kind of physics the ray simulate flexible.

## Novel features
Compared to other ray tracing simulator program used in physics this simulator have the following novel features:

### Data oriented design
    Written with Unity's DOTS package which allows for high performance and scalability out of the box.
    Seperate physics from the 

### One to many scattering
<img src="ParticleBouncingWIP.png" width="500">
    Each in-going ray can create multiple outgoing rays. Beside allowing for more kinds of physics to be simulated, this also helps performance as interaction often yield multiple outgoing rays naturally.

### Modular instrument design
    Each part of the instrument consist of modular components. 
    This allows for building strong maintainable modules and for building instruments without writting code but by using Unity's drag and drop interface


## Future Goal

Stronger Test setup
Comparative performance tests
Finish job model for interactions
Rethink authoring to make sure unphysical setups are harder to make
Handle multiple kind of interaction on the same object
Better real life models implemented
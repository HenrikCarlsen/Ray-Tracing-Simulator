

# Physics Ray Tracing Simulator

If one were to sum up all scientific experiments as trying to maximize the signal to noise ratio for different signals one would not be entirely in the wrong.
Today we can use simulations to find the setup which gives the best ratios before we start the real, and expensive, experiment.

A problem I see in many ray tracing simulations performed is a lack of focus on the noise, leaving out half of the ratio. 
My guess to why this is is simple due to the complexities needed of the simulation and not of the physics.
So my proposal with this ray tracer is to provide a framework allowing for handling these complexities without 

### Modular instrument design

When simulating noise the background must be simulated.
This can increase the number of independent parts of the simulation by orders.
These parts cannot no longer be bestoke without scaling the development time, 
so this ray tracer must provide standardlized tools for making modular parts for many more cases than a ray tracer purely focused on the signal.

### Performance

Simulating only the signal gives many options optimizing the simulation simply by removing rays not behaving ideally.
Noise is by difination noisy and leaves fewer physics based optimization options, hence the underlying simulation must have better performance.

### One to many scattering

By allowing a ray to bisect into multiple rays, or even different rays, simulations that can be difficult to perform become trivial.

# Future Goals
The current state of the project is very much in the prototype state. Future goals include:

    Stronger Test setup
    Comparative performance tests
    Finish job model for interactions
    Rethink authoring to make sure unphysical setups are harder to make
    Handle multiple kind of interaction on the same object
    Better real life models implemented


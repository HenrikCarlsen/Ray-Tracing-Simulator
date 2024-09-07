

```mermaid
  graph TD;
      CreateParticles-->MoveParticles;
      MoveParticles-->InteractionOfParticles;
      InteractionOfParticles-->HistoryOfParticles
      HistoryOfParticles-->CleanupParticles;
      CleanupParticles-->CreateParticles;
```
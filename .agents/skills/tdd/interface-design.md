# Interface Design for Testability

Good interfaces make testing natural:

1. **Accept dependencies, don't create them**

   ```csharp
   // Testable
   public class CombatResolver
   {
       private readonly IRandom _rng;
       public CombatResolver(IRandom rng) { _rng = rng; }
       public HitResult Resolve(Attacker a, Defender d) { ... }
   }

   // Hard to test
   public class CombatResolver
   {
       public HitResult Resolve(Attacker a, Defender d)
       {
           var roll = (float)GD.Randf(); // static, non-injectable
           ...
       }
   }
   ```

2. **Return results, don't produce side effects**

   ```csharp
   // Testable
   public DamageResult CalculateDamage(Attack attack, Defense defense) { ... }

   // Hard to test
   public void ApplyDamage(Character target, Attack attack)
   {
       target.Health -= ComputeDamage(attack); // mutates + computes, hard to assert
   }
   ```

3. **Small surface area**
   - Fewer methods = fewer tests needed
   - Fewer params = simpler test setup

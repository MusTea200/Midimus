## 2024-04-29 - Inline Object Allocation in Hot Paths
**Learning:** Found a codebase-specific pattern in `SimulationManager.cs` where wrapper objects like `BasicInsurancePolicy` are repeatedly allocated inline during mathematical calculations (e.g., `CalculateSuccessProbability`), alongside redundant calls to the same calculation with identical state.
**Action:** When optimizing calculation-heavy game loops or daily simulation ticks, always check for inline object instantiations inside repeated logic, and cache them beforehand.

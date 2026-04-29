# Performance Journal - Zindan Sigorta Simülasyonu

## 2025-05-14 - Enum.GetValues Optimization in SimulationManager
**Critical Learning:** `Enum.GetValues(typeof(T))` is a heavy operation that uses reflection and allocates a new array on every call. In high-frequency methods like `ProcessSingleExpedition` and `ProcessDuoExpedition` (which run daily for every active policy), this causes significant GC pressure and CPU overhead.
**Implementation Pattern:** Cache enum values in `private static readonly` arrays at the class level. Use direct array indexing `_cachedArray[index]` instead of `Array.GetValue(index)` to avoid boxing and runtime overhead.
**Redundancy Check:** Always verify logic flow when optimizing; discovered and removed a duplicate looting block in `ProcessSingleExpedition` that was causing double rewards and redundant allocations.
**Measured Impact:** 1,000,000 iterations of `Enum.GetValues` took ~392ms, vs ~2ms for a cached array access (~196x speedup).

# Zindan Sigorta Simülasyonu - Jules System & Architecture Guide

## 1. Project Context & Lore
"Zindan Sigorta Simülasyonu" (Dungeon Insurance Simulation) is a Dark Fantasy Management RPG. The player manages a shady insurance company that acts as a front for a dark, capitalist empire. 
* **Core Philosophy:** "Equivalent Exchange." Immense power (Cursed Items, Cloning, Mind Uploading via VHS) always comes with extreme costs, daily upkeep, and moral/legal risks.
* **World Reactivity:** The game reacts to *who* the character is (Gender, Monster vs. Human, Traits), not just their base stats.

## 2. Environment & Tech Stack (Setup Hints)
* **Language:** C# (Object-Oriented Programming).
* **Architecture:** Backend-focused system architecture designed to be integrated into an engine (Godot/Unity) later.
* **Environment:** Jules should treat this as a standard C# environment. No external UI frameworks or database setups (like Supabase/Render) are required. Keep the environment lightweight.
* **Testing/Building:** If needed, use standard `dotnet build` or C# compilation commands to validate syntax before finalizing tasks.

## 3. STRICT Workflow Rules (CRITICAL FOR JULES)
Jules MUST follow these workflow directives to match the solo-developer's rapid prototyping style:
1. **NO BRANCHING:** DO NOT create new branches (e.g., `feature/...`, `bolt/...`, `jules/...`). 
2. **NO PULL REQUESTS:** DO NOT ask the user to review Pull Requests. 
3. **DIRECT COMMITS:** ALL code modifications, file creations, and system updates MUST be committed and pushed DIRECTLY to the active `default/main` branch.
4. **FILE PATCHING:** When updating large files (like `SimulationManager.cs`), avoid risky `sed` patches that cause duplicate braces or syntax errors. If a file requires complex logical inserts, safely rewrite the affected methods entirely to ensure C# compilation success.

## 4. Code Generation Standards
* **Performance First:** Optimize for performance. Use efficient LINQ, avoid memory leaks with large Lists, and utilize robust `Action/Delegate/Event` systems.
* **No UI Code:** Unless explicitly requested, do not write UI/Frontend code. Focus strictly on Backend Business Logic.
* **Modularity:** New city buildings must inherit from the `CityFacility` base class.

## 5. Core Game Systems
* **Traits & Acclimatization:** Traits are 4-tier (White, Yellow, Turquoise, Purple) and upgradeable. VHS Mind Transfers (`MindVHSTape`) cause a 20-day decreasing `BodyAcclimatizationTrait` debuff.
* **Mad Science Lab:** Run by Richard Gobrigez. Handles Cybernetics, Mutations, and cloning `VatGrownBody` units. Generates massive daily gold costs (Electricity, VHS cooling).
* **End-Game (Act 3):** Features `UndergroundArenaFacility` for illegal betting and `DungeonSiegeManager` for hostile takeovers using disposable clone armies, eventually leading to dimension-invading lore.

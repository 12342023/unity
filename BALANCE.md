# Gameplay Balance

## Target Experience

- One game session: 3–5 minutes.
- Player pushes through neutral plots toward the enemy base, while defending against periodic enemy attacks.
- Both sides have supply cap constraints — cannot endlessly spam units.
- Victory = destroy EnemyBase (and all enemy buildings/units are cleaned up).
- Defeat = PlayerBase is destroyed.

## Current Key Values

### Supply Cap

| Parameter | Value | Source |
|-----------|-------|--------|
| Base supply cap | 8 | `FactionStatsService` / `GameBalanceConfig.BaseSupplyCap` |
| Per Granary bonus | +4 | `FactionStatsService` / `GameBalanceConfig.GranarySupplyBonus` |

*Player starts with Village (Granary) → cap = 12. Enemy has no Granary → cap = 8.*

### Barracks (unit production)

| Parameter | Value | Source |
|-----------|-------|--------|
| Spawn interval | 5 s | `BarracksSpawner.spawnInterval` (inspector default) |
| Rally threshold | 3 | `BarracksSpawner.rallyThreshold` |
| Unit health | 30 | `BarracksSpawner.unitHealth` |
| Unit damage | 10 | `BarracksSpawner.unitDamage` |
| Unit attack range | 1.5 | `BarracksSpawner.unitAttackRange` |
| Unit attack interval | 1.0 s | `BarracksSpawner.unitAttackInterval` |
| Unit aggro range | 4 | `BarracksSpawner.unitAggroRange` |
| Unit chase range | 7 | `BarracksSpawner.unitChaseRange` |

*Both Player and Enemy Barracks use the same values. Barracks respects supply cap before spawning.*

### Unit Movement Speed

| Unit Type | Speed | Source |
|-----------|-------|--------|
| Samurai | 1.5 | `UnitConfig` |
| Elf Archer | 2.5 | `UnitConfig` |
| Soldier | 3.5 | `UnitConfig` |

*Note: Barracks currently spawns Soldier type only. Samurai/ElfArcher are for test spawning.*

### Tower

| Parameter | Value | Source |
|-----------|-------|--------|
| Damage | 15 | `GameBalanceConfig.TowerDamage` (set in `BuildingFactory`) |
| Attack range | 3.5 | `GameBalanceConfig.TowerAttackRange` (set in `BuildingFactory`) |
| Attack interval | 1.5 s | `GameBalanceConfig.TowerAttackInterval` |

*Tower only attacks units (objects with UnitCombat), not buildings. 15 damage vs 30 HP = kills a soldier in 2 hits.*
*Tower health = 80 (so a soldier takes 8 hits to destroy a tower).*

### Building Health

| Building Type | Health | Source |
|--------------|--------|--------|
| Barracks | 50 | `BuildingFactory` |
| Tower | 80 | `BuildingFactory` |
| Granary | 50 | `BuildingFactory` |

### Enemy Pressure Timing

| Parameter | Value | Source |
|-----------|-------|--------|
| First attack delay | 8–12 s (random) | `GameBalanceConfig.EnemyFirstAttackMin/Max` |
| Repeat attack interval | 20–30 s (random) | `GameBalanceConfig.EnemyRepeatAttackMin/Max` |

### Plot Capture Requirements

| Plot Size | Required Soldiers | Source |
|-----------|-------------------|--------|
| Small | 1 | `PlotCaptureRequirementService` |
| Medium | 2 | `PlotCaptureRequirementService` |
| Large | 3 | `PlotCaptureRequirementService` |

### Match Trigger Keys (debug)

| Key | Effect |
|-----|--------|
| K | Destroy EnemyBase → Victory |
| L | Destroy PlayerBase → Defeat |

## Tuning Notes

- **Enemy pressure timing**: First attack at 8–12 s gives the player just enough time to build up a few soldiers. 20–30 s repeat keeps pressure without being overwhelming.
- **Supply cap 12/8**: The player's +4 advantage (from Village Granary) lets them field more units, necessary for offensive expansion.
- **Tower 15 damage**: A tower kills a soldier in 2 hits (1.5–3 s). Towers are strong defensive tools — the player can overwhelm them with numbers.
- **Spawn rate 5 s**: Fills supply cap in ~60 s (player) / ~40 s (enemy). Creates enough units for a 3–5 min match.
- **Plot requirement**: Small=1, Medium=2, Large=3 means the player needs to commit progressively more soldiers to capture larger plots.

## Future Tuning Suggestions

- If enemy feels too passive: lower repeat interval (e.g., 15–25 s) or increase enemy rally threshold for bigger attack waves.
- If supply cap feels irrelevant: lower base cap (e.g., 6) or increase Granary bonus (e.g., +5).
- If towers are too strong: reduce damage to 12 (3 hits to kill a soldier) or increase attack interval to 2.0 s.
- If match is too long/short: adjust Barracks spawn interval (increase = slower, decrease = faster).
- If capturing is too easy: raise Medium to 3, Large to 4.

# Battle System

This is a library that provides functionality for characters battling against other, targeting .NET 5.0.

[![NuGet](https://img.shields.io/nuget/v/BattleSystem.svg?logo=nuget)](https://www.nuget.org/packages/BattleSystem)

## Character

- has `M` max health
- starts with `M` current health
- dies if current health reaches 0
- has some number of [moves](#moves)
- has a set of [stats](#stats)
- can hold an [item](#items)

<a name="moves"></a>
## Moves

- has `U` max uses
- starts with `U` remaining uses
- cannot be used if remaining uses reaches 0
- contains some number of [actions](#actions) executed in order

<a name="actions"></a>
## Actions

### Damage

- has `P` power
- can be configured to target any number of characters
- damages target for either:
    - `P` health
    - `P` percent of the target's max health
    -  `max{1, P * (A - D)}` health where:
        - `A` is the user's attack stat
        - `D` is the target's defence stat
    - such calculations can be customised

### Buff

- has set of percentage changes for some stats
- changes those stats by the corresponding percentages
- can be configured to target any number of characters

### Heal

- has `H` power
- heals character for either up to `H` health or up to `H`% of their max health
    - such calculations can be customised

### Protect

- nullifies all effects of next action against the target
- default limit of one protect action queued up for a given character

### Protect Limit Change

- alters a character's protect limit

<a name="stats"></a>
## Stats

- has `V` integer starting value
- has `M` decimal multiplier, starting at 1
- `M` can be altered by buffs

### Attack

- determines strength of attack actions that calculate damage based on stats

### Defence

- determines resistance to attack actions that calculate damage based on stats

### Speed

- determines the order in which characters' [moves](#moves) are processed (highest first)

<a name="items"></a>
## Items

- can alter values of holder's base stats
- can alter power of holder's damage actions
- can execute [actions](#actions) against characters arbitrarily

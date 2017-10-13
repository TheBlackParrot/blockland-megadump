// READ THIS, FAGGOT //
// congratulations on your purchase (!?) of the new creeper mod. Here is everything you need to know about how it works,
// what it does, and how to map for it. The fact that you're reading my code shows that you want to know how to mod or map
// for it. Fact.


// GAMEMODE LIST // (This is also in official Map config.cs files)

// 0 = Man Vs Creeper (Erradicate the creeper without dying. Players will use their own /shop CreepKill weapons.)
// 1 = Creepageddon (Survive together until the timer runs out or one person is left - requires a big map to be fun)
// 2 = Rising Creeper (Creepageddon with Push Brooms! Players will be allowed other competitive weapons via /shop)
// 3 = Erradication (Man Vs Creeper but respawning is allowed - creeper wins if not eraddicated after time limit)
// 4 = Karoshi (Everyone gets Creeper Planter, the objective is to cover enough map as quickly as possible. It won't kill)
// 5 = Outbreak (Erradication but this time the creeper needs to spread a certain distance. Time limit causes a player win.)
// 6 = That Bastard (Someone invisible gets a Creeper Planter and Creep Immunity. He has to kill everyone without being shot.)

// only gamemodes 0, 1 and 3 work right now. Do not try to use any others until they are coded in.


// MAPPING //

// If you're making a Man Vs Creeper map:
// Try to keep it small and enclosed. Creeper can climb the walls and the ceiling. Do not use doors, especially the new ones -
// they can break things. Try to put the players in a position where they can fight or die.

// If you're making a Creepageddon or Rising Creeper map:
// Try to have a similar structure to Rising Lava maps. I know it sounds cliche, but you'll need a lot of high spots for a good
// game. Also, make sure the creeper does not start anywhere near the top. Make it nice and large, but still enclosed.

// If you're making an Erradication, Karoshi, Outbreak or That Bastard map:
// Have a a build with lots of open space and lots of walls. ACM City may be appropriate, or perhaps Afganistan if you want
// an idea of how to do it. (Or just package one of those, I'm going to)

// In general:
// - If all the creeper is cleared in Creepageddon, the players all win, just like Man Vs Creeper. You can make Creepageddon
//   maps with a "win" button that does this at the end, if you want.
// - Make sure to tweak the creeper settings properly in your map's config.cs. You don't want a tiny map with lighting creeper
//   or a huge map with creeper that's slow as fuck. Anticipate your ideal playercount. You won't be too far off.
// - Although keeping Man Vs Creeper maps small is always a good idea, please remember to give your players space to move.
// - In a "That Bastard" map, the "Bastard" spawns on a special brick called the "Bastard Spawn". (Apt, I know) Do not forget
//   to place this or I will kick you in the boingwoings.
// - Place your Creeper Spawns in positions where players cannot control them from the start. Your rounds won't last very long
//   if you forget to do this.
// - Have fun mapping.


// MODDING //

// Still here? Dang. I thought I'd gotten rid of you.
// Since I have no idea what you want to do with your "mod of a mod", I'll just give you some functions and let you get on.

// StartCreeper() - Starts the creeper spreading.
// StopCreeper() - Stops the creeper from spreading. Can restart with StartCreeper() in theory, I've not actually tried.
// ClearCreeper() - Clears creeper bricks and puts spawn bricks back where they were.

// Also:
// - If you want to add a gamemode: You're out of luck, this isn't a fancy system like Slayer and TDM. Crowbar it in. You can
//   do it. Be a man.
// - DO NOT mess with $Creep::Active. I have and it does not do what you think it does.
// - I have absolotely no idea how creeper generation works. Wallet didn't annotate his code, and he ain't around to ask.
// - The Map Cycling in this mod is based off the map cycling in Speedkart, Zapk's rising lava and Zapk's Prop Hunt.
// - Refrain from releasing "mods of my mod". I know this is a "mod of a mod" itself but the mod it's a mod of has been broke
//   for a long time. Also, if you release a "mod of my mod" your mod will be a "mod of a mod of a mod". Ain't nobody got time
//   fo' dat.
// - When I say "I Don't know how to do that" on the forum, what I really mean is "I can't be arsed with that." I will never
//   confirm this on the Forum. Only people who read this little manual will know. Huehue.
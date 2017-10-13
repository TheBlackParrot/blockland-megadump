datablock AudioProfile(MinesweeperClick)
{
	filename = "./sounds/click.wav";
	description = AudioClose3d;
	preload = true;
};
datablock AudioProfile(MinesweeperFlagPlace : MinesweeperClick) { filename = "./sounds/flag_place.wav"; };
datablock AudioProfile(MinesweeperFlagRemove : MinesweeperClick) { filename = "./sounds/flag_remove.wav"; };
datablock AudioProfile(MinesweeperClickNo : MinesweeperClick) { filename = "./sounds/click_no.wav"; };
datablock AudioProfile(MinesweeperExplode : MinesweeperClick) { filename = "./sounds/explode.wav"; };
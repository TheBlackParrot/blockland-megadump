#!/usr/bin/python

# included this with the gamemode because it proved to be a useful tool
# uses python, dunno if it works with 2.x, but i know it works with 3.5.1

import sys;
import json;

encode = json.JSONEncoder().encode;

def convert(col="000000"):
	col = col.strip("#");

	red = round(int(col[0:2], 16) / 255, 6);
	green = round(int(col[2:4], 16) / 255, 6);
	blue = round(int(col[4:6], 16) / 255, 6);

	list = [str(red), str(green), str(blue)];

	if(len(col) > 6):
		alpha = round(int(col[6:8], 16) / 255, 6);
		list.append(str(alpha));

	return " ".join(list);

colorList = [];

for i in range(1, len(sys.argv)):
	color = sys.argv[i];
	colorList.append(convert(col=color));

print(encode(colorList));
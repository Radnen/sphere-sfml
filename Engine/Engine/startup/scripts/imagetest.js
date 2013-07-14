// Purpose: Tests the speed and efficiency of images.

function TestImages()
{
	var done = false;
	var blue = CreateColor(0, 0, 255);
	var image = LoadImage("blockman.png");
	var ship = LoadImage("ship.png");
	
	var x = GetScreenWidth()/2, y = GetScreenHeight()/2;
	var rad = 0, time = 0;

	while (!done) {
		for (var i = 0; i < 5; ++i) {
			image.blit(i * 48, 0);
		}
		
		image.rotateBlitMask(5 * 48, 0, rad, blue);
		
		if (time + 25 < GetTime())
		{
			if (IsKeyPressed(KEY_UP)) y--;
			if (IsKeyPressed(KEY_DOWN)) y++;
			if (IsKeyPressed(KEY_LEFT)) x--;
			if (IsKeyPressed(KEY_RIGHT)) x++;
			time = GetTime();
			rad += Math.PI/32;
		}
		
		ship.blit(x, y);
		
		FlipScreen();
	
		while (AreKeysLeft()) {
			if (GetKey() == KEY_ENTER) done = true;
		}
	}
}
// Purpose: Tests the speed and efficiency of images.

RequireSystemScript("time.js");

function TestImages()
{
	var done = false;
	var blue = CreateColor(0, 0, 255);
	blue.alpha = 175.25;
	var image = LoadImage("blockman.png");
	var ship = LoadSurface("ship.png");
		
	var x = GetScreenWidth()/2, y = GetScreenHeight()/2;
	var rad = 0, time = 0;
	var screen = null;
	var SW = GetScreenWidth();
	var SH = GetScreenHeight();

	while (!done) {
		for (var i = 0; i < 5; ++i) {
			image.blit(i * 48, 52);
		}
		
		image.rotateBlitMask(5 * 48, 0, rad, blue);
		
		if (time + 10 < GetTime())
		{
			if (IsKeyPressed(KEY_UP)) y--;
			if (IsKeyPressed(KEY_DOWN)) y++;
			if (IsKeyPressed(KEY_LEFT)) x--;
			if (IsKeyPressed(KEY_RIGHT)) x++;
			time = GetTime();
			rad += Math.PI/32;
			screen = GrabImage(0, 0, SW, SH);
		}
		
		ship.blit(x, y);
		if (screen) screen.transformBlit(0, 0, 50, 0, 50, 50, 0, 50);
		
		FlipScreen();
	
		while (AreKeysLeft()) {
			if (GetKey() == KEY_ENTER) done = true;
		}
	}
}
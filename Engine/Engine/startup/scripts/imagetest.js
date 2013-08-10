// Purpose: Tests the speed and efficiency of images.

RequireSystemScript("time.js");

function TestImages()
{
	var done = false;
	var red   = CreateColor(255, 0, 0);
	var black = CreateColor(0, 0, 0);
	var blue = CreateColor(0, 0, 255);
	blue.alpha = 175.25;
	var green = CreateColor(0, 255, 0);
	var image = LoadImage("blockman.png");
	var ship = LoadSurface("ship.png");
		
	var x = GetScreenWidth()/2, y = GetScreenHeight()/2;
	var rad = 0, time = 0;
	var screen = null;
	var SW = GetScreenWidth();
	var SH = GetScreenHeight();
	
	var start1 = {x: 0, y: 150};
	var end1 = {x: 50, y: 200};
	var start2 = {x: 150, y: 150};
	var end2 = {x: 150, y: 200};

	while (!done) {
		GradientRectangle(0, 0, SW, SH, black, black, blue, blue);

		for (var i = 0; i < 5; ++i) {
			image.blit(i * 48, 52);
		}
		
		image.rotateBlitMask(5 * 48, 0, rad, blue);
		
		if (time + 10 < GetTime())
		{
			if (IsKeyPressed(KEY_UP)) y--;
			if (IsKeyPressed(KEY_DOWN)) y++;
			if (IsKeyPressed(KEY_LEFT)) { x--; start1.x--; end1.x--; }
			if (IsKeyPressed(KEY_RIGHT)) { x++; start1.x++; end1.x++; }
			time = GetTime();
			rad += Math.PI/32;
			screen = GrabImage(0, 0, SW, SH);
		}
		
		ship.blit(x, y);
		if (screen) screen.transformBlit(0, 0, 50, 0, 50, 50, 0, 50);
		
		var intersect = LineIntersects(start1, end1, start2, end2);
		Line(start1.x, start1.y, end1.x, end1.y, intersect ? red : green);
		Line(start2.x, start2.y, end2.x, end2.y, intersect ? red : green);
		
		FlipScreen();
	
		while (AreKeysLeft()) {
			if (GetKey() == KEY_ENTER) done = true;
		}
	}
}
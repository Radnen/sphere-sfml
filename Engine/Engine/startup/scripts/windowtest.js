// Purpose: Tests the speed and efficiency of images.

function TestWindows()
{
	var done = false;
	var wind = LoadWindowStyle("main.rws");
	
	var blue = CreateColor(0, 0, 255);
	var white = CreateColor(255, 255, 255);
	var time = 0, i = 1, x = 0, y = 0;
	
	while (!done) {
		wind.drawWindow(32, 32, 80, 80);
		
		wind.setColorMask(blue);
		wind.drawWindow(128, 32, 80, 80);
		
		wind.drawWindow(32, 128, 80 + x, 80 + y);

		wind.setColorMask(white);
		
		if (time + 25 < GetTime()) {
			if (x == 100 || x == -4) i *= -1;
			x += 4 * i;
			y += i;
			time = GetTime();
		}
		
		FlipScreen();
	
		while (AreKeysLeft()) {
			if (GetKey() == KEY_ENTER) done = true;
		}
	}
}
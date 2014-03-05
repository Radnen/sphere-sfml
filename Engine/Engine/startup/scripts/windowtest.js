// Purpose: Tests the speed and efficiency of images.

function TestWindows()
{
    var done = false,
        wind = LoadWindowStyle("main.rws"),
        blue = CreateColor(0, 0, 255),
        white = CreateColor(255, 255, 255),
        time = 0, i = 1, x = 0, y = 0;
	
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

// Purpose: Tests the speed and efficiency of images.

function TestBigWindow() {
    var done = false,
        wind = LoadWindowStyle("insane.rws"),
        w = GetScreenWidth() - 32,
        h = GetScreenHeight() - 32;

    while (!done) {
        wind.drawWindow(16, 16, w, h);

        FlipScreen();

        while (AreKeysLeft()) {
            if (GetKey() == KEY_ENTER) done = true;
        }
    }
}
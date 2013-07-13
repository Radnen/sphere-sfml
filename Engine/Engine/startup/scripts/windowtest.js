// Purpose: Tests the speed and efficiency of images.

function TestWindows()
{
	var done = false;
	var wind = LoadWindowStyle("main.rws");
	
	var wind2 = wind.clone();
	
	var blue = CreateColor(0, 0, 255);
	var white = CreateColor(255, 255, 255);
	
	while (!done) {
		wind.drawWindow(32, 32, 80, 80);
		
		wind.setColorMask(blue);
		wind.drawWindow(128, 32, 80, 80);
		
		wind2.drawWindow(32, 128, 80, 80);

		wind.setColorMask(white);
		
		FlipScreen();
	
		while (AreKeysLeft()) {
			if (GetKey() == KEY_ENTER) done = true;
		}
	}
}
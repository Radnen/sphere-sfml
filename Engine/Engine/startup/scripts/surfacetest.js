// surfacetest

function TestSurfaces()
{
	var done = false;
	var black = CreateColor(0, 0, 0);
	var white = CreateColor(255, 255, 255);
	var green = CreateColor(0, 255, 0);

	var surf = CreateSurface(48, 48, black);
	surf.gradientRectangle(0, 0, 48, 48, white, white, black, black);
	surf.gradientLine(0, 0, 47, 47, green, black);

    while (!done) {
    	surf.blit(0, 0);
        
        FlipScreen();

		while (AreKeysLeft()) {
			if (GetKey() == KEY_ENTER) done = true;
		}
    }
}
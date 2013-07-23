// surfacetest

function TestSurfaces()
{
	var done = false;
	var black = CreateColor(0, 0, 0);
	var white = CreateColor(255, 255, 255);
	var green = CreateColor(0, 255, 0, 125);
	var red   = CreateColor(255, 0, 0);

	SetClippingRectangle(0, 0, 1, 1);
	var surf = CreateSurface(48, 48, white);
	surf.rectangle(12, 12, 24, 24, green);
	surf.setPixel(47, 47, red);
	
	var surf2 = CreateSurface(50, 50, black);
	var surf3 = LoadSurface("blockman.png");
	
	surf2.blitMaskSurface(surf3, 1, 1, green);
	
	surf2.setBlendMode(BLEND);
	surf2.drawText(sys_font, 0, 0, "Hello");
	SetClippingRectangle(0, 0, GetScreenWidth(), GetScreenHeight());

    while (!done) {
    	surf.blit(0, 0);
    	surf2.blit(48, 0);
        
        FlipScreen();

		while (AreKeysLeft()) {
			if (GetKey() == KEY_ENTER) done = true;
		}
    }
}
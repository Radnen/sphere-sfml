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
	surf.gradientRectangle(0, 0, 48, 48, green, green, white, white);
	surf.line(0, 0, 48, 48, red);
	surf.setPixel(8, 2, red);
	surf.drawText(sys_font, 0, 0, "Hi");

	var surf2 = CreateSurface(50, 50, black);
	var surf3 = LoadSurface("blockman.png");
	surf.blitSurface(surf3, 0, 0);

	var surf4 = LoadSurface("blockman.png");
	surf4.rotate(Math.PI/4, false);
	
	surf2.blitMaskSurface(surf3, 1, 1, green);
	
	surf2.drawText(sys_font, 0, 0, "Hello");
	surf2 = surf2.rescale(75, 75);
	SetClippingRectangle(0, 0, GetScreenWidth(), GetScreenHeight());

	while (!done) {
	    surf.rectangle(12, 12, 24, 24, green);
	    surf.gradientRectangle(0, 0, 48, 48, green, green, white, white);
	    surf.line(0, 0, 48, 48, red);
	    surf.setPixel(8, 2, red);
	    surf.drawText(sys_font, 0, 0, "Hi");
	    //surf.blitSurface(surf3, 0, 0);
	    for (var i = 0; i < 5; ++i) {
	        for (var j = 0; j < 5; ++j) {
	            surf.blit(0 + i * 48, 16 + j * 48);
	        }
	    }
    	//surf4.blit(320-48, 16);
    	//surf4.blit(64, 64);
        
        FlipScreen();

		while (AreKeysLeft()) {
			if (GetKey() == KEY_ENTER) done = true;
		}
    }
}

function TestSurfaceFillRate() {
    var w = GetScreenWidth(),
        h = GetScreenHeight(),
        surf = CreateSurface(w, h, CreateColor(255, 255, 255)),
        green = CreateColor(0, 255, 0),
        done = false;
    
    while (!done) {
        surf.rectangle(0, 0, w, h, green);
        surf.blit(0, 0);
        FlipScreen();

        while (AreKeysLeft()) {
            if (GetKey() == KEY_ENTER) done = true;
        }
    }
}

function TestColorCreationSpeed() {
    var done = false;

    while (!done) {
        CreateColor(255, 255, 255, 255);
        CreateColor(255, 255, 255, 255);
        CreateColor(255, 255, 255, 255);
        CreateColor(255, 255, 255, 255);
        CreateColor(255, 255, 255, 255);
        CreateColor(255, 255, 255, 255);
        CreateColor(255, 255, 255, 255);
        CreateColor(255, 255, 255, 255);
        CreateColor(255, 255, 255, 255);
        CreateColor(255, 255, 255, 255);

        FlipScreen();

        while (AreKeysLeft()) {
            if (GetKey() == KEY_ENTER) done = true;
        }
    }
}

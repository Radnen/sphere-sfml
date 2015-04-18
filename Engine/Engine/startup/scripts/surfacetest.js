// surfacetest

function TestSurfaces()
{
	var done = false;
	var black = CreateColor(0, 0, 0);
	var white = CreateColor(255, 255, 255);
	var green = CreateColor(0, 255, 0, 125);
	var red = CreateColor(255, 0, 0);

	function TestComp() {
	    var surf = CreateSurface(48, 48, white);
	    var img = LoadSurface("blockman.png");

	    var t = GetTime();
	    for (var i = 0; i < 60000; ++i) { surf.blitSurface(img, 0, 0); }
	    Print(GetTime() - t);
	}
	//TestComp();

	SetClippingRectangle(0, 0, 1, 1);
	var surf = CreateSurface(48, 48, white);
    surf.rectangle(12, 12, 24, 24, green);
	/*surf.gradientRectangle(0, 0, 48, 48, green, green, white, white);
	surf.line(0, 0, 48, 48, red);
	surf.setPixel(8, 2, red);
	surf.drawText(sys_font, 0, 0, "Hi");*/

	var surf2 = CreateSurface(50, 50, black);
	var surf3 = LoadSurface("blockman.png");
	surf.blitSurface(surf3, 0, 0);

	var surf4 = LoadSurface("blockman.png");
	surf4.rotate(Math.PI/4, false);
	
	surf2.blitMaskSurface(surf3, 1, 1, green);
	
	surf2.drawText(sys_font, 0, 0, "Hello");
	SetClippingRectangle(0, 0, GetScreenWidth(), GetScreenHeight());
	var sysfont = GetSystemFont();

	while (!done) {
	    //surf.rectangle(12, 12, 24, 24, green);
	    //surf.gradientRectangle(0, 0, 48, 48, green, green, white, white);
	    for (var i = 0; i < 6; i += 2) {
	        surf.line(0, 0, 48, 48, red);
	        surf.line(48, 0, 0, 48, red);
	        surf.line(0, 47, 47, 47, red);
	        surf.line(48, 1, 48, 47, red);
	        surf.line(1, 1, 47, 0, red);
	        surf.line(1, 1, 1, 47, red);
	        surf2.line(0, 0, 48, 48, red);
	        surf3.gradientRectangle(0, 0, 48, 48, green, green, white, white);
	        surf3.line(0, 0, 48, 48, red);
	        surf4.rectangle(0, 0, 105, 65, green);
	        surf4.drawText(sys_font, 0, 0, "Hello");
	        surf3.drawText(sys_font, 0, 12, "Hello");
	        surf2.drawText(sys_font, 0, 24, "Hello");
	        surf.drawText(sys_font, 0, 36, "Hello");
	        surf4.rectangle(0, 0, 48, 48, green);
	        surf4.line(0, 0, 48, 0, white);
	        surf.setPixel(8, 2, red);
	        surf.drawText(sys_font, 0, 0, "Hi");
	        surf.blitSurface(surf2, -24, -24);

	        surf.blit(0 + i * 48, 16);
	        surf2.blit(48+i*48, 16);
	        surf3.blit(0+i*48, 64);
	        surf4.blit(48+i*48, 64);
	    }
    	//surf4.blit(320-48, 16);
	    //surf4.blit(64, 64);

	    //var c = surf.getPixel(0, 0);
	    //sysfont.drawText(0, 0, c.red + ", "+ c.green + ", "+ c.blue + ", " + c.alpha);

	    FlipScreen();
        //GetKey();

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

    //surf.setBlendMode(REPLACE);
        surf.rectangle(0, 0, w, h, green);
    
    while (!done) {
        surf.blit(0, 0);
        FlipScreen();

        /*while (AreKeysLeft()) {
            if (GetKey() == KEY_ENTER) done = true;
        }*/
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

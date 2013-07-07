function game() {
	var image = LoadImage("blockman.png");
	var green = CreateColor(0, 255, 0);
	var red   = CreateColor(255, 0, 0);
	var white = CreateColor(255, 255, 255);
	var gray  = CreateColor(200, 200, 200);
	var tblack = CreateColor(0, 0, 0, 180);
	
	var SW = GetScreenWidth();
	var SH = GetScreenHeight();
	var x = 0, xv = 1;
	
	image.width = 20;
	Print(image.width);
	
	while (true) {
		GradientRectangle(0, 0, SW, SH, white, white, gray, gray);
		Rectangle(0, 0, SW, 16, tblack);
		Triangle(SW / 2, 0, 0, SH, SW, SH, green);
		
		GradientLine(x, 0, SW-x, SH, green, red);
		x += xv;
		if (x == SW || x == 0) xv *= -1;
		
		OutlinedRectangle(SW - image.width, 16, image.width, image.height, green);
		image.blit(SW - image.width, 16);
		
		FlipScreen();
	}
}
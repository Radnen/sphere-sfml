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
	
	var points = [{x: 0, y: 0}, {x: 50, y: 0}, {x: 50, y: 50}, {x: 0, y: 50}];
	
	var canvas1 = CreateSurface(50, 50, white);
	canvas1.setPixel(25, 25, red);
	
	var canvas2 = canvas1.clone();
	canvas2.setPixel(26, 25, green);
	
	while (true) {
		canvas1.blit(50, 0);
		canvas2.blit(50, 50);
		Rectangle(0, 50, 50, 50, white);
		image.blitMask(0, 50, tblack);
		PointSeries(points, red);
		LineSeries(points, green);
		
		FlipScreen();
	}
}
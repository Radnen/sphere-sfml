RequireScript("require.js");

function game() {
	Print("Starting my game!");
	test();
	
	var image = LoadImage("blockman.png");
	var red = CreateColor(255, 0, 0);
	
	Print(red);
	
	while (true) {
		image.blit(0, 0);
		FlipScreen();
	}
}
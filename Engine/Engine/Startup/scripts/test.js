RequireScript("require.js");

function game() {
	Print("Starting my game!");
	test();
	
	var image = LoadImage("blockman.png");
	
	while (true) { image.blit(0, 0); FlipScreen(); }
}
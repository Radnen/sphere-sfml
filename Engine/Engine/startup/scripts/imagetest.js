// Purpose: Tests the speed and efficiency of images.

function TestImages()
{
	var done = false;
	var image = LoadImage("blockman.png");

	while (!done) {
		for (var y = 0; y < 5; ++y) {
			for (var x = 0; x < 5; ++x) {
				image.blit(x*48, y*48);
			}
		}
		
		FlipScreen();
	
		while (AreKeysLeft()) {
			if (GetKey() == KEY_ENTER) done = true;
		}
	}
}
// Purpose: Tests the speed and efficiency of images.

function TestFonts()
{
	var done = false;
	var font = LoadFont("test.rfn");
	var font2 = font.clone();
	
	var green = CreateColor(0, 255, 0);
	var white = CreateColor(255, 255, 255);
	
	var string = "";
	for (var i = 33; i < 128; ++i) {
		string += String.fromCharCode(i);
	}
	
	var strings = font.wordWrapString(string, GetScreenWidth());
	var text = "Once upon a time there was a box with letters that wrapped around the edges...";
	var height = font.getStringHeight(text, 200);
	
	var glyph = font.getCharacterImage(65);
	font2.setCharacterImage(105, GetSystemArrow());
	
	while (!done) {
		font.drawText(0, 0, "Hello World!");
		
		font.setColorMask(green);
		font.drawText(0, 16, "Height: " + font.getHeight());
		
		font2.drawText(80, 16, "A modified copy!");
		
		font.setColorMask(white);
		
		for (var i = 0; i < strings.length; ++i) {
			font.drawText(0, 32+i*16, strings[i]);
		}
		
		OutlinedRectangle(32, 128, 200, height + 4, white);
		font.drawTextBox(34, 130, 200, height, 0, text);
		
		glyph.blit(0, 96);
		font.drawText(64, 96, "Hello All".substr(0, 5));
		
		FlipScreen();
	
		while (AreKeysLeft()) {
			if (GetKey() == KEY_ENTER) done = true;
		}
	}
}
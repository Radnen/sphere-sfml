// spritesettest

function TestSpritesets()
{
	var done = false;
    var spriteset = LoadSpriteset("test.rss");

	var dirs = spriteset.directions;
	var imgs = spriteset.images;
	var w = imgs[0].width;
	var h = imgs[0].height;
	var white = CreateColor(255, 255, 255);
	var black = CreateColor(0  , 0  , 0  );

    while (!done) {
		sys_font.drawText(0, 0, "images: " + imgs.length);
        
        for (var d = 0; d < 4; ++d) {
        	var frames = dirs[d].frames;
        	for (var f = 0; f < frames.length; ++f) {
        	    var image = imgs[frames[f].index];
        	    var x = f * w;
        	    var y = 16 + d * h;
        	    
        	    OutlinedRectangle(x, y, w - 1, h - 1, white);
        		image.blit(x, y);
        		sys_font.drawText(x, y, frames[f].delay);
        	}
        }
        
        FlipScreen();

		while (AreKeysLeft()) {
			if (GetKey() == KEY_ENTER) done = true;
		}
    }
}
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
	var magenta = CreateColor(255, 0, 255);

	var base = spriteset.base;
	var bw = base.x2 - base.x1;
	var bh = base.y2 - base.y1;
	var ox = base.x1, oy = base.y1;
	
	var base_img = CreateSurface(bw, bh, CreateColor(0, 0, 0, 0));
	base_img.outlinedRectangle(0, 0, bw - 1, bh - 1, magenta);
	base_img = base_img.createImage();
	
	var img_bg = CreateSurface(w, h, CreateColor(0, 0, 0, 0));
	img_bg.outlinedRectangle(0, 0, w - 2, h - 2, white);
	img_bg = img_bg.createImage();

    while (!done) {
		sys_font.drawText(0, 0, "images: " + imgs.length);
        
        for (var d = 0; d < 4; ++d) {
        	var frames = dirs[d].frames;
        	for (var f = 0; f < frames.length; ++f) {
        	    var image = imgs[frames[f].index];
        	    var x = f * w;
        	    var y = 16 + d * h;
        	    
        	    img_bg.blit(x, y);
        		image.blit(x, y);
        		base_img.blit(x + ox, y + oy);
        		sys_font.drawText(x, y, frames[f].delay);
        	}
        }
        
        FlipScreen();

		while (AreKeysLeft()) {
			if (GetKey() == KEY_ENTER) done = true;
		}
    }
}
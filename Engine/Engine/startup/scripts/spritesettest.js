// spritesettest

function TestSpritesets()
{
    var done = false;
    var time = GetTime();
    var spriteset = LoadSpriteset("test.rss");
    time = GetTime() - time;

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
	h += 16;

	var yv = 0;
    while (!done) {
        sys_font.drawText(0, 0, "images: " + imgs.length + ", loaded in: " + time + "ms");
        Rectangle(GetScreenWidth() - 8, -(((yv/64)/dirs.length)*GetScreenHeight()), 8, 8, white);

        for (var d = 0; d < dirs.length; ++d) {
            var yy = yv + 32 + d * h;
            if (yy + 48 > GetScreenHeight() || yy < 0) continue;

            sys_font.drawText(0, yy - 16, dirs[d].name);
        	var frames = dirs[d].frames;
        	for (var f = 0; f < frames.length; ++f) {
        	    var x = f * w;
        	    var image = imgs[frames[f].index];
        	    
        	    img_bg.blit(x, yy);
        		image.blit(x, yy);
        		base_img.blit(x + ox, yy + oy);
        		sys_font.drawText(x, yy, frames[f].delay);
        	}
        }
        
        FlipScreen();

        while (AreKeysLeft()) {
            var k = GetKey();
		    if (k == KEY_ENTER) done = true;
		    if (k == KEY_UP) yv += 64;
		    if (k == KEY_DOWN) yv -= 64;
        }
    }
}
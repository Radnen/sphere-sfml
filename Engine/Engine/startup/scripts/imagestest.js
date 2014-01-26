// Purpose: test drawing many images on screen

function TestManyImages() {
    var image = CreateSurface(64, 48, CreateColor(255, 0, 255)).createImage(),
        done = false,
		factor = 2,
        w = GetScreenWidth() * factor,
        h = GetScreenHeight() * factor;

    while (!done) {
        for (var y = 0; y < h; y += image.height) {
            for (var x = 0; x < w; x += image.width) {
                image.blit(x, y);
            }
        }

        FlipScreen();

        while (AreKeysLeft()) {
            if (GetKey() == KEY_ENTER) done = true;
        }
    }
}

function TestManyPrims() {
    var done = false,
        w = 32, h = 24,
        sw = GetScreenWidth(),
        sh = GetScreenHeight(),
        color1 = CreateColor(255, 0, 0),
        color2 = CreateColor(0, 0, 255);

    while (!done) {
        color1.red = 255;

        for (var y = 0; y < sh; y += h) {
            for (var x = 0; x < sw; x += w) {
                GradientRectangle(x, y, w, h, color1, color1, color2, color2);
            }
        }

        FlipScreen();

        while (AreKeysLeft()) {
            if (GetKey() == KEY_ENTER) done = true;
        }
    }
}


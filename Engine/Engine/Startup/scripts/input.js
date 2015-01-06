// Input test

function TestInput()
{
	var done = false;
    var arrow = GetSystemArrow();
    var a_x = GetScreenWidth() / 2 - arrow.width / 2;
    var a_y = GetScreenHeight() / 2 - arrow.height / 2;
	var green = CreateColor(0, 255, 0);
	var key = 0;

    while (!done) {
        var x = GetMouseX();
        var y = GetMouseY();

        var dx = x - (a_x + arrow.width / 2);
        var dy = y - (a_y + arrow.height / 2);

        var angle = Math.atan2(dy, dx);

		Point(x, y, green);
        arrow.rotateBlit(a_x, a_y, angle);

        sys_font.drawText(0, 0, "Key: " + key + ", " + GetKeyString(key, IsKeyPressed(KEY_SHIFT)));
        
        FlipScreen();

		while (AreKeysLeft()) {
			var k = GetKey();
			if (k != KEY_SHIFT) key = k;
			if (k == KEY_ENTER) done = true;
		}
    }
}